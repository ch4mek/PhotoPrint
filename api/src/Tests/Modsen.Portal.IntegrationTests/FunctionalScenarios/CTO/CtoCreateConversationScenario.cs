using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.TeamBoard;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.Identity.Controllers.Contracts.Input;
using Modsen.Portal.Identity.Controllers.Contracts.Output;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.TeamBoard.Controllers.Contracts.Input;
using Modsen.Portal.TeamBoard.Controllers.Contracts.Output;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.OData;
using NScenario;
using Xunit.Abstractions;

namespace Modsen.Portal.IntegrationTests.FunctionalScenarios;

[Collection("FuncScenarioCollection")]
public class CtoCreateConversationScenario
{
    private readonly FunctionalTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ITestOutputHelper _outputHelper;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly JsonSerializerOptions _serializerOptions;

    public CtoCreateConversationScenario(
        FunctionalTestFactory factory,
        ITestOutputHelper outputHelper)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
        _serializerOptions = _factory.JsonOptions;
        _outputHelper = outputHelper;
    }

    [Fact]
    public async Task CreateConversationScenario()
    {
        using var httpClient = _factory.CreateClient();
        await using var outputAdapter = new XUnitOutputAdapter(_outputHelper);

        var scenario = TestScenarioFactory.Default(
            outputAdapter,
            scenarioPrefix: "[CTO]",
            title: "Create conversation for employee scenario");

        var theses = Guid.CreateVersion7().ToAlphabetic();
        var employeeTheses = Guid.CreateVersion7().ToAlphabetic();
        var agreements = Guid.CreateVersion7().ToAlphabetic();
        var date = DateOnly.FromDateTime(DateTime.UtcNow);
        var conversationType = ConversationType.Conversation;
        var conversationResult = ConversationResult.Great;

        await scenario.Step("Login", async () =>
        {
            await scenario.Step(
                "Call auth endpoint",
                () => httpClient.AuthAsCto());

            var user = await scenario.Step("Call 'Me' endpoint for getting info about current user", async () =>
            {
                return await httpClient.GetFromJsonAsync<UserOutput>(
                    "api/users/current",
                    _serializerOptions,
                    _cancellationToken);
            });

            await scenario.Step("Verify info about user", () =>
            {
                user.Should().NotBeNull();
                user.DepartmentId.Should().BeNull();
                user.Email.Should().NotBeNullOrEmpty();
                user.Position.Should().Be(PortalRoleNames.Cto);
            });
        });

        var createdUser = await scenario.Step("Create new employee", async () =>
        {
            await httpClient.AuthAsCto();

            return await scenario.Step(
                "Call 'Users' endpoint to create new user", async () =>
                {
                    var createdUser = await scenario.Step("Create new user", async () =>
                    {
                        var userToCreate = new CreateUserInput
                        {
                            FirstName = Guid.CreateVersion7().ToAlphabetic(),
                            LastName = Guid.CreateVersion7().ToAlphabetic(),
                            FirstNameRu = "Имя",
                            LastNameRu = "Фамилия",
                            PatronymicRu = "Отчество",
                            Email = $"{Guid.CreateVersion7().ToAlphabetic()}@modsen-software.com",
                            Position = PortalRoleNames.Employee,
                            Location = LocationType.Minsk,
                            DepartmentId = "Net",
                            Color = "#ffffff",
                            CountryId = 1,
                            RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId
                        };

                        return await httpClient.CreateUser(userToCreate);
                    });

                    return createdUser;
                });
        });

        var createdConversation = await scenario.Step("Create conversation", async () =>
        {
        var response = await scenario.Step("Create conversation", async () =>
        {
        return await httpClient.PostAsJsonAsync(
                "api/Conversations",
                new CreateConversationInput
                {
                    Date = date,
                    ConversationType = conversationType,
                    ConversationResult = conversationResult,
                    Theses = theses,
                    EmployeeTheses = employeeTheses,
                    Agreements = agreements,
                    PerformanceReviewDate = null,
                    Passed = null,
                    UserPerformanceReviewId = null,
                    UserId = createdUser!.Id,
                    Files = [],
                    DocumentLinks = []
                },
                _jsonOptions,
                _cancellationToken);
            });

            await scenario.Step(
                "Validate response status code",
                () => response.StatusCode.Should().Be(HttpStatusCode.OK));

            return await response.Content.ReadFromJsonAsync<ConversationOutput>(
                _serializerOptions,
                _cancellationToken);
        });

        await scenario.Step("Check created conversation", async () =>
        {
            var conversations = await scenario.Step("Get user conversation", async () =>
            {
                return await httpClient.GetFromJsonAsync<PageableListOfItems<OdpConversation>>(
                    $"api/conversations/?$filter=(id%20eq%20{createdConversation!.Id})%20&$top=1&$skip=0",
                    _cancellationToken);
            });

            await scenario.Step("Validate response body", () =>
            {
                conversations.Should().NotBeNull();
                conversations.Items.Should().HaveCount(1);
                var conversation = conversations.Items.FirstOrDefault();
                conversation.Should().NotBeNull();
                conversation.UserId.Should().Be(createdUser!.Id);
                conversation.Date.Should().Be(date);
                conversation.ConversationType.Should().Be(conversationType.ToString());
                conversation.Theses.Should().Be(theses);
                conversation.EmployeeTheses.Should().Be(employeeTheses);
                conversation.Agreements.Should().Be(agreements);
            });
        });
    }
}
