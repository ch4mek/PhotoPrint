using System.Net;
using System.Text.Json;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TimeTracker;
using Modsen.Portal.TimeTrack.Controllers.Contracts.Output;
namespace Modsen.Portal.IntegrationTests.TimeTracker.Controllers.TimeTrackUserController;

public class LookupUsersWithFiltersForReportsTests : IClassFixture<IntegrationTestFactory>
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;

    public LookupUsersWithFiltersForReportsTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
    }

    [Fact]
    public async Task LookupUsersWithFiltersForReportsShouldReturnOnlyActiveUsers()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsEmployee();

        var employeeUser = await client.GetCurrentUser();
        var employeeId = employeeUser.Id;

        await client.AuthAsCeo();

        var clientForTest = await client.CreateClientAsync();
        var clientId = clientForTest!.Id;

        var userToBeInactive = await client.CreateEmployee(departmentId: "Java");
        var inactiveUserId = userToBeInactive!.Id;

        await client.SetUserInactive(inactiveUserId);

        var createdProject = await client.CreateProject(clientId: clientId);
        var projectId = createdProject!.Id;

        await client.CreateProjectAssignment(userId: employeeId, projectId: projectId);

        var expectedUsers = new List<UserLookupWithFiltersOutput>
        {
            new()
            {
                Id = _factory.EmployeeUserId!.Value,
                Color = employeeUser.Color,
                FirstName = employeeUser.FirstName,
                LastName = employeeUser.LastName,
                AvatarFileId = employeeUser.AvatarFileId
            }
        };

        //Act
        var output = await client.GetLookupUsersReportsWithFiltersAsync(clientId: clientId);

        //Assert
        output.Should().BeEquivalentTo(expectedUsers);
    }

    [Fact]
    public async Task LookupUsersWithFiltersForReportsAuthedAsGroupManagerShouldReturnOnlyActiveUsersFromTheSameResourceGroup()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var createdUserFromGroup = await client.CreateEmployee(resourceGroupId: 1);
        var userId = createdUserFromGroup!.Id;

        var clientForTest = await client.CreateClientAsync();
        var clientId = clientForTest!.Id;

        var createdProject = await client.CreateProject(clientId: clientId);
        var projectId = createdProject!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: projectId);

        var expectedUsers = new List<UserLookupWithFiltersOutput>
        {
            new()
            {
                Id = createdUserFromGroup.Id,
                Color = createdUserFromGroup.Color,
                FirstName = createdUserFromGroup.FirstName,
                LastName = createdUserFromGroup.LastName,
                AvatarFileId = createdUserFromGroup.AvatarFileId
            }
        };

        await client.AuthAsGroupManager();

        //Act
        var response = await client.GetLookupUsersReportsWithFiltersAsync(clientId: clientId);

        //Assert
        response.Should().BeEquivalentTo(expectedUsers);
    }

    [Fact]
    public async Task LookupUsersWithFiltersForReportsWhenEmployeeIsProjectManagerShouldReturnOkResult()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var clientId = _factory.ModsenClientId!.Value;

        var pmoEmployee = await client.CreateEmployee(departmentId: "PMO");
        var managerId = pmoEmployee!.Id;

        var user = await client.CreateEmployee(departmentId: "Java");
        var userId = user!.Id;

        var privateProject = await client.CreateProject(clientId: clientId, managerId: managerId);
        var projectId = privateProject!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: projectId);

        var expectedUsers = new List<UserLookupWithFiltersOutput>
        {
            new()
            {
                Id = user.Id,
                Color = user.Color,
                FirstName = user.FirstName,
                LastName = user.LastName,
                AvatarFileId = user.AvatarFileId
            }
        };

        await client.AuthAs(pmoEmployee.Email);

        //Act
        var response = await client.GetLookupUsersReportsWithFiltersAsync(onlyManagedUsers: true);

        //Assert
        response.Should().BeEquivalentTo(expectedUsers);
    }

    [Fact]
    public async Task LookupUsersWithFiltersForReportsWhenHeadIsProjectManagerHavingTrueOnlyManagedUsersShouldReturnOkResultHavingUsersOnlyFromManagedProjects()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var pmoHeadId = _factory.PmoHeadOfDepartmentUserId!.Value;
        var pmoHeadEmail = _factory.PmoHeadOfDepartmentEmail!;
        var clientId = _factory.ModsenClientId!.Value;

        var user = await client.CreateEmployee(departmentId: "Java");
        var userId = user!.Id;

        var privateProject = await client.CreateProject(clientId: clientId, managerId: pmoHeadId);
        var projectId = privateProject!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: projectId);

        await client.AuthAs(pmoHeadEmail);

        //Act
        var output = await client.GetLookupUsersReportsWithFiltersAsync(onlyManagedUsers: true);

        //Assert
        output.Should().Contain(x=>x.Id == user.Id);
    }

    [Fact]
    public async Task LookupUsersWithFiltersForReportsWhenHeadIsProjectManagerHavingFalseOnlyManagedUsersShouldReturnOkResultHavingUsersOnlyFromHisDepartment()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var clientId = _factory.ModsenClientId!.Value;
        var pmoHeadId = _factory.PmoHeadOfDepartmentUserId!.Value;
        var pmoHeadEmail = _factory.PmoHeadOfDepartmentEmail!;

        var pmoEmployee = await client.CreateEmployee(departmentId: "PMO");
        var pmoEmployeeId = pmoEmployee!.Id;

        var user = await client.CreateEmployee(departmentId: "Java");
        var userId = user!.Id;

        var privateProject = await client.CreateProject(clientId: clientId, managerId: pmoHeadId);
        var projectId = privateProject!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: projectId);

        await client.AuthAs(pmoHeadEmail);

        //Act
        var response = await client.GetLookupUsersReportsWithFiltersAsync(onlyManagedUsers: false);

        //Assert
        response.Should().Contain(x=> x.Id == pmoEmployeeId);
    }

    [Fact]
    public async Task LookupUsersWithFiltersForReportsWhenEmployeeIsProjectManagerHavingFalseOnlyManagedUsersShouldReturnForbidden()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var pmoEmployee = await client.CreateEmployee(departmentId: "PMO");

        await client.AuthAs(pmoEmployee!.Email);

        //Act
        var response = await client.GetTimesheetLookupUsersAsHttpMessageAsync(onlyManagedUsers: false);

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Theory]
    [InlineData(PortalRoleNames.Ceo)]
    [InlineData(PortalRoleNames.Cto)]
    [InlineData(PortalRoleNames.SystemAdministrator)]
    public async Task LookupUsersWithFiltersForReportsWhenUserIsNotEmployeeOrHeadOfDepartmentHavingOnlyManagedUsersShouldReturnForbidden(
        string roleType)
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsRole(roleType);

        //Act
        var response = await client.GetTimesheetLookupUsersAsHttpMessageAsync(onlyManagedUsers: true);

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task LookupUsersWithFiltersForReportsWhenFilterParametersHasClientIdFilterReturnListOfLookupUserOutput()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCto();

        var userId = _factory.EmployeeUserId!.Value;
        var userIdHead = _factory.HeadOfDepartmentUserId!.Value;

        var clientForTest = await client.CreateClientAsync();
        var clientId = clientForTest!.Id;

        var createdProject = await client.CreateProject(clientId: clientId, isPublic: true, isBillable: false);
        var projectId = createdProject!.Id;

        await client.CreateProjectAssignment(userId: userIdHead, projectId: projectId);

        //Act
        var response = await client.GetLookupUsersReportsWithFiltersAsync(clientId: clientId);

        //Assert
        response.Should().NotBeNull();
        response.Count.Should().BeGreaterThan(2);
        response.FirstOrDefault(user => user.Id == userId).Should().NotBeNull();
        response.FirstOrDefault(user => user.Id == userIdHead).Should().NotBeNull();
    }

    [Fact]
    public async Task LookupUsersWithFiltersForReportsWhenFilterParametersHasProjectIdAndKeyWordFilterReturnListOfLookupUserOutput()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCto();

        var userId = _factory.EmployeeUserId!.Value;
        var userIdHead = _factory.HeadOfDepartmentUserId!.Value;

        var clientForTest = await client.CreateClientAsync();
        var clientId = clientForTest!.Id;

        var createdProject = await client.CreateProject(clientId: clientId, isPublic: true, isBillable: false);
        var projectId = createdProject!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: projectId, isTimeBillable: false);
        await client.CreateProjectAssignment(userId: userIdHead, projectId: projectId, isTimeBillable: false);

        const string keyWord = "Head";

        //Act
        var response = await client.GetLookupUsersReportsWithFiltersAsync(projectId: projectId,
            keyWord: keyWord);

        //Assert
        response.Should().NotBeNull();
        response.First().Id.Should().Be(userIdHead);
        response.First().LastName.Should().Contain(keyWord);
    }

    [Fact]
    public async Task LookupUsersWithFiltersForReportsWhenFilterParametersHasProjectIdFilterReturnListOfLookupUserOutput()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCto();

        var userId = _factory.EmployeeUserId!.Value;

        var clientForTest = await client.CreateClientAsync();
        var clientId = clientForTest!.Id;

        var createdProject = await client.CreateProject(clientId: clientId, isPublic: true, isBillable: false);
        var projectId = createdProject!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: projectId, isTimeBillable: false);

        //Act
        var response = await client.GetLookupUsersReportsWithFiltersAsync(projectId: projectId);

        //Assert
        response.Should().NotBeNull();
        response.Count.Should().BeGreaterThan(1);
        response.FirstOrDefault(user => user.Id == userId).Should().NotBeNull();
    }

    [Fact]
    public async Task LookupUsersWithFiltersForReportsWhenHeadOfNetDepartmentTryGetUsersReturnListOfLookupUserOutput()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCto();

        var userIdEmployee = _factory.EmployeeUserId!.Value;
        var userIdHead = _factory.HeadOfDepartmentUserId!.Value;
        var userIdCto = _factory.CtoUserId!.Value;
        var userIdCeo = _factory.CeoUserId!.Value;

        var clientForTest = await client.CreateClientAsync();
        var clientId = clientForTest!.Id;

        var createdProject = await client.CreateProject(clientId: clientId, isPublic: true, isBillable: false);
        var projectId = createdProject!.Id;

        foreach (var userId in new[] { userIdEmployee, userIdHead, userIdCeo, userIdCto })
        {
            await client.CreateProjectAssignment(userId: userId, projectId: projectId, isTimeBillable: false);
        }

        const string keyWord = "Test";
        await client.AuthAsHeadOfDepartment();

        //Act
        var response = await client.GetLookupUsersReportsWithFiltersAsync(keyWord: keyWord);

        //Assert
        response.Should().NotBeNull();
        response.Count.Should().BeGreaterThanOrEqualTo(2);
        response.FirstOrDefault(user => user.Id == userIdEmployee).Should().NotBeNull();
        response.FirstOrDefault(user => user.Id == userIdHead).Should().NotBeNull();
        response.FirstOrDefault(user => user.Id == userIdCto).Should().BeNull();
        response.FirstOrDefault(user => user.Id == userIdCeo).Should().BeNull();
    }

    [Fact]
    public async Task LookupUsersWithFiltersForReportsWhenAuthedAsHeadOfDepartmentAddIsManagerOfProjectAndProjectHasProjectUserFromAnotherDepartmentNotReturnUser()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCto();
        var pmoHeadId = _factory.PmoHeadOfDepartmentUserId!.Value;
        var pmoHeadEmail = _factory.PmoHeadOfDepartmentEmail!;

        var clientForTest = await client.CreateClientAsync();
        var clientId = clientForTest!.Id;

        var userForTest = await client.CreateEmployee(departmentId: "Java");
        var userId = userForTest!.Id;

        var createdProject = await client.CreateProject(clientId: clientId, managerId: pmoHeadId);
        var projectId = createdProject!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: projectId);

        //Act
        await client.AuthAs(pmoHeadEmail);
        var response = await client.GetLookupUsersReportsWithFiltersAsync(projectId: projectId);

        //Assert
        response.Should().NotBeNull();
        response.Count.Should().Be(0);
    }

    [Fact]
    public async Task LookupUsersWithFiltersForReportsWhenAuthedAsHeadOfDepartmentAsManagerOfPublicProjectWithNotProvidedParamsReturnAllActiveUsers()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var pmoHeadId = _factory.PmoHeadOfDepartmentUserId!.Value;
        var pmoHeadEmail = _factory.PmoHeadOfDepartmentEmail!;

        var clientForTest = await client.CreateClientAsync();
        var clientId = clientForTest!.Id;

        var firstUserForTest = await client.CreateEmployee(departmentId: "Java");
        var firstUserId = firstUserForTest!.Id;

        var firstUserLookup = new UserLookupWithFiltersOutput
        {
            Id = firstUserId,
            FirstName = firstUserForTest.FirstName,
            LastName = firstUserForTest.LastName,
            Color = firstUserForTest.Color,
            AvatarFileId = firstUserForTest.AvatarFileId
        };

        var secondUserForTest = await client.CreateEmployee(departmentId: "Net");
        var secondUserId = secondUserForTest!.Id;

        var secondUserLookup = new UserLookupWithFiltersOutput
        {
            Id = secondUserId,
            FirstName = secondUserForTest.FirstName,
            LastName = secondUserForTest.LastName,
            Color = secondUserForTest.Color,
            AvatarFileId = secondUserForTest.AvatarFileId
        };

        await client.CreateProject(clientId: clientId, managerId: pmoHeadId, isPublic: true, isBillable: false);

        await client.AuthAs(pmoHeadEmail);

        //Act
        var response = await client.GetLookupUsersReportsWithFiltersAsync(onlyManagedUsers: true);

        //Assert
        response.Should().NotBeNull();
        response.FirstOrDefault(user => user.Id == firstUserLookup.Id).Should().NotBeNull();
        response.FirstOrDefault(user => user.Id == secondUserLookup.Id).Should().NotBeNull();
    }

    [Fact]
    public async Task LookupUsersWithFiltersForReportsWhenAuthedAsHeadOfDepartmentAsManagerOfPublicProjectWithClientIdParamReturnAllActiveUsers()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var pmoHeadId = _factory.PmoHeadOfDepartmentUserId!.Value;
        var pmoHeadEmail = _factory.PmoHeadOfDepartmentEmail!;

        var clientForTest = await client.CreateClientAsync();
        var clientId = clientForTest!.Id;

        var firstUserForTest = await client.CreateEmployee(departmentId: "Java");
        var firstUserId = firstUserForTest!.Id;

        var firstUserLookup = new UserLookupWithFiltersOutput
        {
            Id = firstUserId,
            FirstName = firstUserForTest.FirstName,
            LastName = firstUserForTest.LastName,
            Color = firstUserForTest.Color,
            AvatarFileId = firstUserForTest.AvatarFileId
        };

        var secondUserForTest = await client.CreateEmployee(departmentId: "Net");
        var secondUserId = secondUserForTest!.Id;

        var secondUserLookup = new UserLookupWithFiltersOutput
        {
            Id = secondUserId,
            FirstName = secondUserForTest.FirstName,
            LastName = secondUserForTest.LastName,
            Color = secondUserForTest.Color,
            AvatarFileId = secondUserForTest.AvatarFileId
        };

        await client.CreateProject(clientId: clientId, managerId: pmoHeadId, isPublic: true, isBillable: false);

        await client.AuthAs(pmoHeadEmail);

        //Act
        var response = await client.GetLookupUsersReportsWithFiltersAsync(onlyManagedUsers: true,
            clientId: clientId);

        //Assert
        response.Should().NotBeNull();
        response.FirstOrDefault(user => user.Id == firstUserLookup.Id).Should().NotBeNull();
        response.FirstOrDefault(user => user.Id == secondUserLookup.Id).Should().NotBeNull();
    }

    [Fact]
    public async Task LookupUsersWithFiltersForReportsWhenAuthedAsHeadOfDepartmentAsManagerOfPublicProjectWithProjectIdParamReturnAllActiveUsers()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var pmoHeadId = _factory.PmoHeadOfDepartmentUserId!.Value;
        var pmoHeadEmail = _factory.PmoHeadOfDepartmentEmail!;

        var clientForTest = await client.CreateClientAsync();
        var clientId = clientForTest!.Id;

        var firstUserForTest = await client.CreateEmployee(departmentId: "Java");
        var firstUserId = firstUserForTest!.Id;

        var firstUserLookup = new UserLookupWithFiltersOutput
        {
            Id = firstUserId,
            FirstName = firstUserForTest.FirstName,
            LastName = firstUserForTest.LastName,
            Color = firstUserForTest.Color,
            AvatarFileId = firstUserForTest.AvatarFileId
        };

        var secondUserForTest = await client.CreateEmployee(departmentId: "Net");
        var secondUserId = secondUserForTest!.Id;

        var secondUserLookup = new UserLookupWithFiltersOutput
        {
            Id = secondUserId,
            FirstName = secondUserForTest.FirstName,
            LastName = secondUserForTest.LastName,
            Color = secondUserForTest.Color,
            AvatarFileId = secondUserForTest.AvatarFileId
        };

        var project = await client.CreateProject(clientId: clientId, managerId: pmoHeadId, isPublic: true, isBillable: false);
        var projectId = project!.Id;

        await client.AuthAs(pmoHeadEmail);

        //Act
        var response = await client.GetLookupUsersReportsWithFiltersAsync(projectId: projectId,
            onlyManagedUsers: true);

        //Assert
        response.Should().NotBeNull();
        response.FirstOrDefault(user => user.Id == firstUserLookup.Id).Should().NotBeNull();
        response.FirstOrDefault(user => user.Id == secondUserLookup.Id).Should().NotBeNull();
    }

    [Theory]
    [InlineData(false, false)]
    [InlineData(true, false)]
    public async Task LookupUsersWithFiltersForReportsWhenAuthedAsHeadOfDepartmentAndManagerOfClientFirstProjectWhenAnotherClientProjectInterfersWithFirstProjectReturnOnlySpecifiedClientEmployeeFromManagedProject(
        bool isInterferingProjectPublic, bool isBillable)
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var pmoHeadId = _factory.PmoHeadOfDepartmentUserId!.Value;
        var pmoHeadEmail = _factory.PmoHeadOfDepartmentEmail!;

        var clientForTest = await client.CreateClientAsync();
        var clientId = clientForTest!.Id;

        var interferingClientForTest = await client.CreateClientAsync();
        var interferingClientId = interferingClientForTest!.Id;

        var redundantUserForTest = await client.CreateEmployee(departmentId: "Java");

        var neededUserForTest = await client.CreateEmployee(departmentId: "Net");

        var interferingProject = await client.CreateProject(
            clientId: isInterferingProjectPublic ? clientId : interferingClientId,
            managerId: isInterferingProjectPublic ? null : pmoHeadId,
            isBillable: isBillable,
            isPublic: isInterferingProjectPublic);
        var interferingProjectId = interferingProject!.Id;

        var secondaryProject = await client.CreateProject(clientId: clientId, isBillable: false);
        var secondaryProjectId = secondaryProject!.Id;

        var managedProject = await client.CreateProject(clientId: clientId, isBillable: false, managerId: pmoHeadId);
        var managedProjectId = managedProject!.Id;

        if (!isInterferingProjectPublic)
        {
            await client.CreateProjectAssignment(userId: redundantUserForTest!.Id, projectId: interferingProjectId,
                isTimeBillable: isBillable);
        }

        await client.CreateProjectAssignment(userId: redundantUserForTest!.Id, projectId: secondaryProjectId,
            isTimeBillable: false);

        await client.CreateProjectAssignment(userId: neededUserForTest!.Id, projectId: managedProjectId,
            isTimeBillable: false);

        var expectedUserLookupWithFiltersOutput = new List<UserLookupWithFiltersOutput>
        {
            new()
            {
                Id = neededUserForTest.Id,
                FirstName = neededUserForTest.FirstName,
                LastName = neededUserForTest.LastName,
                Color = neededUserForTest.Color,
                AvatarFileId = neededUserForTest.AvatarFileId
            }
        };

        await client.AuthAs(pmoHeadEmail);

        //Act
        var response = await client.GetLookupUsersReportsWithFiltersAsync(clientId: clientId,
            onlyManagedUsers: true);

        //Assert
        response.Should().BeEquivalentTo(expectedUserLookupWithFiltersOutput);
    }
}
