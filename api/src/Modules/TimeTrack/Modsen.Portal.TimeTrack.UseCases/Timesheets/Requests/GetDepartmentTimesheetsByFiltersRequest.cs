using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.Builders;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Requests.RequestDto;

namespace Modsen.Portal.TimeTrack.UseCases.Timesheets.Requests;

internal sealed class GetDepartmentTimesheetsByFiltersRequest
    : IQueryHandler<GetDepartmentTimesheetsByFiltersRequest.Query, List<DepartmentTimesheetsDto>>
{
    private readonly ITimesheetQueryBuilder _timesheetQueryBuilder;

    public GetDepartmentTimesheetsByFiltersRequest(ITimesheetQueryBuilder timesheetQueryBuilder)
    {
        _timesheetQueryBuilder = timesheetQueryBuilder;
    }

    public async Task<List<DepartmentTimesheetsDto>> Handle(Query request,
        CancellationToken cancellationToken)
    {
        var timesheets = await _timesheetQueryBuilder
            .WithReportYearFilter(request.StartYear, request.EndYear)
            .WithBillableFilter(request.IsBillable)
            .HasAnyProjects(request.Projects)
            .HasAnyClients(request.Clients)
            .WithDepartmentId(request.DepartmentId)
            .Build()
            .GroupBy(timesheet => new
            {
                timesheet.ReportYear,
                timesheet.ReportMonthOfYear
            })
            .OrderBy(g => g.First().ReportYear)
            .ThenBy(g => g.First().ReportMonthOfYear)
            .Select(group => new DepartmentTimesheetsDto
            {
                Duration = group.Sum(timesheet => timesheet.BillableTrackedInMinutes),
                Month = group.Key.ReportMonthOfYear,
                Year = group.Key.ReportYear,
                Departments = group
                    .GroupBy(timesheet => timesheet.User!.DepartmentId)
                    .Select(department => new TimesheetDepartmentDto
                    {
                        Duration = department.Sum(timesheet => timesheet.BillableTrackedInMinutes),
                        Count = department.Count(),
                        DepartmentId = department.Key!
                    })
                    .ToArray()
            })
            .ToListAsync(cancellationToken);

        return timesheets;
    }

    internal sealed record Query(
        int? StartYear,
        int? EndYear,
        string? DepartmentId,
        IReadOnlyCollection<int>? Projects,
        IReadOnlyCollection<int>? Clients,
        bool? IsBillable) : IQuery<List<DepartmentTimesheetsDto>>;
}
