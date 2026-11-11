using System.Globalization;
using MockQueryable.Moq;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Identity.Requests.Core;
using Modsen.Portal.Identity.Requests.Dto;
using Modsen.Portal.Identity.Requests.PermissionAssignments.Requests;
using Modsen.Portal.Identity.Requests.PermissionAssignments.Requests.RequestDto;
using Modsen.Portal.TimeTrack.Contract;
using Modsen.Portal.TimeTrack.Contract.Contract.Output;
using Moq;

namespace Modsen.Portal.Identity.UnitTests.PermissionAssignments.Requests;

public class GetUserPermissionResourcesRequestTests
{
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock = new();
    private readonly Mock<ITimeTrackContract> _timeTrackContract = new();
    private readonly Mock<IIdentityDbContext> _identityDbContextMock = new();
    private readonly Mock<IUserLoader> _userLoaderMock = new();

    private readonly GetUserPermissionResourcesRequest _handler;

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public GetUserPermissionResourcesRequestTests()
    {
        _handler = new GetUserPermissionResourcesRequest(
            _authenticatedUserContextMock.Object,
            _userLoaderMock.Object,
            _timeTrackContract.Object,
            _identityDbContextMock.Object);
    }

    [Theory]
    [InlineData(PortalRoleNames.Ceo, null)]
    [InlineData(PortalRoleNames.Cto, null)]
    [InlineData(PortalRoleNames.HeadOfDepartment, "Net")]
    public async Task HandleWhenRequestProjectResourcesShouldReturnResources(string role, string? departmentId)
    {
        // Arrange
        const int userId = 1;

        var user = new UserDto
        {
            FirstName = "FirstName",
            Email = "test@mail.com",
            DepartmentId = "Net"
        };

        _userLoaderMock.Setup(x => x.LoadAsync(userId, _cancellationToken))
            .ReturnsAsync(user);

        _authenticatedUserContextMock.Setup(x => x.GetRoles())
            .Returns([role]);

        _authenticatedUserContextMock.Setup(x => x.DepartmentId)
            .Returns(departmentId);

        List<int> projectIds = [1, 2];

        var permissionAssignments = new List<PermissionAssignment>()
        {
            new()
            {
                ResourceId = "1",
                UserId = userId,
                Permission = new PermissionDefinition
                {
                    Name = nameof(PermissionType.TrackBillableWorkingHours)
                }
            },
            new()
            {
                ResourceId = "2",
                UserId = userId,
                Permission = new PermissionDefinition
                {
                    Name = nameof(PermissionType.TrackBillableWorkingHours)
                }
            }
        };

        _identityDbContextMock.Setup(x => x.PermissionAssignments)
            .Returns(permissionAssignments.BuildMockDbSet().Object);

        List<ProjectOutput> projects =
        [
            new()
            {
                Id = 1,
                Color = "#111111",
                ClientId = 1,
                IsActive = true,
                Name = "ProjectName1",
                IsPublic = false,
                IsBillable = true,
                ProjectUsers = []
            },

            new()
            {
                Id = 2,
                Color = "#111112",
                ClientId = 1,
                IsActive = true,
                Name = "ProjectName2",
                IsPublic = false,
                IsBillable = true,
                ProjectUsers = []
            }
        ];

        _timeTrackContract.Setup(x => x.GetListOfProjectsByIds(projectIds, _cancellationToken))
            .ReturnsAsync(projects);

        var expectedResult = new List<PermissionResource>
        {
            new()
            {
                ResourceId = projects[0].Id.ToString(CultureInfo.InvariantCulture),
                ResourceName = projects[0].Name
            },
            new()
            {
                ResourceId = projects[1].Id.ToString(CultureInfo.InvariantCulture),
                ResourceName = projects[1].Name
            }
        };

        // Act
        var query = new GetUserPermissionResourcesRequest.Query(userId,
            PermissionType.TrackBillableWorkingHours,
            PermissionResourceType.Project);

        var actualResult = await _handler.Handle(query, _cancellationToken);

        // Assert
        actualResult.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task HandleWhenUserIsNotFoundShouldThrowEntityNotFoundException()
    {
        // Arrange
        const int userId = 1;

        _userLoaderMock
            .Setup(x => x.LoadAsync(userId, _cancellationToken))
            .Throws<EntityNotFoundException>();

        // Act
        var query = new GetUserPermissionResourcesRequest.Query(userId,
            PermissionType.TrackBillableWorkingHours,
            PermissionResourceType.Project);

        var actionUnderTest = () => _handler.Handle(query, _cancellationToken);

        // Assert
        await actionUnderTest.Should().ThrowAsync<EntityNotFoundException>();
    }

    [Fact]
    public async Task HandleWhenHeadOfDepartmentTriesToGetPernissionResourcesOfUserFromAnotherDepartmentShouldThrowActionNotAllowedException()
    {
        // Arrange
        const int userId = 1;

        var user = new UserDto
        {
            FirstName = "FirstName",
            Email = "test@mail.com",
            DepartmentId = "Net"
        };

        _userLoaderMock.Setup(x => x.LoadAsync(userId, _cancellationToken))
            .ReturnsAsync(user);

        _authenticatedUserContextMock.Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.HeadOfDepartment]);

        _authenticatedUserContextMock.Setup(x => x.DepartmentId)
            .Returns("Qa");

        // Act
        var query = new GetUserPermissionResourcesRequest.Query(userId,
            PermissionType.TrackBillableWorkingHours,
            PermissionResourceType.Project);

        var actionUnderTest = () => _handler.Handle(query, _cancellationToken);

        // Assert
        await actionUnderTest.Should().ThrowAsync<ActionNotAllowedException>();
    }
}
