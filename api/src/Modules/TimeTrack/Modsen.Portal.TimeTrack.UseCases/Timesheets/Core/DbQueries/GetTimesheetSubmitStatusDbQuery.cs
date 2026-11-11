using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.TimeTrack;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;

namespace Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.DbQueries;

internal interface IGetTimesheetSubmitStatusDbQuery
{
    Task<TimesheetSubmitStatusType?> ExecuteAsync(int userId, DateOnly reportDate,
        CancellationToken cancellationToken);
}

internal sealed class GetTimesheetSubmitStatusDbQuery : IGetTimesheetSubmitStatusDbQuery
{
    private readonly ITimeTrackDbContext _dbContext;

    public GetTimesheetSubmitStatusDbQuery(ITimeTrackDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<TimesheetSubmitStatusType?> ExecuteAsync(int userId, DateOnly reportDate,
        CancellationToken cancellationToken)
    {
        var response = await _dbContext.TimesheetSubmitStatuses
            .Where(status =>
                status.UserId == userId &&
                status.Year == reportDate.Year &&
                status.Month == reportDate.Month)
            .FirstOrDefaultAsync(cancellationToken);

        return response?.Status;
    }
}
