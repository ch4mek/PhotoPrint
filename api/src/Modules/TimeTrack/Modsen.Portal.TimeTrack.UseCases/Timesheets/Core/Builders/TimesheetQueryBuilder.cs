using Microsoft.EntityFrameworkCore;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.Builders;

internal interface ITimesheetQueryBuilder
{
    ITimesheetQueryBuilder WithReportYearFilter(int? startYear, int? endYear);

    ITimesheetQueryBuilder HasAnyProjects(IReadOnlyCollection<int>? projects);

    ITimesheetQueryBuilder HasAnyClients(IReadOnlyCollection<int>? clients);

    ITimesheetQueryBuilder WithBillableFilter(bool? isBillable);

    ITimesheetQueryBuilder WithDepartmentId(string? departmentId);

    IQueryable<Timesheet> Build();
}

internal class TimesheetQueryBuilder(ITimeTrackDbContext timeTrackDbContext) : ITimesheetQueryBuilder
{
    private int? _startYear;
    private int? _endYear;
    private IReadOnlyCollection<int>? _projects;
    private IReadOnlyCollection<int>? _clients;
    private bool? _isBillable;
    private string? _departmentId;

    public ITimesheetQueryBuilder WithReportYearFilter(int? startYear, int? endYear)
    {
        _startYear = startYear;
        _endYear = endYear;
        return this;
    }

    public ITimesheetQueryBuilder HasAnyProjects(IReadOnlyCollection<int>? projects)
    {
        _projects = projects;
        return this;
    }

    public ITimesheetQueryBuilder HasAnyClients(IReadOnlyCollection<int>? clients)
    {
        _clients = clients;
        return this;
    }

    public ITimesheetQueryBuilder WithBillableFilter(bool? isBillable)
    {
        _isBillable = isBillable;
        return this;
    }

    public ITimesheetQueryBuilder WithDepartmentId(string? departmentId)
    {
        _departmentId = departmentId;
        return this;
    }

    public IQueryable<Timesheet> Build()
    {
        var query = timeTrackDbContext.Timesheets
            .AsNoTracking()
            .Where(t => t.User!.DepartmentId != null);

        query = FilterByParams(query);

        return query;
    }

    private IQueryable<Timesheet> FilterByParams(IQueryable<Timesheet> timesheets)
    {
        if (_startYear.HasValue && _endYear.HasValue)
        {
            timesheets = timesheets.Where(x => x.ReportYear >= _startYear.Value && x.ReportYear <= _endYear.Value);
        }

        if (_projects is not null && _projects.Count is not 0)
        {
            timesheets = timesheets.Where(x => _projects.Any(id => id == x.ProjectId));
        }

        if (_clients is not null && _clients.Count is not 0)
        {
            timesheets = timesheets.Where(x => _clients.Any(id => id == x.Project!.ClientId));
        }

        if (_isBillable.HasValue)
        {
            timesheets = timesheets.Where(x => _isBillable.Value ?
                                                        x.BillableTrackedInMinutes > 0 :
                                                        x.BillableTrackedInMinutes == 0);
        }

        if (_departmentId is not null)
        {
            timesheets = timesheets.Where(x => x.User!.DepartmentId == _departmentId);
        }

        return timesheets;
    }
}
