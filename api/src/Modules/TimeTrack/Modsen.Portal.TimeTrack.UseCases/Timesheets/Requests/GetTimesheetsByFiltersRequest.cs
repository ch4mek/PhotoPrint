using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.Builders;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Requests.RequestDto;

namespace Modsen.Portal.TimeTrack.UseCases.Timesheets.Requests;

internal sealed class GetTimesheetsByFiltersRequest
    : IQueryHandler<GetTimesheetsByFiltersRequest.Query, List<UsersTimesheetsDto>>
{
    private readonly ITimesheetQueryBuilder _timesheetQueryBuilder;

    public GetTimesheetsByFiltersRequest(ITimesheetQueryBuilder timesheetQueryBuilder)
    {
        _timesheetQueryBuilder = timesheetQueryBuilder;
    }

    public async Task<List<UsersTimesheetsDto>> Handle(Query request, CancellationToken cancellationToken)
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
            .Select(group => new UsersTimesheetsDto
            {
                Duration = group.Sum(timesheet => timesheet.BillableTrackedInMinutes),
                Month = group.Key.ReportMonthOfYear,
                Year = group.Key.ReportYear,
                Users = group
                    .GroupBy(timesheet => new
                    {
                        timesheet.UserId,
                        timesheet.User!.FirstName,
                        timesheet.User.LastName
                    })
                    .Select(user => new TimesheetUserDto
                    {
                        Duration = user.Sum(t => t.BillableTrackedInMinutes),
                        Id = user.Key.UserId,
                        FirstName = user.Key.FirstName,
                        LastName = user.Key.LastName,
                        Count = user.Count()
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
        bool? IsBillable) : IQuery<List<UsersTimesheetsDto>>;
}
