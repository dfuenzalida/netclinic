using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using NetClinic.Api.Dto;
using Xunit;

namespace NetClinic.Api.Tests.Integration.Controllers;

public class PetsControllerIntegrationTests : BaseIntegrationTest
{
    [Fact]
    public async Task GetPetsByOwnerId_ShouldReturnPets_WhenOwnerHasPets()
    {
        // Act - Owner 1 has 2 pets (Buddy and Whiskers)
        var response = await HttpClient.GetAsync("/api/owners/1/pets");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var pets = await response.Content.ReadFromJsonAsync<IEnumerable<PetDto>>();
        pets.Should().NotBeNull();
        pets!.Should().HaveCount(2);
        
        var petNames = pets.Select(p => p.Name).ToList();
        petNames.Should().Contain("Buddy");
        petNames.Should().Contain("Whiskers");
    }

    [Fact]
    public async Task GetPetsByOwnerId_ShouldReturnNotFound_WhenOwnerHasNoPets()
    {
        // First create a new owner with no pets
        var newOwner = new OwnerDto
        {
            FirstName = "TestOwner",
            LastName = "NoPets",
            Address = "123 Test St",
            City = "Test City",
            Telephone = "1234567890"
        };
        
        var ownerResponse = await HttpClient.PostAsJsonAsync("/api/owners", newOwner);
        ownerResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdOwner = await ownerResponse.Content.ReadFromJsonAsync<OwnerDto>();
        
        // Act
        var response = await HttpClient.GetAsync($"/api/owners/{createdOwner!.Id}/pets");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetPetById_ShouldReturnPet_WhenPetExists()
    {
        // Get the first pet ID dynamically 
        var petsResponse = await HttpClient.GetAsync("/api/owners/1/pets");
        var pets = await petsResponse.Content.ReadFromJsonAsync<IEnumerable<PetDto>>();
        var firstPet = pets!.First();

        // Act - Get pet (Buddy) for owner 1
        var response = await HttpClient.GetAsync($"/api/owners/1/pets/{firstPet.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var pet = await response.Content.ReadFromJsonAsync<PetDto>();
        pet.Should().NotBeNull();
        pet!.Id.Should().Be(firstPet.Id);
        pet.Name.Should().Be("Buddy");
        pet.Type.Should().Be("Dog");
    }

    [Fact]
    public async Task GetPetById_ShouldReturnNotFound_WhenPetDoesNotExist()
    {
        // Act
        var response = await HttpClient.GetAsync("/api/owners/1/pets/999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetAllPetTypes_ShouldReturnAllPetTypes()
    {
        // Act
        var response = await HttpClient.GetAsync("/api/pet/types");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var petTypes = await response.Content.ReadFromJsonAsync<IEnumerable<PetTypeDto>>();
        petTypes.Should().NotBeNull();
        petTypes!.Should().HaveCount(4); // Dog, Cat, Bird, Hamster from our seed data
        
        var typeNames = petTypes.Select(pt => pt.Name).ToList();
        typeNames.Should().Contain("Dog");
        typeNames.Should().Contain("Cat");
        typeNames.Should().Contain("Bird");
        typeNames.Should().Contain("Hamster");
    }

    [Fact]
    public async Task GetVisitsByPetId_ShouldReturnEmptyList_WhenPetHasNoVisits()
    {
        // Get the first pet ID dynamically 
        var petsResponse = await HttpClient.GetAsync("/api/owners/1/pets");
        var pets = await petsResponse.Content.ReadFromJsonAsync<IEnumerable<PetDto>>();
        var firstPet = pets!.First();

        // Act - Pet (Buddy) has no visits in our seed data
        var response = await HttpClient.GetAsync($"/api/owners/1/pets/{firstPet.Id}/visits");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var visits = await response.Content.ReadFromJsonAsync<IEnumerable<VisitDto>>();
        visits.Should().NotBeNull();
        visits!.Should().BeEmpty();
    }

    [Fact]
    public async Task CreateVisit_ShouldCreateVisit_WhenValidDataProvided()
    {
        // Arrange
        var newVisit = new VisitDto
        {
            VisitDate = "2023-12-13T10:00:00Z",
            Description = "Annual checkup"
        };

        // Get the first pet ID dynamically 
        var petsResponse = await HttpClient.GetAsync("/api/owners/1/pets");
        var pets = await petsResponse.Content.ReadFromJsonAsync<IEnumerable<PetDto>>();
        var firstPet = pets!.First();

        // Act
        var response = await HttpClient.PostAsJsonAsync($"/api/owners/1/pets/{firstPet.Id}/visits", newVisit);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var createdVisit = await response.Content.ReadFromJsonAsync<VisitDto>();
        createdVisit.Should().NotBeNull();
        createdVisit!.Description.Should().Be("Annual checkup");
        createdVisit.Id.Should().BeGreaterThan(0);

        // Verify the visit was created by trying to get visits for the pet
        var getVisitsResponse = await HttpClient.GetAsync($"/api/owners/1/pets/{firstPet.Id}/visits");
        getVisitsResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var visits = await getVisitsResponse.Content.ReadFromJsonAsync<IEnumerable<VisitDto>>();
        visits.Should().NotBeNull();
        visits!.Should().HaveCount(1);
        visits.First().Description.Should().Be("Annual checkup");
    }

    [Fact]
    public async Task CreateVisit_ShouldReturnBadRequest_WhenInvalidDataProvided()
    {
        // Arrange
        var invalidVisit = new VisitDto
        {
            VisitDate = "", // Invalid - empty
            Description = ""  // Invalid - empty
        };

        // Act
        // Get the first pet ID dynamically 
        var petsResponse = await HttpClient.GetAsync("/api/owners/1/pets");
        var pets = await petsResponse.Content.ReadFromJsonAsync<IEnumerable<PetDto>>();
        var firstPet = pets!.First();

        var response = await HttpClient.PostAsJsonAsync($"/api/owners/1/pets/{firstPet.Id}/visits", invalidVisit);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        
        var errorResponse = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        errorResponse.Should().NotBeNull();
        errorResponse!.Should().ContainKey("description");
        // Note: The validation might only return description error, let's check what the API actually returns
    }

    [Fact]
    public async Task GetPetsByOwnerId_ShouldHandleDifferentOwners()
    {
        // Act - Test different owners
        var owner2Response = await HttpClient.GetAsync("/api/owners/2/pets");
        var owner3Response = await HttpClient.GetAsync("/api/owners/3/pets");

        // Assert
        owner2Response.StatusCode.Should().Be(HttpStatusCode.OK);
        owner3Response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var owner2Pets = await owner2Response.Content.ReadFromJsonAsync<IEnumerable<PetDto>>();
        var owner3Pets = await owner3Response.Content.ReadFromJsonAsync<IEnumerable<PetDto>>();
        
        owner2Pets.Should().NotBeNull();
        owner2Pets!.Should().HaveCount(1); // Owner 2 has 1 pet (Max)
        owner2Pets.First().Name.Should().Be("Max");
        
        owner3Pets.Should().NotBeNull();
        owner3Pets!.Should().HaveCount(1); // Owner 3 has 1 pet (Luna)
        owner3Pets.First().Name.Should().Be("Luna");
    }

    [Fact]
    public async Task GetPetById_ShouldReturnCorrectPetType()
    {
        // Get pets for owner 1 and find the cat (Whiskers)
        var petsResponse = await HttpClient.GetAsync("/api/owners/1/pets");
        var pets = await petsResponse.Content.ReadFromJsonAsync<IEnumerable<PetDto>>();
        var catPet = pets!.First(p => p.Type == "Cat");

        // Act - Get cat pet (Whiskers)
        var response = await HttpClient.GetAsync($"/api/owners/1/pets/{catPet.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var pet = await response.Content.ReadFromJsonAsync<PetDto>();
        pet.Should().NotBeNull();
        pet!.Name.Should().Be("Whiskers");
        pet.Type.Should().Be("Cat");
        pet.BirthDate.Should().NotBeNullOrEmpty();
    }
}