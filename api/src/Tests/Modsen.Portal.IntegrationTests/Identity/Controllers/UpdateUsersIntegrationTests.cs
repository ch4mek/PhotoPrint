using System.Net;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.PortalSettings;
using Modsen.Portal.Core.SettingNames;
using Modsen.Portal.Identity.Controllers.Contracts.Input;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TimeTracker;
namespace Modsen.Portal.IntegrationTests.Identity.Controllers;

[Collection("IdentityCollection")]
public class UpdateUsersIntegrationTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;
    public UpdateUsersIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
    }

    [Fact]
    public async Task UpdateUserWhenUpdateToEmployeeRoleAndDepartmentIsNotProvidedShouldReturnBadRequest()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var employee = await client.CreateEmployee();

        var userToUpdate = new UpdateUserInput
        {
            Id = employee!.Id,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            FirstNameRu = employee.FirstNameRu,
            LastNameRu = employee.LastNameRu,
            PatronymicRu = employee.PatronymicRu,
            Color = employee.Color,
            Email = employee.Email,
            Position = PortalRoleNames.Employee,
            Location = LocationType.Vitebsk,
            DepartmentId = null,
            RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId
        };

        // Act
        var updateResponse = await client.UpdateUserAsHttpMessageAsync(userToUpdate, _jsonOptions);

        // Assert
        updateResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateUserWhenUpdateToCTORoleAndDepartmentIsProvidedShouldReturnBadRequest()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        var ctoUser = await client.GetCurrentUser();

        var userToUpdate = new UpdateUserInput
        {
            Id = ctoUser.Id,
            FirstName = ctoUser.FirstName,
            LastName = ctoUser.LastName,
            FirstNameRu = ctoUser.FirstNameRu,
            LastNameRu = ctoUser.LastNameRu,
            PatronymicRu = ctoUser.PatronymicRu,
            Color = ctoUser.Color,
            Email = ctoUser.Email,
            Position = ctoUser.Position!,
            Location = LocationType.Vitebsk,
            DepartmentId = "Net",
            RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId
        };

        // Act
        var updateResponse = await client.UpdateUserAsHttpMessageAsync(userToUpdate, _jsonOptions);

        // Assert
        updateResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateUserWhenRequestIsValidReturnUpdatedUser()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var employee = await client.CreateEmployee();

        var userToUpdate = new UpdateUserInput
        {
            Id = employee!.Id,
            FirstName = null,
            LastName = null,
            FirstNameRu = employee.FirstNameRu,
            LastNameRu = employee.LastNameRu,
            PatronymicRu = employee.PatronymicRu,
            Color = "#fdfdfd",
            Email = null,
            Position = PortalRoleNames.Employee,
            Location = LocationType.Minsk,
            DepartmentId = "Net",
            CountryId = 1,
            RegisteredLocationId = employee.RegisteredLocationId
        };

        var expected = new UpdateUserInput
        {
            Id = employee.Id,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            FirstNameRu = employee.FirstNameRu,
            LastNameRu = employee.LastNameRu,
            PatronymicRu = employee.PatronymicRu,
            Color = "#fdfdfd",
            Email = employee.Email,
            Position = PortalRoleNames.Employee,
            Location = LocationType.Minsk,
            DepartmentId = "Net",
            CountryId = 1,
            RegisteredLocationId = employee.RegisteredLocationId
        };

        // Act
        var updatedUser = await client.UpdateUserAsync(userToUpdate);

        // Assert
        updatedUser.Should().NotBeNull();
        updatedUser.Should().BeEquivalentTo(expected, options =>
            options.Excluding(x => x.HireDate));
    }

    [Fact]
    public async Task UpdateUserWhenUserPmoDepartmentIsUpdatedReturnUpdatedUser()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var clientId = _factory.ModsenClientId!.Value;

        var createdUser = await client.CreateEmployee(departmentId: "PMO");
        var employeeId = createdUser!.Id;

        var project = await client.CreateProject(
            clientId: clientId,
            isBillable: false,
            isPublic: false,
            managerId: employeeId);

        var userToUpdate = new UpdateUserInput
        {
            Id = employeeId,
            FirstName = null,
            LastName = null,
            FirstNameRu = createdUser.FirstNameRu,
            LastNameRu = createdUser.LastNameRu,
            PatronymicRu = createdUser.PatronymicRu,
            Color = "#fdfdfd",
            Email = null,
            Position = PortalRoleNames.Employee,
            Location = LocationType.Minsk,
            DepartmentId = "Net",
            CountryId = 1,
            RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId
        };

        var expected = new UpdateUserInput
        {
            Id = createdUser.Id,
            FirstName = createdUser.FirstName,
            LastName = createdUser.LastName,
            FirstNameRu = createdUser.FirstNameRu,
            LastNameRu = createdUser.LastNameRu,
            PatronymicRu = createdUser.PatronymicRu,
            Color = "#fdfdfd",
            Email = createdUser.Email,
            Position = PortalRoleNames.Employee,
            Location = LocationType.Minsk,
            DepartmentId = "Net",
            CountryId = 1,
            RegisteredLocationId = userToUpdate.RegisteredLocationId
        };

        // Act
        var updatedUser = await client.UpdateUserAsync(userToUpdate);

        var projectResponse = await client.GetProjectByIdAsync(project!.Id);

        // Assert
        projectResponse.ManagerId.Should().BeNull();
        updatedUser.Should().NotBeNull();
        updatedUser.Should().BeEquivalentTo(expected, options =>
            options.Excluding(x => x.HireDate));
    }

    [Fact]
    public async Task UpdateUserWhenUserDoesNotExistReturnNotFound()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var userToUpdate = new UpdateUserInput
        {
            Id = 99999,
            Color = "#fdfdfd",
            Position = PortalRoleNames.SystemAdministrator,
            Location = LocationType.Vitebsk,
            DepartmentId = "Administration",
            CountryId = 1,
            RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId
        };

        // Act
        var updateResponse = await client.UpdateUserAsHttpMessageAsync(userToUpdate, _jsonOptions);

        // Assert
        updateResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateUserWhenAuthorizedAsEmployeeReturnForbidden()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var createdUser = await client.CreateEmployee(firstNameRu: "ОбновленноеИмя",
            lastNameRu: "ОбновленнаяФамилия",
            patronymicRu: "ОбновленноеОтчество");

        var userToUpdate = new UpdateUserInput
        {
            Id = createdUser!.Id,
            Color = "#fdfdfd",
            Position = PortalRoleNames.SystemAdministrator,
            Location = LocationType.Vitebsk,
            DepartmentId = "Administration",
            FirstName = "FirstName",
            LastName = "LastName",
            FirstNameRu = "ОбновленноеИмя",
            LastNameRu = "ОбновленнаяФамилия",
            PatronymicRu = "ОбновленноеОтчество",
            CountryId = 1,
            RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId
        };

        await client.AuthAsEmployee();

        // Act
        var updateResponse = await client.UpdateUserAsHttpMessageAsync(userToUpdate, _jsonOptions);

        // Assert
        updateResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UpdateUserWhenHeadTryToUpdateUserInDifferentDepartmentReturnForbiddenResponse()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsHeadOfDepartment();

        var createdUser = await client.CreateEmployee();

        var userToUpdate = new UpdateUserInput
        {
            Id = createdUser!.Id,
            FirstName = null,
            LastName = null,
            FirstNameRu = null,
            LastNameRu = null,
            PatronymicRu = null,
            Color = "#fdfdfd",
            Email = null,
            Position = PortalRoleNames.SystemAdministrator,
            Location = LocationType.Vitebsk,
            DepartmentId = "Js",
            CountryId = 1,
            RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId
        };

        // Act
        var updateResponse = await client.UpdateUserAsHttpMessageAsync(userToUpdate, _jsonOptions);

        // Assert
        updateResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UpdateUserWhenHeadTryToUpdateUsersResourceGroupToDifferentDepartmentReturnForbiddenResponse()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsHeadOfDepartment();

        var createdUser = await client.CreateEmployee();

        var userToUpdate = new UpdateUserInput
        {
            Id = createdUser!.Id,
            FirstName = null,
            LastName = null,
            FirstNameRu = null,
            LastNameRu = null,
            PatronymicRu = null,
            Color = "#fdfdfd",
            Email = null,
            Position = PortalRoleNames.SystemAdministrator,
            Location = LocationType.Vitebsk,
            DepartmentId = "Net",
            ResourceGroupId = 2,
            CountryId = 1,
            RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId
        };

        // Act
        var updateResponse = await client.UpdateUserAsHttpMessageAsync(userToUpdate, _jsonOptions);

        // Assert
        updateResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UpdateUserWhenCeoTryToUpdateUserRoleToCeoReturnForbidden()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var createdUser = await client.CreateEmployee();

        var userToUpdate = new UpdateUserInput
        {
            Id = createdUser!.Id,
            FirstName = null,
            LastName = null,
            FirstNameRu = null,
            LastNameRu = null,
            PatronymicRu = null,
            Color = "#fdfdfd",
            Email = null,
            Position = PortalRoleNames.Ceo,
            Location = LocationType.Vitebsk,
            DepartmentId = null,
            CountryId = 1,
            RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId
        };

        // Act
        var updateResponse = await client.UpdateUserAsHttpMessageAsync(userToUpdate, _jsonOptions);

        // Assert
        updateResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UpdateUserWhenTryToChangeDepartmentOfHeadOfDepartmentAndDepartmentHasOwnHeadReturnForbidden()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        var createdHead = await client.CreateHeadOfDepartment(departmentId: "Java");

        var javaHeadToUpdateWithNetDepartment = new UpdateUserInput
        {
            Id = createdHead!.Id,
            FirstName = null,
            LastName = null,
            FirstNameRu = null,
            LastNameRu = null,
            PatronymicRu = null,
            Color = createdHead.Color,
            Email = null,
            Position = createdHead.Position!,
            Location = createdHead.Location!.Value,
            DepartmentId = "Net",
            CountryId = 1,
            RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId
        };

        // Act
        var updateResponse = await client.UpdateUserAsHttpMessageAsync(javaHeadToUpdateWithNetDepartment, _jsonOptions);

        // Assert
        updateResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UpdateUserWhenTryToChangeGroupOfGmAndGroupHasOwnGmReturnForbidden()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();
        var groupManagerId = _factory.GroupManagerId.GetValueOrDefault();

        var groupManager = await client.GetUserAsync(groupManagerId);

        var groupManagerToUpdate = new UpdateUserInput
        {
            Id = groupManager!.Id,
            FirstName = groupManager.FirstName,
            LastName = groupManager.LastName,
            FirstNameRu = groupManager.FirstNameRu,
            LastNameRu = groupManager.LastNameRu,
            PatronymicRu = groupManager.PatronymicRu,
            Color = groupManager.Color,
            Email = null,
            Position = PortalRoleNames.GroupManager,
            Location = groupManager.Location,
            DepartmentId = groupManager.DepartmentId,
            ResourceGroupId = 2,
            CountryId = 1,
            RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId
        };

        // Act
        var updateResponse = await client.UpdateUserAsHttpMessageAsync(groupManagerToUpdate, _jsonOptions);

        // Assert
        updateResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UpdateUserWhenCtoTryToUpdateUserWithCeoRoleReturnForbiddenResponse()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        var ceoId = _factory.CeoUserId;

        var userToUpdate = new UpdateUserInput
        {
            Id = ceoId!.Value,
            FirstName = null,
            LastName = null,
            FirstNameRu = null,
            LastNameRu = null,
            PatronymicRu = null,
            Color = "#fdfdfd",
            Email = null,
            Position = PortalRoleNames.SystemAdministrator,
            Location = LocationType.Vitebsk,
            DepartmentId = "Administration",
            CountryId = 1,
            RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId
        };

        // Act
        var updateResponse = await client.UpdateUserAsHttpMessageAsync(userToUpdate, _jsonOptions);

        // Assert
        updateResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task PutUserSignatureWhenNotAuthorizedShouldReturnNotAuthorizedResponse()
    {
        // Arrange
        using var client = _factory.CreateClient();

        // Act=
        var updateSignatureResponse = await client.UpdateUserSignatureAsHttpMessageAsync(
            fullName: "full name",
            phone: "+375292006791",
            position: "CEO of the world");

        // Assert
        updateSignatureResponse.IsSuccessStatusCode.Should().BeFalse();
        updateSignatureResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task PutUserSignatureWhenAuthAsCeoAndSendInvalidModelShouldReturnBadRequestResult()
    {
        // Arrange
        using var client = _factory.CreateClient();

        // Act
        await client.AuthAsCeo();

        var updateSignatureResponse = await client.UpdateUserSignatureAsHttpMessageAsync(
            fullName: "full name",
            position: "CEO of the world",
            phone: null);

        // Assert
        updateSignatureResponse.IsSuccessStatusCode.Should().BeFalse();
        updateSignatureResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task PutThenGetUserSignatureWhenAuthAsCeoAndUpdateSignatureShouldReturnUpdatedSignature()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        // Act
        var updateSignatureResponse = await client.UpdateUserSignatureAsync(
            fullName: "full name",
            phone: "+375292006791",
            position: "CEO of the world");


        // Assert
        var getSignatureResponse = await client.GetUserSignatureAsync();
        getSignatureResponse.Should().NotBeNull();
        getSignatureResponse.Should().BeEquivalentTo(updateSignatureResponse);
    }

    [Fact]
    public async Task UpdateUserAsyncWhenUpdatingUserIsCtoShouldReturnOkResult()
    {
        //Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var ctoId = _factory.CtoUserId!.Value;

        var ctoOutput = await client.GetUserAsync(ctoId);

        var updateUserInput = new UpdateUserInput
        {
            Id = _factory.CtoUserId!.Value,
            FirstName = null,
            LastName = null,
            FirstNameRu = null,
            LastNameRu = null,
            PatronymicRu = null,
            Email = null,
            Color = ctoOutput.Color,
            Position = ctoOutput.Position!,
            Location = ctoOutput.Location,
            DepartmentId = null,
            CountryId = 1,
            RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId
        };

        //Act
        var response = await client.UpdateUserAsHttpMessageAsync(updateUserInput, _jsonOptions);

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdateUserAsyncWhenAuthedAsHeadOfSysAdminDepartmentAndUserToUpdateIsSysAdminShouldReturnOk()
    {
        //Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        using var scope = _factory.Services.CreateScope();
        var portalSettingsAccessor = scope.ServiceProvider.GetRequiredService<IPortalSettingsAccessor>();
        var departmentId = await portalSettingsAccessor.GetStringValueByName(
            DepartmentSettingNames.SystemAdministrationDepartmentId, CancellationToken.None);

        var createdAdmin = await client.CreateSysAdmin(departmentId: departmentId);

        await client.AuthAsSysAdminHead();

        var updateAdminInput = new UpdateUserInput
        {
            Id = createdAdmin!.Id,
            FirstName = null,
            LastName = null,
            FirstNameRu = null,
            LastNameRu = null,
            PatronymicRu = null,
            Email = null,
            Color = "#abcdef",
            Position = createdAdmin.Position!,
            Location = createdAdmin.Location,
            DepartmentId = createdAdmin.DepartmentId,
            CountryId = 1,
            RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId
        };

        //Act
        var updatedAdmin = await client.UpdateUserAsync(updateAdminInput);

        //Assert
        updatedAdmin.Color.Should().Be(updateAdminInput.Color);
    }

    [Fact]
    public async Task UpdateUserAsyncWhenAuthedAsCeoAndSetGroupManagerResourceGroupToNullShouldReturnOk()
    {
        //Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var createGroupResponse = await client.CreateResourceGroup();
        var groupId = createGroupResponse.Id;

        var createdGm = await client.CreateGroupManager(resourceGroupId: groupId);

        var updateGmInput = new UpdateUserInput
        {
            Id = createdGm!.Id,
            FirstName = null,
            LastName = null,
            FirstNameRu = null,
            LastNameRu = null,
            PatronymicRu = null,
            Email = null,
            Color = "#ffffff",
            Position = createdGm.Position!,
            Location = createdGm.Location,
            DepartmentId = createdGm.DepartmentId,
            ResourceGroupId = null,
            CountryId = 1,
            RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId
        };

        //Act
        var updatedGm = await client.UpdateUserAsync(updateGmInput);

        //Assert
        updatedGm.ResourceGroupId.Should().NotBe(createGroupResponse?.Id);

    }

    [Fact]
    public async Task UpdateUserAsyncWhenSetGroupManagerToResourceGroupWhichAlreadyHasGroupManagerShouldReturnForbidden()
    {
        //Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var createGroupResponse = await client.CreateResourceGroup();
        var groupId = createGroupResponse.Id;

        await client.CreateGroupManager(resourceGroupId: groupId);

        var secondGm = await client.CreateGroupManager();

        var updateGmInput = new UpdateUserInput
        {
            Id = secondGm!.Id,
            FirstName = null,
            LastName = null,
            FirstNameRu = null,
            LastNameRu = null,
            PatronymicRu = null,
            Email = null,
            Color = "#ffffff",
            Position = secondGm.Position!,
            Location = secondGm.Location,
            DepartmentId = secondGm.DepartmentId,
            ResourceGroupId = createGroupResponse?.Id,
            CountryId = 1,
            RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId
        };

        //Act
        var response = await client.UpdateUserAsHttpMessageAsync(updateGmInput, _jsonOptions);

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UpdateUserAsyncWhenSetGroupManagerToResourceGroupWhichHasNoGroupManagerShouldReturnOk()
    {
        //Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var createGroupResponse = await client.CreateResourceGroup();
        var groupId = createGroupResponse.Id;

        var createdGm = await client.CreateGroupManager();

        var updateGmInput = new UpdateUserInput
        {
            Id = createdGm!.Id,
            FirstName = null,
            LastName = null,
            FirstNameRu = null,
            LastNameRu = null,
            PatronymicRu = null,
            Email = null,
            Color = "#ffffff",
            Position = createdGm.Position!,
            Location = createdGm.Location,
            DepartmentId = createdGm.DepartmentId,
            ResourceGroupId = groupId,
            CountryId = 1,
            RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId
        };

        //Act
        var updatedGm = await client.UpdateUserAsync(updateGmInput);

        //Assert
        updatedGm.ResourceGroupId.Should().Be(createGroupResponse.Id);
    }

    [Fact]
    public async Task UpdateUserAsyncWhenSetGroupManagerToEmployeeShouldReturnOk()
    {
        //Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var createGroupResponse = await client.CreateResourceGroup();
        var groupId = createGroupResponse.Id;

        var createdGm = await client.CreateGroupManager(resourceGroupId: groupId);

        var updateGmInput = new UpdateUserInput
        {
            Id = createdGm!.Id,
            FirstName = null,
            LastName = null,
            FirstNameRu = null,
            LastNameRu = null,
            PatronymicRu = null,
            Email = null,
            Color = "#ffffff",
            Position = PortalRoleNames.Employee,
            Location = createdGm.Location,
            DepartmentId = createdGm.DepartmentId,
            ResourceGroupId = groupId,
            CountryId = 1,
            RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId
        };

        //Act
        var updatedGm = await client.UpdateUserAsync(updateGmInput);

        //Assert
        updatedGm.ResourceGroupId.Should().Be(createGroupResponse.Id);
    }

    [Fact]
    public async Task UpdateUserAsyncWhenSetEmployeeToGroupManagerWithResourceGroupWhichAlreadyHasGroupManagerShouldReturnForbidden()
    {
        //Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var createGroupResponse = await client.CreateResourceGroup();
        var groupId = createGroupResponse.Id;

        await client.CreateGroupManager(resourceGroupId: groupId);

        var createdEmployee = await client.CreateEmployee();

        var updateEmployeeInput = new UpdateUserInput
        {
            Id = createdEmployee!.Id,
            FirstName = null,
            LastName = null,
            FirstNameRu = null,
            LastNameRu = null,
            PatronymicRu = null,
            Email = null,
            Color = "#ffffff",
            Position = PortalRoleNames.GroupManager,
            Location = createdEmployee.Location,
            DepartmentId = createdEmployee.DepartmentId,
            ResourceGroupId = groupId,
            CountryId = 1,
            RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId
        };

        //Act
        var response = await client.UpdateUserAsHttpMessageAsync(updateEmployeeInput, _jsonOptions);

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UpdateUserAsyncWhenSetEmployeeToGroupManagerWithResourceGroupWhichHasNoGroupManagerShouldReturnOk()
    {
        //Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var createGroupResponse = await client.CreateResourceGroup();
        var groupId = createGroupResponse.Id;

        var employee = await client.CreateEmployee();

        var updateGmInput = new UpdateUserInput
        {
            Id = employee!.Id,
            FirstName = null,
            LastName = null,
            FirstNameRu = null,
            LastNameRu = null,
            PatronymicRu = null,
            Email = null,
            Color = "#ffffff",
            Position = PortalRoleNames.GroupManager,
            Location = employee.Location,
            DepartmentId = employee.DepartmentId,
            ResourceGroupId = groupId,
            CountryId = 1,
            RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId
        };

        //Act
        var updatedGm = await client.UpdateUserAsync(updateGmInput);

        //Assert
        updatedGm.ResourceGroupId.Should().Be(groupId);
    }

    [Fact]
    public async Task UpdateUserAsyncWhenSetEmployeeToGroupManagerWithNoResourceGroupShouldReturnOk()
    {
        //Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var createdEmployee = await client.CreateEmployee();

        var updateGmInput = new UpdateUserInput
        {
            Id = createdEmployee!.Id,
            FirstName = null,
            LastName = null,
            FirstNameRu = null,
            LastNameRu = null,
            PatronymicRu = null,
            Email = null,
            Color = "#ffffff",
            Position = PortalRoleNames.GroupManager,
            Location = createdEmployee.Location,
            DepartmentId = createdEmployee.DepartmentId,
            ResourceGroupId = null,
            CountryId = 1,
            RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId
        };

        //Act
        var updatedGm = await client.UpdateUserAsync(updateGmInput);

        //Assert
        updatedGm.ResourceGroupId.Should().BeNull();
        updatedGm.Position.Should().Be(PortalRoleNames.GroupManager);
    }

    [Fact]
    public async Task UpdateUserAsyncWhenSetGroupManagerToGroupFromAnotherDepartmentShouldReturnForbidden()
    {
        //Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();
        const string departmentId = "Java";

        var createGroupResponse = await client.CreateResourceGroup(departmentId: departmentId);
        var groupId = createGroupResponse.Id;

        var createdGm = await client.CreateGroupManager();

        var updateGmInput = new UpdateUserInput
        {
            Id = createdGm!.Id,
            FirstName = null,
            LastName = null,
            FirstNameRu = null,
            LastNameRu = null,
            PatronymicRu = null,
            Email = null,
            Color = "#ffffff",
            Position = PortalRoleNames.GroupManager,
            Location = createdGm.Location,
            DepartmentId = createdGm.DepartmentId,
            ResourceGroupId = groupId,
            CountryId = 1,
            RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId
        };


        //Act
        var updatedGm = await client.UpdateUserAsHttpMessageAsync(updateGmInput, _jsonOptions);

        //Assert
        updatedGm.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UpdateUserAsyncWhenSetGroupManagerToAnotherDepartmentAndDontChangeGroupShouldReturnForbidden()
    {
        //Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var createGroupResponse = await client.CreateResourceGroup();
        var groupId = createGroupResponse.Id;

        var createdGm = await client.CreateGroupManager(resourceGroupId: groupId);

        var updateGmInput = new UpdateUserInput
        {
            Id = createdGm!.Id,
            FirstName = null,
            LastName = null,
            FirstNameRu = null,
            LastNameRu = null,
            PatronymicRu = null,
            Email = null,
            Color = "#ffffff",
            Position = PortalRoleNames.GroupManager,
            Location = createdGm.Location,
            DepartmentId = "Java",
            ResourceGroupId = createdGm.ResourceGroupId,
            CountryId = 1,
            RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId
        };

        //Act
        var response = await client.UpdateUserAsHttpMessageAsync(updateGmInput, _jsonOptions);

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UpdateUserAsyncWhenSetEmployeeToAnotherDepartmentAndDontChangeGroupShouldReturnForbidden()
    {
        //Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var createGroupResponse = await client.CreateResourceGroup();
        var groupId = createGroupResponse.Id;

        var createdEmployee = await client.CreateEmployee(resourceGroupId: groupId);

        var updateEmployeeInput = new UpdateUserInput
        {
            Id = createdEmployee!.Id,
            FirstName = null,
            LastName = null,
            FirstNameRu = null,
            LastNameRu = null,
            PatronymicRu = null,
            Email = null,
            Color = "#ffffff",
            Position = PortalRoleNames.Employee,
            Location = createdEmployee.Location,
            DepartmentId = "Java",
            ResourceGroupId = createdEmployee.ResourceGroupId,
            CountryId = 1,
            RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId
        };

        //Act
        var response = await client.UpdateUserAsHttpMessageAsync(updateEmployeeInput, _jsonOptions);

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UpdateUserAsyncWhenSetGroupManagerToAnotherDepartmentAndPassNullGroupShouldResponseOk()
    {
        //Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var createGroupResponse = await client.CreateResourceGroup();
        var groupId = createGroupResponse.Id;

        var createdGm = await client.CreateGroupManager(resourceGroupId: groupId);

        var updateGmInput = new UpdateUserInput
        {
            Id = createdGm!.Id,
            FirstName = null,
            LastName = null,
            FirstNameRu = null,
            LastNameRu = null,
            PatronymicRu = null,
            Email = null,
            Color = "#ffffff",
            Position = PortalRoleNames.GroupManager,
            Location = createdGm.Location,
            DepartmentId = "Java",
            ResourceGroupId = null,
            CountryId = 1,
            RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId
        };

        //Act
        var response = await client.UpdateUserAsHttpMessageAsync(updateGmInput, _jsonOptions);

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdateUserAsyncWhenSetEmployeeToAnotherDepartmentAndPassNullGroupShouldResponseOk()
    {
        //Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var createGroupResponse = await client.CreateResourceGroup();
        var groupId = createGroupResponse.Id;

        var createdEmployee = await client.CreateEmployee(resourceGroupId: groupId);

        var updateEmployeeInput = new UpdateUserInput
        {
            Id = createdEmployee!.Id,
            FirstName = null,
            LastName = null,
            FirstNameRu = null,
            LastNameRu = null,
            PatronymicRu = null,
            Email = null,
            Color = "#ffffff",
            Position = PortalRoleNames.Employee,
            Location = createdEmployee.Location,
            DepartmentId = "Java",
            ResourceGroupId = null,
            CountryId = 1,
            RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId
        };

        //Act
        var response = await client.UpdateUserAsHttpMessageAsync(updateEmployeeInput, _jsonOptions);

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdateUserAsyncWhenSetGroupManagerToAnotherDepartmentAndPassNewGroupFromNewDepartmentShouldResponseOk()
    {
        //Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();
        const string departmentId = "Java";

        var createGroupResponse = await client.CreateResourceGroup(departmentId: departmentId);

        var createdGm = await client.CreateGroupManager();

        var updateGmInput = new UpdateUserInput
        {
            Id = createdGm!.Id,
            FirstName = null,
            LastName = null,
            FirstNameRu = null,
            LastNameRu = null,
            PatronymicRu = null,
            Email = null,
            Color = "#ffffff",
            Position = PortalRoleNames.GroupManager,
            Location = createdGm.Location,
            DepartmentId = "Java",
            ResourceGroupId = createGroupResponse.Id,
            CountryId = 1,
            RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId
        };

        //Act
        var response = await client.UpdateUserAsHttpMessageAsync(updateGmInput, _jsonOptions);

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdateUserAsyncWhenSetEmployeeToAnotherDepartmentAndPassNewGroupFromNewDepartmentShouldResponseOk()
    {
        //Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        const string oldGroupDepartment = "Net";
        const string newGroupDepartment = "Java";

        var createOldGroupResponse = await client.CreateResourceGroup(departmentId: oldGroupDepartment);
        var oldGroupId = createOldGroupResponse.Id;

        var createNewGroupResponse = await client.CreateResourceGroup(departmentId: newGroupDepartment);
        var newGroupId = createNewGroupResponse.Id;

        var createdEmployee = await client.CreateEmployee(resourceGroupId: oldGroupId);

        var updateEmployeeInput = new UpdateUserInput
        {
            Id = createdEmployee!.Id,
            FirstName = null,
            LastName = null,
            FirstNameRu = null,
            LastNameRu = null,
            PatronymicRu = null,
            Email = null,
            Color = "#ffffff",
            Position = PortalRoleNames.GroupManager,
            Location = createdEmployee.Location,
            DepartmentId = "Java",
            ResourceGroupId = newGroupId,
            CountryId = 1,
            RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId
        };

        //Act
        var response = await client.UpdateUserAsHttpMessageAsync(updateEmployeeInput, _jsonOptions);

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
