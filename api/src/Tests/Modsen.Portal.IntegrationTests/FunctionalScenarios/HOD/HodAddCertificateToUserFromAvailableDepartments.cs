using System.Net;
using Bogus;
using Modsen.Portal.Certificate.UseCases.Dto;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Certificate;
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
public class HodAddCertificateToUserFromAvailableDepartments
{
    private static readonly string[] s_availableDepartments = ["Js", "Net", "Java", "Python", "QA"];

    private readonly FunctionalTestFactory _factory;
    private readonly ITestOutputHelper _outputHelper;
    private readonly Faker<CreateUserInput> _createUserInputFaker = new();

    public HodAddCertificateToUserFromAvailableDepartments(
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
                DepartmentId = x.PickRandom(s_availableDepartments),
                CountryId = 1,
                RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId
            });
    }

    [Fact(Skip = "NA")]
    public async Task HeadAddCertificateToUserFromAvailableDepartments()
    {
        if (!_factory.IsCertificateTestsEnabled())
        {
            true.Should().BeTrue();
        }

        using var client = _factory.CreateClient();

        var (headEmail, headDepartment) = await GetPreparedHeadOfDepartmentEmail(client);
        var employeeId = await GetPreparedEmployeeId(client, headDepartment);

        using var httpClient = _factory.CreateClient();
        await using var outputAdapter = new XUnitOutputAdapter(_outputHelper);

        var scenario = TestScenarioFactory.Default(
            outputAdapter,
            scenarioPrefix: "[Head of Department]",
            title: "Head of Department Add certificate to a User from JavaScript/Java/QA/.NET/Python scenario");

        var currentUser = await scenario.Step("Login", async () =>
        {
            await scenario.Step(
                "Call auth endpoint",
                () => httpClient.AuthAs(headEmail));

            var user = await scenario.Step("Call 'Me' endpoint for getting info about current user", async () =>
            {
                return await httpClient.GetCurrentUser();
            });

            await scenario.Step("Verify info about user", () =>
            {
                user.Should().NotBeNull();
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
                () =>
                {
                    return availableEmployees.First(employee => employee.Id == employeeId);
                });

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
            var certificates = await client.GetEmployeeCertificates(id: employee.Id);

            if (certificates is { Items.Count: > 0 })
            {
                var certificateId = certificates.Items.First().CertificateId;

                await client.DeleteCertificateAsHttpMessage(certificateId!);
            }

            var departmentId = currentUser.DepartmentId;

            var certificateToCreate = new CreateCertificateDto
            {
                RecipientId = $"{employee.Id}",
                Recipient = $"{employee.FirstName} {employee.LastName}",
                ReceiptDate = DateTime.UtcNow,
                Technology = departmentId,
                Stack = departmentId == "Js" ? JsStackType.React : null
            };

            var response = await httpClient.CreateCertificateAsHttpMessageAsync(input: certificateToCreate);

            await scenario.Step("Validate response body", async () =>
            {
                response.StatusCode.Should().Be(HttpStatusCode.OK);
            });
        });
    }

    private async Task<(string email, string department)> GetPreparedHeadOfDepartmentEmail(
        HttpClient client)
    {
        await client.AuthAsCeo();

        var headOfDepartmentToCreate = _createUserInputFaker.Generate();

        var filter = $"DepartmentId eq '{headOfDepartmentToCreate.DepartmentId}' and Position eq '{PortalRoleNames.HeadOfDepartment}'";
        var orderBy = "id";

        var headUsers = await client.GetUsersWithSearch(top: 1, filter: filter,
            orderBy: orderBy);

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
