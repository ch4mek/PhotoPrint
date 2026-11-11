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

public class UpdateUserSignatureRequestTests
{
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserMock = new Mock<AuthenticatedUserContext>
        { CallBase = true }.As<IAuthenticatedUserContext>();
    private readonly Mock<IIdentityDbContext> _identityDbContextMock = new();

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    private readonly UpdateUserSignatureRequest _updateUserSignatureRequest;

    public UpdateUserSignatureRequestTests()
    {
        _updateUserSignatureRequest = new UpdateUserSignatureRequest(
            _authenticatedUserMock.Object,
            _identityDbContextMock.Object);
    }

    [Fact]
    public async Task UpdateUserSignatureWhenCurrentUserWasNotFoundThrowEntityNotFoundException()
    {
        // Arrange
        var userId = 99;

        var command = new UpdateUserSignatureRequest.Command(
            "test name",
            "Company CEO",
            "375292005894");

        _authenticatedUserMock
            .Setup(x => x.UserId)
            .Returns(userId);
        _authenticatedUserMock
            .Setup(x => x.GetRequiredUserId())
            .Returns(userId);

        _identityDbContextMock
            .Setup(x => x.Users)
            .Returns((new List<User>()).BuildMockDbSet().Object);

        // Act
        var action = () => _updateUserSignatureRequest.Handle(command, _cancellationToken);

        // Assert
        await action.Should().ThrowAsync<EntityNotFoundException>($"User with {userId} ID was not found");
    }

    [Fact]
    public async Task UpdateUserSignatureWhenCurrentUserWasFoundUpdateAndReturnUserSignature()
    {
        // Arrange
        var user = new User()
        {
            Id = 99,
            FirstName = "firstName",
            LastName = "test",
            Email = "test@modsen-software.com"
        };

        var command = new UpdateUserSignatureRequest.Command(
            "Test fullname",
            "Modsen CEO",
            "375291245322");

        var updatedUser = new User()
        {
            Id = 99,
            FirstName = "firstName",
            LastName = "test",
            Email = "test@modsen-software.com",
            SignatureFullName = "Test fullname",
            SignaturePhone = "375291245322",
            SignaturePosition = "Modsen CEO"
        };

        var userSignatureDto = new UserSignatureDto
        {
            Email = updatedUser.Email,
            FullName = updatedUser.SignatureFullName,
            Phone = updatedUser.SignaturePhone,
            Position = updatedUser.SignaturePosition
        };

        _authenticatedUserMock
            .Setup(x => x.UserId)
            .Returns(user.Id);
        _authenticatedUserMock
            .Setup(x => x.GetRequiredUserId())
            .Returns(user.Id);

        _identityDbContextMock
            .Setup(x => x.Users)
            .Returns(new List<User> { user }.BuildMockDbSet().Object);
        _identityDbContextMock
            .Setup(x => x.Users)
            .Returns(new List<User> { updatedUser }.BuildMockDbSet().Object);

        // Act 
        var result = await _updateUserSignatureRequest.Handle(command, _cancellationToken);

        // Assert
        _identityDbContextMock.Verify(
            x => x.Users,
            Times.Exactly(2));

        result.Should().BeEquivalentTo(userSignatureDto);
    }
}
