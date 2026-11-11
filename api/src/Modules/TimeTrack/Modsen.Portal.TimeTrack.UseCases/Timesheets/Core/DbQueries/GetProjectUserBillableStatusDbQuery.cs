using Microsoft.EntityFrameworkCore;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;

namespace Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.DbQueries;

internal interface IGetProjectUserBillableStatusDbQuery
{
    Task<bool> ExecuteAsync(int projectId, int userId, DateOnly assignmentDate, CancellationToken cancellationToken);
}

internal sealed class GetProjectUserBillableStatusDbQuery : IGetProjectUserBillableStatusDbQuery
{
    private readonly ITimeTrackDbContext _dbContext;

    public GetProjectUserBillableStatusDbQuery(ITimeTrackDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<bool> ExecuteAsync(int projectId, int userId, DateOnly assignmentDate,
        CancellationToken cancellationToken)
    {
        return _dbContext.ProjectUsers
            .Where(pu => pu.ProjectId == projectId &&
                pu.UserId == userId &&
                pu.UserOnProjectFrom <= assignmentDate &&
                (pu.UserOnProjectTo == null || pu.UserOnProjectTo >= assignmentDate))
            .Select(pu => pu.IsTimeOnProjectBillable)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
