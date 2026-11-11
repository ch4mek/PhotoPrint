using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TimeTrack.UseCases.Core.DbQueries;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.BusinessRules;

namespace Modsen.Portal.TimeTrack.UnitTests.Timesheets.Core.BusinessRules;

public class CanGetSubmitStatusBusinessRuleTests
{
    private readonly Mock<IGetTimeTrackUserDbQuery> _getUserDbQueryMock = new();
    private readonly Mock<IAuthenticatedUserContext> _authContextMock = new();

    private readonly CanGetSubmitStatusBusinessRule _businessRule;
    private readonly CancellationToken _token = CancellationToken.None;

    public CanGetSubmitStatusBusinessRuleTests()
    {
        _businessRule = new CanGetSubmitStatusBusinessRule(
            _authContextMock.Object,
            _getUserDbQueryMock.Object);
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenUserNotFoundReturnsFalse()
    {
        //Arrange
        var userId = 1;

        _getUserDbQueryMock
            .Setup(query => query.ExecuteAsync(userId, _token))
            .ReturnsAsync(null as TimeTrackUser);

        //Act
        var result = await _businessRule.IsCorrespondsToBusiness(userId, _token);

        //Assert
        result.Should().BeFalse();

    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenAuthedAsHeadAndUserIsFromAnotherDepartmentReturnsFalse()
    {
        //Arrange
        var userId = 1;

        _getUserDbQueryMock
            .Setup(query => query.ExecuteAsync(userId, _token))
            .ReturnsAsync(new TimeTrackUser
            {
                DepartmentId = "JS",
                FirstName = "test",
                LastName = "test",
                Email = "test"
            });

        _authContextMock.Setup(context => context.DepartmentId).Returns("Net");
        _authContextMock.Setup(context => context.GetRoles()).Returns([PortalRoleNames.HeadOfDepartment]);

        //Act
        var result = await _businessRule.IsCorrespondsToBusiness(userId, _token);

        //Assert
        result.Should().BeFalse();

    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenAuthedAsGroupManagerAndUserIsFromAnotherGroupReturnsFalse()
    {
        //Arrange
        var userId = 1;

        _getUserDbQueryMock
            .Setup(query => query.ExecuteAsync(userId, _token))
            .ReturnsAsync(new TimeTrackUser
            {
                DepartmentId = "Net",
                ResourceGroupId = 1,
                FirstName = "test",
                LastName = "test",
                Email = "test"
            });

        _authContextMock.Setup(context => context.DepartmentId).Returns("Net");
        _authContextMock.Setup(context => context.ResourceGroupId).Returns(2);
        _authContextMock.Setup(context => context.GetRoles()).Returns([PortalRoleNames.GroupManager]);

        //Act
        var result = await _businessRule.IsCorrespondsToBusiness(userId, _token);

        //Assert
        result.Should().BeFalse();

    }

    [Theory]
    [InlineData(PortalRoleNames.SystemAdministrator)]
    [InlineData(PortalRoleNames.Employee)]
    public async Task IsCorrespondsToBusinessWhenAuthedAsEmployeeOrSysadminAccessAnotherUserReturnsFalse(
        string roleName)
    {
        //Arrange
        var userId = 1;

        _getUserDbQueryMock
            .Setup(query => query.ExecuteAsync(userId, _token))
            .ReturnsAsync(new TimeTrackUser
            {
                DepartmentId = "Net",
                FirstName = "test",
                LastName = "test",
                Email = "test"
            });

        _authContextMock.Setup(context => context.DepartmentId).Returns("Net");
        _authContextMock.Setup(context => context.GetRoles()).Returns([roleName]);
        _authContextMock.Setup(context => context.UserId).Returns(2);

        //Act
        var result = await _businessRule.IsCorrespondsToBusiness(userId, _token);

        //Assert
        result.Should().BeFalse();

    }

    [Theory]
    [InlineData(PortalRoleNames.Cto)]
    [InlineData(PortalRoleNames.Ceo)]
    [InlineData(PortalRoleNames.HeadOfDepartment)]
    [InlineData(PortalRoleNames.GroupManager)]
    public async Task IsCorrespondsToBusinessWhenAuthedAsManagerAndRequestValidReturnsTrue(
        string roleName)
    {
        //Arrange
        var userId = 1;

        _getUserDbQueryMock
            .Setup(query => query.ExecuteAsync(userId, _token))
            .ReturnsAsync(new TimeTrackUser
            {
                DepartmentId = "Net",
                ResourceGroupId = 2,
                FirstName = "test",
                LastName = "test",
                Email = "test"
            });

        _authContextMock.Setup(context => context.DepartmentId).Returns("Net");
        _authContextMock.Setup(context => context.ResourceGroupId).Returns(2);
        _authContextMock.Setup(context => context.GetRoles()).Returns([roleName]);

        //Act
        var result = await _businessRule.IsCorrespondsToBusiness(userId, _token);

        //Assert
        result.Should().BeTrue();

    }

    [Theory]
    [InlineData(PortalRoleNames.SystemAdministrator)]
    [InlineData(PortalRoleNames.Employee)]
    public async Task IsCorrespondsToBusinessWhenEmployeeOrSysadminAccessHisStatusReturnsTrue(string roleName)
    {
        //Arrange
        var userId = 1;

        _getUserDbQueryMock
            .Setup(query => query.ExecuteAsync(userId, _token))
            .ReturnsAsync(new TimeTrackUser
            {
                DepartmentId = "Net",
                FirstName = "test",
                LastName = "test",
                Email = "test"
            });

        _authContextMock.Setup(context => context.DepartmentId).Returns("Net");
        _authContextMock.Setup(context => context.GetRoles()).Returns([roleName]);
        _authContextMock.Setup(context => context.UserId).Returns(1);

        //Act
        var result = await _businessRule.IsCorrespondsToBusiness(userId, _token);

        //Assert
        result.Should().BeTrue();

    }
}
