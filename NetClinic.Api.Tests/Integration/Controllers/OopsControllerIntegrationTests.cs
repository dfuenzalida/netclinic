using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace NetClinic.Api.Tests.Integration.Controllers;

public class OopsControllerIntegrationTests : BaseIntegrationTest
{
    [Fact]
    public async Task Get_ShouldThrowExpectedExceptionMessage_WhenCalled()
    {
        // Act & Assert - Verify the specific exception message
        await FluentActions.Invoking(async () => await HttpClient.GetAsync("/api/oops"))
            .Should().ThrowAsync<System.Exception>()
            .WithMessage("Expected: controller used to showcase what happens when an exception is thrown");
    }
}