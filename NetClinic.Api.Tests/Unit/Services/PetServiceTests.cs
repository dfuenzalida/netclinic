using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using FluentAssertions;
using Xunit;
using NetClinic.Api.Data;
using NetClinic.Api.Services;
using NetClinic.Api.Models;
using NetClinic.Api.Dto;

#nullable enable

namespace NetClinic.Api.Tests.Unit.Services;

public class PetServiceTests : IDisposable
{
    private readonly NetClinicDbContext _context;
    private readonly Mock<ILogger<PetService>> _loggerMock;
    private readonly PetService _petService;

    public PetServiceTests()
    {
        var options = new DbContextOptionsBuilder<NetClinicDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new NetClinicDbContext(options);
        _loggerMock = new Mock<ILogger<PetService>>();
        _petService = new PetService(_context, _loggerMock.Object);

        SeedTestData();
    }

    private void SeedTestData()
    {
        // Create pet types
        var dogType = new PetType { Id = 1, Name = "Dog" };
        var catType = new PetType { Id = 2, Name = "Cat" };
        _context.PetTypes.AddRange(dogType, catType);

        // Create owners
        var owner1 = new Owner
        {
            Id = 1,
            FirstName = "John",
            LastName = "Smith",
            Address = "123 Main St",
            City = "Anytown",
            Telephone = "555-1234"
        };

        var owner2 = new Owner
        {
            Id = 2,
            FirstName = "Jane",
            LastName = "Johnson",
            Address = "456 Oak Ave",
            City = "Somewhere",
            Telephone = "555-5678"
        };

        _context.Owners.AddRange(owner1, owner2);

        // Create pets
        var pets = new List<Pet>
        {
            new Pet
            {
                Id = 1,
                Name = "Buddy",
                BirthDate = DateTime.Parse("2020-01-15"),
                PetType = dogType,
                OwnerId = 1,
                Visits = new List<Visit>
                {
                    new Visit { Id = 1, VisitDate = DateTime.Parse("2023-01-15"), Description = "Checkup" }
                }
            },
            new Pet
            {
                Id = 2,
                Name = "Whiskers",
                BirthDate = DateTime.Parse("2019-05-10"),
                PetType = catType,
                OwnerId = 1,
                Visits = new List<Visit>()
            },
            new Pet
            {
                Id = 3,
                Name = "Max",
                BirthDate = DateTime.Parse("2021-03-20"),
                PetType = dogType,
                OwnerId = 2,
                Visits = new List<Visit>()
            }
        };

        _context.Pets.AddRange(pets);
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetPetsByOwnerIdAsync_WithValidOwnerId_ReturnsPets()
    {
        // Act
        var result = await _petService.GetPetsByOwnerIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result!.Select(p => p.Name).Should().Contain("Buddy", "Whiskers");
    }

    [Fact]
    public async Task GetPetsByOwnerIdAsync_WithOwnerWithOnePet_ReturnsOnePet()
    {
        // Act
        var result = await _petService.GetPetsByOwnerIdAsync(2);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result!.First().Name.Should().Be("Max");
    }

    [Fact]
    public async Task GetPetsByOwnerIdAsync_WithNonExistentOwnerId_ReturnsNull()
    {
        // Act
        var result = await _petService.GetPetsByOwnerIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetPetByIdAsync_WithValidPetId_ReturnsPet()
    {
        // Act
        var result = await _petService.GetPetByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.Name.Should().Be("Buddy");
        result.Type.Should().Be("Dog");
        result.BirthDate.Should().Be("2020-01-15");
    }

    [Fact]
    public async Task GetPetByIdAsync_WithInvalidPetId_ReturnsNull()
    {
        // Act
        var result = await _petService.GetPetByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetVisitsByPetIdAsync_WithValidPetId_ReturnsVisits()
    {
        // Act
        var result = await _petService.GetVisitsByPetIdAsync(1, 1);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result!.First().Description.Should().Be("Checkup");
    }

    [Fact]
    public async Task GetVisitsByPetIdAsync_WithPetWithoutVisits_ReturnsEmptyCollection()
    {
        // Act
        var result = await _petService.GetVisitsByPetIdAsync(1, 2);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetVisitsByPetIdAsync_WithInvalidPetId_ReturnsEmptyCollection()
    {
        // Act
        var result = await _petService.GetVisitsByPetIdAsync(1, 999);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllPetTypesAsync_ReturnsAllPetTypes()
    {
        // Act
        var result = await _petService.GetAllPetTypesAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Select(pt => pt.Name).Should().Contain("Dog", "Cat");
    }

    [Fact]
    public async Task CreatePetAsync_WithValidData_CreatesAndReturnsPet()
    {
        // Arrange
        var newPetDto = new PetDto
        {
            Name = "Fluffy",
            Type = "Cat",
            BirthDate = "2022-01-01"
        };

        // Act
        var result = await _petService.CreatePetAsync(newPetDto, 1);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
        result.Name.Should().Be("Fluffy");
        result.Type.Should().Be("Cat");
        result.BirthDate.Should().Be("2022-01-01");

        // Verify in database
        var petInDb = await _context.Pets.Include(p => p.PetType).FirstOrDefaultAsync(p => p.Id == result.Id);
        petInDb.Should().NotBeNull();
        petInDb!.Name.Should().Be("Fluffy");
        petInDb.OwnerId.Should().Be(1);
    }

    [Fact]
    public async Task CreatePetAsync_WithUnknownPetType_CreatesWithUnknownType()
    {
        // Arrange
        var newPetDto = new PetDto
        {
            Name = "Exotic",
            Type = "Snake",
            BirthDate = "2022-01-01"
        };

        // Act
        var result = await _petService.CreatePetAsync(newPetDto, 1);

        // Assert
        result.Should().NotBeNull();
        result.Type.Should().Be("unknown");
    }

    [Fact]
    public async Task UpdatePetAsync_WithValidData_UpdatesAndReturnsPet()
    {
        // Arrange
        var updatePetDto = new PetDto
        {
            Id = 1,
            Name = "Buddy Updated",
            Type = "Cat",
            BirthDate = "2020-02-15"
        };

        // Act
        var result = await _petService.UpdatePetAsync(updatePetDto);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.Name.Should().Be("Buddy Updated");
        result.Type.Should().Be("Cat");
        result.BirthDate.Should().Be("2020-02-15");

        // Verify in database
        var petInDb = await _context.Pets.Include(p => p.PetType).FirstOrDefaultAsync(p => p.Id == 1);
        petInDb.Should().NotBeNull();
        petInDb!.Name.Should().Be("Buddy Updated");
        petInDb.PetType.Name.Should().Be("Cat");
    }

    [Fact]
    public async Task UpdatePetAsync_WithInvalidId_ReturnsNull()
    {
        // Arrange
        var updatePetDto = new PetDto
        {
            Id = 999,
            Name = "NonExistent",
            Type = "Dog",
            BirthDate = "2020-01-01"
        };

        // Act
        var result = await _petService.UpdatePetAsync(updatePetDto);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateVisitAsync_WithValidData_CreatesAndReturnsVisit()
    {
        // Arrange
        var newVisitDto = new VisitDto
        {
            VisitDate = "2023-12-01",
            Description = "Vaccination"
        };

        // Act
        var result = await _petService.CreateVisitAsync(1, newVisitDto);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
        result.VisitDate.Should().Be("2023-12-01");
        result.Description.Should().Be("Vaccination");

        // Verify in database
        var visitInDb = await _context.Visits.FindAsync(result.Id);
        visitInDb.Should().NotBeNull();
        visitInDb!.PetId.Should().Be(1);
        visitInDb.Description.Should().Be("Vaccination");
    }

    [Fact]
    public async Task CreateVisitAsync_WithInvalidPetId_ThrowsArgumentException()
    {
        // Arrange
        var newVisitDto = new VisitDto
        {
            VisitDate = "2023-12-01",
            Description = "Vaccination"
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => _petService.CreateVisitAsync(999, newVisitDto));
    }

    [Theory]
    [InlineData("2020-01-15")]
    [InlineData("2019-12-31")]
    [InlineData("2022-06-15")]
    public async Task CreatePetAsync_WithDifferentBirthDates_CreatesCorrectly(string birthDate)
    {
        // Arrange
        var newPetDto = new PetDto
        {
            Name = "TestPet",
            Type = "Dog",
            BirthDate = birthDate
        };

        // Act
        var result = await _petService.CreatePetAsync(newPetDto, 1);

        // Assert
        result.Should().NotBeNull();
        result.BirthDate.Should().Be(birthDate);
    }

    [Fact]
    public async Task GetPetsByOwnerIdAsync_OrdersByPetName_ReturnsOrderedPets()
    {
        // Act
        var result = await _petService.GetPetsByOwnerIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        var petNames = result!.Select(p => p.Name).ToList();
        petNames.Should().BeInAscendingOrder();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}