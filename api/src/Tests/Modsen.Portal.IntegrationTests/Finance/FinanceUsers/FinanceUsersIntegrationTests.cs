using System.Net;
using System.Text.Json;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Finance;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;

namespace Modsen.Portal.IntegrationTests.Finance.FinanceUsers;

[Collection("GeneralCollection")]

public class FinanceUsersIntegrationTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public FinanceUsersIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
    }

    [Fact]
    public async Task GetFinanceUsersLookupWhenRequestValidResponseWithNotDeletedCategories()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        // Act
        var response = await client.FinanceUsersLookupAsResponseMessageAsync(2026, 6);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
