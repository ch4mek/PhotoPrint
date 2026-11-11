using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.TimeTrack;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Contracts;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.Extensions;
using Modsen.Portal.TimeTrack.UseCases.Reports.Core.Dto;

namespace Modsen.Portal.TimeTrack.UseCases.Reports.Core
{
    internal interface IProjectsWithTotalTrackedTimeResolver
    {
        Task<List<ProjectWithTotalTrackedTimeCoreDto>> ResolveAsync(
            ProjectsWithTotalTrackedTimeResolverRequest request,
            CancellationToken cancellationToken);
    }

    internal class ProjectsWithTotalTrackedTimeResolver : IProjectsWithTotalTrackedTimeResolver
    {
        private readonly ITimeTrackDbContext _timeTrackDbContext;
        public ProjectsWithTotalTrackedTimeResolver(
            ITimeTrackDbContext timeTrackDbContext)
        {
            _timeTrackDbContext = timeTrackDbContext;
        }

        public async Task<List<ProjectWithTotalTrackedTimeCoreDto>> ResolveAsync(ProjectsWithTotalTrackedTimeResolverRequest request, CancellationToken cancellationToken)
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
                .Select(group => new ProjectWithTotalTrackedTimeCoreDto
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
    public record ProjectsWithTotalTrackedTimeResolverRequest(
    int? Year,
    IReadOnlyCollection<int>? MonthOfYear,
    int? WeekOfYear,
    string? DepartmentId,
    int? ClientId,
    int? ProjectId,
    int? UserId,
    int? ResourceGroupId,
    TrackedTimeType TrackedTimeType);
}
