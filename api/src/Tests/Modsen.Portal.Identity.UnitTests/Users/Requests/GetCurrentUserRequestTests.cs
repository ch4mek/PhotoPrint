using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Identity.Requests.Core;
using Modsen.Portal.Identity.Requests.Dto;
using Modsen.Portal.Identity.Requests.Users.Requests;
using Moq;

namespace Modsen.Portal.Identity.UnitTests.Users.Requests;

public class GetCurrentUserRequestTests
{
    private readonly Mock<IUserLoader> _userLoaderMock = new();
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock = new();

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    private readonly GetCurrentUserRequest _handler;

    public GetCurrentUserRequestTests()
    {
        _handler = new GetCurrentUserRequest(
            _authenticatedUserContextMock.Object,
            _userLoaderMock.Object);
    }

    [Fact]
    public async Task GetCurrentUserReturnSuccessResult()
    {
        //Arrange
        var userId = 1;

        var user = new UserDto()
        {
            Id = userId,
            FirstName = "FirstName",
            LastName = "LastName",
            FullName = "LastName FirstName",
            Email = "example1@example.com",
            DepartmentId = "Js",
            ResourceGroupId = null,
            RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId,
        };

        var userDto = new UserDto
        {
            Id = userId,
            FirstName = "FirstName",
            LastName = "LastName",
            FullName = "LastName FirstName",
            Email = "example1@example.com",
            DepartmentId = "Js",
            RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId,
        };

        _authenticatedUserContextMock.Setup(x => x.GetRequiredUserId())
            .Returns(userId);
        _userLoaderMock.Setup(x => x.LoadAsync(user.Id, _cancellationToken))
            .ReturnsAsync(user);

        //Act
        var result = await _handler.Handle(
            new GetCurrentUserRequest.Query(),
            _cancellationToken);

        //Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(userDto);
    }
}
