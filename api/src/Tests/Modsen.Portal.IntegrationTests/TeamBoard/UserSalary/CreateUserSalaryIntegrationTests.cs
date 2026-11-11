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
public class CreateUserSalaryIntegrationTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;

    public CreateUserSalaryIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
    }

    [Fact]
    public async Task AddUserSalaryWhenAuthAsCeoOrCtoResponseWithOkAndSalaryOutputResult()
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
        var userId = user!.Id;

        var salaryFrom = DateOnly.FromDateTime(DateTime.Now);
        var salaryUsd = 1000;
        var createUserSalary = new CreateSalaryDto(userId, salaryFrom, salaryUsd, null);

        // Act
        var responseAfterCreating = await client.CreateUserSalary(createUserSalary);
        var result = await responseAfterCreating.WithResponseType<CreateUserSalaryOutput>();

        // Assert
        responseAfterCreating.StatusCode.Should().Be(HttpStatusCode.OK);
        result.UserId.Should().Be(userId);
        result.SalaryUsd.Should().Be(salaryUsd);
    }

    [Fact]
    public async Task AddUserSalaryWhenAuthAsEmployeeOrSysAdminResponseWithForbiddenResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsEmployee();

        var userId = _factory.EmployeeUserId!.Value;
        var salaryFrom = DateOnly.FromDateTime(DateTime.Now.AddDays(10));
        var salaryUsd = 1000;
        var createUserSalary = new CreateSalaryDto(userId, salaryFrom, salaryUsd, null);

        // Act
        var response = await client.CreateUserSalary(createUserSalary);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task AddUserSalaryWhenAuthAsHeadAddSalaryForUserFromTheSameDepartmentResponseWithOkAndSalaryOutputResult()
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

        var salaryFrom = DateOnly.FromDateTime(DateTime.Now.AddDays(1));
        var salaryUsd = 2000;
        var createUserSalary = new CreateSalaryDto(user!.Id, salaryFrom, salaryUsd, null);

        await client.AuthAsHeadOfDepartment();

        // Act
        var response = await client.CreateUserSalary(createUserSalary);
        var result = await response.WithResponseType<CreateUserSalaryOutput>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.UserId.Should().Be(user.Id);
        result.SalaryUsd.Should().Be(salaryUsd);
    }

    [Fact]
    public async Task AddUserSalaryWhenAuthAsGroupManagerAddSalaryForUserFromTheSameResourceGroupResponseWithForbiddenResult()
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
        var response = await client.CreateUserSalary(createUserSalary);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task AddUserSalaryWhenAuthAsHeadAddSalaryForUserNotFromTheSameDepartmentResponseWithForbiddenResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var createNewUser = new CreateUserInput
        {
            FirstName = "Qwerry",
            LastName = "Qwertwon",
            FirstNameRu = "Имя",
            LastNameRu = "Фамилия",
            PatronymicRu = "Отчество",
            Color = "#ffffff",
            DepartmentId = "Java",
            Email = "Qwerwww@modsen-software.com",
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

        // Act
        var response = await client.CreateUserSalary(new CreateSalaryDto(userId, salaryFrom, salaryUsd, null));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task AddUserSalaryWhenAuthAsGroupManagerAddSalaryForUserNotFromTheSameResourceGroupResponseWithForbiddenResult()
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
        var response = await client.CreateUserSalary(createUserSalary);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
