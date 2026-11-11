using Microsoft.EntityFrameworkCore;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.TimeTrack.UseCases.Dashboard.Core.Builders;

internal interface ITimesheetQueryBuilder
{
    ITimesheetQueryBuilder FromAsNoTracked(int? year, int[]? monthOfYear);
    ITimesheetQueryBuilder WithDepartmentFilter(string? departmentId);
    ITimesheetQueryBuilder WithWeekOfYear(int? weekOfYear);
    ITimesheetQueryBuilder WithUserFilter(int? userId);
    ITimesheetQueryBuilder WithProjectFilter(int? projectId);
    ITimesheetQueryBuilder WithBillableHoursFilter(bool? isBillableHoursNeeded);
    ITimesheetQueryBuilder WithResourceGroupFilter(int? resourceGroupId);
    IQueryable<Timesheet> Build();
}

internal class TimesheetQueryBuilder(ITimeTrackDbContext reportsDbContext) : ITimesheetQueryBuilder
{
    private int? _year;
    private int[]? _monthOfYear;
    private int? _weekOfYear;
    private string? _departmentId;
    private int? _projectId;
    private int? _userId;
    private bool? _taskWithBillableHoursNeeded;
    private int? _resourceGroupId;

    public ITimesheetQueryBuilder FromAsNoTracked(int? year, int[]? monthOfYear)
    {
        _year = year;
        _monthOfYear = monthOfYear;
        return this;
    }

    public ITimesheetQueryBuilder WithWeekOfYear(int? weekOfYear)
    {
        _weekOfYear = weekOfYear;
        return this;
    }

    public ITimesheetQueryBuilder WithDepartmentFilter(string? departmentId)
    {
        _departmentId = departmentId;
        return this;
    }

    public ITimesheetQueryBuilder WithUserFilter(int? userId)
    {
        _userId = userId;
        return this;
    }

    public ITimesheetQueryBuilder WithProjectFilter(int? projectId)
    {
        _projectId = projectId;
        return this;
    }

    public ITimesheetQueryBuilder WithBillableHoursFilter(bool? isBillableHoursNeeded)
    {
        _taskWithBillableHoursNeeded = isBillableHoursNeeded;
        return this;
    }

    public ITimesheetQueryBuilder WithResourceGroupFilter(int? resourceGroupId)
    {
        _resourceGroupId = resourceGroupId;
        return this;
    }

    public IQueryable<Timesheet> Build()
    {
        var query = reportsDbContext.Timesheets.AsNoTracking();

        query = FilterByTime(query);
        query = FilterByParams(query);

        return query;
    }

    private IQueryable<Timesheet> FilterByTime(IQueryable<Timesheet> timesheets)
    {
        if (_year is null or 0)
        {
            return timesheets;
        }

        timesheets = timesheets.Where(x => x.ReportYear == _year);

        if (_monthOfYear is { Length: > 0 })
        {
            timesheets = timesheets.Where(x => _monthOfYear.Contains(x.ReportMonthOfYear));
        }

        if (_weekOfYear.HasValue)
        {
            timesheets = timesheets.Where(x => x.ReportWeekOfYear == _weekOfYear.Value);
        }

        return timesheets;
    }

    private IQueryable<Timesheet> FilterByParams(IQueryable<Timesheet> timesheets)
    {
        if (_departmentId is not null)
        {
            timesheets = timesheets.Where(x => x.User!.DepartmentId == _departmentId);
        }

        if (_projectId.HasValue)
        {
            timesheets = timesheets.Where(x => x.ProjectId == _projectId.Value);
        }

        if (_userId.HasValue)
        {
            timesheets = timesheets.Where(x => x.UserId == _userId.Value);
        }

        if (_taskWithBillableHoursNeeded == false)
        {
            timesheets = timesheets.Where(x => x.FactTrackedInMinutes != 0);
        }

        if (_resourceGroupId is not null)
        {
            timesheets = timesheets.Where(x => x.User!.ResourceGroupId == _resourceGroupId);
        }

        return timesheets;
    }
}
