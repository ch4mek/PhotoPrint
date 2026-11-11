using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.TeamBoard;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.Identity.Controllers.Contracts.Input;
using Modsen.Portal.Identity.Controllers.Contracts.Output;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
namespace Modsen.Portal.IntegrationTests.Identity.Controllers;

[Collection("GeneralCollection")]
public class CreateUserIntegrationTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;

    public CreateUserIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
    }

    [Fact]
    public async Task CreateUserWhenRequestIsValidReturnCreatedUser()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var userToCreate = new CreateUserInput
        {
            FirstName = "TestUserFirstName",
            LastName = "TestUserLastName",
            FirstNameRu = "ТестИмя",
            LastNameRu = "ТестФамилия",
            PatronymicRu = "ТестОтчество",
            Color = "#ffffff",
            Email = $"{Guid.CreateVersion7().ToAlphabetic()}@modsen-software.com",
            CountryId = 1,
            Position = PortalRoleNames.Employee,
            Location = LocationType.Minsk,
            DepartmentId = "Net",
            RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId,
            IsGraduateJobPlacement = true,
            ContractDateFrom = DateOnly.FromDateTime(DateTime.Today).AddDays(-9),
            ContractDateTo = DateOnly.FromDateTime(DateTime.Today).AddDays(3),
            CurrentLocation = "Ghomel",
            DateOfBirth = new DateOnly(1999, 9, 9),
            HireDate = DateOnly.FromDateTime(DateTime.Today).AddDays(-9),
            MilitaryServiceStatus = MilitaryServiceStatus.Fit,
            PersonalEmail = null,
            PersonalEmailSecond = null,
            PersonalPhone = null,
            ResourceGroupId = null,
            Telegram = "@tester"
        };

        var expected = new UserOutput
        {
            IsActive = true,
            ResourceGroupName = null,
            FullName = $"{userToCreate.LastName} {userToCreate.FirstName}",
            FullNameRu = $"{userToCreate.LastNameRu} {userToCreate.FirstNameRu}",
            FirstName = userToCreate.FirstName,
            LastName =  userToCreate.LastName,
            FirstNameRu = userToCreate.FirstNameRu,
            LastNameRu = userToCreate.LastNameRu,
            PatronymicRu = userToCreate.PatronymicRu,
            Color = userToCreate.Color,
            Email = userToCreate.Email,
            CountryId = userToCreate.CountryId,
            Position = userToCreate.Position,
            Location = userToCreate.Location,
            DepartmentId = userToCreate.DepartmentId,
            RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId,
            CurrentLocation = userToCreate.CurrentLocation,
            ResourceGroupId = userToCreate.ResourceGroupId,
        };

        // Act
        var createdUser = await client.CreateUser(userToCreate);

        // Assert
        createdUser.Should().NotBeNull();
        createdUser.Should().BeEquivalentTo(expected, config =>
            config.Excluding(x => x.TimeZone)
                .Excluding(x => x.Id)
                .Excluding(x => x.RightPosition)
                .Excluding(x => x.AvatarFileId));
    }

    [Theory]
    [InlineData("color", "testExample@modsen-software.com")]
    [InlineData("#ffffff", "testExample")]
    public async Task CreateUserWhenDataIsInvalidReturnBadRequest(string color, string email)
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var userToCreate = new CreateUserInput
        {
            FirstName = "TestUserFirstName",
            LastName = "TestUserLastName",
            FirstNameRu = "ТестЮзерИмя",
            LastNameRu = "ТестЮзерФамилия",
            PatronymicRu = "Отчество",
            Color = color,
            Email = email,
            Position = PortalRoleNames.Employee,
            Location = LocationType.Minsk,
            DepartmentId = null,
            RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId
        };

        // Act
        var response = await client.CreateUserAsHttpMessage(userToCreate);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateUserWhenUserExistReturnConflictResponse()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var email = $"{Guid.CreateVersion7().ToAlphabetic()}@modsen-software.com";

        await client.CreateEmployee(email: email);

        // Act
        var response = await client.CreateEmployeeAsHttpMessage(email: email);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task CreateUserWhenEmailExistsInDifferentCaseShouldReturnBadRequest()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        const string emailEnd = "@modsen-software.com";
        var defaultPart = Guid.CreateVersion7().ToAlphabetic();
        var upperCasePart = defaultPart.ToUpperInvariant();

        await client.CreateEmployeeAsHttpMessage(email: defaultPart + emailEnd);

        // Act
        var response = await client.CreateEmployeeAsHttpMessage(email: upperCasePart + emailEnd);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task CreateUserWhenAuthorizedAsEmployeeReturnForbidden()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsEmployee();

        // Act
        var response = await client.CreateEmployeeAsHttpMessage();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task CreateUserWhenHeadTryToCreateUserInDifferentDepartmentReturnForbidden()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsHeadOfDepartment();

        // Act
        var response = await client.CreateEmployeeAsHttpMessage(departmentId: "Js");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task CreateUserWhenHeadTryToCreateUserWithResourceGroupFromDifferentDepartmentReturnForbidden()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsHeadOfDepartment();

        // Act
        var response = await client.CreateEmployeeAsHttpMessage(
            departmentId: "Js",
            resourceGroupId: 1);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task CreateUserWhenCeoTryToCreateUserWithCeoRoleReturnForbidden()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        // Act
        var response = await client.CreateCeoAsHttpMessage();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task CreateUserWhenTryToCreateUserWithOccupiedRoleReturnForbidden()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        // Act
        var response = await client.CreateCtoAsHttpMessage();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task CreateUserWhenCtoTryToCreateUserWithCeoRoleReturnForbidden()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        // Act
        var response = await client.CreateCeoAsHttpMessage();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task CreateUserWhenTryToCreateCTOWithDepartmentIdReturnBadRequest()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var userToCreate = new CreateUserInput
        {
            FirstName = "TestUserFirstName",
            LastName = "TestUserLastName",
            FirstNameRu = "ТестЮзерИмя",
            LastNameRu = "ТестЮзерФамилия",
            PatronymicRu = "Отчество",
            Color = "#ffffff",
            Email = $"{Guid.CreateVersion7().ToAlphabetic()}@modsen-software.com",
            Position = PortalRoleNames.Cto,
            Location = LocationType.Minsk,
            DepartmentId = "Net",
            RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId
        };

        // Act
        var response = await client.CreateUserAsHttpMessage(userToCreate);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateUserWhenTryToCreateEmployeeWithoutDepartmentReturnBadRequest()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var userToCreate = new CreateUserInput
        {
            FirstName = "TestUserFirstName",
            LastName = "TestUserLastName",
            FirstNameRu = "ТестЮзерИмя",
            LastNameRu = "ТестЮзерФамилия",
            PatronymicRu = "Отчество",
            Color = "#ffffff",
            Email = $"{Guid.CreateVersion7().ToAlphabetic()}@modsen-software.com",
            Position = PortalRoleNames.Employee,
            Location = LocationType.Minsk,
            DepartmentId = null,
            RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId
        };

        // Act
        var response = await client.CreateUserAsHttpMessage(userToCreate);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateUserWhenHeadTryToCreateGroupManagerReturnForbidden()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        const string departmentId = "Java";
        var groupName = "Java Minsk Group";

        var createGroupResponse = await client.CreateResourceGroup(
            departmentId: departmentId,
            name: groupName);

        await client.AuthAsHeadOfDepartment();

        // Act
        var response = await client.CreateGroupManagerAsHttpMessage(
            departmentId: departmentId,
            resourceGroupId: createGroupResponse.Id);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task CreateUserWhenCeoCreatesGroupManagerReturnCreatedUser()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        const string departmentId = "Java";
        var groupName = Guid.CreateVersion7().ToAlphabetic();

        var createGroupResponse = await client.CreateResourceGroup(
            departmentId: departmentId,
            name: groupName);

        // Act
        var createdUser = await client.CreateGroupManager(resourceGroupId: createGroupResponse.Id,
            departmentId: departmentId);

        // Assert
        createdUser.Should().NotBeNull();
        createdUser.DepartmentId.Should().BeEquivalentTo(departmentId);
        createdUser.ResourceGroupId.Should().Be(createGroupResponse.Id);
    }

    [Fact]
    public async Task CreateUserWhenAuthedAsGroupManagerReturnCreatedUserFromHisGroup()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsGroupManager();

        // Act
        var response = await client.CreateEmployeeAsHttpMessage();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task CreateUserWhenCreatingGroupManagerForGroupWhereGroupManagerExistsReturnForbidden()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        // Act
        var response = await client.CreateGroupManagerAsHttpMessage(resourceGroupId: 1);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task CreateUserWhenCeoCreatesGroupManagerWithoutResourceGroupReturnCreatedUser()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var email = Guid.CreateVersion7().ToAlphabetic() + "@modsen-software.com";

        // Act
        var createdUser = await client.CreateGroupManager(email: email);

        // Assert
        createdUser.Should().NotBeNull();
        createdUser.Email.Should().Be(email);
        createdUser.ResourceGroupId.Should().BeNull();
        createdUser.Position.Should().Be(PortalRoleNames.GroupManager);
    }

    [Fact]
    public async Task CreateUserWhenHeadFromAnotherDepartmentCreatesGroupManagerWithoutResourceGroupReturForbidden()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsHeadOfDepartment();

        // Act
        var response = await client.CreateGroupManagerAsHttpMessage(departmentId: "Java");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task CreateUserWhenCeoCreatesGroupManagerWithEmptyActiveGroupReturnCreatedUser()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        const string departmentId = "Java";
        var groupName = Guid.CreateVersion7().ToAlphabetic();
        var email = Guid.CreateVersion7().ToAlphabetic() + "@modsen-software.com";

        var createGroupResponse = await client.CreateResourceGroup(departmentId: departmentId, name: groupName);

        // Act
        var createdUser = await client.CreateGroupManager(
            resourceGroupId: createGroupResponse.Id,
            departmentId: departmentId,
            email: email);


        // Assert
        createdUser.Should().NotBeNull();
        createdUser.Email.Should().BeEquivalentTo(email);
        createdUser.DepartmentId.Should().BeEquivalentTo(departmentId);
        createdUser.ResourceGroupId.Should().Be(createGroupResponse.Id);
    }

    [Fact]
    public async Task CreateUserWhenCeoCreatesEmployeeWithEmptyActiveGroupReturnCreatedEmployeeWithGroup()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        const string departmentId = "Java";
        var groupName = Guid.CreateVersion7().ToAlphabetic();
        var email = Guid.CreateVersion7().ToAlphabetic() + "@modsen-software.com";

        var createGroupResponse = await client.CreateResourceGroup(departmentId: departmentId, name: groupName);

        // Act
        var createdUser = await client.CreateEmployee(resourceGroupId: createGroupResponse.Id,
            departmentId: departmentId,
            email: email);

        // Assert
        createdUser.Should().NotBeNull();
        createdUser.Email.Should().BeEquivalentTo(email);
        createdUser.ResourceGroupId.Should().Be(createGroupResponse.Id);
    }

    [Fact]
    public async Task CreateUserWhenHeadWithManageHrModuleCreatesUserFromAnotherDepartmentReturnCreatedUser()
    {
        // Arrange
        using var client = _factory.CreateClient();

        // update user claims
        await client.AuthAsCeo();

        var claimsToUpdate = new UserClaimsInput { ClaimTypes = ["ManageHRModule"], };
        await client.PatchAsJsonAsync($"/api/users/{_factory.HeadOfDepartmentUserId!.Value}/claims", claimsToUpdate);

        await client.AuthAsHeadOfDepartment();

        const string departmentId = "Java";

        var userToCreate = new CreateUserInput
        {
            FirstName = "TestUserFirstName",
            LastName = "TestUserLastName",
            FirstNameRu = "Имя",
            LastNameRu = "Фамилия",
            PatronymicRu = "Отчество",
            Email = $"{Guid.CreateVersion7().ToAlphabetic()}@modsen-software.com",
            Position = PortalRoleNames.Employee,
            CountryId = 1,
            Location = LocationType.Minsk,
            DepartmentId = departmentId,
            RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId
        };

        // Act
        var createdUser = await client.CreateUser(userToCreate);

        // update user claims
        await client.AuthAsCeo();
        var claimsToLeave = new UserClaimsInput { ClaimTypes = [], };
        await client.PatchAsJsonAsync($"/api/users/{_factory.HeadOfDepartmentUserId!.Value}/claims", claimsToLeave);

        // Assert
        createdUser.Should().NotBeNull();
        createdUser.Email.Should().Be(userToCreate.Email);
        createdUser.DepartmentId.Should().Be(departmentId);
        createdUser.Position.Should().Be(PortalRoleNames.Employee);
    }

    [Fact]
    public async Task CreateUserWhenHeadWithoutManageHrModuleCreatesUserFromAnotherDepartmentReturnForbidden()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsHeadOfDepartment();

        const string departmentId = "Java";

        var userToCreate = new CreateUserInput
        {
            FirstName = "TestUserFirstName",
            LastName = "TestUserLastName",
            FirstNameRu = "Имя",
            LastNameRu = "Фамилия",
            PatronymicRu = "Отчество",
            Email = $"{Guid.CreateVersion7().ToAlphabetic()}@modsen-software.com",
            Position = PortalRoleNames.Employee,
            CountryId = 1,
            Location = LocationType.Minsk,
            DepartmentId = departmentId,
            RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId
        };

        // Act
        var response = await client.CreateUserAsHttpMessage(userToCreate);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task CreateUserWhenGroupManagerWithManageHrModuleCreatesUserFromAnotherResourceGroupReturnCreatedUser()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        const string departmentId = "Java";
        var groupName2 = Guid.CreateVersion7().ToAlphabetic();

        var claimsToUpdate = new UserClaimsInput { ClaimTypes = ["ManageHRModule"], };
        await client.PatchAsJsonAsync($"/api/users/{_factory.GroupManagerId!.Value}/claims", claimsToUpdate);

        var resourceGroup2 = await client.CreateResourceGroup(
            departmentId: departmentId,
            name: groupName2);

        await client.AuthAsGroupManager();

        var userToCreate = new CreateUserInput
        {
            FirstName = "TestUserFirstName",
            LastName = "TestUserLastName",
            FirstNameRu = "ТестЮзерИмя",
            LastNameRu = "ТестЮзерФамилия",
            PatronymicRu = "Отчество",
            Color = "#ffffff",
            Email = $"{Guid.CreateVersion7().ToAlphabetic()}@modsen-software.com",
            Position = PortalRoleNames.Employee,
            CountryId = 1,
            Location = LocationType.Minsk,
            DepartmentId = departmentId,
            RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId,
            ResourceGroupId = resourceGroup2.Id
        };

        // Act
        var createdUser = await client.CreateUser(userToCreate);

        // update user claims
        await client.AuthAsCeo();
        var claimsToLeave = new UserClaimsInput { ClaimTypes = [], };
        await client.PatchAsJsonAsync($"/api/users/{_factory.GroupManagerId!.Value}/claims", claimsToLeave);

        // Assert
        createdUser.Should().NotBeNull();
        createdUser.Email.Should().Be(userToCreate.Email);
        createdUser.ResourceGroupId.Should().Be(resourceGroup2.Id);
        createdUser.DepartmentId.Should().Be(departmentId);
        createdUser.Position.Should().Be(PortalRoleNames.Employee);
    }

    [Fact]
    public async Task CreateUserWhenEHrModuleCreatesUserFromAnotherResourceGroupReturnCreatedUser()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        const string departmentId = "Java";
        var groupName2 = Guid.CreateVersion7().ToAlphabetic();

        var claimsToUpdate = new UserClaimsInput { ClaimTypes = ["ManageHRModule"], };
        await client.PatchAsJsonAsync($"/api/users/{_factory.GroupManagerId!.Value}/claims", claimsToUpdate);

        var resourceGroup2 = await client.CreateResourceGroup(
            departmentId: departmentId,
            name: groupName2);

        await client.AuthAsGroupManager();

        var userToCreate = new CreateUserInput
        {
            FirstName = "TestUserFirstName",
            LastName = "TestUserLastName",
            FirstNameRu = "ТестЮзерИмя",
            LastNameRu = "ТестЮзерФамилия",
            PatronymicRu = "Отчество",
            Color = "#ffffff",
            Email = $"{Guid.CreateVersion7().ToAlphabetic()}@modsen-software.com",
            Position = PortalRoleNames.Employee,
            CountryId = 1,
            Location = LocationType.Minsk,
            DepartmentId = departmentId,
            RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId,
            ResourceGroupId = resourceGroup2.Id
        };

        // Act
        var createdUser = await client.CreateUser(userToCreate);

        // update user claims
        await client.AuthAsCeo();
        var claimsToLeave = new UserClaimsInput { ClaimTypes = [], };
        await client.PatchAsJsonAsync($"/api/users/{_factory.GroupManagerId!.Value}/claims", claimsToLeave);

        // Assert
        createdUser.Should().NotBeNull();
        createdUser.Email.Should().Be(userToCreate.Email);
        createdUser.ResourceGroupId.Should().Be(resourceGroup2.Id);
        createdUser.DepartmentId.Should().Be(departmentId);
        createdUser.Position.Should().Be(PortalRoleNames.Employee);
    }
}
