using MockQueryable.Moq;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Identity.Requests.Claims.Requests;
using Moq;

namespace Modsen.Portal.Identity.UnitTests.Claims;

public class GetGroupClaimsRequestTests
{
    private readonly Mock<IIdentityDbContext> _identityDbContextMock = new();
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly GetGroupClaimsRequest _getGroupClaimsRequest;

    public GetGroupClaimsRequestTests()
    {
        _getGroupClaimsRequest = new(_identityDbContextMock.Object);
    }

    [Fact]
    public async Task HandleWhenRequestIsValidShouldReturnClaims()
    {
        // Arrange
        var claim = new ClaimEntity()
        {
            ClaimType = "Type",
            AllowedValueType = AllowedValueType.String,
        };

        var groupName = "groupName";

        var groupClaims = new List<ClaimGroup>
        {
            new()
            {
                Id = "T1",
                Name = groupName,
                Claims = [claim]
            },
        };

        _identityDbContextMock
            .Setup(mock => mock.ClaimGroups)
            .Returns(groupClaims.BuildMockDbSet().Object);

        // Assert
        var result = await _getGroupClaimsRequest.Handle(new GetGroupClaimsRequest.Query(), _cancellationToken);

        // Act
        result.First().Claims.Count.Should().Be(1);
        result.First().Claims.First().AllowedValueType.Should().Be(AllowedValueType.String);
        result.First().Claims.First().ClaimType.Should().Be("Type");
        result.First().Claims.First().ClaimValue.Should().Be(string.Empty);
        result.First().Name.Should().Be(groupName);
    }
}
