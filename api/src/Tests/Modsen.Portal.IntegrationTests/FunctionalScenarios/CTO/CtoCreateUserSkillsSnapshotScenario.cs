using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.Identity.Controllers.Contracts.Output;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.TeamBoard.Controllers.Contracts.Input;
using Modsen.Portal.TeamBoard.Requests.Core.CoreDto;
using Modsen.Portal.TeamBoard.Requests.Skills.UserSpecializations.Requests.RequestDto;
using NScenario;
using Xunit.Abstractions;

namespace Modsen.Portal.IntegrationTests.FunctionalScenarios;

[Collection("FuncScenarioCollection")]
public class CtoCreateUserSkillsSnapshotScenario
{
    private readonly FunctionalTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ITestOutputHelper _outputHelper;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly JsonSerializerOptions _serializerOptions;

    public CtoCreateUserSkillsSnapshotScenario(
        FunctionalTestFactory factory,
        ITestOutputHelper outputHelper)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
        _serializerOptions = _factory.JsonOptions;
        _outputHelper = outputHelper;
    }

    [Fact]
    public async Task CreateUserSkillsSnapshotScenario()
    {
        using var httpClient = _factory.CreateClient();
        await using var outputAdapter = new XUnitOutputAdapter(_outputHelper);

        var scenario = TestScenarioFactory.Default(
            outputAdapter,
            scenarioPrefix: "[CTO]",
            title: "CTO Create user hard skills snapshot scenario");

        var date = DateOnly.FromDateTime(DateTime.UtcNow);
        var seniorityLevelId = "Senior";

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

        var userWithSnapshot = await scenario.Step("Get employee with existing snapshot", async () =>
        {
            await httpClient.AuthAsCto();

            return await scenario.Step(
                "Call 'Users' endpoint to get user with snapshot", async () =>
                {
                    var users = await httpClient.GetFromJsonAsync<PageableListOfItems<UserOutput>>(
                        "api/Users/search?top=10&skip=0&filter=Email eq 'snapshot.test@modsen-software.com'",
                        _serializerOptions,
                        _cancellationToken);

                    return users.Items.First();
                });
        });

        var userSpecializations = await scenario.Step("Get user specializations", async () =>
        {
            return await scenario.Step(
                "Get user specializations", async () =>
                {
                    var assignedSpecializations = await httpClient.GetFromJsonAsync<UserSpecializationTemplateWithHardSkillsDto[]>(
                        $"api/users/{userWithSnapshot.Id}/specializations?areAssigned=true",
                        _serializerOptions,
                        _cancellationToken);

                    return assignedSpecializations!.Select(s => new CreateUserSpecializationDto
                    {
                        SpecializationTemplateId = s.SpecializationTemplateId,
                        SpecializationSeniorityLevelId = "JuniorL2",
                        HardSkills = s.HardSkills.Select(hs => new HardSkillSeniorityDto
                        {
                            Id = hs.Id,
                            SeniorityLevelId = seniorityLevelId
                        }).ToArray()
                    }).ToList();
                });
        });

        await scenario.Step("Create user skills snapshot", async () =>
        {
            var response = await scenario.Step("Create user skills snapshot", async () =>
            {
                return await httpClient.PostAsJsonAsync(
                    "api/user-skills-snapshot",
                    new CreateUserSnapshotInput
                    (
                        UserId: userWithSnapshot.Id,
                        SnapshotDate: date,
                        UserSpecializations: userSpecializations
                    ),
                    _cancellationToken);
            });

            await scenario.Step(
                "Validate response status code",
                () => response.StatusCode.Should().Be(HttpStatusCode.OK));
        });

        await scenario.Step("Check created user skills snapshot", async () =>
        {
            var userSkillsSnapshots = await scenario.Step("Get user skills snapshots by user id", async () =>
            {
                return await httpClient.GetFromJsonAsync<PageableListOfItems<UserSpecializationTemplateWithSnapshotDto>>(
                    $"api/user-skills-snapshot?userId={userWithSnapshot.Id}&$top=50&$skip=0",
                    _cancellationToken);
            });

            await scenario.Step("Validate response body", () =>
            {
                userSkillsSnapshots.Should().NotBeNull();

                var userSkillsSnapshot = userSkillsSnapshots.Items.FirstOrDefault(x => x.IsPrimary)
                .SpecializationSnapshots.FirstOrDefault(x => x.SnapshotDate == date);

                userSkillsSnapshot.SnapshotDate.Should().Be(date);
                userSkillsSnapshot.UserId.Should().Be(userWithSnapshot.Id);
                userSkillsSnapshot.UserSpecializations.FirstOrDefault().Should().NotBeNull();
                userSkillsSnapshot.UserSpecializations.Select(x =>
                x.HardSkills.Select(x => x.SeniorityLevelId).FirstOrDefault()).FirstOrDefault()
                .Should().Be(seniorityLevelId);
            });
        });
    }
}
