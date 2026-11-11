using Microsoft.AspNetCore.OData.Query;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.OData;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.OData;
using Modsen.Portal.TeamBoard.Requests.Interviews.Core.AuthenticationFilters;
using Modsen.Portal.TeamBoard.Requests.TeamBoardPortalSettings;
using Moq;

namespace Modsen.Portal.TeamBoard.UnitTests.Interviews.Core.AuthenticationFilters;

public class InterviewsAuthenticationFilterTests
{
    private readonly InterviewsAuthenticationFilter _filter;
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContext = new();
    private readonly Mock<ITeamBoardPortalSettingsAccessor> _teamBoardPortalSettingsAccessor = new();

    public InterviewsAuthenticationFilterTests()
    {
        _filter = new(_authenticatedUserContext.Object, _teamBoardPortalSettingsAccessor.Object);
    }

    private readonly List<UserInterview> _userInterviews =
    [
        new UserInterview
        {
            Id = 1,
            UserId = 1,
            User = new TeamBoardUser
            {
                FirstName = "Dep Net",
                LastName = "User one",
                FirstNameRu = "CreatedBy 1000",
                Email = "Dep Net",
                DepartmentId = "Net",
                ResourceGroupId = null,
            },
            Specialization = "Spec 1",
            GroupId = Guid.CreateVersion7(),
            IsActive = true,
            CreatedById = 1000,
            CreatedByUser = new TeamBoardUser
            {
                UserId = 1000,
                DepartmentId = "Net",
                ResourceGroupId = null,
                Email = "creator-net@test.com",
                FirstName = "Ivan",
                LastName = "Ivanov",
            },
        },

        new UserInterview
        {
            Id = 2,
            UserId = 1,
            User = new TeamBoardUser
            {
                FirstName = "Dep Net",
                LastName = "User one",
                FirstNameRu = "CreatedBy 1000",
                Email = "Dep Net",
                DepartmentId = "Net",
                ResourceGroupId = null,
            },
            Specialization = "Spec 1",
            GroupId = Guid.CreateVersion7(),
            IsActive = true,
            CreatedById = 1000,
            CreatedByUser = new TeamBoardUser
            {
                UserId = 1000,
                DepartmentId = "Net",
                ResourceGroupId = null,
                Email = "creator-net@test.com",
                FirstName = "Semen",
                LastName = "Semenov",
            },
        },

        new UserInterview
        {
            Id = 3,
            UserId = 2,
            User = new TeamBoardUser
            {
                FirstName = "Dep Js",
                LastName = "User two",
                FirstNameRu = "Group 1",
                Email = "Dep Js",
                DepartmentId = "Js",
                ResourceGroupId = 1,
            },
            Specialization = "Spec 1",
            GroupId = Guid.CreateVersion7(),
            IsActive = true,
            CreatedById = 2000,
            CreatedByUser = new TeamBoardUser
            {
                UserId = 2000,
                DepartmentId = "Js",
                ResourceGroupId = 1,
                Email = "creator-js@test.com",
                FirstName = "John",
                LastName = "Doe",
            },
        },

        new UserInterview
        {
            Id = 4,
            UserId = 2,
            User = new TeamBoardUser
            {
                FirstName = "Dep Js",
                LastName = "User two",
                FirstNameRu = "Group 1",
                Email = "Dep Js",
                DepartmentId = "Js",
                ResourceGroupId = 1,
            },
            Specialization = "Spec 1",
            GroupId = Guid.CreateVersion7(),
            IsActive = true,
            CreatedById = 2000,
            CreatedByUser = new TeamBoardUser
            {
                UserId = 2000,
                DepartmentId = "Js",
                ResourceGroupId = 1,
                Email = "creator-js@test.com",
                FirstName = "Peter",
                LastName = "Parker",
            },
        },

        new UserInterview
        {
            Id = 5,
            UserId = 3,
            User = new TeamBoardUser
            {
                FirstName = "Intern",
                LastName = "Intern",
                FirstNameRu = "Intern",
                Email = "Intern",
                DepartmentId = "LD",
                ResourceGroupId = 1,
                Position = PortalRoleNames.Intern,
            },
            Specialization = "Spec 1",
            GroupId = Guid.CreateVersion7(),
            IsActive = true,
            CreatedById = 2000,
            CreatedByUser = new TeamBoardUser
            {
                UserId = 2000,
                DepartmentId = "Js",
                ResourceGroupId = 1,
                Email = "creator-js@test.com",
                FirstName = "Peter",
                LastName = "Parker",
            },
        }

    ];

    [Fact]
    public void ApplyInterviewsWhenUserHasManageAllInterviewsReturnAllInterviews()
    {
        // Arrange
        _authenticatedUserContext.Setup(x => x.HasClaim(StaffingClaimTypeNames.ManageAllInterviews.Name))
            .Returns(true);

        var queryOptions = ODataQueryOptionsMock.BuildMock<OdpInterview>(new Dictionary<string, string?>
        {
            [nameof(ODataRawQueryOptions.Top)] = "20",
            [nameof(ODataRawQueryOptions.Skip)] = "0",
        });

        // Act
        var result = _filter.Apply(_userInterviews.AsQueryable());

        // Assert
        result.Should().HaveCount(4);
    }

    [Fact]
    public void ApplyInterviewsWhenHodHasManageAllInterviewsReturnAllInterviews()
    {
        // Arrange
        _authenticatedUserContext.Setup(x => x.HasClaim(StaffingClaimTypeNames.ManageAllInterviews.Name))
            .Returns(true);

        _authenticatedUserContext.SetupGet(x => x.DepartmentId).Returns("Net");
        _authenticatedUserContext.Setup(x => x.GetRoles()).Returns([PortalRoleNames.HeadOfDepartment]);

        // Act
        var result = _filter.Apply(_userInterviews.AsQueryable());

        // Assert
        result.Should().HaveCount(4);
    }

    [Fact]
    public void ApplyInterviewsWhenGmHasManageAllInterviewsReturnAllInterviews()
    {
        // Arrange
        _authenticatedUserContext.Setup(x => x.HasClaim(StaffingClaimTypeNames.ManageAllInterviews.Name))
            .Returns(true);

        _authenticatedUserContext.SetupGet(x => x.ResourceGroupId).Returns(1);
        _authenticatedUserContext.Setup(x => x.GetRoles()).Returns([PortalRoleNames.GroupManager]);

        // Act
        var result = _filter.Apply(_userInterviews.AsQueryable());

        // Assert
        result.Should().HaveCount(4);
    }

    [Fact]
    public void ApplyInterviewsWhenHodHasManageInterviewsReturnInterviewsOfGroup()
    {
        // Arrange
        _authenticatedUserContext.Setup(x => x.HasClaim(StaffingClaimTypeNames.ManageAllInterviews.Name))
            .Returns(false);

        _authenticatedUserContext.Setup(x => x.HasClaim(StaffingClaimTypeNames.ManageInterviews.Name))
            .Returns(true);

        _authenticatedUserContext.SetupGet(x => x.DepartmentId).Returns("Net");
        _authenticatedUserContext.SetupGet(x => x.UserId).Returns(12);
        _authenticatedUserContext.Setup(x => x.GetRoles()).Returns([PortalRoleNames.HeadOfDepartment]);

        var queryOptions = ODataQueryOptionsMock.BuildMock<OdpInterview>(new Dictionary<string, string?>
        {
            [nameof(ODataRawQueryOptions.Top)] = "20",
            [nameof(ODataRawQueryOptions.Skip)] = "0",
        });

        // Act
        var result = _filter.Apply(_userInterviews.AsQueryable());

        // Assert
        result.Should().OnlyContain(x => x.User!.FirstName == "Dep Net");
        result.Should().HaveCount(2);
    }

    [Fact]
    public void ApplyInterviewsWhenGmHasManageInterviewsReturnInterviewsOfGroup()
    {
        // Arrange
        _authenticatedUserContext.Setup(x => x.HasClaim(StaffingClaimTypeNames.ManageAllInterviews.Name))
            .Returns(false);

        _authenticatedUserContext.Setup(x => x.HasClaim(StaffingClaimTypeNames.ManageInterviews.Name))
            .Returns(true);

        _authenticatedUserContext.SetupGet(x => x.ResourceGroupId).Returns(1);
        _authenticatedUserContext.SetupGet(x => x.UserId).Returns(12);
        _authenticatedUserContext.Setup(x => x.GetRoles()).Returns([PortalRoleNames.GroupManager]);

        var queryOptions = ODataQueryOptionsMock.BuildMock<OdpInterview>(new Dictionary<string, string?>
        {
            [nameof(ODataRawQueryOptions.Top)] = "20",
            [nameof(ODataRawQueryOptions.Skip)] = "0",
        });

        // Act
        var result = _filter.Apply(_userInterviews.AsQueryable());

        // Assert
        result.Should().OnlyContain(x => x.User!.FirstNameRu == "Group 1");
        result.Should().HaveCount(2);
    }

    [Fact]
    public void ApplyInterviewsWhenUserHasNotClaimShouldThrowNotAllowedException()
    {
        // Arrange
        _authenticatedUserContext.Setup(x => x.HasClaim(StaffingClaimTypeNames.ManageAllInterviews.Name))
            .Returns(false);

        _authenticatedUserContext.Setup(x => x.HasClaim(StaffingClaimTypeNames.ManageInterviews.Name))
            .Returns(false);

        var queryOptions = ODataQueryOptionsMock.BuildMock<OdpInterview>(new Dictionary<string, string?>
        {
            [nameof(ODataRawQueryOptions.Top)] = "20",
            [nameof(ODataRawQueryOptions.Skip)] = "0",
        });

        // Act
        var act = () => _filter.Apply(_userInterviews.AsQueryable());

        // Assert
        act.Should().Throw<ActionNotAllowedException>();
    }

    [Fact]
    public void ApplyInterviewsWhenUserHasManageInterviewsReturnCreatedByHimInterviews()
    {
        // Arrange
        _authenticatedUserContext.Setup(x => x.HasClaim(StaffingClaimTypeNames.ManageAllInterviews.Name))
            .Returns(false);

        _authenticatedUserContext.Setup(x => x.HasClaim(StaffingClaimTypeNames.ManageInterviews.Name))
            .Returns(true);

        _authenticatedUserContext.SetupGet(x => x.UserId).Returns(1000);
        _authenticatedUserContext.Setup(x => x.GetRoles()).Returns([PortalRoleNames.Employee]);

        var queryOptions = ODataQueryOptionsMock.BuildMock<OdpInterview>(new Dictionary<string, string?>
        {
            [nameof(ODataRawQueryOptions.Top)] = "20",
            [nameof(ODataRawQueryOptions.Skip)] = "0",
        });

        // Act
        var result = _filter.Apply(_userInterviews.AsQueryable());

        // Assert
        result.Should().OnlyContain(x => x.User!.FirstNameRu == "CreatedBy 1000");
        result.Should().HaveCount(2);
    }
}
