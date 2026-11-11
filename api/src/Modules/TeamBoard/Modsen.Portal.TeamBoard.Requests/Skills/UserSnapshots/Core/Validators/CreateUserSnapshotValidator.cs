using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services;
using Modsen.Portal.TeamBoard.Requests.Core.DbQueries;
using Modsen.Portal.TeamBoard.Requests.Skills.UserSnapshots.Core.DbQueries;
using Modsen.Portal.TeamBoard.Requests.Skills.UserSnapshots.Requests;

namespace Modsen.Portal.TeamBoard.Requests.Skills.UserSnapshots.Core.Validators;

internal interface ICreateUserSnapshotValidator
{
    Task ValidateAsync(CreateUserSnapshotRequest.Command request, CancellationToken cancellationToken);
}

internal class CreateUserSnapshotValidator : ICreateUserSnapshotValidator
{
    private readonly IIsTeamBoardUserExistsDbQuery _isTeamBoardUserExistsDbQuery;
    private readonly IGetLastSnapshotByUserIdDbQuery _getLastSnapshotByUserIdDbQuery;
    private readonly IClockService _clockService;

    public CreateUserSnapshotValidator(IIsTeamBoardUserExistsDbQuery isTeamBoardUserExistsDbQuery,
        IGetLastSnapshotByUserIdDbQuery getLastSnapshotByUserIdDbQuery,
        IClockService clockService)
    {
        _isTeamBoardUserExistsDbQuery = isTeamBoardUserExistsDbQuery;
        _getLastSnapshotByUserIdDbQuery = getLastSnapshotByUserIdDbQuery;
        _clockService = clockService;
    }

    public async Task ValidateAsync(CreateUserSnapshotRequest.Command request, CancellationToken cancellationToken)
    {
        var isSnapshotForFuture = request.SnapshotDate > DateOnly.FromDateTime(_clockService.UtcNow);
        if (isSnapshotForFuture)
        {
            throw new ValidationFailedException("You cannot create entries for dates in the future");
        }

        var isTeamBoardUserExists = await _isTeamBoardUserExistsDbQuery.ExecuteAsync(request.UserId, cancellationToken);
        if (!isTeamBoardUserExists)
        {
            throw new EntityNotFoundException($"User with id {request.UserId} does not exist");
        }

        var lastUserSnapshot = await _getLastSnapshotByUserIdDbQuery.ExecuteAsync(request.UserId, cancellationToken);

        if (lastUserSnapshot?.SnapshotDate > request.SnapshotDate)
        {
            throw new EntityConflictException("You cannot create entries for dates after the date in the last column");
        }

        if (lastUserSnapshot?.SnapshotDate == request.SnapshotDate)
        {
            throw new EntityConflictException("An entry for the selected date already exists");
        }
    }
}
