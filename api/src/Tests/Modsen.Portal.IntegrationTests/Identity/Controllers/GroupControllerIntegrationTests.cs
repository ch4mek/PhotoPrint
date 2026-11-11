using System.Net;
using System.Text.Json;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
namespace Modsen.Portal.IntegrationTests.Identity.Controllers;

[Collection("GeneralCollection")]
public class GroupControllerIntegrationTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;

    public GroupControllerIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
    }

    [Fact]
    public async Task GetGroupByIdWhenAuthorizedAsEmployeeReturnUnauthorizedResponse()
    {
        // Arrange
        using var client = _factory.CreateClient();

        // Act
        var response = await client.GetResourceGroupAsHttpMessageAsync(1);

        // Assert
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }


    [Fact]
    public async Task GetGroupByIdWhenAuthorizedAsCeoReturnGroup()
    {
        // Arrange
        using var client = _factory.CreateClient();
        var name = Guid.CreateVersion7().ToAlphabetic();

        await client.AuthAsCeo();

        var resourceGroup = await client.CreateResourceGroup(name: name);
        var groupId = resourceGroup.Id;

        // Act
        var response = await client.GetResourceGroupAsync(groupId);

        // Assert
        response.Should().NotBeNull();
        response.Name.Should().BeEquivalentTo(name);
    }

    [Fact]
    public async Task GetGroupByIdWhenAuthorizedAsCeoAndGroupNotFoundReturnNotFound()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        const int nonExistingGroupId = 9999;

        // Act
        var response = await client.GetResourceGroupAsHttpMessageAsync(nonExistingGroupId);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetGroupByIdWhenGroupWithoutGroupManagerReturnGroup()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var name = Guid.CreateVersion7().ToAlphabetic();

        var resourceGroupOutput = await client.CreateResourceGroup(name: name);
        var groupId = resourceGroupOutput.Id;

        // Act
        var group = await client.GetResourceGroupAsync(groupId);

        // Assert
        group!.GroupManagerId.Should().BeNull();
        group.Name.Should().BeEquivalentTo(name);
    }

    [Fact]
    public async Task CreateGroupWhenAuthorizedAsCeoAndRequestValidReturnCreatedGroupId()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var name = Guid.CreateVersion7().ToAlphabetic();

        // Act
        var output = await client.CreateResourceGroup(name: name);

        // Assert
        output.Should().NotBeNull();
        output.Id.Should().BeOfType(typeof(int));
    }

    [Fact]
    public async Task CreateGroupWhenAuthorizedAsHeadAndPassAnotherDepartmentIdResponseWithForbidden()
    {
        // Arrange
        using var client = _factory.CreateClient();

        var groupManagerId = _factory.GroupManagerId.GetValueOrDefault();
        const string departmentId = "Java";
        var groupName = Guid.CreateVersion7().ToAlphabetic();


        await client.AuthAsHeadOfDepartment();

        // Act
        var response = await client.CreateResourceGroupAsHttpMessageAsync(name: groupName,
            departmentId: departmentId,
            managerId: groupManagerId);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task CreateGroupWhenGmHasAnotherActiveGroupReturnBadRequest()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var groupName = Guid.CreateVersion7().ToAlphabetic();

        var createdGroup = await client.CreateResourceGroup(name: groupName);
        var createdGm = await client.CreateGroupManager(resourceGroupId: createdGroup.Id);

        // Act
        var response = await client.CreateResourceGroupAsHttpMessageAsync(managerId: createdGm!.Id);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateGroupWhenGmHasNoActiveGroupResponseWithOk()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var createGmOutput = await client.CreateGroupManager();

        // Act
        var groupOutput = await client.CreateResourceGroup(managerId: createGmOutput!.Id);

        // Assert
        groupOutput.Should().NotBeNull();
        groupOutput.Id.Should().BeOfType(typeof(int));
    }

    [Fact]
    public async Task CreateGroupWhenGroupNameIsNotUniqueReturnBadRequest()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var groupName = Guid.CreateVersion7().ToAlphabetic();

        await client.CreateResourceGroup(name: groupName);

        // Act
        var response = await client.CreateResourceGroupAsHttpMessageAsync(name: groupName);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateGroupWhenGroupManagerIdIsNullCreateAGroupWithoutGroupManager()
    {
        // Arrange
        using var client = _factory.CreateClient();
        var groupName = Guid.CreateVersion7().ToAlphabetic();

        await client.AuthAsCeo();

        // Act
        var output = await client.CreateResourceGroup(name: groupName);

        // Assert
        output.Should().NotBeNull();
        output.Id.Should().BeOfType(typeof(int));

        var response = await client.GetResourceGroupAsync(output.Id);

        response.GroupManagerId.Should().BeNull();
        response.Name.Should().BeEquivalentTo(groupName);

    }

    [Fact]
    public async Task CreateGroupWhenGroupManagerDepartmentIsNotEqualToGroupDepartmentReturnBadRequest()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();
        const string departmentId = "Java";

        var groupManager = await client.CreateGroupManager();

        // Act
        var response = await client.CreateResourceGroupAsHttpMessageAsync(
            departmentId: departmentId,
            managerId: groupManager!.Id);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

    }

    [Fact]
    public async Task DeleteGroupWhenNoGroupIsEmptyResponseOk()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var output = await client.CreateResourceGroup();

        // Act
        var isSuccess = await client.DeleteResourceGroup(output.Id);

        // Assert
        isSuccess.IsSuccessStatusCode.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateGroupWhenUpdateOnlyNameUpdateName()
    {
        //Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();


        var groupName = Guid.CreateVersion7().ToAlphabetic();
        var newGroupName = Guid.CreateVersion7().ToAlphabetic();

        var output = await client.CreateResourceGroup(name: groupName);

        //Act
        var updatedGroup = await client.UpdateResourceGroupAsync(id: output.Id, name: newGroupName);

        //Assert
        updatedGroup.GroupManagerId.Should().BeNull();
        updatedGroup.Name.Should().BeEquivalentTo(newGroupName);
    }

    [Fact]
    public async Task UpdateGroupWhenUpdateDepartamentResponseWithBadRequest()
    {
        //Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var output = await client.CreateResourceGroup();

        //Act
        var response = await client.UpdateResourceGroupAsHttpMessageAsync(output.Id, name: "new", departmentId: "Java");

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateGroupWhenHeadFromAnotherDepartmentResponseWithForbidden()
    {
        //Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();
        const string departmentId = "Js";

        var output = await client.CreateResourceGroup(departmentId: departmentId);

        await client.AuthAsHeadOfDepartment();

        //Act
        var response = await client.UpdateResourceGroupAsHttpMessageAsync(output.Id,
            departmentId: departmentId,
            name: "new");

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UpdateGroupWhenGroupNotExistReturnNotFound()
    {
        //Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();
        const int notExistingGroupId = 999;

        //Act
        var response = await client.UpdateResourceGroupAsHttpMessageAsync(notExistingGroupId);

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateGroupWhenNewGmHasNoActiveGroupResponseWithOk()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var createGmOutput = await client.CreateGroupManager();
        var resourceGroup = await client.CreateResourceGroup();

        // Act
        var updateGroupResponse = await client.UpdateResourceGroupAsHttpMessageAsync(id: resourceGroup.Id,
            managerId: createGmOutput.Id);

        // Assert
        updateGroupResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdateGroupWhenNewGroupManagerIdNullResponseWithOk()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var name = Guid.CreateVersion7().ToAlphabetic();

        var createGmOutput = await client.CreateGroupManager();
        var resourceGroup = await client.CreateResourceGroup(managerId: createGmOutput.Id, name: name);

        // Act
        var updateGroupResponse = await client.UpdateResourceGroupAsync(id: resourceGroup.Id, name: name);

        // Assert
        updateGroupResponse.GroupManagerId.Should().BeNull();
        updateGroupResponse.Id.Should().Be(resourceGroup.Id);
        updateGroupResponse.Name.Should().Be(name);
    }

    [Fact]
    public async Task UpdateGroupWhenNewGmHasActiveGroupResponseBadRequest()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var createGmOutput = await client.CreateGroupManager();

        await client.CreateResourceGroup(managerId: createGmOutput.Id);

        var resourceGroup = await client.CreateResourceGroup();

        // Act
        var updateGroupResponse = await client.UpdateResourceGroupAsHttpMessageAsync(
            id: resourceGroup.Id,
            managerId: createGmOutput.Id);

        // Assert
        updateGroupResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteGroupWhenGroupManagerIsInGroupResponseWithForbidden()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var createGmOutput = await client.CreateGroupManager();

        var resourceGroup = await client.CreateResourceGroup(managerId: createGmOutput.Id);

        // Act
        var deleteResponse = await client.DeleteResourceGroup(resourceGroup.Id);

        // Assert
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task DeleteGroupWhenEmployeeIsInGroupResponseWithForbidden()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var resourceGroup = await client.CreateResourceGroup();
        var groupId = resourceGroup.Id;

        await client.CreateEmployee(resourceGroupId: groupId);

        // Act
        var deleteResponse = await client.DeleteResourceGroup(groupId);

        // Assert
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetGroupMembersWhenEmployeeIsInGroupReturnResponseWithEmployee()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var resourceGroup = await client.CreateResourceGroup();
        var groupId = resourceGroup.Id;

        var createEmployeeOutput = await client.CreateEmployee(resourceGroupId: groupId);
        var createSecondEmployeeOutput = await client.CreateEmployee(resourceGroupId: groupId);

        // Act
        var groupMembers = await client.GetResourceGroupMembersAsync(id: groupId);

        // Assert
        groupMembers.Items.Count.Should().Be(2);
        groupMembers.Items.Any(m => m.Email == createEmployeeOutput.Email).Should().BeTrue();
        groupMembers.Items.Any(m => m.Email == createSecondEmployeeOutput.Email).Should().BeTrue();
    }

    [Fact]
    public async Task AddUserToGroupAsyncWhenAuthorizedAsCeoAndRequestValidReturnEmployeeWithNewGroup()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var employee = await client.CreateEmployee();
        var createGroupOutput = await client.CreateResourceGroup();

        // Act
        var updatedEmployee = await client.AddUserToResourceGroupAsync(createGroupOutput.Id, employee!.Id);

        // Assert
        updatedEmployee.ResourceGroupId.Should().Be(createGroupOutput.Id);
    }

    [Fact]
    public async Task AddUserToGroupAsyncWhenEmployeeIsAlreadyInGroupReturnForbidden()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var employee = await client.CreateEmployee();
        var userId = employee!.Id;

        var createGroupOutput = await client.CreateResourceGroup();
        var groupId = createGroupOutput.Id;

        await client.AddUserToResourceGroupAsync(groupId: groupId, userId: userId);

        // Act
        var response = await client.AddUserToResourceGroupAsHttpMessageAsync(groupId: groupId,
            userId: userId);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task AddUserToGroupAsyncWhenEmployeeIsAlreadyInAnotherGroupReturnForbidden()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var employee = await client.CreateEmployee();
        var userId = employee!.Id;

        var firstGroup = await client.CreateResourceGroup();
        var secondGroup = await client.CreateResourceGroup();

        await client.AddUserToResourceGroupAsync(groupId: firstGroup.Id, userId: userId);

        // Act
        var response = await client.AddUserToResourceGroupAsHttpMessageAsync(
            groupId: secondGroup.Id,
            userId: userId);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task AddUserToGroupAsyncWhenHeadAddEmployeeFromAnotherDepartmentToGroupReturnForbidden()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var employee = await client.CreateEmployee(departmentId: "Java");
        var userId = employee!.Id;

        await client.AuthAsHeadOfDepartment();

        var resourceGroup = await client.CreateResourceGroup();
        var groupId = resourceGroup.Id;

        // Act
        var response = await client.AddUserToResourceGroupAsHttpMessageAsync(
            groupId: groupId,
            userId: userId);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task AddUserToGroupAsyncWhenHeadAddEmployeeToGroupFromAnotherDepartmentReturnForbidden()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();
        const string departmentId = "Java";

        var employee = await client.CreateEmployee();
        var userId = employee!.Id;

        var resourceGroup = await client.CreateResourceGroup(departmentId: departmentId);
        var groupId = resourceGroup.Id;

        await client.AuthAsHeadOfDepartment();

        // Act
        var response = await client.AddUserToResourceGroupAsHttpMessageAsync(
            groupId: groupId,
            userId: userId);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task RemoveUserFromGroupAsyncWhenAuthorizedAsCeoAndRequestValidReturnEmployeeWithOk()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var employee = await client.CreateEmployee();
        var userId = employee!.Id;

        var resourceGroup = await client.CreateResourceGroup();
        var groupId = resourceGroup.Id;

        await client.AddUserToResourceGroupAsync(
            groupId: groupId,
            userId: userId);

        // Act
        var deleteResult = await client.DeleteUserFromResourceGroupAsync(
            groupId: groupId,
            userId: userId);

        // Assert
        deleteResult.ResourceGroupId.Should().BeNull();
    }

    [Fact]
    public async Task RemoveUserFromGroupAsyncWhenEmployeeIsNotInTheGroupReturnForbidden()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var employee = await client.CreateEmployee();
        var userId = employee!.Id;

        var resourceGroup = await client.CreateResourceGroup();
        var groupId = resourceGroup.Id;

        // Act
        var deleteResult = await client.DeleteUserFromResourceGroupAsHttpMessageAsync(
            groupId: groupId,
            userId: userId);

        // Assert
        deleteResult.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task RemoveUserFromGroupAsyncWhenHeadDeleteEmployeeFromAnotherDepartmentReturnForbidden()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();
        const string departmentId = "Java";

        var employee = await client.CreateEmployee(departmentId: departmentId);
        var userId = employee!.Id;

        var resourceGroup = await client.CreateResourceGroup(departmentId: departmentId);
        var groupId = resourceGroup.Id;

        await client.AddUserToResourceGroupAsync(
            groupId: groupId,
            userId: userId);

        await client.AuthAsHeadOfDepartment();

        // Act
        var deleteResult = await client.DeleteUserFromResourceGroupAsHttpMessageAsync(
            groupId: groupId,
            userId: userId);

        // Assert
        deleteResult.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Theory]
    [InlineData("")]
    [InlineData("?departmentId=Net")]
    [InlineData("?hasGroupManager=false")]
    [InlineData("?hasGroupManager=true")]
    public async Task GetInterviewUsersLookupWhenRequestIsValidResponseWithOk(string query)
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        // Act
        var response = await client.GetResourceGroupsLookup(query);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
