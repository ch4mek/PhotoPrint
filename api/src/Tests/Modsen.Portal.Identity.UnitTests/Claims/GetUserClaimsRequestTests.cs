using Modsen.Portal.Identity.Requests.Claims.Core;
using Modsen.Portal.Identity.Requests.Claims.Core.CoreDto;
using Modsen.Portal.Identity.Requests.Claims.Requests;
using Modsen.Portal.Identity.Requests.Dto;
using Moq;

namespace Modsen.Portal.Identity.UnitTests.Claims;

public class GetUserClaimsRequestTests
{
    private readonly Mock<IUserGroupClaimsLoader> _userGroupClaimsLoaderMock = new();
    private readonly GetUserClaimsRequest _getUserClaimsRequest;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public GetUserClaimsRequestTests()
    {
        _getUserClaimsRequest = new GetUserClaimsRequest(
            _userGroupClaimsLoaderMock.Object);
    }

    [Fact]
    public async Task HandleWhenRequestIsValidShouldReturnClaims()
    {
        // Arrange
        var user = new UserDto
        {
            FirstName = "firstName",
            Email = "email",
            RoleNames = ["Employee"]
        };

        var claims = new List<GroupClaimsCoreDto>
        {
            new()
            {
                Name = "Nmae",
                Claims = [],
            }
        };

        _userGroupClaimsLoaderMock
            .Setup(mock => mock.Load(It.IsAny<int>(), _cancellationToken))
            .ReturnsAsync(claims);

        // Act
        var result = await _getUserClaimsRequest.Handle(new GetUserClaimsRequest.Query(1), _cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(claims);
    }
}
