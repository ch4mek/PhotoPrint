using System.Text.Json;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TimeTracker;
using Modsen.Portal.TimeTrack.Controllers.Contracts.Output;
namespace Modsen.Portal.IntegrationTests.TimeTracker.Controllers.TimeTrackUserController;

public class UserLookupWithFiltersTests : IClassFixture<IntegrationTestFactory>
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;

    public UserLookupWithFiltersTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
    }

    [Fact]
    public async Task LookupUsersWithFiltersShouldReturnOnlyActiveUsers()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var keyWord = "UserVadimFromJavaTEST";

        var userToBeInactive = await client.CreateEmployee(departmentId: "Java");
        var inactiveUserId = userToBeInactive!.Id;

        await client.SetUserInactive(inactiveUserId);

        var userToBeActive = await client.CreateEmployee(departmentId: "Java", firstName: keyWord);

        var expectedUsers = new List<UserLookupWithFiltersOutput>
        {
            new()
            {
                Id = userToBeActive!.Id,
                Color = userToBeActive.Color,
                FirstName = userToBeActive.FirstName,
                LastName = userToBeActive.LastName,
                AvatarFileId = userToBeActive.AvatarFileId
            }
        };

        //Act
        var response = await client.GetTimesheetLookupUsersWithFiltersAsync(
            keyWord: keyWord);

        //Assert
        response.Should().BeEquivalentTo(expectedUsers);
    }

    [Fact]
    public async Task LookupUsersWithFiltersAuthedAsGroupManagerShouldReturnFromTheSameResourceGroup()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var createdUserFromGroup = await client.CreateEmployee(resourceGroupId: 1);
        var userId = createdUserFromGroup!.Id;

        await client.AuthAsGroupManager();

        //Act
        var response = await client.GetTimesheetLookupUsersWithFiltersAsync();

        //Assert
        response.Should().NotBeNull();
        response.Should().Contain(x => x.Id == userId);
        response.Should().Contain(x => x.Id == _factory.GroupManagerId);
    }

    [Fact]
    public async Task LookupUsersWithFiltersAuthedAsGroupManagerWithKeyWordShouldReturnUsersFromTheSameResourceGroup()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var keyWord = Guid.CreateVersion7().ToAlphabetic();

        var createdUserFromGroup = await client.CreateEmployee(resourceGroupId: 1, firstName: keyWord);

        var expectedUsers = new List<UserLookupWithFiltersOutput>
        {
            new()
            {
                Id = createdUserFromGroup!.Id,
                Color = createdUserFromGroup.Color,
                FirstName = createdUserFromGroup.FirstName,
                LastName = createdUserFromGroup.LastName,
                AvatarFileId = createdUserFromGroup.AvatarFileId
            }
        };

        await client.AuthAsGroupManager();

        //Act
        var response = await client.GetTimesheetLookupUsersWithFiltersAsync(keyWord: keyWord);

        //Assert
        response.Should().BeEquivalentTo(expectedUsers);
    }

    [Fact]
    public async Task LookupUsersWithFiltersWhenFilterParametersHasProjectIdAndKeyWordFilterReturnListOfLookupUserOutput()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCto();

        var userId = _factory.EmployeeUserId!.Value;
        var userIdHead = _factory.HeadOfDepartmentUserId!.Value;

        var clientForTest = await client.CreateClientAsync();
        var clientId = clientForTest!.Id;

        var project = await client.CreateProject(clientId: clientId);
        var projectId = project!.Id;

        await client.CreateProjectAssignment(userId: userIdHead, projectId: projectId);
        await client.CreateProjectAssignment(userId: userId, projectId: projectId);

        const string keyWord = "Head";

        //Act
        var response = await client.GetTimesheetLookupUsersWithFiltersAsync(
            keyWord: keyWord,
            projectId: projectId);

        //Assert
        response.Should().NotBeNull();
        response.Count.Should().Be(1);
        response.First().Id.Should().Be(userIdHead);
        response.First().LastName.Should().Contain(keyWord);
    }

    [Fact]
    public async Task LookupUsersWithFiltersWhenHeadOfNetDepartmentTriesGetUsersReturnListOfLookupUserOutput()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCto();

        var employeeId = _factory.EmployeeUserId!.Value;
        var userIdHead = _factory.HeadOfDepartmentUserId!.Value;
        var userIdCto = _factory.CtoUserId!.Value;
        var userIdCeo = _factory.CeoUserId!.Value;

        var clientForTest = await client.CreateClientAsync();
        var clientId = clientForTest!.Id;

        var project = await client.CreateProject(clientId: clientId);
        var projectId = project!.Id;

        foreach (var userId in new[] { employeeId, userIdHead, userIdCeo, userIdCto })
        {
            await client.CreateProjectAssignment(userId: userId, projectId: projectId);
        }

        const string keyWord = "Test";
        await client.AuthAsHeadOfDepartment();

        //Act
        var response = await client.GetTimesheetLookupUsersWithFiltersAsync(keyWord: keyWord);

        //Assert
        response.Should().NotBeNull();
        // 6 dep.change.test users
        response.Count.Should().Be(3 + 6); 
        response.FirstOrDefault(user => user.Id == userIdHead).Should().NotBeNull();
        response.FirstOrDefault(user => user.Id == userIdCto).Should().BeNull();
        response.FirstOrDefault(user => user.Id == userIdCeo).Should().BeNull();
    }

    [Fact]
    public async Task LookupUsersWithFiltersWhenGroupManagerTriesGetUsersReturnListOfLookupUserOutput()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();

        await client.CreateEmployee(resourceGroupId: 1);

        const string keyWord = "Test";
        await client.AuthAsGroupManager();

        //Act
        var response = await client.GetTimesheetLookupUsersWithFiltersAsync(keyWord: keyWord);

        //Assert
        response.Should().NotBeNull();
        response.Count.Should().Be(1);
        response[0].Id.Should().Be(_factory.GroupManagerId);
    }

    [Fact]
    public async Task LookupUsersWithFiltersWhenFilterParametersHasProjectIdFilterReturnListOfLookupUserOutput()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCto();

        var userId = _factory.EmployeeUserId!.Value;
        var clientForTest = await client.CreateClientAsync();
        var clientId = clientForTest!.Id;

        var project = await client.CreateProject(clientId: clientId);
        var projectId = project!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: projectId);

        //Act
        var response = await client.GetTimesheetLookupUsersWithFiltersAsync(projectId: projectId);

        //Assert
        response.Should().NotBeNull();
        response.Count.Should().Be(1);
        response.First().Id.Should().Be(userId);
    }

    [Fact]
    public async Task LookupUsersWithFiltersWhenFilterParametersHasClientIdFilterReturnListOfLookupUserOutput()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCto();

        var userId = _factory.EmployeeUserId!.Value;
        var userIdHead = _factory.HeadOfDepartmentUserId!.Value;

        var clientForTest = await client.CreateClientAsync();
        var clientId = clientForTest!.Id;

        var project = await client.CreateProject(clientId: clientId);
        var projectId = project!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: projectId);
        await client.CreateProjectAssignment(userId: userIdHead, projectId: projectId);

        //Act
        var response = await client.GetTimesheetLookupUsersWithFiltersAsync(clientId: clientId);

        //Assert
        response.Should().NotBeNull();
        response.Count.Should().Be(2);
        response.FirstOrDefault(user => user.Id == userId).Should().NotBeNull();
        response.FirstOrDefault(user => user.Id == userIdHead).Should().NotBeNull();
    }
}
