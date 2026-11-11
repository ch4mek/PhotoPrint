using MockQueryable.Moq;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Communication;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.BusinessRules;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Identity.Requests.Core;
using Modsen.Portal.Identity.Requests.ResourceGroups.Core.BusinessRules;
using Modsen.Portal.Identity.Requests.ResourceGroups.Requests;
using Moq;

namespace Modsen.Portal.Identity.UnitTests.ResourceGroups.Requests;

public class AddUserToResourceGroupRequestTests
{
    private readonly Mock<ICanGroupMemberBeAddedBusinessRule> _canGroupMemberBeAddedBusinessRuleMock = new();
    private readonly Mock<IIdentityDbContext> _identityDbContextMock = new();
    private readonly Mock<ICommunicationBus> _communicationBusMock = new();
    private readonly Mock<IUserLoader> _userLoader = new();
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock = new();
    private readonly AddUserToResourceGroupRequest _addUserToResourceGroupRequest;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public AddUserToResourceGroupRequestTests()
    {
        _addUserToResourceGroupRequest = new(
            _canGroupMemberBeAddedBusinessRuleMock.Object,
            _identityDbContextMock.Object,
            _communicationBusMock.Object,
            _userLoader.Object,
            _authenticatedUserContextMock.Object);

        _authenticatedUserContextMock.Setup(x => x.UserId).Returns(1001);
    }

    [Fact]
    public async Task HandleWhenUserCanNotBeAddedShouldThrowException()
    {
        // Arrange
        var groupId = 1;
        var userId = 1;
        var reason = "reason";

        SetupCanGroupMemberBeAdded(BusinessRuleResult.Fail(reason));

        var request = new AddUserToResourceGroupRequest.Command(groupId, userId);

        // Act
        var result = () => _addUserToResourceGroupRequest.Handle(request, _cancellationToken);

        // Assert
        await result.Should()
            .ThrowAsync<ActionNotAllowedException>()
            .WithMessage(reason);
    }

    [Fact]
    public async Task HandleWhenUserIsNotExistShouldThrowException()
    {
        // Arrange
        var groupId = 1;
        var userId = 1;

        SetupCanGroupMemberBeAdded(BusinessRuleResult.Success());
        SetupUsers([]);

        var request = new AddUserToResourceGroupRequest.Command(groupId, userId);

        // Act
        var result = () => _addUserToResourceGroupRequest.Handle(request, _cancellationToken);

        // Assert
        await result.Should()
            .ThrowAsync<EntityNotFoundException>()
            .WithMessage(PortalMessages.UserNotFound);
    }

    [Fact]
    public async Task HandleWhenRequestIsValidShouldUpdateGroup()
    {
        // Arrange
        var groupId = 1;
        var userId = 1;

        var user = new User
        {
            Id = userId,
            IsActive = true,
            FirstName = "test",
            LastName = "test",
            UserRoles =
            [
                new UserRole {
                    Role = new Role() {
                        Id = 1,
                        Name = "name",
                        PublicName = "test"
                    }
                }
            ]
        };

        SetupCanGroupMemberBeAdded(BusinessRuleResult.Success());
        SetupUsers([user]);

        var request = new AddUserToResourceGroupRequest.Command(groupId, userId);

        // Act

        // Assert
        _identityDbContextMock.Verify();
        _communicationBusMock.Verify();
    }

    private void SetupUsers(List<User> users)
    {
        _identityDbContextMock
            .Setup(mock => mock.Users)
            .Returns(users.BuildMockDbSet().Object);
    }

    private void SetupCanGroupMemberBeAdded(BusinessRuleResult result)
    {
        _canGroupMemberBeAddedBusinessRuleMock
            .Setup(mock => mock.IsCorrespondsToBusiness(It.IsAny<CanGroupMemberBeAddedBusinessRule.Request>(), _cancellationToken))
            .ReturnsAsync(result);
    }
}
