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
using System;

namespace NetClinic.Api.Tests.Unit.Controllers;

public class PetsControllerTests
{
    private readonly Mock<ILogger<PetsController>> _loggerMock;
    private readonly Mock<IPetService> _petServiceMock;
    private readonly PetsController _controller;

    public PetsControllerTests()
    {
        _loggerMock = new Mock<ILogger<PetsController>>();
        _petServiceMock = new Mock<IPetService>();
        _controller = new PetsController(_loggerMock.Object, _petServiceMock.Object);
    }

    [Fact]
    public async Task GetPetsByOwnerId_ShouldReturnOkWithPets_WhenPetsExist()
    {
        // Arrange
        const int ownerId = 1;
        var pets = new List<PetDto>
        {
            new() { Id = 1, Name = "Fluffy", Type = "Cat", BirthDate = "2020-01-01" },
            new() { Id = 2, Name = "Buddy", Type = "Dog", BirthDate = "2019-05-15" }
        };

        _petServiceMock.Setup(x => x.GetPetsByOwnerIdAsync(ownerId))
            .ReturnsAsync(pets);

        // Act
        var result = await _controller.GetPetsByOwnerId(ownerId);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(pets);
    }

    [Fact]
    public async Task GetPetsByOwnerId_ShouldReturnNotFound_WhenNoPetsExist()
    {
        // Arrange
        const int ownerId = 1;
        _petServiceMock.Setup(x => x.GetPetsByOwnerIdAsync(ownerId))
            .ReturnsAsync(null as IEnumerable<PetDto>);

        // Act
        var result = await _controller.GetPetsByOwnerId(ownerId);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task GetPetsByOwnerId_ShouldReturnNotFound_WhenPetsListIsEmpty()
    {
        // Arrange
        const int ownerId = 1;
        _petServiceMock.Setup(x => x.GetPetsByOwnerIdAsync(ownerId))
            .ReturnsAsync(new List<PetDto>());

        // Act
        var result = await _controller.GetPetsByOwnerId(ownerId);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task GetPetById_ShouldReturnOkWithPet_WhenPetExists()
    {
        // Arrange
        const int ownerId = 1;
        const int petId = 1;
        var pet = new PetDto { Id = petId, Name = "Fluffy", Type = "Cat", BirthDate = "2020-01-01" };

        _petServiceMock.Setup(x => x.GetPetByIdAsync(petId))
            .ReturnsAsync(pet);

        // Act
        var result = await _controller.GetPetById(ownerId, petId);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(pet);
    }

    [Fact]
    public async Task GetPetById_ShouldReturnNotFound_WhenPetDoesNotExist()
    {
        // Arrange
        const int ownerId = 1;
        const int petId = 999;
        _petServiceMock.Setup(x => x.GetPetByIdAsync(petId))
            .ReturnsAsync(null as PetDto);

        // Act
        var result = await _controller.GetPetById(ownerId, petId);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task GetVisitsByPetId_ShouldReturnOkWithVisits_WhenVisitsExist()
    {
        // Arrange
        const int ownerId = 1;
        const int petId = 1;
        var visits = new List<VisitDto>
        {
            new() { Id = 1, VisitDate = "2023-01-15", Description = "Checkup" },
            new() { Id = 2, VisitDate = "2023-06-20", Description = "Vaccination" }
        };

        _petServiceMock.Setup(x => x.GetVisitsByPetIdAsync(ownerId, petId))
            .ReturnsAsync(visits);

        // Act
        var result = await _controller.GetVisitsByPetId(ownerId, petId);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(visits);
    }

    [Fact]
    public async Task GetVisitsByPetId_ShouldReturnNotFound_WhenNoVisitsExist()
    {
        // Arrange
        const int ownerId = 1;
        const int petId = 1;
        _petServiceMock.Setup(x => x.GetVisitsByPetIdAsync(ownerId, petId))
            .ReturnsAsync(null as IEnumerable<VisitDto>);

        // Act
        var result = await _controller.GetVisitsByPetId(ownerId, petId);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task CreatePet_ShouldReturnCreatedPet_WhenValidPetProvided()
    {
        // Arrange
        const int ownerId = 1;
        var newPetDto = new PetDto
        {
            Name = "Fluffy",
            Type = "Cat",
            BirthDate = "2020-01-01"
        };
        var createdPet = new PetDto
        {
            Id = 1,
            Name = "Fluffy",
            Type = "Cat",
            BirthDate = "2020-01-01"
        };

        _petServiceMock.Setup(x => x.CreatePetAsync(newPetDto, ownerId))
            .ReturnsAsync(createdPet);

        // Act
        var result = await _controller.CreatePet(ownerId, newPetDto);

        // Assert
        result.Result.Should().BeOfType<CreatedAtActionResult>();
        var createdResult = result.Result as CreatedAtActionResult;
        createdResult!.Value.Should().BeEquivalentTo(createdPet);
    }

    [Fact]
    public async Task CreatePet_ShouldReturnBadRequest_WhenNameIsEmpty()
    {
        // Arrange
        const int ownerId = 1;
        var invalidPetDto = new PetDto
        {
            Name = "", // Invalid
            Type = "Cat",
            BirthDate = "2020-01-01"
        };

        // Act
        var result = await _controller.CreatePet(ownerId, invalidPetDto);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result.Result as BadRequestObjectResult;
        var errors = badRequestResult!.Value as Dictionary<string, string>;
        errors.Should().ContainKey("name");
    }

    [Fact]
    public async Task UpdatePet_ShouldReturnOkWithUpdatedPet_WhenValidPetProvided()
    {
        // Arrange
        const int ownerId = 1;
        const int petId = 1;
        var petDto = new PetDto
        {
            Id = petId,
            Name = "Fluffy Updated",
            Type = "Cat",
            BirthDate = "2020-01-01"
        };

        _petServiceMock.Setup(x => x.UpdatePetAsync(It.IsAny<PetDto>()))
            .ReturnsAsync(petDto);

        // Act
        var result = await _controller.UpdatePet(ownerId, petId, petDto);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(petDto);
    }

    [Fact]
    public async Task UpdatePet_ShouldReturnNotFound_WhenPetDoesNotExist()
    {
        // Arrange
        const int ownerId = 1;
        const int petId = 999;
        var petDto = new PetDto
        {
            Id = petId,
            Name = "Fluffy",
            Type = "Cat",
            BirthDate = "2020-01-01"
        };

        _petServiceMock.Setup(x => x.UpdatePetAsync(It.IsAny<PetDto>()))
            .ReturnsAsync(null as PetDto);

        // Act
        var result = await _controller.UpdatePet(ownerId, petId, petDto);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task GetAllPetTypes_ShouldReturnOkWithPetTypes()
    {
        // Arrange
        var petTypes = new List<PetTypeDto>
        {
            new() { Id = 1, Name = "Cat" },
            new() { Id = 2, Name = "Dog" },
            new() { Id = 3, Name = "Bird" }
        };

        _petServiceMock.Setup(x => x.GetAllPetTypesAsync())
            .ReturnsAsync(petTypes);

        // Act
        var result = await _controller.GetAllPetTypes();

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(petTypes);
    }

    [Fact]
    public async Task CreateVisit_ShouldReturnCreatedVisit_WhenValidVisitProvided()
    {
        // Arrange
        const int petId = 1;
        var newVisitDto = new VisitDto
        {
            VisitDate = "2023-12-13",
            Description = "Annual checkup"
        };
        var createdVisit = new VisitDto
        {
            Id = 1,
            VisitDate = "2023-12-13",
            Description = "Annual checkup"
        };

        _petServiceMock.Setup(x => x.CreateVisitAsync(petId, newVisitDto))
            .ReturnsAsync(createdVisit);

        // Act
        var result = await _controller.CreateVisit(petId, newVisitDto);

        // Assert
        result.Result.Should().BeOfType<CreatedAtActionResult>();
        var createdResult = result.Result as CreatedAtActionResult;
        createdResult!.Value.Should().BeEquivalentTo(createdVisit);
    }

    [Fact]
    public async Task CreateVisit_ShouldReturnBadRequest_WhenDescriptionIsEmpty()
    {
        // Arrange
        const int petId = 1;
        var invalidVisitDto = new VisitDto
        {
            VisitDate = "2023-12-13",
            Description = "" // Invalid
        };

        // Act
        var result = await _controller.CreateVisit(petId, invalidVisitDto);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result.Result as BadRequestObjectResult;
        var errors = badRequestResult!.Value as Dictionary<string, string>;
        errors.Should().ContainKey("description");
    }

    [Theory]
    [InlineData("", "Cat", "2020-01-01", "name")]
    [InlineData("Fluffy", "", "2020-01-01", "type")]
    [InlineData("Fluffy", "Cat", "", "birthDate")]
    [InlineData("Fluffy", "Cat", "invalid-date", "birthDate")]
    public void ValidatePetDto_ShouldReturnError_WhenRequiredFieldIsInvalid(
        string name, string type, string birthDate, string expectedErrorKey)
    {
        // Arrange
        var petDto = new PetDto
        {
            Name = name,
            Type = type,
            BirthDate = birthDate
        };

        // Act
        var errors = PetsController.ValidatePetDto(petDto);

        // Assert
        errors.Should().ContainKey(expectedErrorKey);
    }

    [Fact]
    public void ValidatePetDto_ShouldReturnNoErrors_WhenAllFieldsValid()
    {
        // Arrange
        var validPetDto = new PetDto
        {
            Name = "Fluffy",
            Type = "Cat",
            BirthDate = "2020-01-01"
        };

        // Act
        var errors = PetsController.ValidatePetDto(validPetDto);

        // Assert
        errors.Should().BeEmpty();
    }

    [Fact]
    public void ValidateVisitDto_ShouldReturnError_WhenDescriptionIsEmpty()
    {
        // Arrange
        var visitDto = new VisitDto
        {
            Description = "",
            VisitDate = "2023-12-13"
        };

        // Act
        var errors = PetsController.ValidateVisitDto(visitDto);

        // Assert
        errors.Should().ContainKey("description");
    }

    [Fact]
    public void ValidateVisitDto_ShouldReturnNoErrors_WhenAllFieldsValid()
    {
        // Arrange
        var validVisitDto = new VisitDto
        {
            Description = "Annual checkup",
            VisitDate = "2023-12-13"
        };

        // Act
        var errors = PetsController.ValidateVisitDto(validVisitDto);

        // Assert
        errors.Should().BeEmpty();
    }

    [Fact]
    public void ValidateVisitDto_ShouldReturnError_WhenVisitIsNull()
    {
        // Act
        var errors = PetsController.ValidateVisitDto(null);

        // Assert
        errors.Should().ContainKey("visit");
        errors["visit"].Should().Be("must not be blank");
    }
}