using MockQueryable.Moq;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Identity.Contract.Models;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Identity.Requests.Users.Requests;
using Moq;

namespace Modsen.Portal.Identity.UnitTests.Users.Requests;

public class GetProjectManagersLookupRequestTests
{
    private readonly Mock<IIdentityDbContext> _identityDbContextMock = new();
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly GetProjectManagersLookupRequest _handler;

    public GetProjectManagersLookupRequestTests()
    {
        _handler = new GetProjectManagersLookupRequest(_identityDbContextMock.Object);
    }

    [Fact]
    public async Task HandleShouldReturnProjectManagerLookupDtos()
    {
        // Arrange
        var claim = new DepartmentClaim() { ClaimType = IdentityClaimTypeNames.AvailableProjectManagerRole, ClaimValue = "1", DepartmentId = "Net" };
        var role = new Role { Name = PortalRoleNames.Employee, PublicName = "TEST"};
        var users = new List<User>
        {
            new()
            {
                Id = 111,
                FirstName = "FirstName",
                LastName = "LastName",
                IsActive = true,
                DepartmentId ="Net",
                UserRoles = [ new() { Role = role } ],
                Department = new Department
                {
                    Id ="Net",
                    Claims = [claim],
                    ShortName = "net",
                    FullName = "net"
                }
            }
        };
        claim.Department = users.First().Department!;

        var output = new List<ProjectManagerLookupDto>()
        {
            new()
            {
                UserId = users.First().Id,
                FirstName = users.First().FirstName,
                LastName = users.First().LastName,
                Role =  PortalRoleNames.Employee
            }
        };

        _identityDbContextMock.Setup(x => x.Users)
            .Returns(users.BuildMockDbSet().Object);

        // Act
        var result = await _handler.Handle(
            new GetProjectManagersLookupRequest.Query(),
            _cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(output);
        _identityDbContextMock.Verify(x => x.Users, Times.Once);
    }
}
