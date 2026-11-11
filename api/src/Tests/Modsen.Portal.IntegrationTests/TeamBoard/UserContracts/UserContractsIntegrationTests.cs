using System.Net;
using System.Text.Json;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TeamboardHttpClientExtensions;
using Modsen.Portal.TeamBoard.Controllers.Contracts.Input;
using Modsen.Portal.TeamBoard.Controllers.Contracts.Output;
using Modsen.Portal.TeamBoard.Requests.Contracts.Requests.RequestDtos;
namespace Modsen.Portal.IntegrationTests.TeamBoard.UserContracts;

[Collection("GeneralCollection")]
public class UserContractsIntegrationTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;
    public UserContractsIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
    }

    [Fact]
    public async Task CreateUserContractWhenAuthAsCeoOrCtoAndDataIsValidResponseWithOkResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var userId = _factory.EmployeeUserId.Value;

        var input = new CreateUserContractInput(
            userId,
            DateOnly.FromDateTime(DateTime.Now).AddYears(-12),
            DateOnly.FromDateTime(DateTime.Now).AddYears(-11),
            null, null, null, null);
        // Act
        var response = await client.CreateContracts(input)
            .WithResponseType<UserContractOutput>();

        // Assert
        response.UserId.Should().Be(userId);
    }

    [Fact]
    public async Task GetUserContractsWhenAuthAsCeoOrCtoAndDataIsValidResponseWithOkResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var userId = _factory.EmployeeUserId.Value;

        var input = new CreateUserContractInput(
            userId,
            DateOnly.FromDateTime(DateTime.Now).AddYears(-8),
            DateOnly.FromDateTime(DateTime.Now).AddYears(-7),
            null, null, null, null);

        await client.CreateContracts(input)
            .WithResponseType<UserContractOutput>();

        // Act
        var getResponse = await client.GetContracts($"top=100&skip=0&filter=userId eq {userId}");

        // Assert
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CreateUserContractWhenAuthAsCeoOrCtoAndDataIsNotValidResponseBadRequest()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var userId = _factory.EmployeeUserId.Value;

        var body = new CreateUserContractInput(
            userId,
            DateOnly.FromDateTime(DateTime.Now).AddYears(-3),
            DateOnly.FromDateTime(DateTime.Now).AddYears(-4),
            null,null,null,null);

        // Act
        var response = await client.CreateContracts(body);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateUserContractWhenAuthAsCeoOrCtoAndDataIsNotValidResponseBadRequest()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var userId = _factory.EmployeeUserId.Value;

        var input = new CreateUserContractInput(
            userId,
            DateOnly.FromDateTime(DateTime.Now).AddYears(-2),
            DateOnly.FromDateTime(DateTime.Now).AddYears(-1),
            null, null, null, null);

        var response = await client.CreateContracts(input)
            .WithResponseType<UserContractOutput>();

        // Act
        var body = new UpdateUserContractInput(response.Id,
            DateOnly.FromDateTime(DateTime.Now), DateOnly.FromDateTime(DateTime.Now).AddYears(-5),
            null, null, null, null);

        var putResponse = await client.UpdateContracts(body);

        // Assert
        putResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }


    [Fact]
    public async Task GetUserContractByIdWhenAuthAsCeoOrCtoAndDataIsValidResponseWithOkResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var userId = _factory.EmployeeUserId.Value;

        var input = new CreateUserContractInput(
            userId,
            DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now).AddYears(1),
            null, null, null, null);

        var response = await client.CreateContracts(input)
            .WithResponseType<UserContractOutput>();

        // Act
        var getResponse = await client.GetContractById(response.Id)
            .WithResponseType<UserContractDto>();

        // Assert
        getResponse.Id.Should().Be(response.Id);
    }

    [Fact]
    public async Task GetActiveExpiringContractsAsyncWhenAuthAsCeoOrCtoReturnsActiveExpiringContracts()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        // Act
        var httpResponse = await client.GetActiveExpiringContracts();
        var contracts = await httpResponse.WithResponseType<List<ActiveExpiringContractOutput>>();

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        contracts.Should().NotBeNull();
        contracts
            .All(c => !string.IsNullOrEmpty(c.UserFullName) && c.ContractTo != null)
            .Should().BeTrue();
    }

    [Fact]
    public async Task GetActiveExpiringContractsAsyncWhenUnauthorizedReturnsUnauthorized()
    {
        // Arrange
        using var client = _factory.CreateClient();
        // No auth

        // Act
        var response = await client.GetActiveExpiringContracts();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
