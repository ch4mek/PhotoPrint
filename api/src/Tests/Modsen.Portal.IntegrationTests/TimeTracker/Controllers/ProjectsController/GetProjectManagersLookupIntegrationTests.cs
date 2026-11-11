using System.Text.Json;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TimeTracker;
using Modsen.Portal.TimeTrack.Controllers.Contracts.Output;
namespace Modsen.Portal.IntegrationTests.TimeTracker.Controllers.ProjectsController;

public class GetProjectManagersLookupIntegrationTests : IClassFixture<IntegrationTestFactory>
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;

    public GetProjectManagersLookupIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
    }

    [Fact]
    public async Task GetProjectManagersLookupAsyncWhenProjectManagerUserExistsShouldReturnUser()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var firstManager = await client.CreateEmployee(departmentId: "PMO");
        var firstManagerId = firstManager!.Id;
        var secondManager = await client.CreateEmployee(departmentId: "PMO");
        var secondManagerId = secondManager!.Id;

        const string filter = "((departmentId eq 'PMO') and (position eq 'HeadOfDepartment'))";

        var users = await client.GetUsersWithSearch(filter: filter, top: 1);
        var headOfPmo = users.Items.First();

        var expectedResult = new List<ProjectManagersLookupOutput>
        {
            new
            (
                Id: headOfPmo.Id,
                FullName: $"{headOfPmo.LastName} {headOfPmo.FirstName}"
            ),
            new
            (
                Id: firstManagerId,
                FullName: $"{firstManager.LastName} {firstManager.FirstName}"
            ),
            new
            (
                Id: secondManagerId,
                FullName: $"{secondManager.LastName} {secondManager.FirstName}"
            )
        };

        // Act
        var projectManagers = await client.GetProjectManagersLookupAsync();

        // Assert
        projectManagers.Should().BeEquivalentTo(expectedResult);
    }
}

