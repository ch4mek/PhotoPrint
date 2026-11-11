using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.Requests.Core.CoreDto;
using Modsen.Portal.TimeTrack.Contract;

namespace Modsen.Portal.TeamBoard.Requests.Core.DbQueries;

internal interface IUpdateProjectUserDbQuery
{
    Task ExecuteAsync(UpdateProjectUserCoreDto request, CancellationToken cancellationToken);
}

internal class UpdateProjectUserDbQuery : IUpdateProjectUserDbQuery
{
    private readonly ITeamBoardDbContext _teamBoardDbContext;
    private readonly ITimeTrackContract _timeTrackContract;

    public UpdateProjectUserDbQuery(
        ITeamBoardDbContext teamBoardDbContext,
        ITimeTrackContract timeTrackContract)
    {
        _teamBoardDbContext = teamBoardDbContext;
        _timeTrackContract = timeTrackContract;
    }

    public async Task ExecuteAsync(UpdateProjectUserCoreDto request, CancellationToken cancellationToken)
    {
        var projectUser = await _teamBoardDbContext.ProjectUsers
            .FirstAsync(user => user.Id == request.Id, cancellationToken);

        var seniorityLevel = await _teamBoardDbContext.SeniorityLevels.FirstOrDefaultAsync(
            x => x.Id == request.SeniorityLevelId, cancellationToken);

        if (seniorityLevel is null)
        {
            throw new EntityNotFoundException($"SeniorityLevel with id {request.SeniorityLevelId} not found");
        }

        var isTimesheetExist = await _timeTrackContract.IsAssignHasTrackedTime(
            request.UserId, request.ProjectId, request.UserOnProjectFrom, request.UserOnProjectTo, cancellationToken);

        if (isTimesheetExist && projectUser.UserOnProjectSeniorityLevelId != request.SeniorityLevelId)
        {
            throw new ActionNotAllowedException("Assign with tracked time is not full editable");
        }

        projectUser.UserId = request.UserId;
        projectUser.ProjectId = request.ProjectId;
        projectUser.IsTimeOnProjectBillable = request.IsTimeOnProjectBillable;
        projectUser.Fte = request.Fte;
        projectUser.UserOnProjectFrom = request.UserOnProjectFrom;
        projectUser.UserOnProjectTo = request.UserOnProjectTo;

        projectUser.UserOnProjectSeniorityLevelId = seniorityLevel.Id;

        await _teamBoardDbContext.SaveChangesAsync(cancellationToken);
    }
}
