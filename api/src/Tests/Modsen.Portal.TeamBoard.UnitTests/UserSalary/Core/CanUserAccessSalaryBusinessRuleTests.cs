using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.TeamBoard.Requests.Core.CoreDto;
using Modsen.Portal.TeamBoard.Requests.Core.DbQueries;
using Modsen.Portal.TeamBoard.Requests.UserSalary.Core.BusinessRules;
using Moq;

namespace Modsen.Portal.TeamBoard.UnitTests.UserSalary.Core;

public class CanUserAccessSalaryBusinessRuleTests
{
    private readonly Mock<IGetTeamBoardUsersWithNoIncludesDbQuery> _getUserDbQueryMock = new();
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock = new();

    private readonly CanUserAccessSalaryBusinessRule _businessRule;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public CanUserAccessSalaryBusinessRuleTests()
    {
        _businessRule = new CanUserAccessSalaryBusinessRule(
            _getUserDbQueryMock.Object,
            _authenticatedUserContextMock.Object);
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenUserIsNotActiveReturnFalse()
    {
        //Arrange
        const int userId = 1;

        var user = new BasicTeamBoardUserCoreDto()
        {
            UserId = userId,
            DepartmentId = "Net",
            Position = PortalRoleNames.Employee,
            IsActive = false,
            FullName = "fullname",
            ResourceGroupId = null
        };

        _getUserDbQueryMock
            .Setup(query => query.ExecuteAsync(userId, _cancellationToken))
            .ReturnsAsync(user);

        _authenticatedUserContextMock
            .Setup(context => context.UserId)
            .Returns(userId);

        //Act
        var result = await _businessRule.IsCorrespondsToBusiness(userId, _cancellationToken);

        //Assert
        result.Should().BeFalse();

    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenUserCtoGetCeoInfoReturnFalse()
    {
        //Arrange
        const int userId = 1;

        var user = new BasicTeamBoardUserCoreDto()
        {
            UserId = userId,
            DepartmentId = "Net",
            Position = PortalRoleNames.Ceo,
            IsActive = true,
            FullName = "fullname",
            ResourceGroupId = null
        };

        _getUserDbQueryMock
            .Setup(query => query.ExecuteAsync(userId, _cancellationToken))
            .ReturnsAsync(user);

        _authenticatedUserContextMock
            .Setup(context => context.UserId)
            .Returns(userId + 1);

        _authenticatedUserContextMock
            .Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.Cto]);

        //Act
        var result = await _businessRule.IsCorrespondsToBusiness(userId, _cancellationToken);

        //Assert
        result.Should().BeFalse();

    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenHeadGetInfoOfUserFromHisDepartmentReturnTrue()
    {
        //Arrange
        const int userId = 1;

        var user = new BasicTeamBoardUserCoreDto()
        {
            UserId = userId,
            DepartmentId = "Net",
            Position = PortalRoleNames.Employee,
            IsActive = true,
            FullName = "fullname",
            ResourceGroupId = null
        };

        _getUserDbQueryMock
            .Setup(query => query.ExecuteAsync(userId, _cancellationToken))
            .ReturnsAsync(user);

        _authenticatedUserContextMock
            .Setup(context => context.UserId)
            .Returns(userId + 1);

        _authenticatedUserContextMock
            .Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.HeadOfDepartment]);

        _authenticatedUserContextMock
            .Setup(context => context.DepartmentId)
            .Returns("Net");

        //Act
        var result = await _businessRule.IsCorrespondsToBusiness(userId, _cancellationToken);

        //Assert
        result.Should().BeTrue();

    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenHeadGetInfoOfUserNotFromHisDepartmentReturnFalse()
    {
        //Arrange
        const int userId = 1;

        var user = new BasicTeamBoardUserCoreDto()
        {
            UserId = userId,
            DepartmentId = "Net",
            Position = PortalRoleNames.Employee,
            IsActive = true,
            FullName = "fullname",
            ResourceGroupId = null
        };

        _getUserDbQueryMock
            .Setup(query => query.ExecuteAsync(userId, _cancellationToken))
            .ReturnsAsync(user);

        _authenticatedUserContextMock
            .Setup(context => context.UserId)
            .Returns(userId + 1);

        _authenticatedUserContextMock
            .Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.HeadOfDepartment]);

        _authenticatedUserContextMock
            .Setup(context => context.DepartmentId)
            .Returns("JS");

        //Act
        var result = await _businessRule.IsCorrespondsToBusiness(userId, _cancellationToken);

        //Assert
        result.Should().BeFalse();

    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenEmployeeGetsAnotherUserInfoReturnFalse()
    {
        //Arrange
        const int userId = 1;

        var user = new BasicTeamBoardUserCoreDto()
        {
            UserId = userId,
            DepartmentId = "Net",
            Position = PortalRoleNames.Employee,
            IsActive = true,
            FullName = "fullname",
            ResourceGroupId = null
        };

        _getUserDbQueryMock
            .Setup(query => query.ExecuteAsync(userId, _cancellationToken))
            .ReturnsAsync(user);

        _authenticatedUserContextMock
            .Setup(context => context.UserId)
            .Returns(userId + 1);

        _authenticatedUserContextMock
            .Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.Employee]);

        //Act
        var result = await _businessRule.IsCorrespondsToBusiness(userId, _cancellationToken);

        //Assert
        result.Should().BeFalse();

    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenUserCeoGetCtoInfoReturnFalse()
    {
        //Arrange
        const int userId = 1;

        var user = new BasicTeamBoardUserCoreDto()
        {
            UserId = userId,
            DepartmentId = "Net",
            Position = PortalRoleNames.Cto,
            IsActive = true,
            FullName = "fullname",
            ResourceGroupId = null
        };

        _getUserDbQueryMock
            .Setup(query => query.ExecuteAsync(userId, _cancellationToken))
            .ReturnsAsync(user);

        _authenticatedUserContextMock
            .Setup(context => context.UserId)
            .Returns(userId + 1);

        _authenticatedUserContextMock
            .Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.Ceo]);

        //Act
        var result = await _businessRule.IsCorrespondsToBusiness(userId, _cancellationToken);

        //Assert
        result.Should().BeFalse();

    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenCtoGetsEmployeeInformationReturnTrue()
    {
        //Arrange
        const int userId = 1;

        var user = new BasicTeamBoardUserCoreDto
        {
            UserId = userId,
            DepartmentId = "Net",
            Position = PortalRoleNames.Employee,
            IsActive = true,
            FullName = "fullname",
            ResourceGroupId = null
        };

        _getUserDbQueryMock
            .Setup(query => query.ExecuteAsync(userId, _cancellationToken))
            .ReturnsAsync(user);

        _authenticatedUserContextMock
            .Setup(context => context.UserId)
            .Returns(userId + 1);

        _authenticatedUserContextMock
            .Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.Cto]);

        //Act
        var result = await _businessRule.IsCorrespondsToBusiness(userId, _cancellationToken);

        //Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenCeoGetsEmployeeInformationReturnTrue()
    {
        //Arrange
        const int userId = 1;

        var user = new BasicTeamBoardUserCoreDto
        {
            UserId = userId,
            DepartmentId = "Net",
            Position = PortalRoleNames.Employee,
            IsActive = true,
            FullName = "fullname",
            ResourceGroupId = null
        };

        _getUserDbQueryMock
            .Setup(query => query.ExecuteAsync(userId, _cancellationToken))
            .ReturnsAsync(user);

        _authenticatedUserContextMock
            .Setup(context => context.UserId)
            .Returns(userId + 1);

        _authenticatedUserContextMock
            .Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.Ceo]);

        //Act
        var result = await _businessRule.IsCorrespondsToBusiness(userId, _cancellationToken);

        //Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenActiveUserGetsHisOwnInformationReturnTrue()
    {
        //Arrange
        const int userId = 1;

        var user = new BasicTeamBoardUserCoreDto()
        {
            UserId = userId,
            DepartmentId = "Net",
            Position = PortalRoleNames.Employee,
            IsActive = true,
            FullName = "fullname",
            ResourceGroupId = null
        };

        _getUserDbQueryMock
            .Setup(query => query.ExecuteAsync(userId, _cancellationToken))
            .ReturnsAsync(user);

        _authenticatedUserContextMock
            .Setup(context => context.UserId)
            .Returns(userId);

        //Act
        var result = await _businessRule.IsCorrespondsToBusiness(userId, _cancellationToken);

        //Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenSysAdminGetsInformationAboutAnotherUserReturnFalse()
    {
        //Arrange
        const int userId = 1;

        var user = new BasicTeamBoardUserCoreDto()
        {
            UserId = userId,
            DepartmentId = "Net",
            Position = PortalRoleNames.Employee,
            IsActive = true,
            FullName = "fullname",
            ResourceGroupId = 1
        };

        _getUserDbQueryMock
            .Setup(query => query.ExecuteAsync(userId, _cancellationToken))
            .ReturnsAsync(user);

        _authenticatedUserContextMock
            .Setup(context => context.UserId)
            .Returns(userId + 1);

        _authenticatedUserContextMock
            .Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.SystemAdministrator]);

        //Act
        var result = await _businessRule.IsCorrespondsToBusiness(userId, _cancellationToken);

        //Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenGmGetsInformationAboutUserFromHisGroupReturnFalse()
    {
        //Arrange
        const int userId = 1;

        var user = new BasicTeamBoardUserCoreDto()
        {
            UserId = userId,
            DepartmentId = "Net",
            Position = PortalRoleNames.Employee,
            IsActive = true,
            FullName = "fullname",
            ResourceGroupId = 1
        };

        _getUserDbQueryMock
            .Setup(query => query.ExecuteAsync(userId, _cancellationToken))
            .ReturnsAsync(user);

        _authenticatedUserContextMock
            .Setup(context => context.UserId)
            .Returns(userId + 1);

        _authenticatedUserContextMock
            .Setup(context => context.ResourceGroupId)
            .Returns(1);

        //Act
        var result = await _businessRule.IsCorrespondsToBusiness(userId, _cancellationToken);

        //Assert
        result.Should().BeFalse();
    }
}
