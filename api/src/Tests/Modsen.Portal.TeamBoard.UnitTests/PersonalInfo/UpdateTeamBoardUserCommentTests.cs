using MockQueryable.Moq;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.Core.DbQueries;
using Modsen.Portal.TeamBoard.Requests.UserPersonalInfo.Requests;
using Moq;

namespace Modsen.Portal.TeamBoard.UnitTests.PersonalInfo;

public class UpdateTeamBoardUserCommentTests
{
    private readonly Mock<IIsTeamBoardUserExistWithAccessControlDbQuery> _isUserExistByUserIdDbQueryMock = new();
    private readonly Mock<ITeamBoardDbContext> _teamboardDbContextMock = new();
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock = new();
    private readonly CancellationToken _token = CancellationToken.None;

    private readonly UpdateTeamBoardUserCommentRequest _handler;

    public UpdateTeamBoardUserCommentTests()
    {
        _handler = new UpdateTeamBoardUserCommentRequest(
            _isUserExistByUserIdDbQueryMock.Object,
            _teamboardDbContextMock.Object,
            _authenticatedUserContextMock.Object);
    }

    [Fact]
    public async Task HandleWhenTeamBoardUserPersonalInfoDoesNotExistThrowEntityNotFoundException()
    {
        // Arrange
        var userId = 123456;
        var comment = "comment";

        var command = new UpdateTeamBoardUserCommentRequest.Command(userId, comment);

        _isUserExistByUserIdDbQueryMock
            .Setup(x => x.ExecuteAsync(command.UserId, true, _token))
            .ReturnsAsync(false);

        // Act
        var action = () => _handler.Handle(command, _token);

        // Assert
        await action.Should().ThrowAsync<EntityNotFoundException>("User is not found.");
    }

    [Fact]
    public async Task HandleWhenTeamBoardUserExistsShouldReturnSpecialization()
    {
        // Arrange
        var userId = 2;
        var comment = "comment";

        var command = new UpdateTeamBoardUserCommentRequest.Command(userId, comment);

        _isUserExistByUserIdDbQueryMock
            .Setup(x => x.ExecuteAsync(command.UserId, true, _token))
            .ReturnsAsync(true);

        _teamboardDbContextMock.Setup(x => x.TeamBoardUsers)
            .Returns(new List<TeamBoardUser>
            {
                new()
                {
                    UserId = userId,
                    Comments = null,
                    FirstName = "test",
                    LastName = "test",
                    Email = "test"
                }
            }.BuildMockDbSet().Object);

        // Act
        var result = await _handler.Handle(command, _token);

        // Assert
        result.Should().NotBeNull();
        result.Comment.Should().Be(comment);
    }

    [Fact]
    public async Task HandleWhenTeamBoardUserExistsAndCommentIsNullShouldReturnNull()
    {
        // Arrange
        var userId = 2;
        var comment = default(string);

        var command = new UpdateTeamBoardUserCommentRequest.Command(userId, comment);

        _isUserExistByUserIdDbQueryMock
            .Setup(x => x.ExecuteAsync(command.UserId, true, _token))
            .ReturnsAsync(true);

        _teamboardDbContextMock.Setup(x => x.TeamBoardUsers)
            .Returns(new List<TeamBoardUser>
            {
                new()
                {
                    UserId = userId,
                    Comments = "comment",
                    FirstName = "test",
                    LastName = "test",
                    Email = "test"
                }
            }.BuildMockDbSet().Object);

        // Act
        var result = await _handler.Handle(command, _token);

        // Assert
        result.Should().NotBeNull();
        result.Comment.Should().Be(comment);
    }
}
