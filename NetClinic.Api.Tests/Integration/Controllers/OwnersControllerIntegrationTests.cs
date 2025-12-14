using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using NetClinic.Api.Dto;
using Xunit;

namespace NetClinic.Api.Tests.Integration.Controllers;

public class OwnersControllerIntegrationTests : BaseIntegrationTest
{
    [Fact]
    public async Task Get_ShouldReturnOwnerList_WhenNoFilters()
    {
        // Act
        var response = await HttpClient.GetAsync("/owners");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var ownerList = await response.Content.ReadFromJsonAsync<OwnerListDto>();
        ownerList.Should().NotBeNull();
        ownerList!.OwnerList.Should().HaveCount(4);
        ownerList.TotalPages.Should().Be(1);
    }

    [Fact]
    public async Task Get_ShouldFilterByLastName_WhenLastNameProvided()
    {
        // Act
        var response = await HttpClient.GetAsync("/owners?lastName=Smith");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var ownerList = await response.Content.ReadFromJsonAsync<OwnerListDto>();
        ownerList.Should().NotBeNull();
        ownerList!.OwnerList.Should().HaveCount(1);
        ownerList.OwnerList.First().LastName.Should().Be("Smith");
    }

    [Fact]
    public async Task Get_ShouldRespectPagination_WhenPageSizeIsSmall()
    {
        // Act
        var response = await HttpClient.GetAsync("/owners?pageSize=2");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var ownerList = await response.Content.ReadFromJsonAsync<OwnerListDto>();
        ownerList.Should().NotBeNull();
        ownerList!.OwnerList.Should().HaveCount(2);
        ownerList.TotalPages.Should().Be(2);
    }

    [Fact]
    public async Task Get_ShouldReturnSecondPage_WhenPageTwoRequested()
    {
        // Act
        var response = await HttpClient.GetAsync("/owners?page=2&pageSize=2");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var ownerList = await response.Content.ReadFromJsonAsync<OwnerListDto>();
        ownerList.Should().NotBeNull();
        ownerList!.OwnerList.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetOwnerDetailsById_ShouldReturnOwner_WhenOwnerExists()
    {
        // Get the first owner ID dynamically
        var ownersResponse = await HttpClient.GetAsync("/owners");
        var ownerList = await ownersResponse.Content.ReadFromJsonAsync<OwnerListDto>();
        var firstOwner = ownerList!.OwnerList.First();

        // Act
        var response = await HttpClient.GetAsync($"/owners/{firstOwner.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var owner = await response.Content.ReadFromJsonAsync<OwnerDto>();
        owner.Should().NotBeNull();
        owner!.Id.Should().Be(firstOwner.Id);
        owner.FirstName.Should().Be("John");
        owner.LastName.Should().Be("Doe");
    }

    [Fact]
    public async Task GetOwnerDetailsById_ShouldReturnNotFound_WhenOwnerDoesNotExist()
    {
        // Act
        var response = await HttpClient.GetAsync("/owners/999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Post_ShouldCreateOwner_WhenValidDataProvided()
    {
        // Arrange
        var newOwner = new OwnerDto
        {
            FirstName = "Alice",
            LastName = "Brown",
            Address = "789 Oak St",
            City = "Testville",
            Telephone = "5550000005"
        };

        // Act
        var response = await HttpClient.PostAsJsonAsync("/owners", newOwner);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var createdOwner = await response.Content.ReadFromJsonAsync<OwnerDto>();
        createdOwner.Should().NotBeNull();
        createdOwner!.FirstName.Should().Be("Alice");
        createdOwner.LastName.Should().Be("Brown");
        createdOwner.Id.Should().BeGreaterThan(0);

        // Verify it was actually created in the database
        var dbOwner = await GetFromDatabaseAsync(async context => 
            await context.Owners.FindAsync(createdOwner.Id));
        dbOwner.Should().NotBeNull();
        dbOwner!.FirstName.Should().Be("Alice");
    }

    [Fact]
    public async Task Post_ShouldReturnBadRequest_WhenInvalidDataProvided()
    {
        // Arrange
        var invalidOwner = new OwnerDto
        {
            FirstName = "", // Invalid - empty
            LastName = "Brown",
            Address = "789 Oak St",
            City = "Testville",
            Telephone = "5550000005"
        };

        // Act
        var response = await HttpClient.PostAsJsonAsync("/owners", invalidOwner);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Put_ShouldUpdateOwner_WhenValidDataProvided()
    {
        // Arrange
        var updatedOwner = new OwnerDto
        {
            FirstName = "Johnny",
            LastName = "Doe",
            Address = "456 Updated St",
            City = "Updated City",
            Telephone = "5559999999"
        };

        // Get the first owner ID dynamically
        var ownersResponse = await HttpClient.GetAsync("/owners");
        var ownerList = await ownersResponse.Content.ReadFromJsonAsync<OwnerListDto>();
        var firstOwner = ownerList!.OwnerList.First();

        // Act
        var response = await HttpClient.PutAsJsonAsync($"/owners/{firstOwner.Id}", updatedOwner);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var ownerDto = await response.Content.ReadFromJsonAsync<OwnerDto>();
        ownerDto.Should().NotBeNull();
        ownerDto!.FirstName.Should().Be("Johnny");
        ownerDto.Address.Should().Be("456 Updated St");

        // Verify it was actually updated in the database
        var dbOwner = await GetFromDatabaseAsync(async context => 
            await context.Owners.FindAsync(firstOwner.Id));
        dbOwner.Should().NotBeNull();
        dbOwner!.FirstName.Should().Be("Johnny");
        dbOwner.Address.Should().Be("456 Updated St");
    }

    [Fact]
    public async Task Put_ShouldReturnNotFound_WhenOwnerDoesNotExist()
    {
        // Arrange
        var updatedOwner = new OwnerDto
        {
            FirstName = "Johnny",
            LastName = "Doe",
            Address = "456 Updated St",
            City = "Updated City",
            Telephone = "5559999999"
        };

        // Act
        var response = await HttpClient.PutAsJsonAsync("/owners/999", updatedOwner);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Post_ShouldReturnBadRequest_WhenTelephoneIsInvalid()
    {
        // Arrange
        var invalidOwner = new OwnerDto
        {
            FirstName = "Alice",
            LastName = "Brown",
            Address = "789 Oak St",
            City = "Testville",
            Telephone = "invalid-phone" // Invalid phone number
        };

        // Act
        var response = await HttpClient.PostAsJsonAsync("/owners", invalidOwner);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        
        var errorResponse = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        errorResponse.Should().NotBeNull();
        errorResponse!.Should().ContainKey("telephone");
    }
}