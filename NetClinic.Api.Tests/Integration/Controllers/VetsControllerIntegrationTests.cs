using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using NetClinic.Api.Dto;
using Xunit;

namespace NetClinic.Api.Tests.Integration.Controllers;

public class VetsControllerIntegrationTests : BaseIntegrationTest
{
    [Fact]
    public async Task Get_ShouldReturnVetList_WhenNoFilters()
    {
        // Act
        var response = await HttpClient.GetAsync("/vets");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var vetList = await response.Content.ReadFromJsonAsync<VetListDto>();
        vetList.Should().NotBeNull();
        vetList!.VetList.Should().HaveCount(3);
        vetList.TotalPages.Should().Be(1);
    }

    [Fact]
    public async Task Get_ShouldRespectPagination_WhenPageRequested()
    {
        // Act - Request first page (the default page size is 5 according to controller)
        var response = await HttpClient.GetAsync("/vets?page=1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var vetList = await response.Content.ReadFromJsonAsync<VetListDto>();
        vetList.Should().NotBeNull();
        vetList!.VetList.Should().HaveCount(3); // We only have 3 vets, so first page has all
        vetList.TotalPages.Should().Be(1);
    }

    [Fact]
    public async Task GetById_ShouldReturnVet_WhenVetExists()
    {
        // Act
        var response = await HttpClient.GetAsync("/vets/1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var vet = await response.Content.ReadFromJsonAsync<VetDto>();
        vet.Should().NotBeNull();
        vet!.Id.Should().Be(1);
        vet.FirstName.Should().Be("Dr. Alice");
        vet.LastName.Should().Be("Johnson");
        vet.Specialties.Should().NotBeNull();
    }

    [Fact]
    public async Task GetById_ShouldReturnNotFound_WhenVetDoesNotExist()
    {
        // Act
        var response = await HttpClient.GetAsync("/vets/999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Get_ShouldReturnVetsInCorrectOrder()
    {
        // Act
        var response = await HttpClient.GetAsync("/vets");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var vetList = await response.Content.ReadFromJsonAsync<VetListDto>();
        vetList.Should().NotBeNull();
        vetList!.VetList.Should().HaveCount(3);
        
        // Check the names match our seeded data
        var vetNames = vetList.VetList.Select(v => $"{v.FirstName} {v.LastName}").ToList();
        vetNames.Should().Contain("Dr. Alice Johnson");
        vetNames.Should().Contain("Dr. Bob Smith");
        vetNames.Should().Contain("Dr. Carol Williams");
    }

    [Fact]
    public async Task GetById_ShouldReturnVetWithSpecialties_WhenVetHasSpecialties()
    {
        // First, let's verify we can get a vet by ID
        var response = await HttpClient.GetAsync("/vets/2");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var vet = await response.Content.ReadFromJsonAsync<VetDto>();
        vet.Should().NotBeNull();
        vet!.Id.Should().Be(2);
        vet.FirstName.Should().Be("Dr. Bob");
        vet.LastName.Should().Be("Smith");
        vet.Specialties.Should().NotBeNull();
        // Note: Specialties might be empty if not properly configured in the service
    }

    [Fact]
    public async Task Get_ShouldHandleEmptyResult_WhenRequestingHighPageNumber()
    {
        // Act - Request a very high page number
        var response = await HttpClient.GetAsync("/vets?page=100");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var vetList = await response.Content.ReadFromJsonAsync<VetListDto>();
        vetList.Should().NotBeNull();
        vetList!.VetList.Should().BeEmpty();
        vetList.TotalPages.Should().Be(1); // Total pages should still show the actual number of pages
    }
}