using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.TeamBoard.Requests.Contracts.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.Contracts.Core.BusinessRules.Models;
using Moq;

namespace Modsen.Portal.TeamBoard.UnitTests.Contracts.Core
{
    public class IsUserHasPermissionsToMutateContractsBusinessRuleTests
    {
        private readonly Mock<IAuthenticatedUserContext> _mockUserContext;
        private readonly IsUserHasPermissionsToMutateContractsBusinessRule _businessRule;

        public IsUserHasPermissionsToMutateContractsBusinessRuleTests()
        {
            _mockUserContext = new Mock<IAuthenticatedUserContext>();
            _businessRule = new IsUserHasPermissionsToMutateContractsBusinessRule(_mockUserContext.Object);
        }

        [Fact]
        public async Task IsCorrespondsToBusinessShouldReturnSuccessWhenUserHasHRModuleClaim()
        {
            // Arrange

            _mockUserContext
                .Setup(ctx => ctx.HasClaim(HrClaimTypeNames.ManageHrModule.Name))
                .Returns(true);

            // Act
            var result = await _businessRule.IsCorrespondsToBusiness(It.IsAny<IsUserHasPermissionBusinessModel>(), CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
        }

        [Theory]
        [InlineData(PortalRoleNames.Ceo)]
        [InlineData(PortalRoleNames.Cto)]
        public async Task IsCorrespondsToBusinessShouldReturnFailWhenUserIsCeoOrCto(string role)
        {
            // Arrange
            var userId = 12;
            var model = new IsUserHasPermissionBusinessModel(userId, role, null, null, true);

            // Act
            var result = await _businessRule.IsCorrespondsToBusiness(model, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Reason.Should().Be($"There is no user with Id {userId}");
        }

        [Fact]
        public async Task IsCorrespondsToBusinessShouldReturnFailWhenUserTriesToMutateOwnContract()
        {
            // Arrange
            var userId = 12;
            _mockUserContext
                .Setup(ctx => ctx.GetRequiredUserId())
                .Returns(userId);
            var model = new IsUserHasPermissionBusinessModel(userId, null, null, null, true);

            // Act
            var result = await _businessRule.IsCorrespondsToBusiness(model, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Reason.Should().Be($"You don’t have an access to mutate your contracts");
        }

        [Fact]
        public async Task IsCorrespondsToBusinessShouldReturnFailWhenHeadOfDepartmentAccessingOtherDepartment()
        {
            // Arrange
            var userId = 5;

            var model = new IsUserHasPermissionBusinessModel(userId, PortalRoleNames.Employee, "Java", null, true);

            _mockUserContext
                .Setup(ctx => ctx.GetRoles())
                .Returns([PortalRoleNames.HeadOfDepartment]);
            _mockUserContext
                .Setup(ctx => ctx.DepartmentId)
                .Returns("Net");

            // Act
            var result = await _businessRule.IsCorrespondsToBusiness(model, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Reason.Should().Be("You don’t have an access to users from other departments");
        }

        [Fact]
        public async Task IsCorrespondsToBusinessShouldReturnFailWhenGroupManagerAccessingOtherResourceGroup()
        {
            // Arrange
            var userId = 6;

            var model = new IsUserHasPermissionBusinessModel(userId, PortalRoleNames.Employee, "Java", 2, true);

            _mockUserContext
                .Setup(ctx => ctx.GetRoles())
                .Returns([PortalRoleNames.GroupManager]);
            _mockUserContext
                .Setup(ctx => ctx.DepartmentId)
                .Returns("Java");
            _mockUserContext
                .Setup(ctx => ctx.ResourceGroupId)
                .Returns(1); 

            // Act
            var result = await _businessRule.IsCorrespondsToBusiness(model, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Reason.Should().Be("You don’t have an access to users from other groups");
        }

        [Fact]
        public async Task IsCorrespondsToBusinessShouldReturnFailWhenGroupManagerAccessingOtherDepartment()
        {
            // Arrange
            var userId = 6;

            var model = new IsUserHasPermissionBusinessModel(userId, PortalRoleNames.Employee, "Java", 2, true);

            _mockUserContext
                .Setup(ctx => ctx.GetRoles())
                .Returns([PortalRoleNames.GroupManager]);
            _mockUserContext
                .Setup(ctx => ctx.DepartmentId)
                .Returns("Net");
            _mockUserContext
                .Setup(ctx => ctx.ResourceGroupId)
                .Returns(1);

            // Act
            var result = await _businessRule.IsCorrespondsToBusiness(model, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Reason.Should().Be("You don’t have an access to users from other departments");
        }

        [Fact]
        public async Task IsCorrespondsToBusinessShouldReturnSuccessWhenUserMeetsAllCriteria()
        {
            // Arrange
            var userId = 7;
            var model = new IsUserHasPermissionBusinessModel(userId, PortalRoleNames.Employee, "Java", 1, true);

            _mockUserContext
                .Setup(ctx => ctx.GetRoles())
                .Returns([PortalRoleNames.GroupManager]);
            _mockUserContext
                .Setup(ctx => ctx.DepartmentId)
                .Returns("Java");
            _mockUserContext
                .Setup(ctx => ctx.ResourceGroupId)
                .Returns(1); 

            // Act
            var result = await _businessRule.IsCorrespondsToBusiness(model, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
        }
    }
}
