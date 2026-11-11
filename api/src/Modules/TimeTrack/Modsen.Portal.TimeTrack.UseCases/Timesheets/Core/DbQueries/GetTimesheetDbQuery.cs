using Microsoft.EntityFrameworkCore;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.DbQueries;

internal interface IGetTimesheetDbQuery
{
    Task<Timesheet?> ExecuteAsync(int timesheetId, CancellationToken cancellationToken);
}

internal sealed class GetTimesheetDbQuery : IGetTimesheetDbQuery
{
    private readonly ITimeTrackDbContext _dbContext;

    public GetTimesheetDbQuery(ITimeTrackDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Timesheet?> ExecuteAsync(int timesheetId, CancellationToken cancellationToken)
    {
        return _dbContext.Timesheets
            .Include(timesheet => timesheet.User)
            .Include(timesheet => timesheet.Project)
            .Where(timesheet => timesheet.Id == timesheetId)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
