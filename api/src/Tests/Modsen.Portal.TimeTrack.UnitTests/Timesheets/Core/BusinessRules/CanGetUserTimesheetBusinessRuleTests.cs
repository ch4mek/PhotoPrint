using MockQueryable.Moq;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TimeTrack.UseCases.Core.DbQueries;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.BusinessRules;

namespace Modsen.Portal.TimeTrack.UnitTests.Timesheets.Core.BusinessRules;

public class CanGetUserTimesheetBusinessRuleTests
{
    private readonly Mock<IIsUserManagerOfAnyPublicProjectDbQuery> _isUserManagerOfAnyPublicProjectDbQueryMock = new();
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock = new();
    private readonly Mock<IGetTimeTrackUserDbQuery> _getUserDbQueryMock = new();
    private readonly Mock<ITimeTrackDbContext> _dbContextMock = new();

    private readonly CanGetUserTimesheetBusinessRule _canGetUserTimesheetBusinessRule;

    public CanGetUserTimesheetBusinessRuleTests()
    {
        _canGetUserTimesheetBusinessRule = new CanGetUserTimesheetBusinessRule(
            _authenticatedUserContextMock.Object,
            _isUserManagerOfAnyPublicProjectDbQueryMock.Object,
            _getUserDbQueryMock.Object,
            _dbContextMock.Object);
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenUserDoesNotExistsShouldReturnFalse()
    {
        //Act
        var result = await _canGetUserTimesheetBusinessRule.IsCorrespondsToBusiness(
            userId: 1,
            onlyManagedProjects: null,
            CancellationToken.None);

        _authenticatedUserContextMock.Setup(context => context.GetRoles()).Returns(["Employee"]);

        _getUserDbQueryMock.Setup(query => query.ExecuteAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TimeTrackUser?)null);

        //Assert
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData(PortalRoleNames.Employee)]
    [InlineData(PortalRoleNames.SystemAdministrator)]
    public async Task IsCorrespondsToBusinessWhenRoleAllowToGetOnlyOwnTimesheetsAndCurrentUserTryToGetOwnTimesheetsShouldReturnTrue(
        string roleType)
    {
        //Arrange
        const int currentUserId = 1;
        const int timesheetsUserId = 1;

        var userDetails = new TimeTrackUser
        {
            UserId = currentUserId,
            DepartmentId = "Net",
            Email = It.IsAny<string>(),
            ResourceGroupId = null,
            Position = roleType,
            FirstName = "test",
            LastName = "test",
        };

        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns([roleType]);
        _authenticatedUserContextMock.Setup(x => x.UserId).Returns(currentUserId);

        _getUserDbQueryMock.Setup(x => x.ExecuteAsync(currentUserId, CancellationToken.None))
            .ReturnsAsync(userDetails);

        //Act
        var result = await _canGetUserTimesheetBusinessRule.IsCorrespondsToBusiness(
            userId: timesheetsUserId,
            onlyManagedProjects: null,
            CancellationToken.None);

        //Assert
        result.Should().BeTrue();
    }

    [Theory]
    [InlineData(PortalRoleNames.Employee)]
    [InlineData(PortalRoleNames.SystemAdministrator)]
    public async Task IsCorrespondsToBusinessWhenRoleAllowToGetOwnTimesheetsAndCurrentUserTryToGetNotOwnTimesheetsShouldReturnFalse(
        string roleType)
    {
        //Arrange
        const int currentUserId = 1;
        const int timesheetsUserId = 2;

        var userDetails = new TimeTrackUser
        {
            UserId = timesheetsUserId,
            DepartmentId = "Js",
            Email = It.IsAny<string>(),
            FirstName = "test",
            LastName = "test",
            ResourceGroupId = null,
            Position = PortalRoleNames.HeadOfDepartment
        };

        _authenticatedUserContextMock.Setup(context => context.GetRoles()).Returns([roleType]);
        _authenticatedUserContextMock.Setup(context => context.UserId).Returns(currentUserId);

        _getUserDbQueryMock.Setup(query => query.ExecuteAsync(timesheetsUserId, CancellationToken.None))
            .ReturnsAsync(userDetails);

        //Act
        var result = await _canGetUserTimesheetBusinessRule.IsCorrespondsToBusiness(
            userId:timesheetsUserId,
            onlyManagedProjects: null,
            CancellationToken.None);

        //Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenRoleIsHeadOfDepartmentAndUserIsNotFromSameDepartmentAndHeadIsNotManagerOfAnyUserProjectShouldReturnFalse()
    {
        //Arrange
        var headId = 2;
        var currentUserId = 1;
        var userDetails = new TimeTrackUser
        {
            UserId = currentUserId,
            DepartmentId = "Js",
            Email = It.IsAny<string>(),
            ResourceGroupId = null,
            Position = PortalRoleNames.Employee,
            FirstName = "test",
            LastName = "test",
        };

        _authenticatedUserContextMock.Setup(context => context.GetRoles()).Returns([PortalRoleNames.HeadOfDepartment]);
        _authenticatedUserContextMock.Setup(context => context.DepartmentId).Returns("Net");
        _authenticatedUserContextMock.Setup(context => context.UserId).Returns(headId);

        _getUserDbQueryMock.Setup(query => query.ExecuteAsync(currentUserId, CancellationToken.None))
            .ReturnsAsync(userDetails);

        _dbContextMock.Setup(context => context.Projects)
            .Returns(new List<Project>().BuildMockDbSet().Object);

        _isUserManagerOfAnyPublicProjectDbQueryMock
            .Setup(query => query.ExecuteAsync(headId, CancellationToken.None))
            .ReturnsAsync(false);

        //Act
        var result = await _canGetUserTimesheetBusinessRule.IsCorrespondsToBusiness(
            userId: currentUserId,
            onlyManagedProjects: null,
            CancellationToken.None);

        //Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenRoleIsHeadOfDepartmentAndUserIsFromSameDepartmentShouldReturnTrue()
    {
        //Arrange
        var headId = 2;
        var currentUserId = 1;
        var userDetails = new TimeTrackUser
        {
            UserId = currentUserId,
            DepartmentId = "Net",
            Email = It.IsAny<string>(),
            ResourceGroupId = null,
            Position = PortalRoleNames.Employee,
            FirstName = "test",
            LastName = "test",
        };

        _authenticatedUserContextMock.Setup(context => context.GetRoles()).Returns([PortalRoleNames.HeadOfDepartment]);
        _authenticatedUserContextMock.Setup(context => context.DepartmentId).Returns("Net");
        _authenticatedUserContextMock.Setup(context => context.UserId).Returns(headId);

        _getUserDbQueryMock.Setup(dbQuery => dbQuery.ExecuteAsync(currentUserId, CancellationToken.None))
            .ReturnsAsync(userDetails);

        _dbContextMock.Setup(context => context.Projects)
            .Returns(new List<Project>
            {
                new()
                {
                    Name = "test",
                    IsActive = true,
                    ManagerUserId = headId,
                    ProjectUsers = [new ProjectUser { UserId = currentUserId }],
                    Timesheets = [new Timesheet { UserId = currentUserId, TaskName = "Test 1" }]
                }
            }.BuildMockDbSet().Object);

        _isUserManagerOfAnyPublicProjectDbQueryMock
            .Setup(query => query.ExecuteAsync(headId, CancellationToken.None))
            .ReturnsAsync(true);

        //Act
        var result = await _canGetUserTimesheetBusinessRule.IsCorrespondsToBusiness(
            userId: currentUserId,
            onlyManagedProjects: null,
            CancellationToken.None);

        //Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenRoleIsHeadOfDepartmentAndHeadIsManagerOfAnyUserProjectsShouldReturnTrue()
    {
        //Arrange
        var headId = 2;
        var currentUserId = 1;
        var userDetails = new TimeTrackUser
        {
            UserId = currentUserId,
            DepartmentId = "Js",
            Email = It.IsAny<string>(),
            ResourceGroupId = null,
            Position = PortalRoleNames.Employee,
            FirstName = "test",
            LastName = "test",
        };

        _authenticatedUserContextMock.Setup(context => context.GetRoles()).Returns([PortalRoleNames.HeadOfDepartment]);
        _authenticatedUserContextMock.Setup(context => context.DepartmentId).Returns("Net");
        _authenticatedUserContextMock.Setup(context => context.UserId).Returns(headId);

        _dbContextMock.Setup(context => context.Projects)
            .Returns(new List<Project>
            {
                new()
                {
                    Name = "test",
                    IsActive = true,
                    ManagerUserId = headId,
                    ProjectUsers = [new ProjectUser { UserId = currentUserId }]
                }
            }.BuildMockDbSet().Object);

        _getUserDbQueryMock.Setup(query => query.ExecuteAsync(currentUserId, CancellationToken.None))
            .ReturnsAsync(userDetails);

        //Act
        var result = await _canGetUserTimesheetBusinessRule.IsCorrespondsToBusiness(
            userId: currentUserId,
            onlyManagedProjects: null,
            CancellationToken.None);

        //Assert
        result.Should().BeTrue();
    }

    [Theory]
    [InlineData(PortalRoleNames.Cto)]
    [InlineData(PortalRoleNames.Ceo)]
    public async Task IsCorrespondsToBusinessWhenRoleAllowToGetAllTimesheetsShouldReturnTrue(string roleType)
    {
        //Arrange
        var currentUserId = 1;
        var userDetails = new TimeTrackUser
        {
            UserId = currentUserId,
            DepartmentId = "Net",
            Email = It.IsAny<string>(),
            ResourceGroupId = null,
            Position = roleType,
            FirstName = "test",
            LastName = "test",
        };

        _authenticatedUserContextMock.Setup(context => context.GetRoles()).Returns([roleType]);
        _getUserDbQueryMock.Setup(query => query.ExecuteAsync(currentUserId, CancellationToken.None))
            .ReturnsAsync(userDetails);

        //Act
        var result = await _canGetUserTimesheetBusinessRule.IsCorrespondsToBusiness(
            userId: currentUserId,
            onlyManagedProjects: null,
            CancellationToken.None);

        //Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenRoleIsGroupManagerAndUserIsGmShouldReturnTrue()
    {
        //Arrange
        var currentUserId = 1;
        var userDetails = new TimeTrackUser
        {
            UserId = currentUserId,
            DepartmentId = "Js",
            Email = It.IsAny<string>(),
            ResourceGroupId = 1,
            Position = PortalRoleNames.Employee,
            FirstName = "test",
            LastName = "test",
        };

        _authenticatedUserContextMock.Setup(context => context.GetRoles()).Returns([PortalRoleNames.GroupManager]);
        _authenticatedUserContextMock.Setup(context => context.ResourceGroupId).Returns(1);
        _authenticatedUserContextMock.Setup(context => context.UserId).Returns(currentUserId);

        _getUserDbQueryMock.Setup(query => query.ExecuteAsync(currentUserId, CancellationToken.None))
            .ReturnsAsync(userDetails);

        //Act
        var result = await _canGetUserTimesheetBusinessRule.IsCorrespondsToBusiness(
            userId: currentUserId,
            onlyManagedProjects: null,
            CancellationToken.None);

        //Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenRoleIsGroupManagerAndUserIsNotFromSameResourceGroupAndGmIsNotManagerOfAnyUserProjectShouldReturnFalse()
    {
        //Arrange
        var currentUserId = 1;
        var userDetails = new TimeTrackUser
        {
            UserId = currentUserId,
            DepartmentId = "Js",
            Email = It.IsAny<string>(),
            ResourceGroupId = 1,
            Position = PortalRoleNames.Employee,
            FirstName = "test",
            LastName = "test",
        };

        _authenticatedUserContextMock.Setup(context => context.GetRoles()).Returns([PortalRoleNames.GroupManager]);
        _authenticatedUserContextMock.Setup(context => context.ResourceGroupId).Returns(2);
        _authenticatedUserContextMock.Setup(context => context.UserId).Returns(2);

        _getUserDbQueryMock.Setup(query => query.ExecuteAsync(currentUserId, CancellationToken.None))
            .ReturnsAsync(userDetails);

        //Act
        var result = await _canGetUserTimesheetBusinessRule.IsCorrespondsToBusiness(
            userId: currentUserId,
            onlyManagedProjects: null,
            CancellationToken.None);

        //Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenRoleIsGroupManagerAndUserIsFromSameResourceGroupShouldReturnTrue()
    {
        //Arrange
        var currentUserId = 1;
        var userDetails = new TimeTrackUser
        {
            UserId = currentUserId,
            DepartmentId = "Net",
            Email = It.IsAny<string>(),
            ResourceGroupId = 1,
            Position = PortalRoleNames.Employee,
            FirstName = "test",
            LastName = "test",
        };

        _authenticatedUserContextMock.Setup(context => context.GetRoles()).Returns([PortalRoleNames.GroupManager]);
        _authenticatedUserContextMock.Setup(context => context.ResourceGroupId).Returns(1);
        _authenticatedUserContextMock.Setup(context => context.UserId).Returns(2);

        _getUserDbQueryMock.Setup(query => query.ExecuteAsync(currentUserId, CancellationToken.None))
            .ReturnsAsync(userDetails);

        //Act
        var result = await _canGetUserTimesheetBusinessRule.IsCorrespondsToBusiness(
            userId: currentUserId,
            onlyManagedProjects: null,
            CancellationToken.None);

        //Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenRoleIsGroupManagerAndRequestedManagedProjectsShouldReturnFalse()
    {
        //Arrange
        var currentUserId = 1;
        var userDetails = new TimeTrackUser
        {
            UserId = currentUserId,
            DepartmentId = "Net",
            Email = It.IsAny<string>(),
            ResourceGroupId = null,
            Position = PortalRoleNames.Employee,
            FirstName = "test",
            LastName = "test",
        };

        _authenticatedUserContextMock.Setup(context => context.GetRoles()).Returns([PortalRoleNames.GroupManager]);
        _authenticatedUserContextMock.Setup(context => context.ResourceGroupId).Returns(1);
        _authenticatedUserContextMock.Setup(context => context.UserId).Returns(2);

        _getUserDbQueryMock.Setup(query => query.ExecuteAsync(currentUserId, CancellationToken.None))
            .ReturnsAsync(userDetails);

        //Act
        var result = await _canGetUserTimesheetBusinessRule.IsCorrespondsToBusiness(
            userId: currentUserId, onlyManagedProjects: true,
            CancellationToken.None);

        //Assert
        result.Should().BeFalse();
    }
}
