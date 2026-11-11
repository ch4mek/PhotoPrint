using System.Net;
using Bogus;
using Modsen.Portal.Certificate.UseCases.Dto;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.Identity.Controllers.Contracts.Input;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using NScenario;
using Xunit.Abstractions;

namespace Modsen.Portal.IntegrationTests.FunctionalScenarios;

[Collection("FuncScenarioCollection")]
public class HodAddCertificateToUserNotFromAvailableDepartments
{
    private static readonly string[] s_availableDepartments = ["Js", "Net", "Java", "Python", "QA", "HR", "Marketing", "PMO", "Sales", "BA", "Mobile", "DevOps", "Design", "Administration", "SysAdmin"];

    private static readonly string[] s_departments = ["Js", "Net", "Java", "Python", "QA", "HR",
                        "Marketing", "PMO", "Sales", "BA", "Mobile", "DevOps", "Design", "Administration", "SysAdmin", "PHP"];

    private readonly FunctionalTestFactory _factory;
    private readonly ITestOutputHelper _outputHelper;
    private readonly Faker<CreateUserInput> _createUserInputFaker = new();

    public HodAddCertificateToUserNotFromAvailableDepartments(
        FunctionalTestFactory testFactory,
        ITestOutputHelper outputHelper)
    {
        _factory = testFactory;
        _outputHelper = outputHelper;

        _createUserInputFaker = _createUserInputFaker
            .CustomInstantiator(x => new CreateUserInput
            {
                FirstName = Guid.CreateVersion7().ToAlphabetic(),
                LastName = Guid.CreateVersion7().ToAlphabetic(),
                FirstNameRu = "Имя",
                LastNameRu = "Фамилия",
                PatronymicRu = "Отчество",
                Color = x.Internet.Color(),
                Email = x.Internet.Email(provider: "modsen-software.com", uniqueSuffix: Guid.CreateVersion7().ToAlphabetic()),
                Position = PortalRoleNames.HeadOfDepartment,
                Location = x.PickRandom<LocationType>(),
                DepartmentId = x.PickRandom(s_departments.Except(s_availableDepartments)),
                CountryId = 1,
                RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId
            });
    }

    [Fact]
    public async Task HeadAddCertificateToUserNotFromAvailableDepartments()
    {
        using var client = _factory.CreateClient();

        var (headEmail, headDepartment) = await GetPreparedHeadOfDepartmentEmail(client);
        await GetPreparedEmployeeId(client, headDepartment);

        using var httpClient = _factory.CreateClient();
        await using var outputAdapter = new XUnitOutputAdapter(_outputHelper);

        var scenario = TestScenarioFactory.Default(
            outputAdapter,
            scenarioPrefix: "[Head of Department]",
            title: "Head of Department Add certificate to a User not from JavaScript/Java/QA/.NET/Python/QA/HR/Marketing/PMO/Sales/BA/Mobile/DevOps/Design/System Administrators departments scenario");

        var currentUser = await scenario.Step("Login", async () =>
        {
            await scenario.Step(
                "Call auth endpoint",
                () => httpClient.AuthAs(headEmail));

            var user = await scenario.Step("Call 'Me' endpoint for getting info about current user", async () =>
            {
                return await httpClient.GetCurrentUser();
            });

            await scenario.Step("Verify info about user", async () =>
            {
                user.Should().NotBeNull();
                await scenario.Step("UserDepartment: " + user.DepartmentId, () => { });
                user.DepartmentId.Should().NotBeNull();
                user.Email.Should().NotBeNullOrEmpty();
                user.Position.Should().Be(PortalRoleNames.HeadOfDepartment);
            });

            return user;
        });

        var employee = await scenario.Step("Get an employee", async () =>
        {
            var availableEmployees = await scenario.Step("Get all available employees", async () =>
            {
                return await httpClient.GetUsersLookup(roles: [PortalRoleNames.Employee], isActive: true);
            });

            var pickedEmployee = await scenario.Step(
                "Pick last Employee",
                () => availableEmployees[^1]);

            var employeeFullName = $"{pickedEmployee.FirstName} {pickedEmployee.LastName}";

            var fullEmployeeInfo = await scenario.Step($"[{employeeFullName}] Get full user info", async () =>
            {
                return await httpClient.GetUserAsync(pickedEmployee.Id);
            });

            await scenario.Step($"[{employeeFullName}] Verify user", () =>
            {
                fullEmployeeInfo.Should().NotBeNull();
                fullEmployeeInfo.Id.Should().Be(pickedEmployee.Id);
                fullEmployeeInfo.FirstName.Should().Be(pickedEmployee.FirstName);
                fullEmployeeInfo.LastName.Should().Be(pickedEmployee.LastName);
                fullEmployeeInfo.Color.Should().Be(pickedEmployee.Color);
                fullEmployeeInfo.AvatarFileId.Should().Be(pickedEmployee.AvatarFileId);
                fullEmployeeInfo.DepartmentId.Should().Be(currentUser.DepartmentId);
            });

            return fullEmployeeInfo!;
        });

        await scenario.Step("Add a certificate to the employee", async () =>
        {
            var departmentId = currentUser.DepartmentId;

            var certificateToCreate = new CreateCertificateDto
            {
                RecipientId = employee.Id.ToString(),
                Recipient = $"{employee.FirstName} {employee.LastName}",
                ReceiptDate = DateTime.UtcNow,
                Technology = departmentId,
                Stack = null
            };

            var response = await httpClient.CreateCertificateAsHttpMessageAsync(certificateToCreate);

            await scenario.Step("Validate response body", async () =>
            {
                response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            });
        });
    }

    private async Task<(string email, string department)> GetPreparedHeadOfDepartmentEmail(
        HttpClient client)
    {
        await client.AuthAsCeo();

        var headOfDepartmentToCreate = _createUserInputFaker.Generate();

        var filter =
            $"DepartmentId eq '{headOfDepartmentToCreate.DepartmentId}' and Position eq '{PortalRoleNames.HeadOfDepartment}'";

        var headUsers = await client.GetUsersWithSearch(filter: filter, orderBy: "id",
            top: 1);

        var isHeadAlreadyExisting = headUsers.Items.Count > 0;

        var head = isHeadAlreadyExisting
            ? headUsers.Items.First()
            : await client.CreateUser(headOfDepartmentToCreate);

        return (head.Email, head.DepartmentId);
    }

    private async Task<int> GetPreparedEmployeeId(HttpClient client, string departmentId)
    {
        await client.AuthAsCeo();

        var employee = await client.CreateEmployee(departmentId: departmentId);

        return employee.Id;
    }
}
