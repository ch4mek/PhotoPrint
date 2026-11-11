using System.Net;
using System.Text.Json;
using Modsen.Portal.Cv.Requests.CvUsers.Core.CoreDto;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Cv;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
namespace Modsen.Portal.IntegrationTests.Cv.CvUsers;

[Collection("CvCollection")]
public class CvUsersControllerIntegrationTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;

    public CvUsersControllerIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
    }

    [Fact]
    public async Task GetCvUsersLookupWhenUserHasManageCvsClaimResponseWithOkResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        // Act
        var response = await client.GetCvUsersLookupAsHttpMessage();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetCvUsersLookupWhenUserHasNoManageCvsClaimResponseWithForbiddenResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsEmployee();

        // Act
        var response = await client.GetCvUsersLookupAsHttpMessage();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetCvUsersLookupWhenUserNotAuthenticatedResponseWithUnauthorizedResult()
    {
        // Arrange
        using var client = _factory.CreateClient();

        // Act
        var response = await client.GetCvUsersLookupAsHttpMessage();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetCvUsersLookupWhenUsersExistResponseWithUserLookup()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        // Act
        var users = await client.GetCvUsersLookup();

        // Assert
        users.Should().NotBeNull();
        users.Should().Contain(x => x.Id == _factory.CtoUserId);
        users.Should().Contain(x => x.Id == _factory.HeadOfDepartmentUserId);
        users.Should().Contain(x => x.Id == _factory.GroupManagerId);
    }

    [Fact]
    public async Task GetCvUsersLookupWhenAuthAsHeadReturnUsersFromHeadsDepartment()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var employeeInDept = await client.CreateEmployee(departmentId: "Net", resourceGroupId: 1);
        var employeeOtherDept = await client.CreateEmployee(departmentId: "Java", resourceGroupId: 2);

        var expected = new List<CvUserLookupDto>
        {
            new() { FullName = "HeadOfDepartment Test" },
            new()
            {
                FullName = $"{employeeInDept!.LastName} {employeeInDept.FirstName}",
                FullNameRu = $"{employeeInDept.LastNameRu} {employeeInDept.FirstNameRu}"
            }
        };

        var unexpectedUsers = new List<CvUserLookupDto>
        {
            new() { FullName = "CEO Test" },
            new() { FullName = "CTO Test" },
            new()
            {
                FullName = $"{employeeOtherDept!.LastName} {employeeOtherDept.FirstName}",
                FullNameRu = $"{employeeOtherDept.LastNameRu} {employeeOtherDept.FirstNameRu}"
            },
            new() { FullName = "SysAdmin Test" }
        };

        await client.AuthAsHeadOfDepartment();

        // Act
        var actual = await client.GetCvUsersLookup();

        // Assert
        expected.ForEach(expectedUser => actual.Should().ContainEquivalentOf(expectedUser, config =>
        {
            config.Excluding(x => x.Id);
            return config;
        }));
        unexpectedUsers.ForEach(unexpectedUser => actual.Should().NotContainEquivalentOf(unexpectedUser, config =>
        {
            config.Excluding(x => x.Id);
            return config;
        }));
    }

    [Fact]
    public async Task GetCvUsersLookupWhenAuthAsGroupManagerReturnUsersFromHisGroup()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var createdUserFromGroup = await client.CreateEmployee(departmentId: "Net", resourceGroupId: 1);
        var createdUserNotFromGroup = await client.CreateEmployee(departmentId: "Java", resourceGroupId: 2);

        var unexpectedUsersIds = new[]
        {
            _factory.CeoUserId, _factory.CtoUserId, _factory.EmployeeUserId, _factory.HeadOfDepartmentUserId,
            _factory.SystemUserId, createdUserNotFromGroup!.Id
        };
        var expectedUsersIds = new[]
        {
            _factory.GroupManagerId, createdUserFromGroup!.Id
        };

        await client.AuthAsGroupManager();

        // Act
        var actual = await client.GetCvUsersLookup();

        // Assert
        actual.Should().NotBeNull();
        expectedUsersIds.All(id => actual.Any(x => x.Id == id)).Should().BeTrue();
        unexpectedUsersIds.All(id => actual.Any(x => x.Id == id)).Should().BeFalse();
    }
}
