
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.Domain;
using Modsen.Portal.TimeTrack.UseCases.Dashboard.Core.Builders;
using Modsen.Portal.TimeTrack.UseCases.Dashboard.Core.CoreDto;

namespace Modsen.Portal.TimeTrack.UseCases.Dashboard.Core.Loaders
{
    internal interface IDashboardUsersLoader
    {
        Task<DashboardUsersCoreDto> LoadAsync(
            int pageNo,
            int rowsCount,
            int year,
            int[] monthOfYear,
            string? accessedDepartmentId, 
            int? groupManagersResourceGroupId, 
            CancellationToken cancellationToken);
    }

    internal class DashboardUsersLoader : IDashboardUsersLoader
    {
        private readonly ITimesheetQueryBuilder _timesheetQueryBuilder;
        private readonly ITimeTrackDbContext _timeTrackDbContext;

        public DashboardUsersLoader(ITimesheetQueryBuilder timesheetQueryBuilder, ITimeTrackDbContext timeTrackDbContext)
        {
            _timesheetQueryBuilder = timesheetQueryBuilder;
            _timeTrackDbContext = timeTrackDbContext;
        }

        public async Task<DashboardUsersCoreDto> LoadAsync(
            int pageNo,
            int rowsCount,
            int year,
            int[] monthOfYear,
            string? accessedDepartmentId,
            int? groupManagersResourceGroupId,
            CancellationToken cancellationToken)
        {
            var query = _timesheetQueryBuilder
                       .FromAsNoTracked(year, monthOfYear)
                       .WithDepartmentFilter(accessedDepartmentId)
                       .WithResourceGroupFilter(groupManagersResourceGroupId)
                       .WithBillableHoursFilter(false)
                       .Build()
                       .Include(x => x.User);

            var dashboards = query
                    .GroupBy(t => new
                    {
                        t.UserId,
                        t.User!.FirstName,
                        t.User.LastName,
                        t.User.Color,
                        t.User.IsActive
                    })
                    .Select(g => new
                    {
                        g.Key.UserId,
                        g.Key.FirstName,
                        g.Key.LastName,
                        g.Key.Color,
                        TotalTrackedInMinutes = g.Sum(x => x.FactTrackedInMinutes),
                        CountOfProjects = g.Select(x => x.ProjectId).Distinct().Count(),
                        g.Key.IsActive
                    });

            var usersWithoutDashboards = _timeTrackDbContext.TimeTrackUsers
                .AsNoTracking()
                .Where(u => dashboards.All(r => r.UserId != u.UserId))
                .Where(u => accessedDepartmentId == null || u.DepartmentId == accessedDepartmentId)
                .Where(u => groupManagersResourceGroupId == null || u.ResourceGroupId == groupManagersResourceGroupId);

            var voidDashboards = usersWithoutDashboards
                .Select(u => new
                {
                    u.UserId,
                    u.FirstName,
                    u.LastName,
                    u.Color,
                    TotalTrackedInMinutes = 0,
                    CountOfProjects = 0,
                    u.IsActive
                });

            var allDashboards = dashboards.Union(voidDashboards);

            var pagedDashboards = await allDashboards
                .OrderBy(d => d.LastName)
                .ThenBy(d => d.FirstName)
                .Skip((pageNo - 1) * rowsCount)
                .Take(rowsCount)
                .Select(d => new DashboardUser
                {
                    UserId = d.UserId,
                    UserName = $"{d.LastName} {d.FirstName}",
                    Color = d.Color,
                    TotalTrackedInMinutes = d.TotalTrackedInMinutes,
                    CountOfProjects = d.CountOfProjects,
                    IsActive = d.IsActive
                })
                .ToListAsync(cancellationToken);

            return new DashboardUsersCoreDto
            {
                Page = pageNo,
                PageSize = rowsCount,
                TotalCount = await allDashboards.CountAsync(cancellationToken),
                Items = pagedDashboards
            };
        }
    }
}
