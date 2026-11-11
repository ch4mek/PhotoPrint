using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Identity.Requests.PermissionAssignments.Core.BusinessRules;
using Modsen.Portal.TimeTrack.Contract;
using Modsen.Portal.TimeTrack.Contract.Contract.Output;
using Moq;

namespace Modsen.Portal.Identity.UnitTests.PermissionAssignments.Core.BusinessRules;

public class CanUserAccessResourcesBusinessRuleTests
{
    private readonly Mock<ITimeTrackContract> _timeTrackContract = new();

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    private readonly CanUserAccessResourcesBusinessRule _businessRule;

    public CanUserAccessResourcesBusinessRuleTests()
    {
        _businessRule = new CanUserAccessResourcesBusinessRule(_timeTrackContract.Object);
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenModelIsCorrectShouldReturnSuccessfulBusinessRuleResult()
    {
        // Arrange
        const int userId = 1;
        const PermissionResourceType resourceType = PermissionResourceType.Project;
        var resources = new Dictionary<string, string>
        {
            { "1", "Test1" },
            { "2", "Test2" }
        };

        var projectIds = new List<int> { 1, 2 };
        var projectModels = new List<ProjectOutput>
        {
            new()
            {
                Id = 1,
                IsPublic = true,
                IsActive = true,
                ClientId = 1,
                Color = "#111111",
                IsBillable = false,
                Name = "Project 1",
                ProjectUsers = []
            },
            new()
            {
                Id = 2,
                IsPublic = false,
                IsActive = true,
                ClientId = 1,
                Color = "#111111",
                IsBillable = false,
                Name = "Project 1",
                ProjectUsers =
                    [
                        new ProjectUserOutput
                        {
                            UserId = userId
                        }
                    ]
            }
        };

        var model = new CanUserAccessResourcesBusinessRule.Request(userId, resourceType, resources);

        _timeTrackContract.Setup(x =>
                x.GetListOfProjectsByIds(projectIds, _cancellationToken))
            .ReturnsAsync(projectModels);

        // Act
        var result = await _businessRule.IsCorrespondsToBusiness(model, _cancellationToken);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenSomeProjectsDoNotExistShouldThrowNotFoundException()
    {
        // Arrange
        const int userId = 1;
        const PermissionResourceType resourceType = PermissionResourceType.Project;
        var resources = new Dictionary<string, string>
        {
            { "1", "Test1" },
            { "2", "Test2" }
        };

        var projectIds = new List<int> { 1, 2 };
        var existingProjects = new List<ProjectOutput>
        {
            new()
            {
                Id = 1,
                IsPublic = true,
                IsActive = true,
                ClientId = 1,
                Color = "#111111",
                IsBillable = false,
                Name = "Project 1",
                ProjectUsers = []
            }
        };

        var model = new CanUserAccessResourcesBusinessRule.Request(userId, resourceType, resources);

        _timeTrackContract.Setup(x =>
                x.GetListOfProjectsByIds(projectIds, _cancellationToken))
            .ReturnsAsync(existingProjects);

        // Act
        var actionUnderTest = () => _businessRule.IsCorrespondsToBusiness(model, _cancellationToken);

        // Assert
        await actionUnderTest.Should().ThrowAsync<EntityNotFoundException>();
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenUserDoesNotHaveAccessToSomeProjectsShouldReturnFailedBusinessRuleResult()
    {
        // Arrange
        const int userId = 1;
        const PermissionResourceType resourceType = PermissionResourceType.Project;
        var resources = new Dictionary<string, string>
        {
            { "1", "Test1" },
            { "2", "Test2" }
        };

        var projectIds = new List<int> { 1, 2 };
        var existingProjects = new List<ProjectOutput>
        {
            new()
            {
                Id = 1,
                IsPublic = true,
                IsActive = true,
                ClientId = 1,
                Color = "#111111",
                IsBillable = false,
                Name = "Project 1",
                ProjectUsers = []
            },
            new()
            {
                Id = 2,
                IsPublic = false,
                IsActive = true,
                ClientId = 1,
                Color = "#111111",
                IsBillable = false,
                Name = "Project 1",
                ProjectUsers =
                [
                    new ProjectUserOutput
                    {
                        UserId = userId + 1
                    }
                ]
            }
        };

        var model = new CanUserAccessResourcesBusinessRule.Request(userId, resourceType, resources);

        _timeTrackContract.Setup(x =>
                x.GetListOfProjectsByIds(projectIds, _cancellationToken))
            .ReturnsAsync(existingProjects);

        // Act
        var result = await _businessRule.IsCorrespondsToBusiness(model, _cancellationToken);

        // Assert
        result.Should().BeFalse();
    }
}
