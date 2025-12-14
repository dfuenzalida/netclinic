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

public class VetsControllerTests
{
    private readonly Mock<ILogger<VetsController>> _loggerMock;
    private readonly Mock<IVetService> _vetServiceMock;
    private readonly VetsController _controller;

    public VetsControllerTests()
    {
        _loggerMock = new Mock<ILogger<VetsController>>();
        _vetServiceMock = new Mock<IVetService>();
        _controller = new VetsController(_loggerMock.Object, _vetServiceMock.Object);
    }

    [Fact]
    public async Task Get_ShouldReturnVetListDto_WhenVetsExist()
    {
        // Arrange
        var vets = new List<VetDto>
        {
            new() { Id = 1, FirstName = "Dr. John", LastName = "Smith", Specialties = new List<SpecialtyDto>() },
            new() { Id = 2, FirstName = "Dr. Jane", LastName = "Doe", Specialties = new List<SpecialtyDto>() }
        };
        const int totalVets = 2;
        const int page = 1;

        _vetServiceMock.Setup(x => x.GetVeterinariansCountAsync())
            .ReturnsAsync(totalVets);
        _vetServiceMock.Setup(x => x.GetAllVeterinariansAsync(page, 5))
            .ReturnsAsync(vets);

        // Act
        var result = await _controller.Get(page);

        // Assert
        result.Should().NotBeNull();
        result.VetList.Should().HaveCount(2);
        result.TotalPages.Should().Be(1);
        result.VetList.Should().BeEquivalentTo(vets);
    }

    [Fact]
    public async Task Get_ShouldCalculateCorrectTotalPages()
    {
        // Arrange
        const int totalVets = 12;
        const int expectedPages = 3; // (12 / 5) + 1 = 3

        _vetServiceMock.Setup(x => x.GetVeterinariansCountAsync())
            .ReturnsAsync(totalVets);
        _vetServiceMock.Setup(x => x.GetAllVeterinariansAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(new List<VetDto>());

        // Act
        var result = await _controller.Get();

        // Assert
        result.TotalPages.Should().Be(expectedPages);
    }

    [Fact]
    public async Task Get_ShouldReturnEmptyList_WhenNoVetsExist()
    {
        // Arrange
        const int totalVets = 0;
        var emptyVetList = new List<VetDto>();

        _vetServiceMock.Setup(x => x.GetVeterinariansCountAsync())
            .ReturnsAsync(totalVets);
        _vetServiceMock.Setup(x => x.GetAllVeterinariansAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(emptyVetList);

        // Act
        var result = await _controller.Get();

        // Assert
        result.Should().NotBeNull();
        result.VetList.Should().BeEmpty();
        result.TotalPages.Should().Be(0);
    }

    [Fact]
    public async Task GetById_ShouldReturnVet_WhenVetExists()
    {
        // Arrange
        const int vetId = 1;
        var vet = new VetDto 
        { 
            Id = vetId, 
            FirstName = "Dr. John", 
            LastName = "Smith", 
            Specialties = new List<SpecialtyDto>()
        };

        _vetServiceMock.Setup(x => x.GetVeterinarianByIdAsync(vetId))
            .ReturnsAsync(vet);

        // Act
        var result = await _controller.GetById(vetId);

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().BeEquivalentTo(vet);
    }

    [Fact]
    public async Task GetById_ShouldReturnNotFound_WhenVetDoesNotExist()
    {
        // Arrange
        const int vetId = 999;
        _vetServiceMock.Setup(x => x.GetVeterinarianByIdAsync(vetId))
            .ReturnsAsync(null as VetDto);

        // Act
        var result = await _controller.GetById(vetId);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public async Task Get_ShouldCallServiceWithCorrectPageNumber(int page)
    {
        // Arrange
        _vetServiceMock.Setup(x => x.GetVeterinariansCountAsync())
            .ReturnsAsync(10);
        _vetServiceMock.Setup(x => x.GetAllVeterinariansAsync(page, 5))
            .ReturnsAsync(new List<VetDto>());

        // Act
        await _controller.Get(page);

        // Assert
        _vetServiceMock.Verify(x => x.GetAllVeterinariansAsync(page, 5), Times.Once);
    }

    [Fact]
    public async Task GetById_ShouldCallServiceWithCorrectId()
    {
        // Arrange
        const int vetId = 42;
        _vetServiceMock.Setup(x => x.GetVeterinarianByIdAsync(vetId))
            .ReturnsAsync(new VetDto { Id = vetId, FirstName = "Test", LastName = "Vet", Specialties = new List<SpecialtyDto>() });

        // Act
        await _controller.GetById(vetId);

        // Assert
        _vetServiceMock.Verify(x => x.GetVeterinarianByIdAsync(vetId), Times.Once);
    }
}