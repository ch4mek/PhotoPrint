using System.Net;
using System.Text.Json;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Cv;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
namespace Modsen.Portal.IntegrationTests.Cv.CvProjects;

[Collection("CvCollection")]
public class CvProjectsControllerIntegrationTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;

    public CvProjectsControllerIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
    }

    [Fact]
    public async Task GetCvProjectsLookupWhenUserHasManageCvsClaimResponseWithOkResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        // Act
        var response = await client.GetCvProjectsLookupAsHttpMessage();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetCvProjectsLookupWhenUserHasNoManageCvsClaimResponseWithForbiddenResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsEmployee();

        // Act
        var response = await client.GetCvProjectsLookupAsHttpMessage();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetCvProjectsLookupWhenUserNotAuthenticatedResponseWithUnauthorizedResult()
    {
        // Arrange
        using var client = _factory.CreateClient();

        // Act
        var response = await client.GetCvProjectsLookupAsHttpMessage();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
