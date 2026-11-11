using Microsoft.EntityFrameworkCore;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Projects;

namespace Modsen.Portal.TimeTrack.DataAccess.Npgsql.Projects.DbQueries;

internal class UpdateProjectUserDbQuery : IUpdateProjectUserDbQuery
{
    private readonly ITimeTrackDbContext _timeTrackDbContext;
    
    public UpdateProjectUserDbQuery(ITimeTrackDbContext timeTrackDbContext)
    {
        _timeTrackDbContext = timeTrackDbContext;
    }
    
    public async Task<UpdateProjectUserDbResponse> ExecuteAsync(UpdateProjectUserDbRequest request, CancellationToken cancellationToken)
    {
        var projectUser = await _timeTrackDbContext.ProjectUsers
            .FirstAsync(x => x.Id == request.Id, cancellationToken);

        projectUser.UserId = request.UserId;
        projectUser.ProjectId = request.ProjectId;
        projectUser.IsTimeOnProjectBillable = request.IsTimeOnProjectBillable;
        projectUser.Fte = request.Fte;
        projectUser.UserOnProjectFrom = request.UserOnProjectFrom;
        projectUser.UserOnProjectTo = request.UserOnProjectTo;

        await _timeTrackDbContext.SaveChanges(cancellationToken);

        return new UpdateProjectUserDbResponse(
            projectUser.Id,
            projectUser.UserId,
            projectUser.ProjectId);
    }
}
