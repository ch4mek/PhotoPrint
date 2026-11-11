using System.Text.Json;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
namespace Modsen.Portal.IntegrationTests.Identity.Controllers.RedirectRulesController;

[Collection("GeneralCollection")]
public class RedirectRulesControllerIntegrationTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;

    public RedirectRulesControllerIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
    }

    [Fact]
    public async Task GetRedirectRuleWhenItExistsShouldReturnOkResult()
    {
        //Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        const string redirectTo = "https://test.net";

        var redirectRule = await client.CreateRedirectRule(redirectTo: redirectTo);

        //Act
        await client.AuthAsEmployee();

        var receivedRedirectRule = await client.GetRedirectRule(redirectRule!.RedirectKey);


        //Assert
        receivedRedirectRule.Should().NotBeNull();
        receivedRedirectRule.Should().BeEquivalentTo(redirectRule);
    }
}
