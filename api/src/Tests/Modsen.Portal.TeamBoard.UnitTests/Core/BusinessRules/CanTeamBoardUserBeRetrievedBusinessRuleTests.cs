using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.TeamBoard.Requests.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.Core.CoreDto;
using Modsen.Portal.TeamBoard.Requests.Core.DbQueries;
using Modsen.Portal.TimeTrack.Contract;
using Moq;

namespace Modsen.Portal.TeamBoard.UnitTests.Core.BusinessRules;

public class CanTeamBoardUserBeRetrievedBusinessRuleTests
{
    private readonly Mock<IGetTeamBoardUsersWithNoIncludesDbQuery> _getTeamBoardUserWithNoIncludesDbQuery = new();
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContext = new();
    private readonly Mock<ITimeTrackContract> _timeTrackContract = new();
    private readonly CanTeamBoardUserBeRetrievedBusinessRule _businessRule;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public CanTeamBoardUserBeRetrievedBusinessRuleTests()
    {
        _businessRule = new CanTeamBoardUserBeRetrievedBusinessRule(_getTeamBoardUserWithNoIncludesDbQuery.Object, _authenticatedUserContext.Object,
            _timeTrackContract.Object);
    }

    [Theory]
    [InlineData(AccessOptions.Self)]
    [InlineData(AccessOptions.Manager | AccessOptions.Self)]
    [InlineData(AccessOptions.Manager | AccessOptions.Self | AccessOptions.Hr)]
    [InlineData(AccessOptions.Manager | AccessOptions.Self | AccessOptions.Hr | AccessOptions.Accountant)]
    public async Task IsCorrespondsToBusinessWhenSelfGetReturnTrue(object accessOptions)
    {
        // Arrange
        var userId = 11;
        _authenticatedUserContext.SetupGet(x => x.UserId).Returns(userId);

        // Act
        var result =
            await _businessRule.IsCorrespondsToBusiness(userId, (AccessOptions)accessOptions, _cancellationToken);

        // Assert
        result.Should().BeTrue();
    }

    [Theory]
    [InlineData(PortalRoleNames.Ceo)]
    [InlineData(PortalRoleNames.Cto)]
    public async Task IsCorrespondsToBusinessBatchWhenChiefIsInBatchDoesNotThrowAndExcludesChief(string chiefPosition)
    {
        // Arrange
        const int regularUserId = 11;
        const int chiefUserId = 22;
        const int currentUserId = 99;

        _authenticatedUserContext.SetupGet(x => x.UserId).Returns(currentUserId);
        _authenticatedUserContext.Setup(x => x.GetRequiredUserId()).Returns(currentUserId);
        _authenticatedUserContext.Setup(x => x.GetRoles()).Returns([PortalRoleNames.HeadOfDepartment]);
        _authenticatedUserContext.Setup(x => x.HasClaim(HrClaimTypeNames.ManageHrModule.Name)).Returns(false);
        _authenticatedUserContext.Setup(x => x.DepartmentId).Returns("Department");

        var users = new List<BasicTeamBoardUserCoreDto>
        {
            new()
            {
                UserId = regularUserId,
                IsActive = true,
                DepartmentId = "Department",
                Position = PortalRoleNames.Employee,
            },
            new()
            {
                UserId = chiefUserId, IsActive = true, DepartmentId = "Department", Position = chiefPosition,
            },
        };

        _getTeamBoardUserWithNoIncludesDbQuery
            .Setup(x => x.ExecuteRangeAsync(
                It.Is<ICollection<int>>(ids => ids.Contains(regularUserId) && ids.Contains(chiefUserId)),
                _cancellationToken))
            .ReturnsAsync(users);

        _timeTrackContract
            .Setup(x => x.GetUserIdsByProjectManagerAsync(currentUserId, _cancellationToken))
            .ReturnsAsync([]);

        const AccessOptions accessOptions = AccessOptions.Self | AccessOptions.Manager | AccessOptions.Hr |
            AccessOptions.Accountant | AccessOptions.Coordinator;

        // Act
        var result = await _businessRule.IsCorrespondsToBusinessBatch(
            [regularUserId, chiefUserId], accessOptions, _cancellationToken);

        // Assert
        result.Should().ContainKey(regularUserId).WhoseValue.Should().BeTrue();
        result.Should().NotContainKey(chiefUserId);
    }

    [Theory]
    [InlineData(PortalRoleNames.Ceo)]
    [InlineData(PortalRoleNames.Cto)]
    public async Task IsCorrespondsToBusinessBatchWhenChiefRetrievesSelfReturnsTrue(string chiefPosition)
    {
        // Arrange
        const int chiefUserId = 11;

        _authenticatedUserContext.SetupGet(x => x.UserId).Returns(chiefUserId);
        _authenticatedUserContext.Setup(x => x.GetRequiredUserId()).Returns(chiefUserId);
        _authenticatedUserContext.Setup(x => x.GetRoles()).Returns([chiefPosition]);

        var users = new List<BasicTeamBoardUserCoreDto>
        {
            new() { UserId = chiefUserId, IsActive = true, Position = chiefPosition, },
        };

        _getTeamBoardUserWithNoIncludesDbQuery
            .Setup(x => x.ExecuteRangeAsync(It.IsAny<ICollection<int>>(), _cancellationToken))
            .ReturnsAsync(users);

        _timeTrackContract
            .Setup(x => x.GetUserIdsByProjectManagerAsync(chiefUserId, _cancellationToken))
            .ReturnsAsync([]);

        var accessOptions = AccessOptions.Self | AccessOptions.Manager;

        // Act
        var result = await _businessRule.IsCorrespondsToBusinessBatch(
            [chiefUserId], accessOptions, _cancellationToken);

        // Assert
        result.Should().ContainKey(chiefUserId).WhoseValue.Should().BeTrue();
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenUserIsInactiveReturnThrowEntityConflict()
    {
        // Arrange
        var userId = 11;
        _authenticatedUserContext.SetupGet(x => x.UserId).Returns(userId + 1);
        _authenticatedUserContext.Setup(x => x.GetRoles()).Returns([PortalRoleNames.HeadOfDepartment]);
        _authenticatedUserContext.Setup(x => x.HasClaim(HrClaimTypeNames.ManageHrModule.Name)).Returns(false);

        var user = new BasicTeamBoardUserCoreDto { UserId = userId, IsActive = false, };

        _getTeamBoardUserWithNoIncludesDbQuery.Setup(x => x.ExecuteAsync(userId, _cancellationToken))
            .ReturnsAsync(user);

        var accessOptions = AccessOptions.Self | AccessOptions.Manager | AccessOptions.Hr;

        // Act
        var act = () => _businessRule.IsCorrespondsToBusiness(userId, accessOptions, _cancellationToken);

        // Assert
        await act.Should().ThrowAsync<EntityConflictException>().WithMessage(PortalMessages.UserIsInactive);
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenUserWithHrClaimAndAllowedToHrReturnTrue()
    {
        // Arrange
        var userId = 11;
        _authenticatedUserContext.SetupGet(x => x.UserId).Returns(userId + 1);
        _authenticatedUserContext.Setup(x => x.GetRoles()).Returns([PortalRoleNames.Employee]);
        _authenticatedUserContext.Setup(x => x.HasClaim(HrClaimTypeNames.ManageHrModule.Name)).Returns(true);

        var user = new BasicTeamBoardUserCoreDto { UserId = userId, IsActive = true, };

        _getTeamBoardUserWithNoIncludesDbQuery.Setup(x => x.ExecuteAsync(userId, _cancellationToken))
            .ReturnsAsync(user);

        var accessOptions = AccessOptions.Self | AccessOptions.Manager | AccessOptions.Hr;

        // Act
        var result = await _businessRule.IsCorrespondsToBusiness(userId, accessOptions, _cancellationToken);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenUserWithAccountantClaimAndAllowedToAccountantReturnTrue()
    {
        // Arrange
        var userId = 11;
        _authenticatedUserContext.SetupGet(x => x.UserId).Returns(userId + 1);
        _authenticatedUserContext.Setup(x => x.GetRoles()).Returns([PortalRoleNames.Employee]);
        _authenticatedUserContext.Setup(x => x.HasClaim(AccountantClaimTypeNames.ManageAccountant.Name)).Returns(true);

        var user = new BasicTeamBoardUserCoreDto { UserId = userId, IsActive = true, };

        _getTeamBoardUserWithNoIncludesDbQuery.Setup(x => x.ExecuteAsync(userId, _cancellationToken))
            .ReturnsAsync(user);

        var accessOptions = AccessOptions.Accountant;

        // Act
        var result = await _businessRule.IsCorrespondsToBusiness(userId, accessOptions, _cancellationToken);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenAuthedAsHoDRetrievesUserFromAnotherDepartmentReturnFalse()
    {
        // Arrange
        var userId = 11;
        _authenticatedUserContext.Setup(x => x.IsManager).Returns(true);
        _authenticatedUserContext.SetupGet(x => x.UserId).Returns(userId + 1);
        _authenticatedUserContext.Setup(x => x.GetRoles()).Returns([PortalRoleNames.HeadOfDepartment]);
        _authenticatedUserContext.Setup(x => x.DepartmentId).Returns("Department");
        _authenticatedUserContext.Setup(x => x.HasClaim(HrClaimTypeNames.ManageHrModule.Name)).Returns(false);

        var user = new BasicTeamBoardUserCoreDto
        {
            UserId = userId, IsActive = true, DepartmentId = "Another department"
        };

        _getTeamBoardUserWithNoIncludesDbQuery.Setup(x => x.ExecuteAsync(userId, _cancellationToken))
            .ReturnsAsync(user);

        var accessOptions = AccessOptions.Self | AccessOptions.Manager | AccessOptions.Hr;

        // Act
        var result = await _businessRule.IsCorrespondsToBusiness(userId, accessOptions, _cancellationToken);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenAuthedAsGmRetrievesUserFromAnotherGroupReturnFalse()
    {
        // Arrange
        var userId = 11;
        var resourceGroup = 1;
        _authenticatedUserContext.SetupGet(x => x.UserId).Returns(userId + 1);
        _authenticatedUserContext.Setup(x => x.IsManager).Returns(true);
        _authenticatedUserContext.Setup(x => x.GetRoles()).Returns([PortalRoleNames.GroupManager]);
        _authenticatedUserContext.Setup(x => x.ResourceGroupId).Returns(resourceGroup);
        _authenticatedUserContext.Setup(x => x.HasClaim(HrClaimTypeNames.ManageHrModule.Name)).Returns(false);

        var user = new BasicTeamBoardUserCoreDto
        {
            UserId = userId, IsActive = true, ResourceGroupId = resourceGroup + 1,
        };

        _getTeamBoardUserWithNoIncludesDbQuery.Setup(x => x.ExecuteAsync(userId, _cancellationToken))
            .ReturnsAsync(user);

        var accessOptions = AccessOptions.Self | AccessOptions.Manager | AccessOptions.Hr;

        // Act
        var result = await _businessRule.IsCorrespondsToBusiness(userId, accessOptions, _cancellationToken);

        // Assert
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData(PortalRoleNames.Ceo, PortalRoleNames.Cto)]
    [InlineData(PortalRoleNames.Cto, PortalRoleNames.Cto)]
    [InlineData(PortalRoleNames.Ceo, PortalRoleNames.Ceo)]
    [InlineData(PortalRoleNames.Cto, PortalRoleNames.Ceo)]
    public async Task IsCorrespondsToBusinessWhenRetrieveChiefThrowEntityNotFound(string userPosition,
        string currentUserPosition)
    {
        // Arrange
        var userId = 11;
        _authenticatedUserContext.SetupGet(x => x.UserId).Returns(userId + 1);
        _authenticatedUserContext.Setup(x => x.IsManager).Returns(true);
        _authenticatedUserContext.Setup(x => x.GetRoles()).Returns([currentUserPosition]);
        _authenticatedUserContext.Setup(x => x.HasClaim(HrClaimTypeNames.ManageHrModule.Name)).Returns(false);

        var user = new BasicTeamBoardUserCoreDto { UserId = userId, IsActive = true, Position = userPosition };

        _getTeamBoardUserWithNoIncludesDbQuery.Setup(x => x.ExecuteAsync(userId, _cancellationToken))
            .ReturnsAsync(user);

        var accessOptions = AccessOptions.Self | AccessOptions.Manager | AccessOptions.Hr;

        // Act
        var act = () => _businessRule.IsCorrespondsToBusiness(userId, accessOptions, _cancellationToken);

        // Assert
        await act.Should().ThrowAsync<EntityNotFoundException>().WithMessage(PortalMessages.UserNotFound);
    }
}
