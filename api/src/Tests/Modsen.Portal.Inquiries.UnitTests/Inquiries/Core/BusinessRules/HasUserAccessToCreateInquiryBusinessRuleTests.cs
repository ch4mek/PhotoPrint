using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Identity.Contract;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.BusinessRules;
using Moq;

namespace Modsen.Portal.Inquiries.UnitTests.Inquiries.Core.BusinessRules;

public class hasUserAccessToMutateInquiryBusinessRuleTests
{
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock = new();
    private readonly Mock<IIdentityContract> _identityContractMock = new();

    private readonly CancellationToken _token = CancellationToken.None;

    private readonly HasUserAccessToMutateInquiryBusinessRule _rule;

    public hasUserAccessToMutateInquiryBusinessRuleTests()
    {
        _rule = new HasUserAccessToMutateInquiryBusinessRule(_identityContractMock.Object,
            _authenticatedUserContextMock.Object);
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenUserCreatesInquiriesForHimReturnsTrue()
    {
        //Arrange
        const int userId = 1;

        _authenticatedUserContextMock.Setup(x => x.GetRequiredUserId()).Returns(userId);

        //Act
        var result = await _rule.IsCorrespondsToBusiness(userId, _token);

        //Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenCeoCreatesUserInquiriesReturnsTrue()
    {
        //Arrange
        const int userId = 1;

        _authenticatedUserContextMock.Setup(x => x.GetRequiredUserId()).Returns(userId + 1);
        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.Ceo]);

        //Act
        var result = await _rule.IsCorrespondsToBusiness(userId, _token);

        //Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenCtoCreatesUserInquiriesReturnsTrue()
    {
        //Arrange
        const int userId = 1;

        _authenticatedUserContextMock.Setup(x => x.GetRequiredUserId()).Returns(userId + 1);
        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.Cto]);

        //Act
        var result = await _rule.IsCorrespondsToBusiness(userId, _token);

        //Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenHigherManagerCreatesUserInquiriesReturnsTrue()
    {
        //Arrange
        const int userId = 1;
        const int managerId = 2;

        _authenticatedUserContextMock.Setup(x => x.GetRequiredUserId()).Returns(managerId);
        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.GroupManager]);

        _identityContractMock
            .Setup(contract => contract.GetHigherManagerIds(userId, _token))
            .ReturnsAsync([managerId]);

        //Act
        var result = await _rule.IsCorrespondsToBusiness(userId, _token);

        //Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenNotHigherManagerCreatesUserInquiriesReturnsFalse()
    {
        //Arrange
        const int userId = 1;
        const int managerId = 2;

        _authenticatedUserContextMock.Setup(x => x.GetRequiredUserId()).Returns(managerId);
        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.HeadOfDepartment]);

        _identityContractMock
            .Setup(contract => contract.GetHigherManagerIds(userId, _token))
            .ReturnsAsync([managerId + 1]);

        //Act
        var result = await _rule.IsCorrespondsToBusiness(userId, _token);

        //Assert
        result.Should().BeFalse();
    }
}
