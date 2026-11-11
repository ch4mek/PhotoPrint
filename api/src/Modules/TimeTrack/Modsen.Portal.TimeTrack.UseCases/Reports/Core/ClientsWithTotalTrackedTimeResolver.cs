using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.TimeTrack;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Contracts;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.Extensions;
using Modsen.Portal.TimeTrack.UseCases.Reports.Core.Dto;

namespace Modsen.Portal.TimeTrack.UseCases.Reports.Core
{
    internal interface IClientsWithTotalTrackedTimeResolver
    {
        Task<List<ClientWithTotalTrackedTimeCoreDto>> ResolveAsync(
            ClientsWithTotalTrackedTimeResolverRequest request,
            CancellationToken cancellationToken);
    }

    internal class ClientsWithTotalTrackedTimeResolver : IClientsWithTotalTrackedTimeResolver
    {
        private readonly ITimeTrackDbContext _timeTrackDbContext;

        public ClientsWithTotalTrackedTimeResolver(
            ITimeTrackDbContext timeTrackDbContext)
        {
            _timeTrackDbContext = timeTrackDbContext;
        }

        public async Task<List<ClientWithTotalTrackedTimeCoreDto>> ResolveAsync(ClientsWithTotalTrackedTimeResolverRequest request, CancellationToken cancellationToken)
        {
            var filter = new TimesheetFilter
            {
                ClientId = request.ClientId,
                ProjectId = request.ProjectId,
                DepartmentId = request.DepartmentId,
                UserId = request.UserId,
                ResourceGroupId = request.ResourceGroupId,
                TrackedTimeType = request.TrackedTimeType
            };

            var query = _timeTrackDbContext.Timesheets
                .AsNoTracking()
                .FilterByPeriod(request.Year, request.MonthOfYear, request.WeekOfYear)
                .ApplyTimesheetFilter(filter)
                .GroupBy(t => new
                {
                    t.Project!.ClientId,
                    ClientName = t.Project.Client!.Name,
                    ClientColor = t.Project.Client.Color
                })
                .Select(group => new ClientWithTotalTrackedTimeCoreDto
                {
                    Name = group.Key.ClientName,
                    Color = group.Key.ClientColor,
                    TotalTimeInMinutes = group.Sum(x => request.TrackedTimeType == TrackedTimeType.Billable
                        ? x.BillableTrackedInMinutes
                        : x.FactTrackedInMinutes)
                })
                .OrderBy(c => c.Name);

            return await query.ToListAsync(cancellationToken);
        }
    }

    public record ClientsWithTotalTrackedTimeResolverRequest(
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
