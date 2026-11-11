using System.Net;
using System.Text.Json;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TimeTracker;
using Modsen.Portal.TimeTrack.Controllers.Contracts.Output;
namespace Modsen.Portal.IntegrationTests.TimeTracker.Controllers.ClientsController.ClientsControllerIntegrationTests;

[Collection("TimeTrackerCollection")]
public class ClientsControllerIntegrationTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;

    public ClientsControllerIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
    }

    [Fact]
    public async Task CreateClientAsyncWhenAuthedAsEmployeeShouldResponseWithForbiddenResult()
    {
        // Arrange
        var httpClient = _factory.CreateClient();
        await httpClient.AuthAsEmployee();

        // Act
        var response = await httpClient.CreateClientAsHttpMessageAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task CreateClientAsyncWhenClientWithSameNameAlreadyExistShouldResponseWithBadRequestResult()
    {
        // Arrange
        var httpClient = _factory.CreateClient();
        await httpClient.AuthAsCeo();
        var clientName = Guid.CreateVersion7().ToAlphabetic();

        await httpClient.CreateClientAsHttpMessageAsync(name: clientName);

        // Act
        var response = await httpClient.CreateClientAsHttpMessageAsync(name: clientName);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Theory]
    [MemberData(nameof(GetInvalidClientParams))]
    public async Task CreateClientAsyncWhenRequestIsInvalidShouldResponseWithBadRequestResult(
        string name,
        string? comments,
        string? color,
        int registeredLocationType)
    {
        // Arrange
        var httpClient = _factory.CreateClient();
        await httpClient.AuthAsCeo();

        // Act
        var response = await httpClient.CreateClientAsHttpMessageAsync(
            name: name,
            color: color,
            comments: comments,
            registeredLocationId: registeredLocationType);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateClientAsyncWhenRequestIsValidShouldResponseWithOkResultAndCreatedClient()
    {
        // Arrange
        var httpClient = _factory.CreateClient();
        await httpClient.AuthAsCeo();

        const string comments = "Ordinal comment";
        const string color = "#00FFFF";
        var name = new string('a', 250);
        const int registeredLocation = PortalSharedConstants.DefaultRegisteredLocationId;

        var expected = new CreateClientOutput
        {
            Id = 0,
            Name = name,
            LegalEntityName = name,
            Comments = comments,
            Color = color,
            RegisteredLocation = "Belarus"
        };

        // Act
        var output = await httpClient.CreateClientAsync(
            name: name,
            color: color,
            comments: comments,
            registeredLocationId: registeredLocation);

        // Assert
        output.Should().BeEquivalentTo(
            expected,
            opt => opt.Excluding(x => x.Id));
        output.Id.Should().NotBe(0);
    }

    [Fact]
    public async Task UpdateClientStatusAsyncWhenAuthedAsEmployeeShouldResponseWithForbiddenResult()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsEmployee();

        var clientId = _factory.ModsenClientId!.Value;

        // Act
        var response = await client.SetClientActiveAsHttpMessageAsync(clientId);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UpdateClientStatusAsyncWhenClientDoesNotExistShouldResponseWithNotFoundResult()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();

        const int clientId = int.MaxValue;

        // Act
        var response = await client.SetClientInactiveAsHttpMessageAsync(clientId);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateClientStatusAsyncWhenClientHasActiveProjectsShouldResponseWithOkResultAndUnsuccessfulBody()
    {
        // Arrange
        var httpClient = _factory.CreateClient();
        await httpClient.AuthAsCeo();

        var client = await httpClient.CreateClientAsync();
        var clientId = client!.Id;

        await httpClient.CreateProject(
            clientId: clientId,
            isBillable: true,
            isPublic: false);

        // Act
        var output = await httpClient.SetClientInactiveAsync(clientId);

        // Assert
        output.IsSuccessful.Should().BeFalse();
    }

    [Fact]
    public async Task UpdateClientStatusAsyncWhenClientHasNoActiveProjectsShouldResponseWithOkResultAndSuccessfulBody()
    {
        // Arrange
        var httpClient = _factory.CreateClient();
        await httpClient.AuthAsCeo();

        var client = await httpClient.CreateClientAsync();
        var clientId = client!.Id;

        // Act
        var output = await httpClient.SetClientInactiveAsync(clientId);

        // Assert
        output.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateClientAsyncWhenAuthedAsEmployeeShouldResponseWithForbiddenResult()
    {
        // Arrange
        var httpClient = _factory.CreateClient();
        await httpClient.AuthAsEmployee();

        // Act
        var response = await httpClient.UpdateClientAsHttpMessageAsync(id: 1);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UpdateClientAsyncWhenClientDoesNotExistShouldResponseWithNotFoundResult()
    {
        // Arrange
        var httpClient = _factory.CreateClient();
        await httpClient.AuthAsCeo();

        const int nonExistingClientId = int.MaxValue;

        // Act
        var response = await httpClient.UpdateClientAsHttpMessageAsync(id: nonExistingClientId);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateClientAsyncWhenClientWithSameNameAlreadyExistsShouldResponseWithBadRequestResult()
    {
        // Arrange
        var httpClient = _factory.CreateClient();
        await httpClient.AuthAsCeo();

        var existingClient = await httpClient.CreateClientAsync();

        var clientToUpdate = await httpClient.CreateClientAsync();

        // Act
        var response = await httpClient.UpdateClientAsHttpMessageAsync(
            id: clientToUpdate!.Id,
            name: existingClient!.Name);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Theory]
    [MemberData(nameof(GetInvalidClientParams))]
    public async Task UpdateClientAsyncWhenUpdateParamsAreInvalidShouldResponseWithBadRequestResult(
        string name,
        string? comments,
        string? color,
        int registeredLocationType)
    {
        // Arrange
        var httpClient = _factory.CreateClient();
        await httpClient.AuthAsCeo();

        var clientToUpdate = await httpClient.CreateClientAsync();
        var clientId = clientToUpdate!.Id;

        // Act
        var response = await httpClient.UpdateClientAsHttpMessageAsync(
            id: clientId,
            name: name,
            comments: comments,
            color: color,
            registeredLocationId: registeredLocationType);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateClientAsyncWhenRequestIsValidShouldUpdateClient()
    {
        // Arrange
        var httpClient = _factory.CreateClient();
        await httpClient.AuthAsCeo();

        var client = await httpClient.CreateClientAsync();
        var clientId = client!.Id;
        var newClientName = Guid.CreateVersion7().ToAlphabetic();
        const string newClientColor = "#000040";
        const string newClientComments = "New comments";
        const int newRegisteredLocationId = 2;

        var expectedResult = new ClientOutput
        {
            Id = clientId,
            Name = newClientName,
            Color = newClientColor,
            Comments = newClientComments,
            IsActive = true,
            IsProjectExist = false,
            FactTrackedTimeInMinutes = 0,
            BillableTrackedTimeInMinutes = 0,
            RegisteredLocation = "Poland",
        };

        // Act
        var output = await httpClient.UpdateClientAsync(
            id: clientId,
            name: newClientName,
            comments: newClientComments,
            color: newClientColor,
            registeredLocationId: newRegisteredLocationId);

        // Assert
        output.Should().BeEquivalentTo(expectedResult);
    }



    public static IEnumerable<object?[]> GetInvalidClientParams()
    {
        yield return [null, "comment", "#0f0", 0];
        yield return [string.Empty, " comment", "#777777", -1];
        yield return ["U", "comment ", "#4D4", 0];
        yield return ["     ", "com", "#000000", 0];
        yield return [new string('0', 251), string.Empty, "#123456", 1000];
        yield return ["Lorem Ipsum", new string('5', 1001), "#000", 1000];
        yield return ["Pellentesque ultricies", null, "#FF", 0];
        yield return ["Duis mollis", "scelerisque", "000000", 0];
    }
}
