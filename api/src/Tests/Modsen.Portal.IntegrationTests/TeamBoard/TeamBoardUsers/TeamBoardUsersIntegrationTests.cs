using System.Net;
using System.Text.Json;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TeamboardHttpClientExtensions;
using Modsen.Portal.TeamBoard.Requests.Users.Requests.RequestDto;
namespace Modsen.Portal.IntegrationTests.TeamBoard.TeamBoardUsers;

[Collection("GeneralCollection")]
public class TeamBoardUsersIntegrationTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;

    public TeamBoardUsersIntegrationTests(IntegrationTestFactory factory)
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
    public async Task GetOdpTeamBoardUserWhenRequestIsInvalidResponseWithBadRequest(string query)
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        // Act
        var response = await client.GetTeamboardUsersFromQuery(query);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Theory]
    [InlineData("?top=20&skip=0")]
    [InlineData("?top=10&skip=50")]
    [InlineData("?top=50&skip=50&orderby=Id")]
    [InlineData("?top=20&skip=0&filter=FirstName eq 'System'")]
    [InlineData("?top=10&skip=0&filter=odpNonPrimarySpecializations/any(s: s/name eq 'string')")]
    [InlineData("?top=20&skip=0&orderby=Id&filter=date(careerStartedAt) ge 2020-05-10 and date(careerStartedAt) le 2023-05-10")]
    [InlineData("?top=20&skip=0&orderby=FirstName&filter=seniorityLevelId eq 'SeniorL1'")]
    [InlineData("?top=10&skip=50&filter=position eq 'Employee' and location eq 'Vitebsk'")]
    [InlineData("?top=10&skip=50&filter=departmentId eq 'Net'")]
    [InlineData("?top=10&skip=50&orderby=departmentOrderWeight asc")]
    public async Task GetOdpTeamBoardUserWhenRequestIsValidResponseWithOk(string query)
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        // Act
        var response = await client.GetTeamboardUsersFromQuery(query);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetOdpTeamBoardUserWhenAuthAsEmployeeResponseWithForbidden()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsEmployee();

        // Act
        var response = await client.GetTeamboardUsersFromQuery("");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetTeamBoardLookupWhenRequestIsValidResponseWithOk()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        // Act
        var response = await client.GetTeamboardUsersLookup()
            .WithResponseType<List<TeamBoardUserRequestDto>>();

        // Assert
        response.Count.Should().BeGreaterThan(0);
        response[0].Should().NotBeNull();
        response[0].FirstName.Should().NotBeEmpty();
        response[0].LastName.Should().NotBeEmpty();
    }

    [Theory]
    [InlineData("")]
    [InlineData("?hasInterviews=true")]
    [InlineData("?onlyActive=true")]
    [InlineData("?asInterviews=true&onlyActive=true")]
    public async Task GetSaleManagersLookupWhenRequestIsValidResponseWithOk(string query)
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        // Act
        var response = await client.GetSaleManagersLookup(query);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Theory]
    [InlineData("")]
    [InlineData("?onlyActive=true")]
    [InlineData("?onlyActive=false")]
    public async Task GetInterviewUsersLookupWhenRequestIsValidResponseWithOk(string query)
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        // Act
        var response = await client.GetInterviewUsersLookup(query);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
