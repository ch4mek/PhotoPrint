using Microsoft.EntityFrameworkCore;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Contracts;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.Extensions;

namespace Modsen.Portal.TimeTrack.UseCases.Core.Queries;

internal interface IGetTimesheetsByPeriodWithFilterDbQuery
{
    Task<List<Timesheet>> ExecuteAsync(
        GetTimesheetsByPeriodWithFilterDbRequest request,
        CancellationToken cancellationToken);
}

internal record GetTimesheetsByPeriodWithFilterDbRequest(
    int? Year,
    int[]? MonthOfYear,
    int? WeekOfYear,
    TimesheetFilter Filter);

internal class GetTimesheetsByPeriodWithFilterDbQuery : IGetTimesheetsByPeriodWithFilterDbQuery
{
    private readonly ITimeTrackDbContext _dbContext;

    public GetTimesheetsByPeriodWithFilterDbQuery(ITimeTrackDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<List<Timesheet>> ExecuteAsync(
        GetTimesheetsByPeriodWithFilterDbRequest request,
        CancellationToken cancellationToken)
    {
        var timesheets = _dbContext.Timesheets
            .AsNoTracking()
            .Include(x => x.Project)
            .FilterByPeriod(request.Year, request.MonthOfYear, request.WeekOfYear)
            .ApplyTimesheetFilter(request.Filter)
            .OrderBy(timesheet => timesheet.Id);

        return timesheets.ToListAsync(cancellationToken);
    }
}
