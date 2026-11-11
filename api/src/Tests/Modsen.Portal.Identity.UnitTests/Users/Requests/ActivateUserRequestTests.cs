using MockQueryable.Moq;
using Modsen.Portal.Core.Communication;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.BusinessRules;
using Modsen.Portal.Identity.Contract.Events;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Identity.Requests.Core;
using Modsen.Portal.Identity.Requests.Dto;
using Modsen.Portal.Identity.Requests.Users.Core.BusinessRules;
using Modsen.Portal.Identity.Requests.Users.Requests;
using Moq;

namespace Modsen.Portal.Identity.UnitTests.Users.Requests;

public class ActivateUserRequestTests
{
    private readonly Mock<IIdentityDbContext> _identityDbContextMock = new();
    private readonly Mock<IUserLoader> _userLoaderMock = new();
    private readonly Mock<ICanIdentityUserBeActivatedBusinessRule> _canUserBeActivatedBusinessRuleMock = new();
    private readonly Mock<ICommunicationBus> _outboxContractMock = new();
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock = new();

    private readonly ActivateUserRequest _handler;

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public ActivateUserRequestTests()
    {
        _handler = new ActivateUserRequest(
            _canUserBeActivatedBusinessRuleMock.Object,
            _identityDbContextMock.Object,
            _userLoaderMock.Object,
            _outboxContractMock.Object,
            _authenticatedUserContextMock.Object);
    }

    [Fact]
    public async Task HandleWhenUserDoesNotExistShouldThrowEntityNotFoundException()
    {
        // Arrange
        const int userId = 1;

        _userLoaderMock
            .Setup(x => x.LoadAsync(1, _cancellationToken))
            .ThrowsAsync(new EntityNotFoundException($"User with id {userId} does not exist"));

        // Act
        var actionUnderTest = () => _handler.Handle(
            new ActivateUserRequest.Command(userId),
            _cancellationToken);

        // Assert
        await actionUnderTest.Should().ThrowAsync<EntityNotFoundException>();

        _outboxContractMock.Verify(x => x.PublishAsync(
                It.IsAny<UserActivatedEvent>(),
                _cancellationToken),
            Times.Never);
    }

    [Fact]
    public async Task HandleWhenUserCanNotBeActivatedShouldThrowActionNotAllowedException()
    {
        // Arrange
        var userToActivate = new User
        {
            Id = 1,
            FirstName = "FirstName",
            LastName = "test",
            Email = "example@example.com",
            IsActive = false,
        };
        var activatedUser = new UserDto
        {
            Id = 1,
            FirstName = "FirstName",
            Email = "example@example.com",
            IsActive = false,
        };

        _identityDbContextMock
            .Setup(x => x.Users)
            .Returns(new List<User> { userToActivate }.BuildMockDbSet().Object);

        _userLoaderMock
            .Setup(x => x.LoadAsync(userToActivate.Id, _cancellationToken))
            .ReturnsAsync(activatedUser);

        _canUserBeActivatedBusinessRuleMock.Setup(x =>
                x.IsCorrespondsToBusiness(activatedUser, _cancellationToken))
            .ReturnsAsync(BusinessRuleResult.Fail("Reason to fail"));

        // Act
        var actionUnderTest = () => _handler.Handle(
            new ActivateUserRequest.Command(userToActivate.Id),
            _cancellationToken);

        // Assert
        await actionUnderTest.Should().ThrowAsync<ActionNotAllowedException>();

        _outboxContractMock.Verify(x => x.PublishAsync(
                It.IsAny<UserActivatedEvent>(),
                _cancellationToken),
            Times.Never);
    }

    [Fact]
    public async Task HandleWhenCurrentUserIsSysAdminAndUserIsNotEmployeeShouldThrowActionNotAllowedException()
    {
        // Arrange
        var userToActivate = new User
        {
            Id = 1,
            FirstName = "FirstName",
            LastName = "test",
            Email = "example@example.com",
            IsActive = false,
        };

        var activatedUser = new UserDto
        {
            Id = 1,
            FirstName = "FirstName",
            Email = "example@example.com",
            IsActive = false,
            RoleNames = [PortalRoleNames.GroupManager]
        };

        _identityDbContextMock
            .Setup(x => x.Users)
            .Returns(new List<User> { userToActivate }.BuildMockDbSet().Object);

        _userLoaderMock
            .Setup(x => x.LoadAsync(userToActivate.Id, _cancellationToken))
            .ReturnsAsync(activatedUser);

        _canUserBeActivatedBusinessRuleMock.Setup(x =>
                x.IsCorrespondsToBusiness(activatedUser, _cancellationToken))
            .ReturnsAsync(BusinessRuleResult.Fail(""));

        // Act
        var actionUnderTest = () => _handler.Handle(
            new ActivateUserRequest.Command(userToActivate.Id),
            _cancellationToken);

        // Assert
        await actionUnderTest.Should().ThrowAsync<ActionNotAllowedException>();

        _outboxContractMock.Verify(x => x.PublishAsync(
                It.IsAny<UserActivatedEvent>(),
                _cancellationToken),
            Times.Never);
    }
}
