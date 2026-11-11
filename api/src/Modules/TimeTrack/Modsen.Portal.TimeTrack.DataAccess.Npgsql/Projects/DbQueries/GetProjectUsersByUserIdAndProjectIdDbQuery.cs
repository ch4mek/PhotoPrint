using Microsoft.EntityFrameworkCore;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Projects;

namespace Modsen.Portal.TimeTrack.DataAccess.Npgsql.Projects.DbQueries;

internal class GetProjectUsersByUserIdAndProjectIdDbQuery : IGetProjectUsersByUserIdAndProjectIdDbQuery
{
    private readonly ITimeTrackDbContext _timeTrackDbContext;
    
    public GetProjectUsersByUserIdAndProjectIdDbQuery(ITimeTrackDbContext timeTrackDbContext)
    {
        _timeTrackDbContext = timeTrackDbContext;
    }

    public Task<List<GetProjectUsersByUserIdAndProjectIdDbResponse>> ExecuteAsync(
        GetProjectUsersByUserIdAndProjectIdDbRequest request, CancellationToken cancellationToken)
    {
        return _timeTrackDbContext.ProjectUsers.Where(x => x.ProjectId == request.ProjectId && x.UserId == request.Userid)
            .Select(x => new GetProjectUsersByUserIdAndProjectIdDbResponse(x.Id, x.UserOnProjectFrom, x.UserOnProjectTo))
            .ToListAsync(cancellationToken);
    }
}
