using System.Net;
using System.Text.Json;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
namespace Modsen.Portal.IntegrationTests.Identity.Controllers.DepartmentsController;

[Collection("GeneralCollection")]
public class DepartmentsControllerIntegrationTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;

    public DepartmentsControllerIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
    }

    [Fact]
    public async Task GetDepartmentsWhenNotAuthorizedReturnUnauthorizedResponse()
    {
        // Arrange
        using var client = _factory.CreateClient();

        // Act
        var response = await client.GetDepartmentsAsHttpMessage();

        // Assert
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetDepartmentsWhenAuthorizedAsEmployeeReturnForbiddenResponse()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsEmployee();

        // Act
        var response = await client.GetDepartmentsAsHttpMessage();

        // Assert
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetDepartmentsWhenAuthorizedAsCeoAndNewPhpHeadAddedReturnValidDepartmentsListWithPhpHead()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();
        const string departmentId = "Net";

        await client.CreateResourceGroup(departmentId: departmentId);
        var createdPhpHead = await client.CreateHeadOfDepartment("PHP");

        // Act
        var departments = await client.GetDepartments();

        // Assert
        departments.Should().NotBeNullOrEmpty();

        var departmentNames = new HashSet<string>
        {
            "JavaScript", ".NET", "Java", "Python", "QA", "BA", "HR", "Marketing",
            "Sales", "PMO", "Mobile", "PHP", "DevOps", "Administration", "Design",
            "Ruby", "System Administrators", "1C", "Recruitment"
        };

        departments.Select(d => d.DepartmentName).Should().Contain(departmentNames);

        departments.Should().ContainSingle(d => d.DepartmentName == "PHP" && d.HeadOfDepartmentId == createdPhpHead!.Email);
        departments.Should().Contain(d => d.DepartmentName == ".NET" && d.Groups != null && d.Groups.Count >= 2);
        departments.Should().Contain(d => d.DepartmentName == "Java" && d.Groups != null && d.Groups.Count >= 1);
    }

    [Fact]
    public async Task GetDepartmentsWhenAuthorizedAsHeadReturnValidDepartmentsListWithGroupsOfHeadDepartment()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        const string departmentId = "Java";
        await client.CreateResourceGroup(departmentId: departmentId);

        await client.AuthAsHeadOfDepartment();

        // Act
        var departments = await client.GetDepartments();

        // Assert
        departments.Should().NotBeNullOrEmpty();

        var departmentNames = new HashSet<string>
        {
            "JavaScript", ".NET", "Java", "Python", "QA", "BA", "HR", "Marketing",
            "Sales", "PMO", "Mobile", "PHP", "DevOps", "Administration", "Design",
            "Ruby", "System Administrators", "1C", "Recruitment"
        };

        departments.Select(d => d.DepartmentName).Should().Contain(departmentNames);

        departments.Should().Contain(d => d.DepartmentName == ".NET" && d.Groups != null && d.Groups.Count >= 1);
        departments.Should().Contain(d => d.DepartmentName == "Java" && d.Groups != null && d.Groups.Count >= 1);
    }
}
