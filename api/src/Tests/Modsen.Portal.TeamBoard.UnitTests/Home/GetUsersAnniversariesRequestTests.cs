using MockQueryable.Moq;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Core.UseCases.Core.Calculators;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.Home.Requests;
using Moq;

namespace Modsen.Portal.TeamBoard.UnitTests.Home;

public class GetUsersAnniversariesRequestTests
{
    private readonly Mock<ITeamBoardDbContext> _teamBoardDbContextMock;
    private readonly Mock<IClockService> _clockServiceMock;
    private readonly Mock<IDateCalculator> _dateCalculatorMock;
    private readonly GetUsersAnniversariesRequest _handler;

    public GetUsersAnniversariesRequestTests()
    {
        _teamBoardDbContextMock = new Mock<ITeamBoardDbContext>();
        _clockServiceMock = new Mock<IClockService>();
        _dateCalculatorMock = new Mock<IDateCalculator>();
        _handler = new GetUsersAnniversariesRequest(
            _teamBoardDbContextMock.Object,
            _clockServiceMock.Object,
            _dateCalculatorMock.Object
        );
    }

    [Fact]
    public async Task HandleWithAnniversariesReturnsUserAnniversaryDates()
    {
        // Arrange
        var currentDate = DateOnly.FromDateTime(DateTime.UtcNow);

        var users = new List<TeamBoardUser>
        {
            new()
            {
                UserId = 1,
                Email = "john.doe@example.com",
                FirstName = "John",
                LastName = "Doe",
                HireDate = currentDate.AddYears(-1),
                IsActive = true,
                DepartmentId = "Net",
            },
            new()
            {
                UserId = 2,
                Email = "jane.doe@example.com",
                FirstName = "Jane",
                LastName = "Doe",
                HireDate = currentDate.AddYears(-2),
                IsActive = false,
                DepartmentId = "Net",
            },
        };

        _teamBoardDbContextMock
            .Setup(db => db.TeamBoardUsers)
            .Returns(users.BuildMockDbSet().Object);

        _clockServiceMock
            .Setup(cs => cs.TodayUtc())
            .Returns(currentDate);
        _dateCalculatorMock
            .Setup(calc => calc.IsWholeYearsPassed(It.IsAny<DateOnly>(), It.IsAny<DateOnly>()))
            .Returns(true);

        var query = new GetUsersAnniversariesRequest.Query();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeEmpty();
        result.Should().HaveCount(1);
        result[0].UserId.Should().Be(1);
        result[0].Email.Should().Be("john.doe@example.com");
        result[0].DepartmentId.Should().Be("Net");
    }
}
