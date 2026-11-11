using System.Net;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.IntegrationTests.Mocks;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
namespace Modsen.Portal.IntegrationTests.Identity.Controllers.AccountController;

[CollectionDefinition("IpLogoutFeatureCollection", DisableParallelization = true)]
public class AccountControllerIntegrationTests : IClassFixture<IntegrationTestFactory>
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;

    public AccountControllerIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
    }

    [Fact(Skip = "For Staging or Prod env")]
    public async Task LoginWhenIpAddressChangedShouldTurnOffFirstSession()
    {
        // Arrange
        using var phoneClient = _factory.CreateClient();
        using var laptopClient = _factory.CreateClient();
        using var scope = _factory.Services.CreateScope();

        var phoneIp = "127.1.1.1";
        var laptopIp = "127.2.2.2";

        var executionService = scope.ServiceProvider.GetRequiredService<RequestExecutionContextMock>();

        // Act-Assert
        executionService.ChangeMockIpAddress(phoneIp);
        await phoneClient.AuthAsCeo();

        executionService.ChangeMockIpAddress(laptopIp);
        await laptopClient.AuthAsCeo();

        executionService.ChangeMockIpAddress(phoneIp);
        var currentUserResponsePhone = await phoneClient.GetCurrentUserAsHttpMessage();
        var response = await currentUserResponsePhone.Content.ReadAsStringAsync();
        response.Should().Contain("Another user (ip address) has signed into your account.");

        currentUserResponsePhone.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        await phoneClient.LogoutAsHttpMessage();

        executionService.ChangeMockIpAddress(laptopIp);
        var currentUserResponseLaptop = await laptopClient.GetCurrentUserAsHttpMessage();
        currentUserResponseLaptop.IsSuccessStatusCode.Should().BeTrue();
        currentUserResponseLaptop.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact(Skip = "For Staging or Prod env")]
    public async Task LoginWhenIpAddressNotChangedShouldTurnOffSecondSession()
    {
        // Arrange
        using var chromeClient = _factory.CreateClient();
        using var firefoxClient = _factory.CreateClient();
        using var scope = _factory.Services.CreateScope();

        scope.ServiceProvider
            .GetRequiredService<RequestExecutionContextMock>()
            .ChangeMockIpAddress("127.1.1.1");

        // Act-Assert
        await chromeClient.AuthAsCeo();
        await firefoxClient.AuthAsCeo();

        var chromeResponse = await chromeClient.GetCurrentUserAsHttpMessage();
        chromeResponse.IsSuccessStatusCode.Should().BeFalse();
        chromeResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        var firefoxResponse = await firefoxClient.GetCurrentUserAsHttpMessage();
        firefoxResponse.IsSuccessStatusCode.Should().BeTrue();
        firefoxResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
