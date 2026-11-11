using System.Net;
using System.Text.Json;
using Modsen.Portal.Identity.Controllers.Contracts.Output;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
namespace Modsen.Portal.IntegrationTests.Identity.Controllers;

[Collection("GeneralCollection")]
public class GetSignatureIntegrationTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;

    public GetSignatureIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
    }

    [Fact]
    public async Task GetUserSignatureWhenNotAuthorizedShouldReturnNotAuthorizedResponse()
    {
        // Arrange
        using var client = _factory.CreateClient();

        // Act
        var response = await client.GetCurrentUserSignatureAsHttpMessage();

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetUserSignatureWhenAuthAsEmployeeWithNoSignatureShouldReturnDefaultSignature()
    {
        // Arrange
        var expectedResponse = new UserSignatureOutput
        {
            Email = EmailConstants.EmployeeEmail,
            FullName = "Employee Test",
            Position = null
        };

        using var client = _factory.CreateClient();

        // Act
        await client.AuthAsEmployee();
        var getSignatureResponse = await client.GetCurrentUserSignature();

        // Assert
        getSignatureResponse.Should().NotBeNull();
        getSignatureResponse.Should().BeEquivalentTo(expectedResponse);
    }
}
