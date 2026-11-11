using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.Contracts.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.Contracts.Core.BusinessRules.Models;
using Moq;

namespace Modsen.Portal.TeamBoard.UnitTests.Contracts.Core;

public class IsUserHasPermissionsToViewContractsBusinessRuleTests
{
    private readonly IsUserHasPermissionsToViewContractsBusinessRule _businessRule;
    private readonly Mock<IIsUserHasPermissionsToMutateContractsBusinessRule> _isUserHasPermissionsToMutateBusinessRule = new();
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContext = new();

    public IsUserHasPermissionsToViewContractsBusinessRuleTests()
    {
        _businessRule = new IsUserHasPermissionsToViewContractsBusinessRule(
            _isUserHasPermissionsToMutateBusinessRule.Object, _authenticatedUserContext.Object);
    }

    [Fact]
    public async Task IsCorrespondingToBusinessWhenUserHasManageAccountantClaimReturnSuccess()
    {
        // Arrange
        _authenticatedUserContext.Setup(x => x.HasClaim(AccountantClaimTypeNames.ManageAccountant.Name)).Returns(true);
        var model = It.IsAny<IsUserHasPermissionBusinessModel>();

        // Act
        var result = await _businessRule.IsCorrespondsToBusiness(model, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Theory]
    [InlineData(true, null)]
    [InlineData(false, "fail reason")]
    public async Task IsCorrespondingToBusinessWhenUserDoesNotHaveManageAccountantClaimReturnValid(bool isSuccess, string? failReason)
    {
        // Arrange
        _authenticatedUserContext.Setup(x => x.HasClaim(AccountantClaimTypeNames.ManageAccountant.Name)).Returns(false);

        var businessResult = isSuccess
            ? BusinessRuleResult.Success()
            : BusinessRuleResult.Fail(failReason!);

        var model = It.IsAny<IsUserHasPermissionBusinessModel>();
        _isUserHasPermissionsToMutateBusinessRule.Setup(x => x.IsCorrespondsToBusiness(model, CancellationToken.None))
            .ReturnsAsync(businessResult);

        // Act
        var result = await _businessRule.IsCorrespondsToBusiness(model, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(businessResult);
    }
}
