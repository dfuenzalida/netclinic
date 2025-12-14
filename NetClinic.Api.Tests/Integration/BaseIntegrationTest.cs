using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NetClinic.Api.Data;
using NetClinic.Api.Models;
using Testcontainers.PostgreSql;
using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace NetClinic.Api.Tests.Integration;

public abstract class BaseIntegrationTest : IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:15-alpine")
        .WithDatabase("netclinic_test")
        .WithUsername("test_user")
        .WithPassword("test_password")
        .WithCleanUp(true)
        .Build();

    protected WebApplicationFactory<Program> Factory { get; private set; } = null!;
    protected HttpClient HttpClient { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();

        Factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Testing");
                
                builder.ConfigureServices(services =>
                {
                    // Remove the existing DbContext registration
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<NetClinicDbContext>));
                    if (descriptor != null)
                        services.Remove(descriptor);

                    // Add DbContext using the test container
                    services.AddDbContext<NetClinicDbContext>(options =>
                    {
                        options.UseNpgsql(_dbContainer.GetConnectionString());
                    });

                    // Configure logging for tests
                    services.AddLogging(logging =>
                    {
                        logging.ClearProviders();
                        logging.AddConsole();
                        logging.SetMinimumLevel(LogLevel.Warning);
                    });
                });
            });

        HttpClient = Factory.CreateClient();

        // Initialize database with test data
        await SeedDatabaseAsync();
    }

    public async Task DisposeAsync()
    {
        HttpClient.Dispose();
        await Factory.DisposeAsync();
        await _dbContainer.DisposeAsync();
    }

    private async Task SeedDatabaseAsync()
    {
        using var scope = Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<NetClinicDbContext>();
        
        // Ensure database is created
        await context.Database.EnsureCreatedAsync();

        // Clear existing data in proper order (respecting foreign key constraints)
        var pets = context.Pets.ToList();
        context.Pets.RemoveRange(pets);
        await context.SaveChangesAsync();
        
        var owners = context.Owners.ToList();
        context.Owners.RemoveRange(owners);
        await context.SaveChangesAsync();
        
        var vets = context.Veterinarians.ToList();
        context.Veterinarians.RemoveRange(vets);
        await context.SaveChangesAsync();
        
        var petTypes = context.PetTypes.ToList();
        context.PetTypes.RemoveRange(petTypes);
        await context.SaveChangesAsync();

        // Reset identity sequences for PostgreSQL
        await context.Database.ExecuteSqlRawAsync("ALTER SEQUENCE owners_id_seq RESTART WITH 1");
        await context.Database.ExecuteSqlRawAsync("ALTER SEQUENCE vets_id_seq RESTART WITH 1");
        await context.Database.ExecuteSqlRawAsync("ALTER SEQUENCE pets_id_seq RESTART WITH 1");
        await context.Database.ExecuteSqlRawAsync("ALTER SEQUENCE types_id_seq RESTART WITH 1");

        // Seed test data - each step saves its own changes
        await SeedPetTypesAsync(context);
        await SeedVetsAsync(context);
        await SeedOwnersAsync(context);
        await SeedPetsAsync(context);
    }

    private static async Task SeedPetTypesAsync(NetClinicDbContext context)
    {
        var petTypes = new List<PetType>
        {
            new() { Name = "Dog" },
            new() { Name = "Cat" },
            new() { Name = "Bird" },
            new() { Name = "Hamster" }
        };

        await context.PetTypes.AddRangeAsync(petTypes);
        await context.SaveChangesAsync();
    }

    private static async Task SeedVetsAsync(NetClinicDbContext context)
    {
        var vets = new List<Veterinarian>
        {
            new()
            {
                FirstName = "Dr. Alice",
                LastName = "Johnson"
            },
            new()
            {
                FirstName = "Dr. Bob",
                LastName = "Smith"
            },
            new()
            {
                FirstName = "Dr. Carol",
                LastName = "Williams"
            }
        };

        await context.Veterinarians.AddRangeAsync(vets);
        await context.SaveChangesAsync();
    }

    private static async Task SeedOwnersAsync(NetClinicDbContext context)
    {
        var owners = new List<Owner>
        {
            new()
            {
                FirstName = "John",
                LastName = "Doe",
                Address = "123 Main St",
                City = "Anytown",
                Telephone = "555-0001"
            },
            new()
            {
                FirstName = "Jane",
                LastName = "Smith",
                Address = "456 Oak Ave",
                City = "Somewhere",
                Telephone = "555-0002"
            },
            new()
            {
                FirstName = "Michael",
                LastName = "Johnson",
                Address = "789 Pine St",
                City = "Elsewhere",
                Telephone = "555-0003"
            },
            new()
            {
                FirstName = "Sarah",
                LastName = "Williams",
                Address = "321 Elm Dr",
                City = "Nowhere",
                Telephone = "555-0004"
            }
        };

        await context.Owners.AddRangeAsync(owners);
        await context.SaveChangesAsync();
    }

    private static async Task SeedPetsAsync(NetClinicDbContext context)
    {
        var dogType = await context.PetTypes.FirstAsync(pt => pt.Name == "Dog");
        var catType = await context.PetTypes.FirstAsync(pt => pt.Name == "Cat");
        
        // Get the actual owner IDs that were generated
        var owners = await context.Owners.OrderBy(o => o.Id).ToListAsync();

        var pets = new List<Pet>
        {
            new()
            {
                Name = "Buddy",
                BirthDate = new DateTime(2020, 5, 15, 0, 0, 0, DateTimeKind.Utc),
                TypeId = dogType.Id,
                PetType = dogType,
                OwnerId = owners[0].Id  // John Doe
            },
            new()
            {
                Name = "Whiskers",
                BirthDate = new DateTime(2019, 3, 22, 0, 0, 0, DateTimeKind.Utc),
                TypeId = catType.Id,
                PetType = catType,
                OwnerId = owners[0].Id  // John Doe
            },
            new()
            {
                Name = "Max",
                BirthDate = new DateTime(2021, 1, 10, 0, 0, 0, DateTimeKind.Utc),
                TypeId = dogType.Id,
                PetType = dogType,
                OwnerId = owners[1].Id  // Jane Smith
            },
            new()
            {
                Name = "Luna",
                BirthDate = new DateTime(2018, 11, 5, 0, 0, 0, DateTimeKind.Utc),
                TypeId = catType.Id,
                PetType = catType,
                OwnerId = owners[2].Id  // Michael Johnson
            },
            new()
            {
                Name = "Rocky",
                BirthDate = new DateTime(2022, 7, 18, 0, 0, 0, DateTimeKind.Utc),
                TypeId = dogType.Id,
                PetType = dogType,
                OwnerId = owners[3].Id  // Sarah Williams
            }
        };

        await context.Pets.AddRangeAsync(pets);
        await context.SaveChangesAsync();
    }

    protected async Task<T> GetFromDatabaseAsync<T>(Func<NetClinicDbContext, Task<T>> selector)
    {
        using var scope = Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<NetClinicDbContext>();
        return await selector(context);
    }
}