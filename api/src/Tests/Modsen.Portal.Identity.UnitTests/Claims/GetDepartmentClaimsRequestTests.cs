using MockQueryable.Moq;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Identity.Requests.Claims.Requests;
using Moq;

namespace Modsen.Portal.Identity.UnitTests.Claims;


public class GetDepartmentClaimsRequestTests
{
    private readonly Mock<IIdentityDbContext> _identityDbContextMock = new();
    private readonly GetDepartmentClaimsRequest _getDepartmentClaimsRequest;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public GetDepartmentClaimsRequestTests()
    {
        _getDepartmentClaimsRequest = new(_identityDbContextMock.Object);
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

        var departmentId = "DepartmentId";

        var departmentClaims = new List<DepartmentClaim>
        {
            new()
            {
                DepartmentId = departmentId,
                Claim = claim,
                ClaimType = "TEST",
                ClaimValue = "1"
            },
        };

        _identityDbContextMock
            .Setup(mock => mock.DepartmentClaims)
            .Returns(departmentClaims.BuildMockDbSet().Object);

        // Assert
        var result = await _getDepartmentClaimsRequest.Handle(new GetDepartmentClaimsRequest.Query(), _cancellationToken);

        // Act
        result.First().Claims.Count.Should().Be(1);
        result.First().Claims.First().AllowedValueType.Should().Be(AllowedValueType.String);
        result.First().Claims.First().ClaimType.Should().Be("TEST");
        result.First().Claims.First().ClaimValue.Should().Be("1");
        result.First().DepartmentId.Should().Be(departmentId);
    }
}
