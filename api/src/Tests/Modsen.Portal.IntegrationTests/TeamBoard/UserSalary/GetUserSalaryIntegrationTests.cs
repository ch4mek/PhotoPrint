using System.Net;
using System.Text.Json;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.Identity.Controllers.Contracts.Input;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TeamboardHttpClientExtensions;
using Modsen.Portal.TeamBoard.Controllers.Contracts.Output;
using Modsen.Portal.TeamBoard.Requests.UserSalary.Requests.RequestDto;
namespace Modsen.Portal.IntegrationTests.TeamBoard.UserSalary;

[Collection("GeneralCollection")]
public class GetUserSalaryIntegrationTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;

    public GetUserSalaryIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
    }

    [Fact]
    public async Task GetUserSalaryWhenAuthAsCeoCtoResponseWithOkAndSalaryOutputResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var userId = _factory.EmployeeUserId!.Value;
        var salaryFrom = DateOnly.FromDateTime(DateTime.Now.AddDays(2));
        var salary = 100;

        // Act
        await client.CreateUserSalary(
            new CreateSalaryDto(
                userId,
                salaryFrom,
                salary, null)).WithResponseType<CreateUserSalaryOutput>();

        var getSalaryResponse = await client.GetTeamboardUsers(filter: $"Id eq {userId}",
            top: 100,
            skip: 0);

        // Assert
        getSalaryResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetUserSalaryWhenAuthAsSysAdminOrEmployeeResponseWithForbiddenResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var userId = _factory.EmployeeUserId!.Value;
        var salaryFrom = DateOnly.FromDateTime(DateTime.Now.AddDays(4));
        var salary = 100;

        // Act
        await client.CreateUserSalary(
            new CreateSalaryDto(
                userId,
                salaryFrom,
                salary,
                null));

        await client.AuthAsEmployee();

        var getSalaryResponse = await client.GetTeamboardUsers($"Id eq {userId}", top: 100);

        // Assert
        getSalaryResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetUserSalaryWhenAuthAsHeadForUserFromHisDepartmentResponseWithOkResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsHeadOfDepartment();

        var userId = _factory.EmployeeUserId!.Value;
        var salaryFrom = DateOnly.FromDateTime(DateTime.Now.AddDays(3));
        var salary = 100;

        // Act
        await client.CreateUserSalary(
            new CreateSalaryDto(
                userId,
                salaryFrom,
                salary, null));

        var getSalaryResponse = await client.GetTeamboardUsers($"Id eq {userId}", top: 100);


        // Assert
        getSalaryResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetUserSalaryWhenAuthAsHeadForUserNotFromHisDepartmentResponseWithOkResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var salaryFrom = DateOnly.FromDateTime(DateTime.Now);
        var oldSalary = 100;

        var createNewUser = new CreateUserInput
        {
            FirstName = "Qwerty",
            LastName = "Qwerton",
            FirstNameRu = "Имя",
            LastNameRu = "Фамилия",
            PatronymicRu = "Отчество",
            Color = "#ffffff",
            DepartmentId = "Java",
            Email = "Qwertowewenite@modsen-software.com",
            Location = LocationType.Minsk,
            Position = PortalRoleNames.Employee,
            CountryId = 1,
            RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId
        };

        // Act
        var responseAfterCreatingUser = await client.CreateUser(createNewUser);

        var userId = responseAfterCreatingUser.Id;

        await client.CreateUserSalary(
            new CreateSalaryDto(
                userId,
                salaryFrom,
                oldSalary,
                null));


        await client.AuthAsHeadOfDepartment();

        var getSalaryResponse = await client.GetTeamboardUsers($"Id eq {userId}", top: 100);

        // Assert
        getSalaryResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetUserSalaryWhenAuthAsGroupManagerForUserFromHisResourceGroupResponseWithOkResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var salaryFrom = DateOnly.FromDateTime(DateTime.Now);
        var oldSalary = 100;

        var createNewUser = new CreateUserInput
        {
            FirstName = "Qwerty",
            LastName = "Qwerton",
            FirstNameRu = "Имя",
            LastNameRu = "Фамилия",
            PatronymicRu = "Отчество",
            Color = "#ffffff",
            DepartmentId = "Net",
            Email = "TErewqfdjkyk.tf12s342132@modsen-software.com",
            Location = LocationType.Minsk,
            Position = PortalRoleNames.Employee,
            ResourceGroupId = 1,
            CountryId = 1,
            RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId
        };


        // Act
        var responseAfterCreatingUser = await client.CreateUser(createNewUser);

        var userId = responseAfterCreatingUser.Id;

        await client.CreateUserSalary(
            new CreateSalaryDto(
                userId,
                salaryFrom,
                oldSalary,
                null));

        await client.AuthAsGroupManager();

        var getSalaryResponse = await client.GetTeamboardUsers($"Id eq {userId}", top: 100);

        // Assert
        getSalaryResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetUserSalaryWhenAuthAsGroupManagerForUserNotFromHisResourceGroupResponseWithOkResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var salaryFrom = DateOnly.FromDateTime(DateTime.Now);
        var oldSalary = 100;

        var createNewUser = new CreateUserInput
        {
            FirstName = "Qwerty",
            LastName = "Qwerton",
            FirstNameRu = "Имя",
            LastNameRu = "Фамилия",
            PatronymicRu = "Отчество",
            Color = "#ffffff",
            DepartmentId = "Net",
            Email = "TESTfdjkyk.t12342132@modsen-software.com",
            Location = LocationType.Minsk,
            Position = PortalRoleNames.Employee,
            CountryId = 1,
            RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId
        };

        // Act
        var responseAfterCreatingUser = await client.CreateUser(createNewUser);

        var userId = responseAfterCreatingUser.Id;

        await client.CreateUserSalary(
            new CreateSalaryDto(
                userId,
                salaryFrom,
                oldSalary,
                null));

        await client.AuthAsGroupManager();

        var getSalaryResponse = await client.GetTeamboardUsers(filter: $"Id eq {userId}",
            top: 100,
            skip: 0);


        // Assert
        getSalaryResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
