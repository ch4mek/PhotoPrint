using System.Net;
using System.Text.Json;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TeamboardHttpClientExtensions;
namespace Modsen.Portal.IntegrationTests.TeamBoard.PerformanceReview;

[Collection("GeneralCollection")]
public class GetPerformanceReviewIntegrationTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public GetPerformanceReviewIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
    }

    [Theory]
    [InlineData("")]
    [InlineData("?")]
    [InlineData("?top=50")]
    [InlineData("?skip=0")]
    [InlineData("?top=0&skip=50")]
    [InlineData("?top=50&skip=-1")]
    [InlineData("?top=50&skip=15")]
    [InlineData("?top=26&skip=50")]
    [InlineData("?top=50&skip=50&filter=Id eq ll")]
    public async Task GetOdpTeamBoardUserWithPerformanceReviewWhenRequestIsInvalidResponseWithBadRequest(string query)
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        // Act
        var response = await client.GetAsync(
            $"api/performance-review{query}",
            _cancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Theory]
    [InlineData("?top=20&skip=0")]
    [InlineData("?top=10&skip=50")]
    [InlineData("?top=50&skip=50&orderby=Id")]
    [InlineData("?top=20&skip=0&overdueDaysCount=7")]
    [InlineData("?top=10&skip=0&overdueDaysCount=7&filter=fullName eq 'Userovich User'")]
    [InlineData("?top=20&skip=0&overdueDaysCount=7&year=2024")]
    [InlineData("?top=20&skip=0&overdueDaysCount=7&monthOfYear=6")]
    public async Task GetOdpTeamBoardUserWithPerformanceReviewWhenRequestIsValidResponseWithOk(string query)
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        // Act
        var response = await client.GetPerfomanceReview(query);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetOdpTeamBoardUserWithPerformanceReviewWhenAuthAsEmployeeResponseWithForbidden()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsEmployee();

        // Act
        var response = await client.GetPerfomanceReview("");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
