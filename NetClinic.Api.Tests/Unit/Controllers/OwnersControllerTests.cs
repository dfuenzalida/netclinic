using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NetClinic.Api.Controllers;
using NetClinic.Api.Dto;
using NetClinic.Api.Services;
using Xunit;
using FluentAssertions;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace NetClinic.Api.Tests.Unit.Controllers;

public class OwnersControllerTests
{
    private readonly Mock<ILogger<OwnersController>> _loggerMock;
    private readonly Mock<IOwnerService> _ownerServiceMock;
    private readonly OwnersController _controller;

    public OwnersControllerTests()
    {
        _loggerMock = new Mock<ILogger<OwnersController>>();
        _ownerServiceMock = new Mock<IOwnerService>();
        _controller = new OwnersController(_loggerMock.Object, _ownerServiceMock.Object);
    }

    [Fact]
    public async Task Get_ShouldReturnOwnerListDto_WhenOwnersExist()
    {
        // Arrange
        var owners = new List<OwnerDto>
        {
            new() { Id = 1, FirstName = "John", LastName = "Doe", Address = "123 Main St", City = "Anytown", Telephone = "1234567890" },
            new() { Id = 2, FirstName = "Jane", LastName = "Smith", Address = "456 Oak Ave", City = "Somewhere", Telephone = "0987654321" }
        };
        const int totalOwners = 2;
        const int page = 1;
        const int pageSize = 5;

        _ownerServiceMock.Setup(x => x.GetOwnersByLastNameCountAsync(null))
            .ReturnsAsync(totalOwners);
        _ownerServiceMock.Setup(x => x.GetOwnersByLastNameAsync(null, page, pageSize))
            .ReturnsAsync(owners);

        // Act
        var result = await _controller.Get();

        // Assert
        result.Should().NotBeNull();
        result.OwnerList.Should().HaveCount(2);
        result.TotalPages.Should().Be(1);
        result.OwnerList.Should().BeEquivalentTo(owners);
    }

    [Fact]
    public async Task Get_ShouldReturnFilteredOwners_WhenLastNameProvided()
    {
        // Arrange
        const string lastName = "Doe";
        var owners = new List<OwnerDto>
        {
            new() { Id = 1, FirstName = "John", LastName = "Doe", Address = "123 Main St", City = "Anytown", Telephone = "1234567890" }
        };
        const int totalOwners = 1;

        _ownerServiceMock.Setup(x => x.GetOwnersByLastNameCountAsync(lastName))
            .ReturnsAsync(totalOwners);
        _ownerServiceMock.Setup(x => x.GetOwnersByLastNameAsync(lastName, 1, 5))
            .ReturnsAsync(owners);

        // Act
        var result = await _controller.Get(lastName);

        // Assert
        result.Should().NotBeNull();
        result.OwnerList.Should().HaveCount(1);
        result.OwnerList.First().LastName.Should().Be("Doe");
    }

    [Fact]
    public async Task Get_ShouldCalculateCorrectTotalPages()
    {
        // Arrange
        const int totalOwners = 12;
        const int pageSize = 5;
        const int expectedPages = 3; // (12 / 5) + 1 = 3

        _ownerServiceMock.Setup(x => x.GetOwnersByLastNameCountAsync(null))
            .ReturnsAsync(totalOwners);
        _ownerServiceMock.Setup(x => x.GetOwnersByLastNameAsync(null, 1, pageSize))
            .ReturnsAsync(new List<OwnerDto>());

        // Act
        var result = await _controller.Get(pageSize: pageSize);

        // Assert
        result.TotalPages.Should().Be(expectedPages);
    }

    [Fact]
    public async Task GetOwnerDetailsById_ShouldReturnOwner_WhenOwnerExists()
    {
        // Arrange
        const int ownerId = 1;
        var owner = new OwnerDto 
        { 
            Id = ownerId, 
            FirstName = "John", 
            LastName = "Doe", 
            Address = "123 Main St", 
            City = "Anytown", 
            Telephone = "1234567890" 
        };

        _ownerServiceMock.Setup(x => x.GetOwnerDetailsByIdAsync(ownerId))
            .ReturnsAsync(owner);

        // Act
        var result = await _controller.GetOwnerDetailsById(ownerId);

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().BeEquivalentTo(owner);
    }

    [Fact]
    public async Task GetOwnerDetailsById_ShouldReturnNotFound_WhenOwnerDoesNotExist()
    {
        // Arrange
        const int ownerId = 999;
        _ownerServiceMock.Setup(x => x.GetOwnerDetailsByIdAsync(ownerId))
            .ReturnsAsync(null as OwnerDto);

        // Act
        var result = await _controller.GetOwnerDetailsById(ownerId);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task CreateOwner_ShouldReturnCreatedOwner_WhenValidOwnerProvided()
    {
        // Arrange
        var newOwnerDto = new OwnerDto
        {
            FirstName = "John",
            LastName = "Doe",
            Address = "123 Main St",
            City = "Anytown",
            Telephone = "1234567890"
        };
        var createdOwner = new OwnerDto
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
            Address = "123 Main St",
            City = "Anytown",
            Telephone = "1234567890"
        };

        _ownerServiceMock.Setup(x => x.CreateOwnerAsync(newOwnerDto))
            .ReturnsAsync(createdOwner);

        // Act
        var result = await _controller.CreateOwner(newOwnerDto);

        // Assert
        result.Result.Should().BeOfType<CreatedAtActionResult>();
        var createdResult = result.Result as CreatedAtActionResult;
        createdResult!.Value.Should().BeEquivalentTo(createdOwner);
    }

    [Fact]
    public async Task CreateOwner_ShouldReturnBadRequest_WhenFirstNameIsEmpty()
    {
        // Arrange
        var invalidOwnerDto = new OwnerDto
        {
            FirstName = "", // Invalid
            LastName = "Doe",
            Address = "123 Main St",
            City = "Anytown",
            Telephone = "1234567890"
        };

        // Act
        var result = await _controller.CreateOwner(invalidOwnerDto);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result.Result as BadRequestObjectResult;
        var errors = badRequestResult!.Value as Dictionary<string, string>;
        errors.Should().ContainKey("firstName");
    }

    [Fact]
    public async Task CreateOwner_ShouldReturnBadRequest_WhenTelephoneIsInvalid()
    {
        // Arrange
        var invalidOwnerDto = new OwnerDto
        {
            FirstName = "John",
            LastName = "Doe",
            Address = "123 Main St",
            City = "Anytown",
            Telephone = "123" // Invalid - not 10 digits
        };

        // Act
        var result = await _controller.CreateOwner(invalidOwnerDto);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result.Result as BadRequestObjectResult;
        var errors = badRequestResult!.Value as Dictionary<string, string>;
        errors.Should().ContainKey("telephone");
        errors!["telephone"].Should().Be("Telephone must be a 10-digit number");
    }

    [Fact]
    public async Task UpdateOwner_ShouldReturnUpdatedOwner_WhenValidOwnerProvided()
    {
        // Arrange
        const int ownerId = 1;
        var ownerDto = new OwnerDto
        {
            Id = ownerId,
            FirstName = "John",
            LastName = "Doe",
            Address = "123 Main St",
            City = "Anytown",
            Telephone = "1234567890"
        };

        _ownerServiceMock.Setup(x => x.UpdateOwnerAsync(It.IsAny<OwnerDto>()))
            .ReturnsAsync(ownerDto);

        // Act
        var result = await _controller.UpdateOwner(ownerId, ownerDto);

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().BeEquivalentTo(ownerDto);
    }

    [Fact]
    public async Task UpdateOwner_ShouldReturnNotFound_WhenOwnerDoesNotExist()
    {
        // Arrange
        const int ownerId = 999;
        var ownerDto = new OwnerDto
        {
            Id = ownerId,
            FirstName = "John",
            LastName = "Doe",
            Address = "123 Main St",
            City = "Anytown",
            Telephone = "1234567890"
        };

        _ownerServiceMock.Setup(x => x.UpdateOwnerAsync(It.IsAny<OwnerDto>()))
            .ReturnsAsync(null as OwnerDto);

        // Act
        var result = await _controller.UpdateOwner(ownerId, ownerDto);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Theory]
    [InlineData("", "Doe", "123 Main St", "Anytown", "1234567890", "firstName")]
    [InlineData("John", "", "123 Main St", "Anytown", "1234567890", "lastName")]
    [InlineData("John", "Doe", "", "Anytown", "1234567890", "address")]
    [InlineData("John", "Doe", "123 Main St", "", "1234567890", "city")]
    [InlineData("John", "Doe", "123 Main St", "Anytown", "", "telephone")]
    public void ValidateOwnerDto_ShouldReturnError_WhenRequiredFieldIsEmpty(
        string firstName, string lastName, string address, string city, string telephone, string expectedErrorKey)
    {
        // Arrange
        var ownerDto = new OwnerDto
        {
            FirstName = firstName,
            LastName = lastName,
            Address = address,
            City = city,
            Telephone = telephone
        };

        // Act
        var errors = OwnersController.ValidateOwnerDto(ownerDto);

        // Assert
        errors.Should().ContainKey(expectedErrorKey);
    }

    [Fact]
    public void ValidateOwnerDto_ShouldReturnNoErrors_WhenAllFieldsValid()
    {
        // Arrange
        var validOwnerDto = new OwnerDto
        {
            FirstName = "John",
            LastName = "Doe",
            Address = "123 Main St",
            City = "Anytown",
            Telephone = "1234567890"
        };

        // Act
        var errors = OwnersController.ValidateOwnerDto(validOwnerDto);

        // Assert
        errors.Should().BeEmpty();
    }

    [Fact]
    public void ValidateOwnerDto_ShouldReturnError_WhenOwnerIsNull()
    {
        // Act
        var errors = OwnersController.ValidateOwnerDto(null);

        // Assert
        errors.Should().ContainKey("owner");
        errors["owner"].Should().Be("must not be blank");
    }
}