using MockQueryable.Moq;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Identity.Requests.Claims.Requests;
using Moq;

namespace Modsen.Portal.Identity.UnitTests.Claims;

public class GetRoleClaimsRequestTest
{
    private readonly Mock<IIdentityDbContext> _identityDbContextMock = new();
    private readonly GetRoleClaimsRequest _getRoleClaimsRequest;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public GetRoleClaimsRequestTest()
    {
        _getRoleClaimsRequest = new GetRoleClaimsRequest(_identityDbContextMock.Object);
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

        var roleName = "Employee";

        var role = new Role()
        {
            Id = 1,
            Name = roleName,
            PublicName = "Test"
        };

        var roleClaims = new List<ClaimEntity>
        {
            new()
            {
                ClaimType = "Type",
                AllowedValueType = AllowedValueType.String
            },
        };

        _identityDbContextMock
            .Setup(mock => mock.Claims)
            .Returns(roleClaims.BuildMockDbSet().Object);

        var userRoles = new List<UserRole>
        {
            new()
            {
                User = new() {FirstName = "test",
                    LastName = "test"},
                Role = role
            }
        };

        _identityDbContextMock
            .Setup(mock => mock.UserRoles)
            .Returns(userRoles.BuildMockDbSet().Object);

        // Assert
        var result = await _getRoleClaimsRequest.Handle(new GetRoleClaimsRequest.Query(), _cancellationToken);

        // Act
        result.First().Claims.Count.Should().Be(1);
        result.First().Claims.First().AllowedValueType.Should().Be(AllowedValueType.String);
        result.First().Claims.First().ClaimType.Should().Be("Type");
        result.First().Claims.First().ClaimValue.Should().Be(string.Empty);
        result.First().RoleName.Should().Be(roleName);
    }
}
