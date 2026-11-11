using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TimeTrack.UseCases.Dashboard.Core.Builders;
using Modsen.Portal.TimeTrack.UseCases.Dashboard.Core.CoreDto;

namespace Modsen.Portal.TimeTrack.UseCases.Dashboard.Requests;

internal sealed class GetDashboardCurrentUserAllTrackedHoursRequest
    : IQueryHandler<GetDashboardCurrentUserAllTrackedHoursRequest.Query, DashboardCurrentUserAllTrackedHoursCoreDto>
{
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly IClockService _clockService;
    private readonly ITimesheetQueryBuilder _timesheetQueryBuilder;
    public GetDashboardCurrentUserAllTrackedHoursRequest(
        IAuthenticatedUserContext authenticatedUserContext,
        IClockService clockService,
        ITimesheetQueryBuilder timesheetQueryBuilder)
    {
        _authenticatedUserContext = authenticatedUserContext;
        _clockService = clockService;
        _timesheetQueryBuilder = timesheetQueryBuilder;
    }

    public async Task<DashboardCurrentUserAllTrackedHoursCoreDto> Handle(Query request, CancellationToken cancellationToken)
    {
        var currentDateTime = _clockService.UtcNow;

        var date = DateOnly.FromDateTime(currentDateTime);
        var trackedHours = await _timesheetQueryBuilder
            .FromAsNoTracked(date.Year, [date.Month])
            .WithUserFilter(_authenticatedUserContext.GetRequiredUserId())
            .Build()
            .GroupBy(timesheet => timesheet.ReportForDate)
            .OrderBy(groupingResult => groupingResult.Key)
            .Select(orderResult =>
                new DateAndTotalTrackedTimeInMinutesCoreDto(orderResult.Key, orderResult.Sum(e => e.FactTrackedInMinutes))
            )
            .ToListAsync(cancellationToken);

        return new DashboardCurrentUserAllTrackedHoursCoreDto(trackedHours);
    }
    internal sealed record Query : IQuery<DashboardCurrentUserAllTrackedHoursCoreDto>;

}
