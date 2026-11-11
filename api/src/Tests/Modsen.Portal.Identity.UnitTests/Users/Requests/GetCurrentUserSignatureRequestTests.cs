using Modsen.Portal.Core.Api.Context;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Identity.Requests.Core;
using Modsen.Portal.Identity.Requests.Dto;
using Modsen.Portal.Identity.Requests.Users.Requests;
using Moq;

namespace Modsen.Portal.Identity.UnitTests.Users.Requests;

public class GetCurrentUserSignatureRequestTests
{
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserMock = new Mock<AuthenticatedUserContext>
        { CallBase = true }.As<IAuthenticatedUserContext>();
    private readonly Mock<IUserLoader> _userLoaderMock = new();
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    private readonly GetCurrentUserSignatureRequest _getCurrentUserSignatureRequest;

    public GetCurrentUserSignatureRequestTests()
    {
        _getCurrentUserSignatureRequest = new GetCurrentUserSignatureRequest
        (
            _userLoaderMock.Object,
            _authenticatedUserMock.Object
        );
    }

    [Fact]
    public async Task GetCurrentUserSignatureWhenUserDoesNotExistThrowEntityNotFoundException()
    {
        // Arrange
        var userId = 99;

        _authenticatedUserMock
            .Setup(x => x.UserId)
            .Returns(userId);
        _authenticatedUserMock
            .Setup(x => x.GetRequiredUserId())
            .Returns(userId);

        _userLoaderMock
            .Setup(x => x.LoadAsync(userId, _cancellationToken))
            .Throws(new EntityNotFoundException($"User with {userId} ID was not found"));

        // Act
        var action = () => _getCurrentUserSignatureRequest
            .Handle(new GetCurrentUserSignatureRequest.Query(), _cancellationToken);

        // Assert
        await action.Should().ThrowAsync<EntityNotFoundException>($"User with {userId} ID was not found");
    }

    [Fact]
    public async Task GetCurrentUserSignatureWhenSignatureFullNameIsNullReturnResponseWithUserFirstAndLastName()
    {
        // Arrange
        var currentUser = new UserDto()
        {
            Id = 99,
            FirstName = "Denis",
            LastName = "Leonenko",
            RoleNames = ["Employee"],
            Email = "denis.leonenko@modsen-software.com",
            Signature = new UserSignatureDto
            {
                FullName = null,
                Position = ".Net developer",
                Phone = "375295004781"
            }
        };

        var expectedResult = new UserSignatureDto
        {
            Email = currentUser.Email,
            FullName = $"{currentUser.LastName} {currentUser.FirstName}",
            Phone = currentUser.Signature.Phone,
            Position = currentUser.Signature.Position
        };

        _authenticatedUserMock
            .Setup(x => x.UserId)
            .Returns(currentUser.Id);
        _authenticatedUserMock
            .Setup(x => x.GetRequiredUserId())
            .Returns(currentUser.Id);

        _userLoaderMock
            .Setup(x => x.LoadAsync(currentUser.Id, _cancellationToken))
            .ReturnsAsync(currentUser);

        // Act
        var result = await _getCurrentUserSignatureRequest
            .Handle(new GetCurrentUserSignatureRequest.Query(), _cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task GetCurrentUserSignatureWhenSignaturePositionIsNullReturnResponseWithPositionIsNull()
    {
        // Arrange
        var currentUser = new UserDto()
        {
            Id = 99,
            FirstName = "Denis",
            LastName = "Leonenko",
            RoleNames = ["Employee"],
            Email = "denis.leonenko@modsen-software.com",
            Signature = new UserSignatureDto
            {
                FullName = "Denis Leonenko",
                Position = null,
                Phone = "375295004781"
            }
        };

        var expectedResult = new UserSignatureDto
        {
            Email = currentUser.Email,
            FullName = currentUser.Signature.FullName,
            Phone = currentUser.Signature.Phone,
            Position = null
        };

        _authenticatedUserMock
            .Setup(x => x.UserId)
            .Returns(currentUser.Id);
        _authenticatedUserMock
            .Setup(x => x.GetRequiredUserId())
            .Returns(currentUser.Id);

        _userLoaderMock
            .Setup(x => x.LoadAsync(currentUser.Id, _cancellationToken))
            .ReturnsAsync(currentUser);

        // Act
        var result = await _getCurrentUserSignatureRequest
            .Handle(new GetCurrentUserSignatureRequest.Query(), _cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedResult);
    }
}
