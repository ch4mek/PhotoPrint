using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.Core.DbQueries;
using Modsen.Portal.TeamBoard.Requests.Skills.UserSnapshots.Core.DbQueries;
using Modsen.Portal.TeamBoard.Requests.Skills.UserSnapshots.Core.Validators;
using Modsen.Portal.TeamBoard.Requests.Skills.UserSnapshots.Requests;
using Moq;

namespace Modsen.Portal.TeamBoard.UnitTests.Skills.UserSnapshots.Validators;

public class CreateUserSnapshotValidatorTests
{
    private readonly CreateUserSnapshotValidator _validator;
    private readonly Mock<IIsTeamBoardUserExistsDbQuery> _isTeamBoardUserExistsDbQuery = new();
    private readonly Mock<IGetLastSnapshotByUserIdDbQuery> _getLastSnapshotByUserIdDbQuery = new();
    private readonly Mock<IClockService> _clockService = new();
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public CreateUserSnapshotValidatorTests()
    {
        _validator = new CreateUserSnapshotValidator(_isTeamBoardUserExistsDbQuery.Object,
            _getLastSnapshotByUserIdDbQuery.Object, _clockService.Object);

        _clockService.Setup(x => x.UtcNow).Returns(DateTime.UtcNow);
    }

    [Fact]
    public async Task ValidateWhenSnapshotForFutureThrowValidationFailedException()
    {
        // Arrange
        var userId = 12;
        var snapshotDate = DateOnly.FromDateTime(DateTime.Today).AddDays(1);

        var request = new CreateUserSnapshotRequest.Command(userId, snapshotDate, []);

        // Act
        var act = () => _validator.ValidateAsync(request, _cancellationToken);

        // Assert
        await act.Should().ThrowAsync<ValidationFailedException>()
            .WithMessage("You cannot create entries for dates in the future");
    }

    [Fact]
    public async Task ValidateWhenUserIsNotFoundThrowEntityNotFoundException()
    {
        // Arrange
        var userId = 12;
        var snapshotDate = DateOnly.FromDateTime(DateTime.Today);

        var request = new CreateUserSnapshotRequest.Command(userId, snapshotDate, []);

        _isTeamBoardUserExistsDbQuery.Setup(x => x.ExecuteAsync(userId, _cancellationToken))
            .ReturnsAsync(false);

        // Act
        var act = () => _validator.ValidateAsync(request, _cancellationToken);

        // Assert
        await act.Should().ThrowAsync<EntityNotFoundException>()
            .WithMessage($"User with id {request.UserId} does not exist");
    }

    [Fact]
    public async Task ValidateWhenAlreadyLaterSnapshotExistsThrowEntityConflictException()
    {
        // Arrange
        var userId = 12;
        var snapshotDate = DateOnly.FromDateTime(DateTime.Today).AddDays(-1);

        var request = new CreateUserSnapshotRequest.Command(userId, snapshotDate, []);

        _isTeamBoardUserExistsDbQuery.Setup(x => x.ExecuteAsync(userId, _cancellationToken))
            .ReturnsAsync(true);

        var lastSnapshot = new UserSkillsSnapshot
        {
            SnapshotDate = snapshotDate.AddDays(1),
        };

        _getLastSnapshotByUserIdDbQuery.Setup(x => x.ExecuteAsync(userId, _cancellationToken))
            .ReturnsAsync(lastSnapshot);

        // Act
        var act = () => _validator.ValidateAsync(request, _cancellationToken);

        // Assert
        await act.Should().ThrowAsync<EntityConflictException>()
            .WithMessage("You cannot create entries for dates after the date in the last column");
    }

    [Fact]
    public async Task ValidateWhenAlreadySnapshotForTodayExistsThrowEntityConflictException()
    {
        // Arrange
        var userId = 12;
        var snapshotDate = DateOnly.FromDateTime(DateTime.Today);

        var request = new CreateUserSnapshotRequest.Command(userId, snapshotDate, []);

        _isTeamBoardUserExistsDbQuery.Setup(x => x.ExecuteAsync(userId, _cancellationToken))
            .ReturnsAsync(true);

        var lastSnapshot = new UserSkillsSnapshot
        {
            SnapshotDate = snapshotDate,
        };

        _getLastSnapshotByUserIdDbQuery.Setup(x => x.ExecuteAsync(userId, _cancellationToken))
            .ReturnsAsync(lastSnapshot);

        // Act
        var act = () => _validator.ValidateAsync(request, _cancellationToken);

        // Assert
        await act.Should().ThrowAsync<EntityConflictException>()
            .WithMessage("An entry for the selected date already exists");
    }

    [Fact]
    public async Task ValidateWhenRequestIsValidAndThereIsNoSnapshotsForUserShouldNotThrowException()
    {
        // Arrange
        var userId = 12;
        var snapshotDate = DateOnly.FromDateTime(DateTime.Today);

        var request = new CreateUserSnapshotRequest.Command(userId, snapshotDate, []);

        _isTeamBoardUserExistsDbQuery.Setup(x => x.ExecuteAsync(userId, _cancellationToken))
            .ReturnsAsync(true);

        _getLastSnapshotByUserIdDbQuery.Setup(x => x.ExecuteAsync(userId, _cancellationToken))
            .ReturnsAsync((UserSkillsSnapshot?)null);

        // Act
        var act = () => _validator.ValidateAsync(request, _cancellationToken);

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task ValidateWhenRequestIsValidShouldNotThrowException()
    {
        // Arrange
        var userId = 12;
        var snapshotDate = DateOnly.FromDateTime(DateTime.Today);

        var request = new CreateUserSnapshotRequest.Command(userId, snapshotDate, []);

        _isTeamBoardUserExistsDbQuery.Setup(x => x.ExecuteAsync(userId, _cancellationToken))
            .ReturnsAsync(true);

        var lastSnapshot = new UserSkillsSnapshot
        {
            SnapshotDate = snapshotDate.AddDays(-10),
        };

        _getLastSnapshotByUserIdDbQuery.Setup(x => x.ExecuteAsync(userId, _cancellationToken))
            .ReturnsAsync(lastSnapshot);

        // Act
        var act = () => _validator.ValidateAsync(request, _cancellationToken);

        // Assert
        await act.Should().NotThrowAsync();
    }
}
