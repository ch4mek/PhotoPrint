using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.TimeTrack;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Clients.GetTotalTrackedTime;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Clients.GetWithTotalTrackedTime;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Contracts;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.Extensions;

namespace Modsen.Portal.TimeTrack.DataAccess.Npgsql.Clients;

internal class GetClientsWithTotalTrackedTimeDbQuery : IGetClientsWithTotalTrackedTimeDbQuery
{
    private readonly ITimeTrackDbContext _timeTrackDbContext;

    public GetClientsWithTotalTrackedTimeDbQuery(
        ITimeTrackDbContext timeTrackDbContext)
    {
        _timeTrackDbContext = timeTrackDbContext;
    }

    public async Task<List<ClientWithTotalTrackedTimeModel>> ExecuteAsync(GetClientsWithTotalTrackedTimeDbRequest request, CancellationToken cancellationToken)
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
            .Select(group => new ClientWithTotalTrackedTimeModel
            {
                Name = group.Key.ClientName,
                Color = group.Key.ClientColor,
                TotalTimeInMinutes = group.Sum(x => request.TrackedTimeType == TrackedTimeType.Billable
                    ? x.BillableTrackedInMinutes
                    : x.FactTrackedInMinutes)
            })
            .OrderBy(c => c.Name);

        var clientsWithTotalTime = await query.ToListAsync(cancellationToken);

        return clientsWithTotalTime;
    }
}
