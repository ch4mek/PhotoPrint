using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TeamboardHttpClientExtensions;
using Modsen.Portal.TeamBoard.Controllers.Contracts.Input;
using Modsen.Portal.TeamBoard.Controllers.Contracts.Output;

namespace Modsen.Portal.IntegrationTests.TeamBoard.EmploymentHistory;

[Collection("GeneralCollection")]
public class EmploymentHistoryIntegrationTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly JsonSerializerOptions _serializerOptions;

    public EmploymentHistoryIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
        _serializerOptions = _jsonOptions;
    }

    [Fact]
    public async Task GetEmploymentHistoryWhenTeamBoardUserDoesNotExistResponseWithNotFoundResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var userId = 9999;

        // Act
        var response = await client.GetEmploymentHistory(userId);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetEmploymentHistoryWhenRequestIsValidResponseWithOk()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var userId = _factory.EmployeeUserId!.Value;

        // Act
        var response = await client.GetEmploymentHistory(userId);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdateEmploymentHistoryWhenTeamBoardUserDoesNotExistResponseWithNotFoundResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var userId = 9999;
        var body = new UpdateEmploymentHistoryInput(userId, null, null);

        // Act
        var response = await client.UpdateEmploymentHistory(body);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateEmploymentHistoryWhenAuthAsEmployeeResponseWithForbiddenResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsEmployee();

        var userId = _factory.EmployeeUserId!.Value;
        var body = new UpdateEmploymentHistoryInput(userId, null, null);

        // Act
        var response = await client.UpdateEmploymentHistory(body);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UpdateEmploymentHistoryWhenRequestIsValidResponseWithOk()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var userId = _factory.EmployeeUserId!.Value;
        var body = new UpdateEmploymentHistoryInput(
            userId,
            new DateOnly(2020, 1, 3),
            null);

        // Act
        var response = await client.UpdateEmploymentHistory(body);

        var result = await response.Content.ReadFromJsonAsync<EmploymentHistoryOutput>(_serializerOptions, _cancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.ContractEndDate.Should().Be(null);
        result.ProfessionalActivityStartDate.Should().Be(body.ProfessionalActivityStartDate);
        result.FirstProjectStartDate.Should().Be(null);
    }
}
