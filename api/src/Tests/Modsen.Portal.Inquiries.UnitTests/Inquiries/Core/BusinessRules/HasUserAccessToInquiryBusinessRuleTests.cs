using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Identity.Contract;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.BusinessRules;
using Modsen.Portal.TimeTrack.Contract;
using Moq;

namespace Modsen.Portal.Inquiries.UnitTests.Inquiries.Core.BusinessRules;

public class HasUserAccessToInquiryBusinessRuleTests
{
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock = new();
    private readonly Mock<IIdentityContract> _identityContractMock = new();
    private readonly Mock<ITimeTrackContract> _timeTrackContractMock = new();

    private readonly CancellationToken _token = CancellationToken.None;

    private readonly HasUserAccessToInquiryBusinessRule _rule;

    public HasUserAccessToInquiryBusinessRuleTests()
    {
        _rule = new HasUserAccessToInquiryBusinessRule(_authenticatedUserContextMock.Object,
            _identityContractMock.Object, _timeTrackContractMock.Object);
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenUserGetHisOwnInquiriesReturnsTrue()
    {
        //Arrange
        const int userId = 1;

        _authenticatedUserContextMock.Setup(x => x.GetRequiredUserId()).Returns(userId);

        //Act
        var result = await _rule.IsCorrespondsToBusiness(userId, AccessOptions.Manager, _token);

        //Assert
        result.Should().BeTrue();

    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenCeoGetsUserInquiriesReturnsTrue()
    {
        //Arrange
        const int userId = 1;

        _authenticatedUserContextMock.Setup(x => x.GetRequiredUserId()).Returns(userId + 1);
        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.Ceo]);

        //Act
        var result = await _rule.IsCorrespondsToBusiness(userId, AccessOptions.Manager, _token);

        //Assert
        result.Should().BeTrue();

    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenCtoGetsUserInquiriesReturnsTrue()
    {
        //Arrange
        const int userId = 1;

        _authenticatedUserContextMock.Setup(x => x.GetRequiredUserId()).Returns(userId + 1);
        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.Cto]);

        //Act
        var result = await _rule.IsCorrespondsToBusiness(userId, AccessOptions.Manager, _token);

        //Assert
        result.Should().BeTrue();

    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenHigherManagerGetsUserInquiriesReturnsTrue()
    {
        //Arrange
        const int userId = 1;
        const int managerId = 2;

        _authenticatedUserContextMock.Setup(x => x.GetRequiredUserId()).Returns(managerId);
        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.Employee]);

        _identityContractMock
            .Setup(contract => contract.GetHigherManagerIds(userId, _token))
            .ReturnsAsync([managerId]);

        //Act
        var result = await _rule.IsCorrespondsToBusiness(userId, AccessOptions.Manager, _token);

        //Assert
        result.Should().BeTrue();

    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenNotHigherManagerGetsUserInquiriesReturnsFalse()
    {
        //Arrange
        const int userId = 1;
        const int managerId = 2;

        _authenticatedUserContextMock.Setup(x => x.GetRequiredUserId()).Returns(managerId);
        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.Employee]);

        _identityContractMock
            .Setup(contract => contract.GetHigherManagerIds(userId, _token))
            .ReturnsAsync([managerId + 1]);

        //Act
        var result = await _rule.IsCorrespondsToBusiness(userId, AccessOptions.Manager, _token);

        //Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenUserHasManageHRClaimReturnsTrue()
    {
        //Arrange
        const int userId = 1;
        const int managerId = 2;

        _authenticatedUserContextMock.Setup(x => x.GetRequiredUserId()).Returns(managerId);
        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.Employee]);
        _authenticatedUserContextMock.Setup(x => x.HasClaim(HrClaimTypeNames.ManageHrModule.Name)).Returns(true);

        _identityContractMock
            .Setup(contract => contract.GetHigherManagerIds(userId, _token))
            .ReturnsAsync([managerId + 1]);

        //Act
        var result = await _rule.IsCorrespondsToBusiness(userId, AccessOptions.Hr, _token);

        //Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenUserIsCoordinatorAndInquiryIsFromProjectUserReturnsTrue()
    {
        //Arrange
        const int userId = 1;
        const int managerId = 2;

        _authenticatedUserContextMock.Setup(x => x.GetRequiredUserId()).Returns(managerId);
        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.Employee]);
        _authenticatedUserContextMock.Setup(x => x.HasClaim(CoordinatorClaimTypeNames.ManageProjectCoordinator.Name)).Returns(true);

        _timeTrackContractMock.Setup(x => x.GetUserIdsByProjectManagerAsync(managerId, _token)).ReturnsAsync([userId]);

        //Act
        var result = await _rule.IsCorrespondsToBusiness(userId, AccessOptions.Coordinator, _token);

        //Assert
        result.Should().BeTrue();
    }


    [Fact]
    public async Task IsCorrespondsToBusinessWhenUserIsCoordinatorAndInquiryIsNotFromProjectUserReturnsFalse()
    {
        //Arrange
        const int userId = 1;
        const int managerId = 2;
        const int anotherUserId = 3;

        _authenticatedUserContextMock.Setup(x => x.GetRequiredUserId()).Returns(managerId);
        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.Employee]);
        _authenticatedUserContextMock.Setup(x => x.HasClaim(CoordinatorClaimTypeNames.ManageProjectCoordinator.Name)).Returns(true);

        _timeTrackContractMock.Setup(x => x.GetUserIdsByProjectManagerAsync(managerId, _token)).ReturnsAsync([anotherUserId]);

        //Act
        var result = await _rule.IsCorrespondsToBusiness(userId, AccessOptions.Coordinator, _token);

        //Assert
        result.Should().BeFalse();
    }
}
