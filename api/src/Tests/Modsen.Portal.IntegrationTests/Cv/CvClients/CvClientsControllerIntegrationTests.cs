using System.Net;
using System.Text.Json;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Cv;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
namespace Modsen.Portal.IntegrationTests.Cv.CvClients;

[Collection("CvCollection")]
public class CvClientsControllerIntegrationTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;

    public CvClientsControllerIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
    }

    [Fact]
    public async Task GetCvClientsLookupWhenUserHasManageCvsClaimResponseWithOkResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        // Act
        var response = await client.GetCvClientsLookupAsHttpMessage();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetCvClientsLookupWhenUserHasNoManageCvsClaimResponseWithForbiddenResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsEmployee();

        // Act
        var response = await client.GetCvClientsLookupAsHttpMessage();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetCvClientsLookupWhenUserNotAuthenticatedResponseWithUnauthorizedResult()
    {
        // Arrange
        using var client = _factory.CreateClient();

        // Act
        var response = await client.GetCvClientsLookupAsHttpMessage();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
