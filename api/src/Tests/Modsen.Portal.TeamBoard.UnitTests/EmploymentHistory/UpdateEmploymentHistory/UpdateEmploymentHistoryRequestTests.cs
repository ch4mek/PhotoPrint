using MockQueryable.Moq;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.Core.DbQueries;
using Modsen.Portal.TeamBoard.Requests.EmploymentHistory.Requests;
using Moq;

namespace Modsen.Portal.TeamBoard.UnitTests.EmploymentHistory.UpdateEmploymentHistory;

public class UpdateEmploymentHistoryRequestTests
{
    private readonly Mock<IIsTeamBoardUserExistWithHRAccessControlDbQuery> _isTeamBoardUserExistingByUserIdDbQueryMock = new();
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock = new();
    private readonly Mock<ITeamBoardDbContext> _dbContextMock = new();
    private readonly Mock<IClockService> _clockServiceMock = new();
    private readonly CancellationToken _token = CancellationToken.None;

    private readonly UpdateEmploymentHistoryRequest _handler;

    public UpdateEmploymentHistoryRequestTests()
    {
        _handler = new UpdateEmploymentHistoryRequest(
            _isTeamBoardUserExistingByUserIdDbQueryMock.Object,
            _authenticatedUserContextMock.Object,
            _dbContextMock.Object,
            _clockServiceMock.Object);
    }

    [Fact]
    public async Task HandleWhenTeamBoardUserDoesNotExistThrowEntityNotFoundException()
    {
        // Arrange
        var initiatotId = 1;
        _authenticatedUserContextMock
            .Setup(x => x.GetRequiredUserId())
            .Returns(initiatotId);

        var userId = 123456;

        var query = new UpdateEmploymentHistoryRequest.Command(userId, null, null);

        _isTeamBoardUserExistingByUserIdDbQueryMock
            .Setup(x => x.ExecuteAsync(query.UserId, _token))
            .ReturnsAsync(false);

        // Act
        var action = () => _handler.Handle(query, _token);

        // Assert
        await action.Should().ThrowAsync<EntityNotFoundException>("User is not found.");
    }

    [Fact]
    public async Task HandleWhenTeamBoardUserExistsShouldReturnEmploymentHistory()
    {
        // Arrange
        var initiatotId = 1;
        _authenticatedUserContextMock
            .Setup(x => x.GetRequiredUserId())
            .Returns(initiatotId);

        var userId = 5;

        var professionalActivityStartDate = new DateOnly(2020, 6, 14);

        var command = new UpdateEmploymentHistoryRequest.Command(
            userId,
            professionalActivityStartDate,
            null);

        _isTeamBoardUserExistingByUserIdDbQueryMock
            .Setup(x => x.ExecuteAsync(command.UserId, _token))
            .ReturnsAsync(true);

        _authenticatedUserContextMock.Setup(x => x.UserId)
            .Returns(1);

        _clockServiceMock.Setup(x => x.TodayUtc())
            .Returns(DateOnly.FromDateTime(DateTime.UtcNow));

        _dbContextMock.Setup(x => x.TeamBoardUsers)
            .Returns(new List<TeamBoardUser>()
            {
                new()
                {
                    UserId = userId,
                    FirstName = "test",
                    LastName = "test",
                    Email = "test",
                    Contracts =
                    [
                        new() { ContractFrom = DateOnly.FromDateTime(DateTime.UtcNow) }
                    ]
                }
            }.BuildMockDbSet().Object);

        _dbContextMock.Setup(x => x.InternshipDetails)
            .Returns(new List<InternshipDetail>().BuildMockDbSet().Object);

        // Act
        var result = await _handler.Handle(command, _token);

        // Assert
        result.Should().NotBeNull();
        result.ContractEndDate.Should().Be(null);
        result.ProfessionalActivityStartDate.Should().Be(professionalActivityStartDate);
        result.FirstProjectStartDate.Should().Be(null);
    }

    [Fact]
    public async Task HandleWhenUserUpdatesThemselfThrowActionNotAllowedException()
    {
        //Arrange
        var initiatotId = 1;
        _authenticatedUserContextMock
            .Setup(x => x.GetRequiredUserId())
            .Returns(initiatotId);

        var query = new UpdateEmploymentHistoryRequest.Command(initiatotId, null, null);

        // Act
        var action = () => _handler.Handle(query, _token);

        // Assert
        await action.Should().ThrowAsync<EntityNotFoundException>("You are not allowed to edit your own employment history");
    }
}
