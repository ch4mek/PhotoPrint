using MockQueryable.Moq;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Identity.Requests.Claims.Core;
using Modsen.Portal.Identity.Requests.Claims.Requests;
using Moq;

namespace Modsen.Portal.Identity.UnitTests.Claims;

public class UpdateUserClaimsRequestTests
{
    private readonly Mock<IIdentityDbContext> _identityDbContextMock = new();
    private readonly Mock<IAuthenticationTokenContext> _authenticationTokenContextMock = new();
    private readonly Mock<IUserGroupClaimsLoader> _userGroupClaimsLoaderMock = new();
    private readonly UpdateUserClaimsRequest _updateUserClaimsRequest;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public UpdateUserClaimsRequestTests()
    {
        _updateUserClaimsRequest = new(
            _identityDbContextMock.Object,
            _authenticationTokenContextMock.Object,
            _userGroupClaimsLoaderMock.Object);
    }

    [Fact]
    public async Task HandleWhenUserIsNotExistShouldThrowNotFoundException()
    {
        // Arrange
        SetupUsers([]);

        // Act
        var result = () => _updateUserClaimsRequest.Handle(new(1, []), _cancellationToken);

        // Assert
        await result.Should().ThrowAsync<EntityNotFoundException>();
    }

    [Fact]
    public async Task HandleWhenClaimIsNotExistShouldThrowInvalidDataAppException()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            FirstName = "test",
            LastName = "test",
            UserRoles = [
                new() {
                    Role = new() {
                        Name = PortalRoleNames.Employee,
                        PublicName = "Test"
                    }
                }
            ],
        };

        SetupUsers([user]);
        SetupClaimGroups([]);

        // Act
        var result = () => _updateUserClaimsRequest.Handle(new(1, ["type"]), _cancellationToken);

        // Assert
        await result.Should().ThrowAsync<InvalidDataAppException>();
    }

    [Fact]
    public async Task HandleWhenRequestIsValidShouldNotThrowException()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            FirstName = "test",
            LastName = "test",
            UserRoles = [
                new() {
                    Role = new() {
                        Name = PortalRoleNames.Employee,
                        PublicName = "Test"
                    }
                }
            ],
        };

        SetupUsers([user]);
        SetupClaimGroups([]);
        SetupUserClaims([]);

        // Act
        var result = () => _updateUserClaimsRequest.Handle(new(1, []), _cancellationToken);

        // Assert
        await result.Should().NotThrowAsync();
    }

    private void SetupUsers(List<User> users)
    {
        _identityDbContextMock
            .Setup(mock => mock.Users)
            .Returns(users.BuildMockDbSet().Object);
    }

    private void SetupClaimGroups(List<ClaimGroup> claimGroups)
    {
        _identityDbContextMock
            .Setup(mock => mock.ClaimGroups)
            .Returns(claimGroups.BuildMockDbSet().Object);
    }

    private void SetupUserClaims(List<UserClaim> userClaims)
    {
        _identityDbContextMock
            .Setup(mock => mock.UserClaims)
            .Returns(userClaims.BuildMockDbSet().Object);
    }
}
