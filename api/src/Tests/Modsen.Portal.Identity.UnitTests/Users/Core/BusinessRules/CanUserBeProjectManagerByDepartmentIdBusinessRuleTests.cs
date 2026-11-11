using MockQueryable.Moq;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Identity.Requests.Users.Core.BusinessRules;
using Moq;

namespace Modsen.Portal.Identity.UnitTests.Users.Core.BusinessRules;

public class CanUserBeProjectManagerByDepartmentIdBusinessRuleTests
{
    private readonly Mock<IIdentityDbContext> _identityDbContextMock = new();
    private readonly CanUserBeProjectManagerByDepartmentIdBusinessRule _canUserBeProjectManagerByDepartmentIdBusinessRul;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public CanUserBeProjectManagerByDepartmentIdBusinessRuleTests()
    {
        _canUserBeProjectManagerByDepartmentIdBusinessRul = new CanUserBeProjectManagerByDepartmentIdBusinessRule(
            _identityDbContextMock.Object);
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenUserHaveDepartmentWithClaimShouldReturnTrue()
    {
        // Arrange
        const string departmentId = "Net";

        var user = new User
        {
            Id = 1,
            FirstName = "test",
            LastName = "test",
            DepartmentId = departmentId
        };

        var claim = new DepartmentClaim
        {
            Id = 1,
            DepartmentId = user.DepartmentId,
            ClaimType = IdentityClaimTypeNames.AvailableProjectManagerRole,
            ClaimValue = "1"
        };

        _identityDbContextMock
            .Setup(x => x.Users)
            .Returns(new List<User> { user }.BuildMockDbSet().Object);

        _identityDbContextMock
            .Setup(x => x.DepartmentClaims)
            .Returns(new List<DepartmentClaim> { claim }.BuildMockDbSet().Object);

        // Act
        var actualResult = await _canUserBeProjectManagerByDepartmentIdBusinessRul
            .IsCorrespondsToBusiness(user.Id, _cancellationToken);

        // Assert
        actualResult.Should().BeTrue();
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenUserNotFoundShouldReturnFalse()
    {
        // Arrange
        _identityDbContextMock
            .Setup(x => x.Users)
            .Returns(new List<User>().BuildMockDbSet().Object);

        // Act
        var actualResult = await _canUserBeProjectManagerByDepartmentIdBusinessRul
            .IsCorrespondsToBusiness(1, _cancellationToken);

        // Assert
        actualResult.Should().BeFalse();
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenUserDoesntHaveDepartmentShouldReturnFalse()
    {
        // Arrange

        var user = new User
        {
            Id = 1,
            FirstName = "test",
            LastName = "test"
        };

        _identityDbContextMock
            .Setup(x => x.Users)
            .Returns(new List<User> { user }.BuildMockDbSet().Object);

        // Act
        var actualResult = await _canUserBeProjectManagerByDepartmentIdBusinessRul
            .IsCorrespondsToBusiness(user.Id, _cancellationToken);

        // Assert
        actualResult.Should().BeFalse();
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenUserHaveDepartmentWithoutClaimShouldReturnFalse()
    {
        // Arrange
        const string departmentId = "Net";

        var user = new User
        {
            Id = 1,
            DepartmentId = departmentId,
            FirstName = "test",
            LastName = "test"
        };

        var claim = new DepartmentClaim
        {
            Id = 1,
            DepartmentId = user.DepartmentId,
            ClaimValue = "1",
            ClaimType = "TEST"
        };

        _identityDbContextMock
            .Setup(x => x.Users)
            .Returns(new List<User> { user }.BuildMockDbSet().Object);

        _identityDbContextMock
            .Setup(x => x.DepartmentClaims)
            .Returns(new List<DepartmentClaim> { claim }.BuildMockDbSet().Object);

        // Act
        var actualResult = await _canUserBeProjectManagerByDepartmentIdBusinessRul
            .IsCorrespondsToBusiness(user.Id, _cancellationToken);

        // Assert
        actualResult.Should().BeFalse();
    }
}
