using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.TimeTrack;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Contracts;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Projects.GetWithTotalTrackedTime;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.Extensions;

namespace Modsen.Portal.TimeTrack.DataAccess.Npgsql.Projects.DbQueries;

internal class GetProjectsWithTotalTrackedTimeDbQuery : IGetProjectsWithTotalTrackedTimeDbQuery
{
    private readonly ITimeTrackDbContext _timeTrackDbContext;

    public GetProjectsWithTotalTrackedTimeDbQuery(ITimeTrackDbContext trackDbContext)
    {
        _timeTrackDbContext = trackDbContext;
    }

    public async Task<List<ProjectWithTotalTrackedTimeModel>> ExecuteAsync(GetProjectsWithTotalTrackedTimeDbRequest request, CancellationToken cancellationToken)
    {
        var filter = new TimesheetFilter
        {
            ClientId = request.ClientId,
            ProjectId = request.ProjectId,
            DepartmentId = request.DepartmentId,
            UserId = request.UserId,
            TrackedTimeType = request.TrackedTimeType,
            ResourceGroupId = request.ResourceGroupId
        };

        var query = _timeTrackDbContext.Timesheets
            .AsNoTracking()
            .FilterByPeriod(request.Year, request.MonthOfYear, request.WeekOfYear)
            .ApplyTimesheetFilter(filter)
            .GroupBy(t => new
            {
                t.ProjectId,
                ProjectName = t.Project!.Name,
                ProjectColor = t.Project.Color
            })
            .Select(group => new ProjectWithTotalTrackedTimeModel
            {
                Name = group.Key.ProjectName,
                Color = group.Key.ProjectColor,
                TotalTimeInMinutes = group.Sum(x => request.TrackedTimeType == TrackedTimeType.Billable
                    ? x.BillableTrackedInMinutes
                    : x.FactTrackedInMinutes)
            })
            .OrderBy(c => c.Name);

        var projects = await query.ToListAsync(cancellationToken);

        return projects;
    }
}
