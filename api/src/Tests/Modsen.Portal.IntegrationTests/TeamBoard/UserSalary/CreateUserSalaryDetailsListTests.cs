using System.Net;
using System.Text.Json;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.Identity.Controllers.Contracts.Input;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TeamboardHttpClientExtensions;
using Modsen.Portal.TeamBoard.Requests.UserSalary.Requests.RequestDto;
namespace Modsen.Portal.IntegrationTests.TeamBoard.UserSalary;

[Collection("GeneralCollection")]
public class CreateUserSalaryDetailsListTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;

    public CreateUserSalaryDetailsListTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
    }

    [Fact]
    public async Task AddUserSalariesWhenAuthAsCeoOrCtoResponseWithOkAndSalaryOutputResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var userId = _factory.EmployeeUserId!.Value;
        var salaryFrom = DateOnly.FromDateTime(DateTime.Now);
        var salaryUsd = 1000;
        var createUserSalary = new CreateSalaryDto(userId, salaryFrom, salaryUsd, null);

        // Act
        var responseAfterCreating = await client.CreateUserSalaryDetailsList([createUserSalary])
            .WithResponseType<List<CreateSalaryDto>>();

        // Assert
        responseAfterCreating.First().UserId.Should().Be(userId);
        responseAfterCreating.First().SalaryUsd.Should().Be(salaryUsd);
    }

    [Fact]
    public async Task AddUserSalariesWhenAuthAsEmployeeOrSysAdminResponseWithForbiddenResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsEmployee();

        var userId = _factory.EmployeeUserId!.Value;
        var salaryFrom = DateOnly.FromDateTime(DateTime.Now.AddDays(10));
        var salaryUsd = 1000;
        var createUserSalary = new CreateSalaryDto(userId, salaryFrom, salaryUsd, null);

        // Act
        var response = await client.CreateUserSalaryDetailsList([createUserSalary]);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task AddUserSalariesWhenAuthAsHeadAddSalaryForUserFromTheSameDepartmentResponseWithOkAndSalaryOutputResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsHeadOfDepartment();

        var userId = _factory.EmployeeUserId!.Value;
        var salaryFrom = DateOnly.FromDateTime(DateTime.Now.AddDays(1));
        var salaryUsd = 2000;
        var createUserSalary = new CreateSalaryDto(userId, salaryFrom, salaryUsd, null);

        // Act
        var response = await client.CreateUserSalaryDetailsList([createUserSalary]);
        var result = await response.WithResponseType<List<CreateSalaryDto>>();

        // Assert
        result.First().UserId.Should().Be(userId);
        result.First().SalaryUsd.Should().Be(salaryUsd);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task AddUserSalariesWhenAuthAsGroupManagerAddSalaryForUserFromTheSameResourceGroupResponseWithForbiddenResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var userToCreate = new CreateUserInput
        {
            FirstName = Guid.CreateVersion7().ToAlphabetic(),
            LastName = Guid.CreateVersion7().ToAlphabetic(),
            FirstNameRu = "Имя",
            LastNameRu = "Фамилия",
            PatronymicRu = "Отчество",
            Email = $"{Guid.CreateVersion7().ToAlphabetic()}@modsen-software.com",
            Color = "#ffffff",
            Location = LocationType.Minsk,
            Position = PortalRoleNames.Employee,
            DepartmentId = "Net",
            ResourceGroupId = 1,
            CountryId = 1,
            RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId
        };

        var user = await client.CreateUser(userToCreate);

        await client.AuthAsGroupManager();

        var salaryFrom = DateOnly.FromDateTime(DateTime.Now);
        var salaryUsd = 2000;
        var createUserSalary = new CreateSalaryDto(user.Id, salaryFrom, salaryUsd, null);

        // Act
        var response = await client.CreateUserSalaryDetailsList([createUserSalary]);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task AddUserSalariesWhenAuthAsHeadAddSalaryForUserNotFromTheSameDepartmentResponseWithForbiddenResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var createNewUser = new CreateUserInput
        {
            FirstName = "John",
            LastName = "Doe",
            FirstNameRu = "Имя",
            LastNameRu = "Фамилия",
            PatronymicRu = "Отчество",
            Color = "#ffffff",
            DepartmentId = "Java",
            Email = "john.doe@modsen-software.com",
            Location = LocationType.Minsk,
            Position = PortalRoleNames.Employee,
            CountryId = 1,
            RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId
        };

        var responseAfterCreatingUser = await client.CreateUser(createNewUser);

        var userId = responseAfterCreatingUser.Id;

        await client.AuthAsHeadOfDepartment();

        var salaryFrom = DateOnly.FromDateTime(DateTime.Now);
        var salaryUsd = 1000;
        var createUserSalary = new CreateSalaryDto(userId, salaryFrom, salaryUsd, null);

        // Act
        var response = await client.CreateUserSalaryDetailsList([createUserSalary]);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task AddUserSalariesWhenAuthAsGroupManagerAddSalaryForUserNotFromTheSameResourceGroupResponseWithForbiddenResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var userToCreate = new CreateUserInput
        {
            FirstName = Guid.CreateVersion7().ToAlphabetic(),
            LastName = Guid.CreateVersion7().ToAlphabetic(),
            FirstNameRu = "Имя",
            LastNameRu = "Фамилия",
            PatronymicRu = "Отчество",
            Email = $"{Guid.CreateVersion7().ToAlphabetic()}@modsen-software.com",
            Color = "#ffffff",
            Location = LocationType.Minsk,
            Position = PortalRoleNames.Employee,
            DepartmentId = "Net",
            CountryId = 1,
            RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId
        };

        var user = await client.CreateUser(userToCreate);

        await client.AuthAsGroupManager();

        var salaryFrom = DateOnly.FromDateTime(DateTime.Now);
        var salaryUsd = 2000;
        var createUserSalary = new CreateSalaryDto(user.Id, salaryFrom, salaryUsd, null);

        // Act
        var response = await client.CreateUserSalaryDetailsList([createUserSalary]);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
