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

public class OwnerServiceTests : IDisposable
{
    private readonly NetClinicDbContext _context;
    private readonly Mock<ILogger<OwnerService>> _loggerMock;
    private readonly OwnerService _ownerService;

    public OwnerServiceTests()
    {
        var options = new DbContextOptionsBuilder<NetClinicDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new NetClinicDbContext(options);
        _loggerMock = new Mock<ILogger<OwnerService>>();
        _ownerService = new OwnerService(_context, _loggerMock.Object);

        SeedTestData();
    }

    private void SeedTestData()
    {
        // Create a pet type first
        var petType = new PetType { Id = 1, Name = "Dog" };
        _context.PetTypes.Add(petType);

        var owners = new List<Owner>
        {
            new Owner
            {
                Id = 1,
                FirstName = "John",
                LastName = "Smith",
                Address = "123 Main St",
                City = "Anytown",
                Telephone = "555-1234",
                Pets = new List<Pet>
                {
                    new Pet { Id = 1, Name = "Buddy", BirthDate = DateTime.Parse("2020-01-15"), PetType = petType }
                }
            },
            new Owner
            {
                Id = 2,
                FirstName = "Jane",
                LastName = "Johnson",
                Address = "456 Oak Ave",
                City = "Somewhere",
                Telephone = "555-5678",
                Pets = new List<Pet>()
            },
            new Owner
            {
                Id = 3,
                FirstName = "Bob",
                LastName = "Smith",
                Address = "789 Pine St",
                City = "Anywhere",
                Telephone = "555-9012",
                Pets = new List<Pet>()
            }
        };

        _context.Owners.AddRange(owners);
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetOwnersByLastNameAsync_WithoutFilter_ReturnsAllOwners()
    {
        // Act
        var result = await _ownerService.GetOwnersByLastNameAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        result.Select(o => o.LastName).Should().Contain("Smith", "Johnson");
    }

    [Fact]
    public async Task GetOwnersByLastNameAsync_WithFilter_ReturnsFilteredOwners()
    {
        // Act
        var result = await _ownerService.GetOwnersByLastNameAsync("Smith");

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.All(o => o.LastName.StartsWith("Smith")).Should().BeTrue();
    }

    [Fact]
    public async Task GetOwnersByLastNameAsync_WithPagination_ReturnsCorrectPage()
    {
        // Act
        var result = await _ownerService.GetOwnersByLastNameAsync(null, page: 1, pageSize: 2);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetOwnersByLastNameAsync_WithNonExistentFilter_ReturnsEmptyList()
    {
        // Act
        var result = await _ownerService.GetOwnersByLastNameAsync("NonExistent");

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetOwnersByLastNameCountAsync_WithoutFilter_ReturnsCorrectCount()
    {
        // Act
        var result = await _ownerService.GetOwnersByLastNameCountAsync();

        // Assert
        result.Should().Be(3);
    }

    [Fact]
    public async Task GetOwnersByLastNameCountAsync_WithFilter_ReturnsFilteredCount()
    {
        // Act
        var result = await _ownerService.GetOwnersByLastNameCountAsync("Smith");

        // Assert
        result.Should().Be(2);
    }

    [Fact]
    public async Task GetOwnerDetailsByIdAsync_WithValidId_ReturnsOwnerDetails()
    {
        // Act
        var result = await _ownerService.GetOwnerDetailsByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.FirstName.Should().Be("John");
        result.LastName.Should().Be("Smith");
        result.Address.Should().Be("123 Main St");
        result.City.Should().Be("Anytown");
        result.Telephone.Should().Be("555-1234");
    }

    [Fact]
    public async Task GetOwnerDetailsByIdAsync_WithInvalidId_ReturnsNull()
    {
        // Act
        var result = await _ownerService.GetOwnerDetailsByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateOwnerAsync_WithValidData_CreatesAndReturnsOwner()
    {
        // Arrange
        var newOwnerDto = new OwnerDto
        {
            FirstName = "Alice",
            LastName = "Brown",
            Address = "321 Elm St",
            City = "Newtown",
            Telephone = "555-3456"
        };

        // Act
        var result = await _ownerService.CreateOwnerAsync(newOwnerDto);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
        result.FirstName.Should().Be("Alice");
        result.LastName.Should().Be("Brown");
        result.Address.Should().Be("321 Elm St");
        result.City.Should().Be("Newtown");
        result.Telephone.Should().Be("555-3456");

        // Verify in database
        var ownerInDb = await _context.Owners.FindAsync(result.Id);
        ownerInDb.Should().NotBeNull();
        ownerInDb!.FirstName.Should().Be("Alice");
    }

    [Fact]
    public async Task UpdateOwnerAsync_WithValidData_UpdatesAndReturnsOwner()
    {
        // Arrange
        var updateOwnerDto = new OwnerDto
        {
            Id = 1,
            FirstName = "John Updated",
            LastName = "Smith Updated",
            Address = "123 Main St Updated",
            City = "Anytown Updated",
            Telephone = "555-1234"
        };

        // Act
        var result = await _ownerService.UpdateOwnerAsync(updateOwnerDto);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.FirstName.Should().Be("John Updated");
        result.LastName.Should().Be("Smith Updated");
        result.Address.Should().Be("123 Main St Updated");
        result.City.Should().Be("Anytown Updated");

        // Verify in database
        var ownerInDb = await _context.Owners.FindAsync(1);
        ownerInDb.Should().NotBeNull();
        ownerInDb!.FirstName.Should().Be("John Updated");
    }

    [Fact]
    public async Task UpdateOwnerAsync_WithInvalidId_ReturnsNull()
    {
        // Arrange
        var updateOwnerDto = new OwnerDto
        {
            Id = 999,
            FirstName = "John",
            LastName = "Smith",
            Address = "123 Main St",
            City = "Anytown",
            Telephone = "555-1234"
        };

        // Act
        var result = await _ownerService.UpdateOwnerAsync(updateOwnerDto);

        // Assert
        result.Should().BeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task GetOwnersByLastNameAsync_WithEmptyOrNullFilter_ReturnsAllOwners(string? lastName)
    {
        // Act
        var result = await _ownerService.GetOwnersByLastNameAsync(lastName);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
    }

    [Fact]
    public async Task GetOwnersByLastNameAsync_WithWhitespaceFilter_ReturnsNoResults()
    {
        // The service treats whitespace as a filter, not as null
        // Act
        var result = await _ownerService.GetOwnersByLastNameAsync("   ");

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetOwnersByLastNameAsync_WithCaseInsensitiveFilter_ReturnsCorrectOwners()
    {
        // Act
        var result = await _ownerService.GetOwnersByLastNameAsync("smith");

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.All(o => o.LastName.StartsWith("Smith")).Should().BeTrue();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}