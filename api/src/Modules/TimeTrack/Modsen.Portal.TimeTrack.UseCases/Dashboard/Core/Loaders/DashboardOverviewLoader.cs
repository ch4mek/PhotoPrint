using Microsoft.EntityFrameworkCore;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TimeTrack.UseCases.Dashboard.Core.Builders;
using Modsen.Portal.TimeTrack.UseCases.Dashboard.Core.CoreDto;

namespace Modsen.Portal.TimeTrack.UseCases.Dashboard.Core.Loaders
{
    internal interface IDashboardOverviewLoader
    {
        Task<DashboardOverviewCoreDto> LoadAsync(
            int year,
            int[] monthOfYear,
            int? weekOfYear,
            string? departmentId,
            int? resourceGroupId,
            CancellationToken token);
    }
    internal class DashboardOverviewLoader : IDashboardOverviewLoader
    {
        private readonly ITimesheetQueryBuilder _builder;
        private readonly ITimeTrackDbContext _dbContext;

        public DashboardOverviewLoader(
            ITimesheetQueryBuilder builder,
            ITimeTrackDbContext dbContext)
        {
            _builder = builder;
            _dbContext = dbContext;
        }

        public async Task<DashboardOverviewCoreDto> LoadAsync(
            int year, int[] monthOfYear, int? weekOfYear, string? departmentId, int? resourceGroupId, CancellationToken token)
        {
            var query = _builder
                .FromAsNoTracked(year, monthOfYear)
                .WithWeekOfYear(weekOfYear)
                .WithDepartmentFilter(departmentId)
                .WithResourceGroupFilter(resourceGroupId)
                .Build();

            var topBillableProjectId = await GetTopBillableProjectId(query, token);
            var topFactProjectId = await GetTopFactProjectId(query, token);

            var topBillableClientId = await GetTopBillableClientId(query, token);
            var topFactClientId = await GetTopFactClientId(query, token);

            return new DashboardOverviewCoreDto
            {
                TotalBillableTime = await GetBillableTimeInMinutes(query, token),
                TotalFactTime = await GetFactTimeInMinutes(query, token),
                TopBillableProject = await GetProjectName(topBillableProjectId, token) ?? "There is no top project",
                TopFactProject = await GetProjectName(topFactProjectId, token) ?? "There is no top project",
                TopBillableClient = await GetClientName(topBillableClientId, token) ?? "There is no top client",
                TopFactClient = await GetClientName(topFactClientId, token) ?? "There is no top client",
                TopBillableClientId = topBillableClientId,
                TopFactClientId = topFactClientId,
                TopBillableProjectId = topBillableProjectId,
                TopFactProjectId = topFactProjectId
            };
        }

        private static Task<int> GetTopBillableProjectId(IQueryable<Timesheet> query, CancellationToken token)
        {
            return query
                .GroupBy(x => x.ProjectId)
                .Select(x => new
                {
                    ProjectId = x.Key,
                    TrackedTime = x.Sum(f => f.BillableTrackedInMinutes)
                })
                .OrderByDescending(x => x.TrackedTime)
                .Select(x => x.ProjectId)
                .FirstOrDefaultAsync(token);
        }

        private static Task<int> GetTopFactProjectId(IQueryable<Timesheet> query, CancellationToken token)
        {
            return query
                .GroupBy(x => x.ProjectId)
                .Select(x => new
                {
                    ProjectId = x.Key,
                    TrackedTime = x.Sum(f => f.FactTrackedInMinutes)
                })
                .OrderByDescending(x => x.TrackedTime)
                .Select(x => x.ProjectId)
                .FirstOrDefaultAsync(token);
        }

        private static Task<int> GetTopBillableClientId(IQueryable<Timesheet> query, CancellationToken token)
        {
            return query
                .GroupBy(x => x.Project!.ClientId)
                .Select(x => new
                {
                    ClientId = x.Key,
                    TrackedTime = x.Sum(f => f.BillableTrackedInMinutes)
                })
                .OrderByDescending(x => x.TrackedTime)
                .Select(x => x.ClientId)
                .FirstOrDefaultAsync(token);
        }

        private static Task<int> GetTopFactClientId(IQueryable<Timesheet> query, CancellationToken token)
        {
            return query
                .GroupBy(x => x.Project!.ClientId)
                .Select(x => new
                {
                    ClientId = x.Key,
                    TrackedTime = x.Sum(f => f.FactTrackedInMinutes)
                })
                .OrderByDescending(x => x.TrackedTime)
                .Select(x => x.ClientId)
                .FirstOrDefaultAsync(token);
        }

        private Task<string?> GetProjectName(int projectId, CancellationToken token)
        {
            return _dbContext.Projects
                .Where(x => x.Id == projectId)
                .Select(x => x.Name)
                .FirstOrDefaultAsync(token);
        }

        private Task<string?> GetClientName(int clientId, CancellationToken token)
        {
            return _dbContext.Clients
                .Where(x => x.Id == clientId)
                .Select(x => x.Name)
                .FirstOrDefaultAsync(token);
        }

        private static Task<int> GetBillableTimeInMinutes(IQueryable<Timesheet> query, CancellationToken token)
        {
            return query
                .Select(x => x.BillableTrackedInMinutes)
                .SumAsync(token);
        }

        private static Task<int> GetFactTimeInMinutes(IQueryable<Timesheet> query, CancellationToken token)
        {
            return query
                .Select(x => x.FactTrackedInMinutes)
                .SumAsync(token);
        }

    }
}
