using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Inquiries.Requests.Core;
using Modsen.Portal.Inquiries.Requests.InquiryTemplates.Core;
using Modsen.Portal.Inquiries.Requests.InquiryTemplates.Core.CoreDto;
using Moq;

namespace Modsen.Portal.Inquiries.UnitTests.InquiryTemplates.Core {
    public class AcceptPoliciesValidatorTests
    {
        private readonly Mock<IUserLoader> _userLoaderMock;
        private readonly AcceptPoliciesValidator _validator;

        public AcceptPoliciesValidatorTests()
        {
            _userLoaderMock = new Mock<IUserLoader>(MockBehavior.Strict);
            _validator = new AcceptPoliciesValidator(_userLoaderMock.Object);
        }

        [Fact]
        public async Task ValidateAllUsersExistShouldNotThrow()
        {
            // Arrange
            var policies = new List<InquiryTemplateAcceptPolicyDto>
        {
            new() { Id = 1, PolicyScope = InquiryAcceptPolicyScopes.User, UserId = 1 },
            new() { Id = 2, PolicyScope = InquiryAcceptPolicyScopes.User, UserId = 2 }
        };

            _userLoaderMock
                .Setup(x => x.GetUsersByIdsAsync(It.IsAny<int[]>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync([
                    new() { Id = 1, FirstName = "John", LastName = "Doe", Email = "John.Doe@modsensoftware.com" },
                    new() { Id = 2, FirstName = "Jane", LastName = "Smith", Email = "Jane.Smith@modsensoftware.com" }
                ]);

            // Act
            var action = async () => await _validator.Validate(policies, CancellationToken.None);

            // Assert
            await action.Should().NotThrowAsync();
        }

        [Fact]
        public async Task ValidateSomeUsersMissingShouldThrowEntityNotFoundException()
        {
            // Arrange
            var policies = new List<InquiryTemplateAcceptPolicyDto>
        {
            new() { Id = 1, PolicyScope = InquiryAcceptPolicyScopes.User, UserId = 1 },
            new() { Id = 2, PolicyScope = InquiryAcceptPolicyScopes.User, UserId = 2 },
            new() { Id = 3, PolicyScope = InquiryAcceptPolicyScopes.User, UserId = 3 }
        };

            _userLoaderMock
                .Setup(x => x.GetUsersByIdsAsync(It.IsAny<int[]>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync([
                    new() { FirstName = "Test", LastName = "User", Email = "Test.User@modsen-software.com", Id = 1 },
                    new() { FirstName = "Test2", LastName = "User2", Email = "Test.Usera@modsen-software.com", Id = 3 }
                ]);

            // Act
            var action = async () => await _validator.Validate(policies, CancellationToken.None);

            // Assert
            var ex = await action.Should().ThrowAsync<EntityNotFoundException>();
            ex.Which.Message.Should().Contain("Users [2]");
        }

        [Fact]
        public async Task ValidateNoPoliciesShouldNotCallUserLoader()
        {
            // Arrange
            var policies = new List<InquiryTemplateAcceptPolicyDto>();

            _userLoaderMock
                .Setup(x => x.GetUsersByIdsAsync(
                    It.Is<int[]>(ids => ids.Length == 0),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync([]);

            // Act
            await _validator.Validate(policies, CancellationToken.None);

            // Assert
            _userLoaderMock.Verify(x => x.GetUsersByIdsAsync(
                It.Is<int[]>(ids => ids.Length == 0),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ValidatePoliciesWithoutUserIdShouldIgnoreThem()
        {
            // Arrange
            var policies = new List<InquiryTemplateAcceptPolicyDto>
        {
            new() { Id = 1, PolicyScope = InquiryAcceptPolicyScopes.GroupManager, UserId = null },
            new() { Id = 2, PolicyScope = InquiryAcceptPolicyScopes.User, UserId = 5 }
        };

            _userLoaderMock
                .Setup(x => x.GetUsersByIdsAsync(It.Is<int[]>(ids => ids.SequenceEqual(new[] { 5 })), It.IsAny<CancellationToken>()))
                .ReturnsAsync([
                    new() { FirstName = "Test", LastName = "User", Email = "Test.User@modsen-software.com", Id = 5 }
                ]);

            // Act
            var action = async () => await _validator.Validate(policies, CancellationToken.None);

            // Assert
            await action.Should().NotThrowAsync();
            _userLoaderMock.VerifyAll();
        }
    }
}
