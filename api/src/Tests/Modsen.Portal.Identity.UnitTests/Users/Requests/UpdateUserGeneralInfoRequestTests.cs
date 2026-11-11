using MockQueryable.Moq;
using Modsen.Portal.Core.Communication;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.BusinessRules;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.DataAccess.Npgsql.Cache;
using Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Identity.Requests.Core;
using Modsen.Portal.Identity.Requests.Dto;
using Modsen.Portal.Identity.Requests.Users.Core;
using Modsen.Portal.Identity.Requests.Users.Core.BusinessRules;
using Modsen.Portal.Identity.Requests.Users.Core.Validators;
using Modsen.Portal.Identity.Requests.Users.Requests;
using Moq;

namespace Modsen.Portal.Identity.UnitTests.Users.Requests
{
    public class UpdateUserGeneralInfoRequestTests
    {
        private readonly Mock<ICommunicationBus> _communicationBusMock;
        private readonly Mock<IAuthenticationTokenContext> _authenticationTokenContextMock;
        private readonly Mock<ICanUserBeUpdatedBusinessRule> _canUserBeUpdatedBusinessRuleMock;
        private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock;
        private readonly Mock<IIdentityDbContext> _identityDbContextMock;
        private readonly Mock<IUserLoader> _userLoaderMock;
        private readonly Mock<IUpdateUserValidator> _updateUserValidatorMock;
        private readonly Mock<IRolesCache> _rolesCacheMock;
        private readonly Mock<IUserDepartmentHistoryComposer> _userDepartmentHistoryComposerMock = new();

        private readonly UpdateUserGeneralInfoRequest _handler;

        public UpdateUserGeneralInfoRequestTests()
        {
            _communicationBusMock = new Mock<ICommunicationBus>();
            _authenticationTokenContextMock = new Mock<IAuthenticationTokenContext>();
            _canUserBeUpdatedBusinessRuleMock = new Mock<ICanUserBeUpdatedBusinessRule>();
            _authenticatedUserContextMock = new Mock<IAuthenticatedUserContext>();
            _identityDbContextMock = new Mock<IIdentityDbContext>();
            _userLoaderMock = new Mock<IUserLoader>();
            _updateUserValidatorMock = new Mock<IUpdateUserValidator>();
            _rolesCacheMock = new Mock<IRolesCache>();

            _handler = new UpdateUserGeneralInfoRequest(
                _communicationBusMock.Object,
                _authenticationTokenContextMock.Object,
                _canUserBeUpdatedBusinessRuleMock.Object,
                _authenticatedUserContextMock.Object,
                _identityDbContextMock.Object,
                _userLoaderMock.Object,
                _updateUserValidatorMock.Object,
                _rolesCacheMock.Object,
                _userDepartmentHistoryComposerMock.Object);
        }

        [Fact]
        public async Task HandleShouldThrowEntityNotFoundExceptionWhenUserDoesNotExist()
        {
            // Arrange
            var command = new UpdateUserGeneralInfoRequest.InternalCommand(1, null, null, null, null, null, null, null, null, null);

            _userLoaderMock
                .Setup(loader => loader.LoadAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync((UserDto?)null);

            // Act
            var result = () => _handler.Handle(command, CancellationToken.None);

            // Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(result);
        }

        [Fact]
        public async Task HandleShouldThrowActionNotAllowedExceptionWhenBusinessRuleValidationFails()
        {
            // Arrange
            var command = new UpdateUserGeneralInfoRequest.InternalCommand(1, "FirstName", "LastName", null, null, null, null, null, null, null);
            var existingUser = new UserDto
            {
                Id = 1,
                FirstName = "test",
                Email = "test@example.com"
            };

            _userLoaderMock
                .Setup(loader => loader.LoadAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingUser);
            _canUserBeUpdatedBusinessRuleMock
                .Setup(rule => rule.IsCorrespondsToBusiness(It.IsAny<CanUserBeUpdatedBusinessRule.Request>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(BusinessRuleResult.Fail("Action not allowed"));

            // Act
            var result = () => _handler.Handle(command, CancellationToken.None);

            // Assert
            await Assert.ThrowsAsync<ActionNotAllowedException>(result);
        }

        [Fact]
        public async Task HandleShouldUpdateUserWhenRequestIsValid()
        {
            // Arrange
            var command = new UpdateUserGeneralInfoRequest
                .InternalCommand(1, "NewFirstName", "NewLastName", null, null, null, null, null, null, "Employee");

            var updatedUser = new UserDto
            {
                Id = 1,
                Email = "test@example.com",
                FirstName = "NewFirstName",
                LastName = "NewLastName",
                IsActive = true,
            };

            var roleCacheRecords = new List<RoleCacheRecord>
            {
                new()
                {
                    Id = 1,
                    Name = PortalRoleNames.Employee,
                },
                new()
                {
                    Id = 2,
                    Name = PortalRoleNames.Ceo,
                }
            };

            _canUserBeUpdatedBusinessRuleMock
                .Setup(rule => rule.IsCorrespondsToBusiness(It.IsAny<CanUserBeUpdatedBusinessRule.Request>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(BusinessRuleResult.Success());
            _identityDbContextMock
                .Setup(db => db.Users)
                .Returns(new List<User>
                {
                    new()
                    {
                        Id = updatedUser.Id,
                        Email = updatedUser.Email,
                        FirstName = "OldFirstName",
                        LastName= "OldFirstName",
                        IsActive = true,
                        UserRoles = [new() { Role = new Role { PublicName = PortalRoleNames.Employee } }]
                    }
                }
                .BuildMockDbSet().Object);
            _userLoaderMock
                .Setup(loader => loader.LoadAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(updatedUser);
            _rolesCacheMock
                .Setup(cache => cache.GetAllRoles(It.IsAny<CancellationToken>()))
                .ReturnsAsync(roleCacheRecords);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.FirstName.Should().Be("NewFirstName");
            result.LastName.Should().Be("NewLastName");
            _identityDbContextMock.Verify(db => db.SaveChanges(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
