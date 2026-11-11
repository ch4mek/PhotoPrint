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
public class UpdateUserSalaryDetailsListTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;

    public UpdateUserSalaryDetailsListTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
    }

    [Fact]
    public async Task UpdateUserSalariesWhenAuthAsCeoOrCtoResponseWithOkAndOutputResult()
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

        var salaryFrom = DateOnly.FromDateTime(DateTime.Now.AddDays(5));
        var oldSalary = 100;
        var newSalary = 1000;

        var newSalaryFrom = DateOnly.FromDateTime(DateTime.Now.AddDays(6));

        var createUserSalary = new CreateSalaryDto(userId, salaryFrom, oldSalary, null);

        var responseAfterCreating = await client.CreateUserSalary(createUserSalary)
            .WithResponseType<CreateUserSalaryOutput>();

        var response = await client
            .UpdateUserSalaryDetailsList([new UpdateSalaryDto(responseAfterCreating.Id, userId, newSalaryFrom, newSalary)]);
        var result = await response.WithResponseType<List<UpdateUserSalaryOutput>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.First().UserId.Should().Be(userId);
    }

    [Fact]
    public async Task UpdateUserSalariesWhenAuthAsEmployeeOrSysAdminResponseWithForbiddenResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var userId = _factory.EmployeeUserId!.Value;

        var salaryFrom = DateOnly.FromDateTime(DateTime.Now.AddDays(7));
        var oldSalary = 100;
        var newSalary = 1000;

        var newSalaryFrom = DateOnly.FromDateTime(DateTime.Now.AddDays(8));

        var createUserSalary = new CreateSalaryDto(userId, salaryFrom, oldSalary, null);

        var responseAfterCreating = await client.CreateUserSalary(createUserSalary)
            .WithResponseType<CreateUserSalaryOutput>();

        // Act
        await client.AuthAsEmployee();
        var responseAfterUpdating = await client
            .UpdateUserSalaryDetailsList([new UpdateSalaryDto(responseAfterCreating.Id, userId, newSalaryFrom, newSalary)]);

        // Assert
        responseAfterUpdating.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UpdateUserSalariesWhenAuthAsHeadUpdateSalaryForUserFromTheSameDepartmentResponseWithOkAndSalaryOutputResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsHeadOfDepartment();

        var userId = _factory.EmployeeUserId!.Value;
        var salaryFrom = DateOnly.FromDateTime(DateTime.Now.AddDays(9));
        var oldSalary = 100;
        var newSalary = 1000;

        var newSalaryFrom = DateOnly.FromDateTime(DateTime.Now.AddDays(10));

        var createUserSalary = new CreateSalaryDto(userId, salaryFrom, oldSalary, null);

        var responseAfterCreating = await client.CreateUserSalary(createUserSalary)
            .WithResponseType<CreateUserSalaryOutput>();

        // Act
        var response = await client.UpdateUserSalaryDetailsList([new UpdateSalaryDto(responseAfterCreating.Id, userId, newSalaryFrom, newSalary)]);
        var result = await response.WithResponseType<List<UpdateUserSalaryOutput>>();

        // Assert
        result.First().UserId.Should().Be(userId);
        result.First().SalaryUsd.Should().Be(newSalary);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdateUserSalariesWhenSameSalaryDateAlreadyExistsResponseWithForbiddenResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var userId = _factory.EmployeeUserId!.Value;

        var salaryFrom = DateOnly.FromDateTime(DateTime.Now.AddDays(31));
        var oldSalary = 100;
        var newSalary = 1000;

        var newSalaryFrom = DateOnly.FromDateTime(DateTime.Now.AddDays(30));

        var createUserSalary = new CreateSalaryDto(userId, salaryFrom, oldSalary, null);

        await client.CreateUserSalary(new CreateSalaryDto(userId, newSalaryFrom, oldSalary, null));

        var responseAfterCreating = await client.CreateUserSalary(createUserSalary)
            .WithResponseType<CreateUserSalaryOutput>();

        // Act
        var response = await client
            .UpdateUserSalaryDetailsList([new UpdateSalaryDto(responseAfterCreating.Id, userId, newSalaryFrom, newSalary)]);
       // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task UpdateUserSalariesWhenAuthAsGroupManagerUpdateSalaryForUserFromTheSameResourceGroupResponseWithForbiddenResult()
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

        var salaryFrom = DateOnly.FromDateTime(DateTime.Now);
        var oldSalary = 100;
        var newSalary = 1000;

        var newSalaryFrom = DateOnly.FromDateTime(DateTime.Now.AddDays(1));

        var createUserSalary = new CreateSalaryDto(user.Id, salaryFrom, oldSalary, null);

        var responseAfterCreating = await client.CreateUserSalary(createUserSalary)
            .WithResponseType<CreateUserSalaryOutput>();

        await client.AuthAsGroupManager();

        // Act
        var responseAfterUpdating = await client.UpdateUserSalaryDetailsList([new UpdateSalaryDto(responseAfterCreating.Id, user.Id, newSalaryFrom, newSalary)]);

        // Assert
        responseAfterUpdating.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UpdateUserSalariesWhenAuthAsHeadUpdateSalaryForUserNotFromTheSameDepartmentResponseWithForbiddenResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var salaryFrom = DateOnly.FromDateTime(DateTime.Now);
        var oldSalary = 100;
        var newSalary = 1000;

        var newSalaryFrom = DateOnly.FromDateTime(DateTime.Now.AddDays(1));

        var createNewUser = new CreateUserInput
        {
            FirstName = "Qwerty",
            LastName = "Qwerton",
            FirstNameRu = "Имя",
            LastNameRu = "Фамилия",
            PatronymicRu = "Отчество",
            Color = "#ffffff",
            DepartmentId = "Java",
            Email = "Qwertoeww@modsen-software.com",
            Location = LocationType.Minsk,
            Position = PortalRoleNames.Employee,
            CountryId = 1,
            RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId
        };

        // Act
        var responseAfterCreatingUser = await client.CreateUser(createNewUser);

        var userId = responseAfterCreatingUser.Id;

        var createUserSalary = new CreateSalaryDto(userId, salaryFrom, oldSalary, null);

        var responseAfterCreating = await client.CreateUserSalary(createUserSalary)
            .WithResponseType<CreateUserSalaryOutput>();

        // Act
        await client.AuthAsHeadOfDepartment();

        var responseAfterUpdatingSalary = await client
            .UpdateUserSalaryDetailsList([new UpdateSalaryDto(responseAfterCreating.Id, userId, newSalaryFrom, newSalary)]);

        // Assert
        responseAfterUpdatingSalary.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UpdateUserSalariesWhenAuthAsGroupManagerUpdateSalaryForUserNotFromTheSameResourceGroupResponseWithForbiddenResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var salaryFrom = DateOnly.FromDateTime(DateTime.Now);
        var oldSalary = 100;
        var newSalary = 1000;

        var createNewUser = new CreateUserInput
        {
            FirstName = Guid.CreateVersion7().ToAlphabetic(),
            LastName = Guid.CreateVersion7().ToAlphabetic(),
            FirstNameRu = "Имя",
            LastNameRu = "Фамилия",
            PatronymicRu = "Отчество",
            Email = $"{Guid.CreateVersion7().ToAlphabetic()}@modsen-software.com",
            Color = "#ffffff",
            DepartmentId = "Net",
            Location = LocationType.Minsk,
            Position = PortalRoleNames.Employee,
            CountryId = 1,
            RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId
        };

        var newSalaryFrom = DateOnly.FromDateTime(DateTime.Now.AddDays(1));

        // Act
        var responseAfterCreatingUser = await client.CreateUser(createNewUser);

        var userId = responseAfterCreatingUser.Id;

        var createUserSalary = new CreateSalaryDto(userId, salaryFrom, oldSalary, null);

        var responseAfterCreating = await client.CreateUserSalary(createUserSalary)
            .WithResponseType<CreateUserSalaryOutput>();

        // Act
        await client.AuthAsGroupManager();

        var responseAfterUpdatingSalary = await client.UpdateUserSalaryDetailsList([new UpdateSalaryDto(responseAfterCreating.Id, userId, newSalaryFrom, newSalary)]);

        // Assert
        responseAfterUpdatingSalary.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
