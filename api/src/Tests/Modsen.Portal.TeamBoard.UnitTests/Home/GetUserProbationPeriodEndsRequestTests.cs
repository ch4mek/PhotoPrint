using MockQueryable.Moq;
using Modsen.Portal.Core.Services;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.Home.Requests;
using Moq;

namespace Modsen.Portal.TeamBoard.UnitTests.Home;

public class GetUserProbationPeriodEndsRequestTests
{
    private readonly Mock<ITeamBoardDbContext> _teamBoardDbContextMock;
    private readonly Mock<IClockService> _clockServiceMock;
    private readonly GetUserProbationPeriodEndsRequest _handler;

    public GetUserProbationPeriodEndsRequestTests()
    {
        _teamBoardDbContextMock = new Mock<ITeamBoardDbContext>();
        _clockServiceMock = new Mock<IClockService>();
        _handler = new GetUserProbationPeriodEndsRequest(
            _teamBoardDbContextMock.Object,
            _clockServiceMock.Object
        );
    }

    [Fact]
    public async Task HandleWhenNoUsersReturnsEmptyList()
    {
        // Arrange
        var query = new GetUserProbationPeriodEndsRequest.Query(null);
        _teamBoardDbContextMock
            .Setup(db => db.TeamBoardUsers)
            .Returns(new List<TeamBoardUser>().BuildMockDbSet().Object);
        _clockServiceMock
            .Setup(cs => cs.TodayUtc())
            .Returns(DateOnly.FromDateTime(DateTime.UtcNow));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task HandleWhenUsersWithMatchingProbationEndsReturnsUserProbationPeriodEnds()
    {
        // Arrange
        var today = new DateOnly(2026, 05, 15);
        var users = new List<TeamBoardUser>
        {
            new()
            {
                UserId = 1,
                Email = "john.doe@example.com",
                FirstName = "John",
                LastName = "Doe",
                DepartmentId = "Net",
                HireDate = today.AddDays(1).AddMonths(-3),
                IsActive = true
            },
            new()
            {
                UserId = 2,
                Email = "jane.doe@example.com",
                FirstName = "Jane",
                LastName = "Doe",
                DepartmentId = "Java",
                HireDate = today.AddDays(2).AddMonths(-3),
                IsActive = true
            }
        };

        _teamBoardDbContextMock
            .Setup(db => db.TeamBoardUsers)
            .Returns(users.BuildMockDbSet().Object);
        _clockServiceMock
            .Setup(cs => cs.TodayUtc())
            .Returns(today);

        var query = new GetUserProbationPeriodEndsRequest.Query(today);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeEmpty().And.HaveCount(1);
        result[0].UserId.Should().Be(1);
        result[0].Email.Should().Be("john.doe@example.com");
        result[0].FullName.Should().Be("Doe John");
        result[0].DepartmentId.Should().Be("Net");
    }
}
