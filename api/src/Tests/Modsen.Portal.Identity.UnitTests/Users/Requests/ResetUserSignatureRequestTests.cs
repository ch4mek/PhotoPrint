using MockQueryable.Moq;
using Modsen.Portal.Core.Api.Context;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Identity.Requests.Dto;
using Modsen.Portal.Identity.Requests.Users.Requests;
using Moq;

namespace Modsen.Portal.Identity.UnitTests.Users.Requests;

public class ResetUserSignatureRequestTests
{
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserMock = new Mock<AuthenticatedUserContext>
    { CallBase = true }.As<IAuthenticatedUserContext>();
    private readonly Mock<IIdentityDbContext> _identityDbContextMock = new();

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    private readonly ResetUserSignatureRequest _resetUserSignatureRequest;

    public ResetUserSignatureRequestTests()
    {
        _resetUserSignatureRequest = new ResetUserSignatureRequest(
            _authenticatedUserMock.Object,
            _identityDbContextMock.Object);
    }

    [Fact]
    public async Task ResetUserSignatureWhenCurrentUserWasNotFoundThrowEntityNotFoundException()
    {
        // Arrange
        var userId = 99;

        var users = new List<User>
        {
            new()
            {
                Id = 1,
                FirstName = "test",
                LastName = "test",
                SignatureFullName = "Test",
                SignaturePosition = "Employee",
                SignaturePhone = "1234567",
            }
        };

        var command = new ResetUserSignatureRequest.Command();

        _authenticatedUserMock
            .Setup(x => x.UserId)
            .Returns(userId);
        _authenticatedUserMock
            .Setup(x => x.GetRequiredUserId())
            .Returns(userId);

        _identityDbContextMock
            .Setup(x => x.Users)
            .Returns(users.BuildMockDbSet().Object);

        // Act
        var action = () => _resetUserSignatureRequest.Handle(command, _cancellationToken);

        // Assert
        await action.Should().ThrowAsync<EntityNotFoundException>($"User with {userId} ID was not found");
    }

    [Fact]
    public async Task ResetUserSignatureWhenCurrentUserWasFoundResetAndReturnUserSignature()
    {
        // Arrange
        var users = new List<User>()
        {
            new()
            {
                Id = 99,
                FirstName = "firstName",
                LastName = "lastName",
                Email = "test@modsen-software.com",
            }
        };

        var command = new ResetUserSignatureRequest.Command();

        var updatedUser = new User
        {
            Id = 99,
            FirstName = "firstName",
            LastName = "lastName",
            Email = "test@modsen-software.com",
            
            SignatureFullName = null,
            SignaturePhone = null,
            SignaturePosition = null,            
        };

        var updatedUsers = new List<User>() { updatedUser };

        var userSignatureDto = new UserSignatureDto
        {
            Email = updatedUser.Email,
            FullName = $"{updatedUser.LastName} {updatedUser.FirstName}",
            Phone = updatedUser.SignaturePhone,
            Position = updatedUser.SignaturePosition
        };

        _authenticatedUserMock
            .Setup(x => x.UserId)
            .Returns(users[0].Id);
        _authenticatedUserMock
            .Setup(x => x.GetRequiredUserId())
            .Returns(users[0].Id);

        _identityDbContextMock
            .Setup(x => x.Users)
            .Returns(users.BuildMockDbSet().Object);
        _identityDbContextMock
            .Setup(x => x.Users)
            .Returns(updatedUsers.BuildMockDbSet().Object);

        // Act
        var result = await _resetUserSignatureRequest.Handle(command, _cancellationToken);

        // Assert
        _identityDbContextMock.Verify(
            x => x.Users,
            Times.Exactly(2));

        result.Should().BeEquivalentTo(userSignatureDto);
    }
}
