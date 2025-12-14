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

public class VetServiceTests : IDisposable
{
    private readonly NetClinicDbContext _context;
    private readonly Mock<ILogger<VetService>> _loggerMock;
    private readonly VetService _vetService;

    public VetServiceTests()
    {
        var options = new DbContextOptionsBuilder<NetClinicDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new NetClinicDbContext(options);
        _loggerMock = new Mock<ILogger<VetService>>();
        _vetService = new VetService(_context, _loggerMock.Object);

        SeedTestData();
    }

    private void SeedTestData()
    {
        // Create specialties
        var cardiologySpecialty = new Specialty { Id = 1, Name = "Cardiology" };
        var surgerySpecialty = new Specialty { Id = 2, Name = "Surgery" };
        var dermatologySpecialty = new Specialty { Id = 3, Name = "Dermatology" };

        _context.Add(cardiologySpecialty);
        _context.Add(surgerySpecialty);
        _context.Add(dermatologySpecialty);

        // Create veterinarians
        var vets = new List<Veterinarian>
        {
            new Veterinarian
            {
                Id = 1,
                FirstName = "Dr. Sarah",
                LastName = "Johnson",
                Specialties = new List<Specialty> { cardiologySpecialty, surgerySpecialty }
            },
            new Veterinarian
            {
                Id = 2,
                FirstName = "Dr. Michael",
                LastName = "Brown",
                Specialties = new List<Specialty> { dermatologySpecialty }
            },
            new Veterinarian
            {
                Id = 3,
                FirstName = "Dr. Emily",
                LastName = "Davis",
                Specialties = new List<Specialty>() // No specialties
            }
        };

        _context.Veterinarians.AddRange(vets);
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetAllVeterinariansAsync_WithoutPagination_ReturnsAllVets()
    {
        // Act
        var result = await _vetService.GetAllVeterinariansAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        result.Select(v => v.FirstName).Should().Contain("Dr. Sarah", "Dr. Michael", "Dr. Emily");
    }

    [Fact]
    public async Task GetAllVeterinariansAsync_WithPagination_ReturnsCorrectPage()
    {
        // Act
        var result = await _vetService.GetAllVeterinariansAsync(page: 1, pageSize: 2);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetAllVeterinariansAsync_WithSecondPage_ReturnsRemainingVets()
    {
        // Act
        var result = await _vetService.GetAllVeterinariansAsync(page: 2, pageSize: 2);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetAllVeterinariansAsync_VetsHaveSpecialties_IncludesSpecialtyInformation()
    {
        // Act
        var result = await _vetService.GetAllVeterinariansAsync();

        // Assert
        result.Should().NotBeNull();
        
        var vetWithSpecialties = result.FirstOrDefault(v => v.Id == 1);
        vetWithSpecialties.Should().NotBeNull();
        vetWithSpecialties!.Specialties.Should().HaveCount(2);
        vetWithSpecialties.Specialties.Select(s => s.Name).Should().Contain("Cardiology", "Surgery");
    }

    [Fact]
    public async Task GetAllVeterinariansAsync_SpecialtiesAreOrdered_OrdersSpecialtiesAlphabetically()
    {
        // Act
        var result = await _vetService.GetAllVeterinariansAsync();

        // Assert
        result.Should().NotBeNull();
        
        var vetWithSpecialties = result.FirstOrDefault(v => v.Id == 1);
        vetWithSpecialties.Should().NotBeNull();
        vetWithSpecialties!.Specialties.Should().BeInAscendingOrder(s => s.Name);
    }

    [Fact]
    public async Task GetVeterinariansCountAsync_ReturnsCorrectCount()
    {
        // Act
        var result = await _vetService.GetVeterinariansCountAsync();

        // Assert
        result.Should().Be(3);
    }

    [Fact]
    public async Task GetVeterinarianByIdAsync_WithValidId_ReturnsVetDetails()
    {
        // Act
        var result = await _vetService.GetVeterinarianByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.FirstName.Should().Be("Dr. Sarah");
        result.LastName.Should().Be("Johnson");
        result.Specialties.Should().HaveCount(2);
        result.Specialties.Select(s => s.Name).Should().Contain("Cardiology", "Surgery");
    }

    [Fact]
    public async Task GetVeterinarianByIdAsync_WithInvalidId_ReturnsNull()
    {
        // Act
        var result = await _vetService.GetVeterinarianByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetVeterinarianByIdAsync_WithVetWithoutSpecialties_ReturnsVetWithEmptySpecialties()
    {
        // Act
        var result = await _vetService.GetVeterinarianByIdAsync(3);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(3);
        result.FirstName.Should().Be("Dr. Emily");
        result.LastName.Should().Be("Davis");
        result.Specialties.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllVeterinariansAsync_WithLargePageSize_ReturnsAllVets()
    {
        // Act
        var result = await _vetService.GetAllVeterinariansAsync(page: 1, pageSize: 100);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
    }

    [Fact]
    public async Task GetVeterinariansCountAsync_AfterAddingVet_ReturnsUpdatedCount()
    {
        // Arrange
        var newVet = new Veterinarian
        {
            Id = 4,
            FirstName = "Dr. New",
            LastName = "Veterinarian",
            Specialties = new List<Specialty>()
        };
        _context.Veterinarians.Add(newVet);
        await _context.SaveChangesAsync();

        // Act
        var result = await _vetService.GetVeterinariansCountAsync();

        // Assert
        result.Should().Be(4);
    }

    [Fact]
    public async Task GetVeterinarianByIdAsync_WithVetWithMultipleSpecialties_ReturnsOrderedSpecialties()
    {
        // Act
        var result = await _vetService.GetVeterinarianByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Specialties.Should().HaveCount(2);
        result.Specialties.Should().BeInAscendingOrder(s => s.Name);
    }

    [Theory]
    [InlineData(1, 1)]
    [InlineData(1, 2)]
    [InlineData(2, 1)]
    [InlineData(3, 1)]
    public async Task GetAllVeterinariansAsync_WithDifferentPaginationParameters_ReturnsCorrectCount(int page, int pageSize)
    {
        // Act
        var result = await _vetService.GetAllVeterinariansAsync(page, pageSize);

        // Assert
        result.Should().NotBeNull();
        
        var expectedCount = Math.Min(pageSize, Math.Max(0, 3 - (page - 1) * pageSize));
        result.Should().HaveCount(expectedCount);
    }

    [Fact]
    public async Task GetAllVeterinariansAsync_WithEmptyDatabase_ReturnsEmptyCollection()
    {
        // Arrange - Clear all data
        _context.Veterinarians.RemoveRange(_context.Veterinarians);
        await _context.SaveChangesAsync();

        // Act
        var result = await _vetService.GetAllVeterinariansAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllVeterinariansAsync_WithZeroPageSize_ReturnsEmptyResult()
    {
        // Act
        var result = await _vetService.GetAllVeterinariansAsync(page: 1, pageSize: 0);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetVeterinarianByIdAsync_MultipleCallsWithSameId_ReturnsSameResult()
    {
        // Act
        var result1 = await _vetService.GetVeterinarianByIdAsync(1);
        var result2 = await _vetService.GetVeterinarianByIdAsync(1);

        // Assert
        result1.Should().NotBeNull();
        result2.Should().NotBeNull();
        result1!.Id.Should().Be(result2!.Id);
        result1.FirstName.Should().Be(result2.FirstName);
        result1.LastName.Should().Be(result2.LastName);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}