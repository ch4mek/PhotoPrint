using MockQueryable.Moq;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Core.Services.PortalSettings;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.Home.Requests;
using Moq;

namespace Modsen.Portal.TeamBoard.UnitTests.Home;

public class GetProbationPeriodEndRequestTests
{
    private readonly Mock<ITeamBoardDbContext> _teamBoardDbContextMock;
    private readonly Mock<IClockService> _clockServiceMock;
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock;
    private readonly Mock<IPortalSettingsAccessor> _portalSettingsAccessorMock;
    private readonly GetProbationPeriodEndRequest _handler;

    public GetProbationPeriodEndRequestTests()
    {
        _teamBoardDbContextMock = new Mock<ITeamBoardDbContext>();
        _clockServiceMock = new Mock<IClockService>();
        _authenticatedUserContextMock = new Mock<IAuthenticatedUserContext>();
        _portalSettingsAccessorMock = new Mock<IPortalSettingsAccessor>();
        _handler = new GetProbationPeriodEndRequest(
            _teamBoardDbContextMock.Object,
            _clockServiceMock.Object,
            _authenticatedUserContextMock.Object,
            _portalSettingsAccessorMock.Object
        );
    }

    [Fact]
    public async Task HandleWithNoActiveUsersReturnsEmptyList()
    {
        // Arrange
        var query = new GetProbationPeriodEndRequest.Query(null);
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        _clockServiceMock
            .Setup(cs => cs.TodayUtc())
            .Returns(today);
        _teamBoardDbContextMock
            .Setup(db => db.TeamBoardUsers)
            .Returns(new List<TeamBoardUser>().BuildMockDbSet().Object);
        _portalSettingsAccessorMock
            .Setup(x => x.GetStringValueByName(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("14");

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
    }

    [Theory]
    [InlineData(PortalRoleNames.Employee, 0, null)]
    [InlineData(PortalRoleNames.SystemAdministrator, 0, null)]
    [InlineData(PortalRoleNames.GroupManager, 1, null)]
    [InlineData(PortalRoleNames.HeadOfDepartment, 2, null)]
    [InlineData(PortalRoleNames.Cto, 3, null)]
    [InlineData(PortalRoleNames.Ceo, 3, null)]
    [InlineData(PortalRoleNames.Cto, 2, "Net")]
    public async Task HandleWithActiveUsersReturnsAvaliableUserDates(string role, int count, string? queryDepartmentId)
    {
        // Arrange
        var query = new GetProbationPeriodEndRequest.Query(queryDepartmentId);
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var hireDate = today.AddMonths(-3).AddDays(7);
        var departmentId = "Net";
        var groupId = 1;

        var users = new List<TeamBoardUser>
        {
            new() {
                UserId = 1,
                FirstName = "John",
                LastName = "Doe",
                FirstNameRu = "Джон",
                LastNameRu = "Доу",
                HireDate = hireDate,
                Email = "Test",
                IsActive = true,
                DepartmentId = departmentId,
                ResourceGroupId = groupId,
            },
            new() {
                UserId = 1,
                FirstName = "John",
                LastName = "Doe",
                FirstNameRu = "Джон",
                LastNameRu = "Доу",
                HireDate = hireDate,
                Email = "Test",
                IsActive = true,
                DepartmentId = departmentId,
                ResourceGroupId = 999,
            },
            new() {
                UserId = 1,
                FirstName = "John",
                LastName = "Doe",
                FirstNameRu = "Джон",
                LastNameRu = "Доу",
                HireDate = hireDate,
                Email = "Test",
                IsActive = true,
                DepartmentId = "Java",
                ResourceGroupId = 999,
            }
        };

        _clockServiceMock
            .Setup(cs => cs.TodayUtc())
            .Returns(today);
        _teamBoardDbContextMock
            .Setup(db => db.TeamBoardUsers)
            .Returns(users.BuildMockDbSet().Object);

        _authenticatedUserContextMock.Setup(c => c.GetRoles()).Returns([role]);
        _authenticatedUserContextMock.Setup(c => c.ResourceGroupId).Returns(groupId);
        _authenticatedUserContextMock.Setup(c => c.DepartmentId).Returns(departmentId);
        _portalSettingsAccessorMock
            .Setup(x => x.GetStringValueByName(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("14");

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(count);
    }

    [Fact]
    public async Task HandleWithActiveUsersReturnsValidDto()
    {
        // Arrange
        var query = new GetProbationPeriodEndRequest.Query(null);
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var hireDate = today.AddMonths(-3).AddDays(7);

        var users = new List<TeamBoardUser>
        {
            new() {
                UserId = 1,
                FirstName = "John",
                LastName = "Doe",
                FirstNameRu = "Джон",
                LastNameRu = "Доу",
                HireDate = hireDate,
                Email = "Test",
                IsActive = true,
            }
        };

        _clockServiceMock
            .Setup(cs => cs.TodayUtc())
            .Returns(today);
        _teamBoardDbContextMock
            .Setup(db => db.TeamBoardUsers)
            .Returns(users.BuildMockDbSet().Object);

        _authenticatedUserContextMock.Setup(c => c.GetRoles()).Returns([PortalRoleNames.Cto]);
        _portalSettingsAccessorMock
            .Setup(x => x.GetStringValueByName(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("14");

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        result[0].UserId.Should().Be(users[0].UserId);
        result[0].UserFullName.Should().Be("Доу Джон (Doe John)");
        result[0].Date.Should().Be(hireDate.AddMonths(3).AddDays(-1));
    }
}
