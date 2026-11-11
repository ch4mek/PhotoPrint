using MockQueryable.Moq;
using Modsen.Portal.Core.UseCases.Core.Calculators;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.Calendar.Core.Dto;
using Modsen.Portal.TeamBoard.Requests.Calendar.Requests;
using Moq;

namespace Modsen.Portal.TeamBoard.UnitTests.Calendar.Requests;

public class GetUsersByAnniversariesIntervalRequestTests
{
    private readonly GetUsersByAnniversariesIntervalRequest _getUsersByAnniversariesIntervalRequest;
    private readonly Mock<ITeamBoardDbContext> _teamBoardDbContextMock = new();
    private readonly IDateCalculator _dateCalculator = new DateCalculator();
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    private readonly List<TeamBoardUser> _teamBoardUsers =
    [
        new()
        {
            UserId = 1,
            FirstName = "FirstName",
            LastName = "LastName",
            Email = "Email",
            HireDate = new DateOnly(2023, 10, 10),
            DateOfBirth = new DateOnly(2004, 4, 16)
        },

        new()
        {
            UserId = 2,
            FirstName = "FirstName",
            LastName = "LastName",
            Email = "Email",
            HireDate = new DateOnly(2024, 10, 10),
            DateOfBirth = new DateOnly(2005, 4, 16)
        },

        new()
        {
            UserId = 3,
            FirstName = "FirstName",
            LastName = "LastName",
            Email = "Email",
            HireDate = new DateOnly(2025, 10, 10),
            DateOfBirth = new DateOnly(2004, 4, 17)
        }

    ];

    public GetUsersByAnniversariesIntervalRequestTests()
    {
        _teamBoardDbContextMock
            .Setup(mock => mock.TeamBoardUsers)
            .Returns(_teamBoardUsers.BuildMockDbSet().Object);

        _getUsersByAnniversariesIntervalRequest = new GetUsersByAnniversariesIntervalRequest(
            _teamBoardDbContextMock.Object,
            _dateCalculator);
    }

    [Fact]
    public async Task HandleWhenDateCorrectShouldReturnUsers()
    {
        //Arrange
        var from = new DateOnly(2025, 9, 9);
        var to = new DateOnly(2026, 1, 1);

        var expectedResult = new List<TeamBoardUserBirthdayDto>
        {
            new()
            {
                Id = 1,
                FirstName = "FirstName",
                LastName = "LastName",
                Email = "Email",
                WorkedFullYears = 2,
                AvatarFileId = null,
            },
            new()
            {
                Id = 2,
                FirstName = "FirstName",
                LastName = "LastName",
                Email = "Email",
                WorkedFullYears = 1,
                AvatarFileId = null,
            },
        };

        var query = new GetUsersByAnniversariesIntervalRequest.Query(from, to);

        //Act
        var result = await _getUsersByAnniversariesIntervalRequest.Handle(query, _cancellationToken);

        //Assert
        result.Should().BeEquivalentTo(expectedResult);
    }
}
