using MockQueryable.Moq;
using Modsen.Portal.Core.UseCases.Core.Calculators;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.Calendar.Core.Dto;
using Modsen.Portal.TeamBoard.Requests.Calendar.Requests;
using Moq;

namespace Modsen.Portal.TeamBoard.UnitTests.Calendar.Requests;

public class GetUsersByBirthdayDateRequestTests
{
    private readonly GetUsersByBirthdayDateRequest _getUsersByBirthdayDateRequest;
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
            HireDate = new DateOnly(2023, 10, 10),
            DateOfBirth = new DateOnly(2004, 4, 17)
        }

    ];

    public GetUsersByBirthdayDateRequestTests()
    {
        _teamBoardDbContextMock
            .Setup(mock => mock.TeamBoardUsers)
            .Returns(_teamBoardUsers.BuildMockDbSet().Object);

        _getUsersByBirthdayDateRequest = new GetUsersByBirthdayDateRequest(
            _teamBoardDbContextMock.Object,
            _dateCalculator);
    }

    [Fact]
    public async Task HandleWhenDateCorrectShouldReturnUsers()
    {
        //Arrange
        var date = new DateOnly(2025, 4, 16);

        var expectedResult = new List<TeamBoardUserBirthdayDto>
        {
            new()
            {
                Id = 1,
                FirstName = "FirstName",
                LastName = "LastName",
                Email = "Email",
                WorkedFullYears = 1,
                AvatarFileId = null,
            },
            new()
            {
                Id = 2,
                FirstName = "FirstName",
                LastName = "LastName",
                Email = "Email",
                WorkedFullYears = 0,
                AvatarFileId = null,
            },
        };

        var query = new GetUsersByBirthdayDateRequest.Query(date);

        //Act
        var result = await _getUsersByBirthdayDateRequest.Handle(query, _cancellationToken);

        //Assert
        result.Should().BeEquivalentTo(expectedResult);
    }
}
