using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Inquiries.Requests.Assigns.Core.BusinessRules;
using Modsen.Portal.Inquiries.Requests.Core;
using Modsen.Portal.Inquiries.Requests.Dto;
using Modsen.Portal.TimeTrack.Contract;
using Moq;

namespace Modsen.Portal.Inquiries.UnitTests.Assigns.Core.BusinessRules;

public class CanAccessUserAssignsBusinessRuleTests
{
    private readonly Mock<IAuthenticatedUserContext> _userContextMock;
    private readonly Mock<IUserLoader> _userLoaderMock;
    private readonly CanAccessUserAssignsBusinessRule _rule;
    private readonly Mock<ITimeTrackContract> _timeTrackContractMock;

    public CanAccessUserAssignsBusinessRuleTests()
    {
        _userContextMock = new Mock<IAuthenticatedUserContext>(MockBehavior.Strict);
        _userLoaderMock = new Mock<IUserLoader>(MockBehavior.Strict);
        _timeTrackContractMock = new Mock<ITimeTrackContract>(MockBehavior.Strict);
        _rule = new CanAccessUserAssignsBusinessRule(_userContextMock.Object, _userLoaderMock.Object, _timeTrackContractMock.Object);
    }

    [Fact]
    public async Task ReturnsFalseWhenUserNotFound()
    {
        const int userId = 1;
        _userLoaderMock.Setup(x => x.GetUserByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((InquiriesUserDto?)null);

        var result = await _rule.IsCorrespondsToBusiness(userId, CancellationToken.None);

        Assert.False(result);
    }

    [Fact]
    public async Task ReturnsTrueWhenUserHasManageHrClaimAndAllowedToHr()
    {
        const int userId = 1;
        var user = new InquiriesUserDto { FirstName = "Test", LastName = "User", Email = "Test.User@modsen-software.com" };
        _userLoaderMock.Setup(x => x.GetUserByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _userContextMock.Setup(x => x.UserId).Returns(99);
        _userContextMock.Setup(x => x.GetRoles()).Returns(["Employee"]);
        _userContextMock.Setup(x => x.HasClaim(HrClaimTypeNames.ManageHrModule.Name)).Returns(true);
        const AccessOptions accessOptions = AccessOptions.Manager | AccessOptions.Hr;

        var result = await _rule.IsCorrespondsToBusiness(userId, accessOptions, CancellationToken.None);

        Assert.True(result);
    }

    [Fact]
    public async Task ReturnTrueWhenUserHasManageAccountantClaimAndAllowedToAccountant()
    {
        const int userId = 1;
        var user = new InquiriesUserDto { FirstName = "Test", LastName = "User", Email = "Test.User@modsen-software.com" };
        _userLoaderMock.Setup(x => x.GetUserByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _userContextMock.Setup(x => x.UserId).Returns(99);
        _userContextMock.Setup(x => x.GetRoles()).Returns(["Employee"]);
        _userContextMock.Setup(x => x.HasClaim(AccountantClaimTypeNames.ManageAccountant.Name)).Returns(true);
        const AccessOptions accessOptions = AccessOptions.Accountant;

        var result = await _rule.IsCorrespondsToBusiness(userId, accessOptions, CancellationToken.None);

        Assert.True(result);
    }

    [Theory]
    [InlineData("CEO")]
    [InlineData("CTO")]
    public async Task ReturnsTrueWhenCurrentUserIsCeoOrCto(string roleName)
    {
        const int userId = 1;
        var user = new InquiriesUserDto { FirstName = "Test", LastName = "User", Email = "Test.User@modsen-software.com", DepartmentId = "Net", ResourceGroupId = 20 };
        _userLoaderMock.Setup(x => x.GetUserByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _userContextMock.Setup(x => x.UserId).Returns(99);
        _userContextMock.Setup(x => x.GetRoles()).Returns([roleName]);
        _userContextMock.Setup(x => x.HasClaim(HrClaimTypeNames.ManageHrModule.Name)).Returns(false);

        var result = await _rule.IsCorrespondsToBusiness(userId, CancellationToken.None);

        Assert.True(result);
    }

    [Theory]
    [InlineData("Employee")]
    [InlineData("SystemAdministrator")]
    public async Task ReturnsTrueWhenCurrentUserIsEmployeeOrAdminAndSameUser(string roleName)
    {
        const int userId = 1;
        var user = new InquiriesUserDto { FirstName = "Test", LastName = "User", Email = "Test.User@modsen-software.com", Id = userId };
        _userLoaderMock.Setup(x => x.GetUserByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _userContextMock.Setup(x => x.GetRoles()).Returns([roleName]);
        _userContextMock.Setup(x => x.UserId).Returns(userId);
        _userContextMock.Setup(x => x.HasClaim(HrClaimTypeNames.ManageHrModule.Name)).Returns(false);

        var result = await _rule.IsCorrespondsToBusiness(userId, CancellationToken.None);

        Assert.True(result);
    }

    [Fact]
    public async Task ReturnsTrueWhenHeadOfDepartmentAndSameDepartment()
    {
        const int userId = 1;
        var user = new InquiriesUserDto { FirstName = "Test", LastName = "User", Email = "Test.User@modsen-software.com", DepartmentId = "Net" };
        _userLoaderMock.Setup(x => x.GetUserByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _userContextMock.Setup(x => x.UserId).Returns(99);
        _userContextMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.HeadOfDepartment]);
        _userContextMock.Setup(x => x.DepartmentId).Returns("Net");
        _userContextMock.Setup(x => x.HasClaim(HrClaimTypeNames.ManageHrModule.Name)).Returns(false);
        _userContextMock.Setup(x => x.IsHeadOfDepartmentFrom(user.DepartmentId)).Returns(true);
        _userContextMock.Setup(x => x.IsGroupManagerFrom(user.ResourceGroupId)).Returns(false);

        var result = await _rule.IsCorrespondsToBusiness(userId, CancellationToken.None);

        Assert.True(result);
    }

    [Fact]
    public async Task ReturnsTrueWhenGroupManagerAndSameResourceGroup()
    {
        const int userId = 1;
        const string departmentId = "Net";

        var user = new InquiriesUserDto
        {
            FirstName = "Test",
            LastName = "User",
            Email = "Test.User@modsen-software.com",
            ResourceGroupId = 20,
            DepartmentId = departmentId
        };
        _userLoaderMock.Setup(x => x.GetUserByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _userContextMock.Setup(x => x.UserId).Returns(99);
        _userContextMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.GroupManager]);
        _userContextMock.Setup(x => x.ResourceGroupId).Returns(20);
        _userContextMock.Setup(x => x.HasClaim(HrClaimTypeNames.ManageHrModule.Name)).Returns(false);
        _userContextMock.Setup(x => x.DepartmentId).Returns(departmentId);
        _userContextMock.Setup(x => x.IsHeadOfDepartmentFrom(user.DepartmentId)).Returns(false);
        _userContextMock.Setup(x => x.IsGroupManagerFrom(user.ResourceGroupId)).Returns(true);

        var result = await _rule.IsCorrespondsToBusiness(userId, CancellationToken.None);

        Assert.True(result);
    }

    [Fact]
    public async Task ReturnsFalseWhenNoMatchingRules()
    {
        const int userId = 1;
        var user = new InquiriesUserDto { FirstName = "Test", LastName = "User", Email = "Test.User@modsen-software.com", DepartmentId = "Net", ResourceGroupId = 20 };
        _userLoaderMock.Setup(x => x.GetUserByIdAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _userContextMock.Setup(x => x.GetRoles()).Returns(["SomeOtherRole"]);
        _userContextMock.Setup(x => x.UserId).Returns(2);
        _userContextMock.Setup(x => x.DepartmentId).Returns("Js");
        _userContextMock.Setup(x => x.IsHeadOfDepartmentFrom(user.DepartmentId)).Returns(false);
        _userContextMock.Setup(x => x.IsGroupManagerFrom(user.ResourceGroupId)).Returns(false);
        _userContextMock.Setup(x => x.ResourceGroupId).Returns(99);
        _userContextMock.Setup(x => x.HasClaim(HrClaimTypeNames.ManageHrModule.Name)).Returns(false);

        var result = await _rule.IsCorrespondsToBusiness(userId, CancellationToken.None);

        Assert.False(result);
    }

    [Fact]
    public async Task ReturnsTrueWhenUserTriesToAccessAnotherUserWithProjectCoordinatorClaim()
    {
        const int managerId = 1;
        const int someEmployeeId = 2;
        const string departmentId = "Net";
        const AccessOptions accessOptions = AccessOptions.Manager | AccessOptions.Hr | AccessOptions.Accountant | AccessOptions.Coordinator;

        var user = new InquiriesUserDto
        {
            Id = someEmployeeId,
            FirstName = "Some employee first name",
            LastName = "Some employee last name",
            Email = "Test.User@modsen-software.com",
            DepartmentId = departmentId
        };
        _userLoaderMock.Setup(x => x.GetUserByIdAsync(someEmployeeId, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _userContextMock.Setup(x => x.UserId).Returns(managerId);
        _userContextMock.Setup(x => x.GetRoles()).Returns(["SomeRole"]);
        _userContextMock.Setup(x => x.DepartmentId).Returns(departmentId);
        _userContextMock.Setup(x => x.GetRequiredUserId()).Returns(managerId);
        _userContextMock.Setup(x => x.HasClaim(It.Is<string>(str => CoordinatorClaimTypeNames.ManageProjectCoordinator.Name.Equals(str)))).Returns(true);
        _userContextMock.Setup(x => x.HasClaim(It.Is<string>(str => CoordinatorClaimTypeNames.ManageProjectCoordinator.Name.Equals(str) == false))).Returns(false);
        _timeTrackContractMock.Setup(x => x.GetUserIdsByProjectManagerAsync(managerId, It.IsAny<CancellationToken>())).ReturnsAsync([someEmployeeId]);

        var result = await _rule.IsCorrespondsToBusiness(someEmployeeId, accessOptions,  CancellationToken.None);

        Assert.True(result);
    }

    [Fact]
    public async Task ReturnsTrueWhenUserTriesToAccessAnotherUserWithProjectCoordinatorClaimButWithoutRelationToUserButHavingHeadOfDepartmentRole()
    {
        const int managerId = 1;
        const int someEmployeeId = 2;
        const string departmentId = "Net";
        const AccessOptions accessOptions = AccessOptions.Manager | AccessOptions.Hr | AccessOptions.Accountant | AccessOptions.Coordinator;

        var user = new InquiriesUserDto
        {
            Id = someEmployeeId,
            FirstName = "Some employee first name",
            LastName = "Some employee last name",
            Email = "Test.User@modsen-software.com",
            DepartmentId = departmentId
        };
        _userLoaderMock.Setup(x => x.GetUserByIdAsync(someEmployeeId, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _userContextMock.Setup(x => x.UserId).Returns(managerId);
        _userContextMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.HeadOfDepartment]);
        _userContextMock.Setup(x => x.DepartmentId).Returns(departmentId);
        _userContextMock.Setup(x => x.IsHeadOfDepartmentFrom(user.DepartmentId)).Returns(true);
        _userContextMock.Setup(x => x.GetRequiredUserId()).Returns(managerId);
        _userContextMock.Setup(x => x.HasClaim(It.Is<string>(str => CoordinatorClaimTypeNames.ManageProjectCoordinator.Name.Equals(str)))).Returns(true);
        _userContextMock.Setup(x => x.HasClaim(It.Is<string>(str => CoordinatorClaimTypeNames.ManageProjectCoordinator.Name.Equals(str) == false))).Returns(false);
        _timeTrackContractMock.Setup(x => x.GetUserIdsByProjectManagerAsync(managerId, It.IsAny<CancellationToken>())).ReturnsAsync([]);

        var result = await _rule.IsCorrespondsToBusiness(someEmployeeId, accessOptions,  CancellationToken.None);

        Assert.True(result);
    }
}
