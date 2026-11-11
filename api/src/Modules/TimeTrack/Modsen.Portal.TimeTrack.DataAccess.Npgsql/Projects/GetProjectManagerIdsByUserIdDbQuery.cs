using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Services;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Projects;

namespace Modsen.Portal.TimeTrack.DataAccess.Npgsql.Projects;

internal class GetProjectManagerIdsByUserIdDbQuery : IGetProjectManagerIdsByUserIdDbQuery
{
    private readonly ITimeTrackDbContext _dbContext;
    private readonly IClockService _clockService;

    public GetProjectManagerIdsByUserIdDbQuery(ITimeTrackDbContext timeTrackDbContext, IClockService clockService)
    {
        _dbContext = timeTrackDbContext;
        _clockService = clockService;
    }

    public Task<List<int>> ExecuteAsync(int userId, CancellationToken cancellationToken)
    {
        return _dbContext.Projects
            .Where(x => x.IsActive)
            .Where(x => x.ProjectUsers.Any(pu => pu.UserId == userId
                && pu.UserOnProjectFrom <= _clockService.TodayUtc()
                && (pu.UserOnProjectTo == null || pu.UserOnProjectTo >= _clockService.TodayUtc())))
            .Where(x => x.ManagerUserId.HasValue)
            .Select(x => x.ManagerUserId!.Value)
            .ToListAsync(cancellationToken);
    }
}
