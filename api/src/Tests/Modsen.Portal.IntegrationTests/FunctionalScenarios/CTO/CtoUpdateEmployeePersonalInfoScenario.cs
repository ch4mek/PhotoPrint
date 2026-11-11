using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.Identity.Controllers.Contracts.Input;
using Modsen.Portal.Identity.Controllers.Contracts.Output;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.TeamBoard.Controllers.Contracts.Output;
using Modsen.Portal.TeamBoard.Requests.UserPersonalInfo.Requests.RequestDto;
using NScenario;
using Xunit.Abstractions;

namespace Modsen.Portal.IntegrationTests.FunctionalScenarios;

[Collection("FuncScenarioCollection")]
public class CtoUpdateEmployeePersonalInfoScenario
{
    private readonly FunctionalTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ITestOutputHelper _outputHelper;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly JsonSerializerOptions _serializerOptions;

    public CtoUpdateEmployeePersonalInfoScenario(
        FunctionalTestFactory factory,
        ITestOutputHelper outputHelper)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
        _serializerOptions = _factory.JsonOptions;
        _outputHelper = outputHelper;
    }

    [Fact]
    public async Task UpdateEmployeePersonalInfoScenario()
    {
        using var httpClient = _factory.CreateClient();
        await using var outputAdapter = new XUnitOutputAdapter(_outputHelper);

        var scenario = TestScenarioFactory.Default(
            outputAdapter,
            scenarioPrefix: "[CTO]",
            title: "CTO Update employee personal info scenario");

        var corporateEmail = $"{Guid.CreateVersion7().ToAlphabetic()}@modsen-software.com";
        var personalEmail = $"{Guid.CreateVersion7().ToAlphabetic()}@gmail.com";
        var personalEmailSecond = $"{Guid.CreateVersion7().ToAlphabetic()}@gmail.com";
        var englishLevel = "A2";
        var personalPhone = "+275297777777";
        var telegram = "@testTg";
        var dateOfBirth = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-20));

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

        await scenario.Step("Update user personal info", async () =>
        {
            var response = await scenario.Step("Update user personal info", async () =>
            {
                return await httpClient.PatchAsJsonAsync(
                    "api/personal-info",
                    new UpdatePersonalDataRequestDto
                    (
                        UserId: createdUser!.Id,
                        FirstName: "test",
                        LastName: "test",
                        FirstNameRu: "тест",
                        LastNameRu: "тест",
                        PatronymicRu: "тест",
                        GroupId: null,
                        DepartmentId: "Net",
                        CorporateEmail: corporateEmail,
                        PersonalEmail: personalEmail,
                        PersonalEmailSecond: personalEmailSecond,
                        PersonalPhone: personalPhone,
                        Telegram: telegram,
                        LinkedIn: null,
                        EnglishLevel: englishLevel,
                        DateOfBirth: dateOfBirth,
                        Position: PortalRoleNames.Employee,
                        MilitaryServiceStatus: null,
                        IsGraduatedJobPlacement: null),
                    _cancellationToken);
            });

            await scenario.Step(
                "Validate response status code",
                () => response.StatusCode.Should().Be(HttpStatusCode.OK));
        });

        await scenario.Step("Check updated personal info", async () =>
        {
            var personalInfo = await scenario.Step("Get user personal info", async () =>
            {
                return await httpClient.GetFromJsonAsync<PersonalInfoOutput>(
                    $"api/personal-info?userId={createdUser!.Id}",
                    _cancellationToken);
            });

            await scenario.Step("Validate response body", () =>
            {
                personalInfo.Should().NotBeNull();
                personalInfo.Should().NotBeNull();
                personalInfo.PersonalEmail.Should().Be(personalEmail);
                personalInfo.PersonalPhone.Should().Be(personalPhone);
                personalInfo.EnglishLevel.Should().Be(englishLevel);
                personalInfo.DateOfBirth.Should().Be(dateOfBirth);
            });
        });
    }
}
