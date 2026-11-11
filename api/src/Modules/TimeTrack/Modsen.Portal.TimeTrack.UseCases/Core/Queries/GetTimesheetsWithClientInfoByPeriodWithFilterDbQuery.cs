using Microsoft.EntityFrameworkCore;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Contracts;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.Extensions;

namespace Modsen.Portal.TimeTrack.UseCases.Core.Queries;

internal interface IGetTimesheetsWithClientInfoByPeriodWithFilterDbQuery
{
    public Task<List<Timesheet>> ExecuteAsync(
        GetTimesheetsWithClientInfoByPeriodWithFilterDbRequest request,
        CancellationToken cancellationToken);
}

internal record GetTimesheetsWithClientInfoByPeriodWithFilterDbRequest(
    int? Year,
    int[]? MonthOfYear,
    int? WeekOfYear,
    TimesheetFilter Filter);

internal sealed class GetTimesheetsWithClientInfoByPeriodWithFilterDbQuery
    : IGetTimesheetsWithClientInfoByPeriodWithFilterDbQuery
{
    private readonly ITimeTrackDbContext _timeTrackDbContext;

    public GetTimesheetsWithClientInfoByPeriodWithFilterDbQuery(
        ITimeTrackDbContext timeTrackDbContext)
    {
        _timeTrackDbContext = timeTrackDbContext;
    }

    public Task<List<Timesheet>> ExecuteAsync(
        GetTimesheetsWithClientInfoByPeriodWithFilterDbRequest request,
        CancellationToken cancellationToken)
    {
        return _timeTrackDbContext.Timesheets
            .AsNoTracking()
            .Include(timesheet => timesheet.Project)
            .ThenInclude(project => project!.Client)
            .Include(timesheet => timesheet.User)
            .FilterByPeriod(request.Year, request.MonthOfYear, request.WeekOfYear)
            .ApplyTimesheetFilter(request.Filter)
            .OrderBy(timesheet => timesheet.Id)
            .ToListAsync(cancellationToken);
    }
}
