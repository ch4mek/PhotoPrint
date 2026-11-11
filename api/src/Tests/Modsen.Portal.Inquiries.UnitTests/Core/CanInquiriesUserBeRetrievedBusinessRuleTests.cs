using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;
using Modsen.Portal.Inquiries.Requests.Core.BusinessRules;
using Moq;

namespace Modsen.Portal.Inquiries.UnitTests.Core;

public class CanInquiriesUserBeRetrievedBusinessRuleTests
{
    private readonly Mock<IAuthenticatedUserContext> _userContextMock;
    private readonly Mock<IInquiriesDbContext> _dbContextMock;
    private readonly CanInquiriesUserBeRetrievedBusinessRule _rule;

    public CanInquiriesUserBeRetrievedBusinessRuleTests()
    {
        _userContextMock = new Mock<IAuthenticatedUserContext>(MockBehavior.Strict);
        _dbContextMock = new Mock<IInquiriesDbContext>(MockBehavior.Strict);
        _rule = new CanInquiriesUserBeRetrievedBusinessRule(_userContextMock.Object, _dbContextMock.Object);
    }

    [Fact]
    public async Task ReturnsTrue_WhenCurrentUserChecksSelf()
    {
        var userId = 1;
        _userContextMock.Setup(c => c.UserId).Returns(userId);

        var result = await _rule.IsCorrespondsToBusiness(userId, CancellationToken.None);

        Assert.True(result);
    }

    [Theory]
    [InlineData("SystemAdministrator")]
    [InlineData("Employee")]
    public async Task ReturnsFalse_ForSystemAdministratorOrEmployee(string role)
    {
        var userId = 2;
        _userContextMock.Setup(c => c.UserId).Returns(1);
        _userContextMock.Setup(c => c.GetRoles()).Returns([role]);

        var result = await _rule.IsCorrespondsToBusiness(userId, CancellationToken.None);

        Assert.False(result);
    }

    [Fact]
    public async Task Throws_WhenUserNotFound()
    {
        var userId = 2;
        _userContextMock.Setup(c => c.UserId).Returns(1);
        _userContextMock.Setup(c => c.GetRoles()).Returns(["HeadOfDepartment"]);

        _dbContextMock.Setup(c => c.InquiriesUsers.FindAsync(new object[] { userId }, It.IsAny<CancellationToken>()))
            .ReturnsAsync((InquiriesUserEntity?)null);

        await Assert.ThrowsAsync<EntityNotFoundException>(() => _rule.IsCorrespondsToBusiness(userId, CancellationToken.None));
    }

    [Fact]
    public async Task Throws_WhenUserInactive()
    {
        var userId = 2;
        _userContextMock.Setup(c => c.UserId).Returns(1);
        _userContextMock.Setup(c => c.GetRoles()).Returns(["HeadOfDepartment"]);

        var user = new InquiriesUserEntity { FirstName = "Test", LastName="User", Email="Test.User@modsen-software.com", IsActive = false };
        _dbContextMock.Setup(c => c.InquiriesUsers.FindAsync(new object[] { userId }, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        await Assert.ThrowsAsync<EntityNotFoundException>(() => _rule.IsCorrespondsToBusiness(userId, CancellationToken.None));
    }

    [Fact]
    public async Task ReturnsFalse_WhenNotManagerAndOtherUser()
    {
        var userId = 2;
        _userContextMock.Setup(c => c.UserId).Returns(1);
        _userContextMock.Setup(c => c.GetRoles()).Returns(["HeadOfDepartment"]);
        _userContextMock.Setup(c => c.IsManager).Returns(false);

        var user = new InquiriesUserEntity { FirstName = "Test", LastName = "User", Email = "Test.User@modsen-software.com", IsActive = true, DepartmentId = "Net" };
        _dbContextMock.Setup(c => c.InquiriesUsers.FindAsync(new object[] { userId }, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var result = await _rule.IsCorrespondsToBusiness(userId, CancellationToken.None);

        Assert.False(result);
    }

    [Fact]
    public async Task ReturnsFalse_WhenHeadOfDepartmentAndDifferentDepartment()
    {
        var userId = 2;
        _userContextMock.Setup(c => c.UserId).Returns(1);
        _userContextMock.Setup(c => c.GetRoles()).Returns(["HeadOfDepartment"]);
        _userContextMock.Setup(c => c.IsManager).Returns(true);
        _userContextMock.Setup(c => c.DepartmentId).Returns("Net");

        var user = new InquiriesUserEntity { FirstName = "Test", LastName = "User", Email = "Test.User@modsen-software.com", IsActive = true, DepartmentId = "HR" };
        _dbContextMock.Setup(c => c.InquiriesUsers.FindAsync(new object[] { userId }, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var result = await _rule.IsCorrespondsToBusiness(userId, CancellationToken.None);

        Assert.False(result);
    }

    [Fact]
    public async Task ReturnsFalse_WhenGroupManagerAndDifferentResourceGroup()
    {
        var userId = 2;
        _userContextMock.Setup(c => c.UserId).Returns(1);
        _userContextMock.Setup(c => c.GetRoles()).Returns(["GroupManager"]);
        _userContextMock.Setup(c => c.IsManager).Returns(true);
        _userContextMock.Setup(c => c.ResourceGroupId).Returns(1);

        var user = new InquiriesUserEntity { FirstName = "Test", LastName = "User", Email = "Test.User@modsen-software.com", IsActive = true, ResourceGroupId = 2 };
        _dbContextMock.Setup(c => c.InquiriesUsers.FindAsync(new object[] { userId }, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var result = await _rule.IsCorrespondsToBusiness(userId, CancellationToken.None);

        Assert.False(result);
    }

    [Fact]
    public async Task ReturnsTrue_WhenAllConditionsPass()
    {
        var userId = 2;
        _userContextMock.Setup(c => c.UserId).Returns(1);
        _userContextMock.Setup(c => c.GetRoles()).Returns(["HeadOfDepartment"]);
        _userContextMock.Setup(c => c.IsManager).Returns(true);
        _userContextMock.Setup(c => c.DepartmentId).Returns("Net");

        var user = new InquiriesUserEntity { FirstName = "Test", LastName = "User", Email = "Test.User@modsen-software.com", IsActive = true, DepartmentId = "Net", ResourceGroupId = 1, Position = "Employee" };
        _dbContextMock.Setup(c => c.InquiriesUsers.FindAsync(new object[] { userId }, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var result = await _rule.IsCorrespondsToBusiness(userId, CancellationToken.None);

        Assert.True(result);
    }
}
