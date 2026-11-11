using Modsen.Portal.Core.Services;
using Modsen.Portal.TimeTrack.UseCases.Dtos.Reports;
using Modsen.Portal.TimeTrack.UseCases.Reports.Requests;
using Modsen.Portal.TimeTrack.UseCases.Services.Contracts;

namespace Modsen.Portal.TimeTrack.UseCases.Services;

internal class DateService : IDateService
{
    private readonly IClockService _clock;
    private readonly IPeriodService _periodService;

    public DateService(IClockService clock, IPeriodService periodService)
    {
        _clock = clock;
        _periodService = periodService;
    }

    public DateSpanDto GetDateSpan(IPeriodRequest request)
    {
        if (_periodService.IsFilterForEntireYear(request))
        {
            return GetYearPeriod(request.Year!.Value);
        }

        if (_periodService.IsFilterForOneSelectedMonth(request)
            || _periodService.IsFilterForSelectedQuarter(request))
        {
            return GetMonthPeriod(request.Year!.Value, request.MonthOfYear);
        }

        return new DateSpanDto
        {
            StartDate = DateTime.MinValue,
            EndDate = _clock.UtcNow.Date
        };
    }

    private static DateSpanDto GetYearPeriod(int year)
    {
        var startDate = new DateTime(
            year, month: 1, day: 1,
            hour: 0, minute: 0, second: 0,
            DateTimeKind.Utc);

        var endDate = new DateTime(
            year, month: 12, day: DateTime.DaysInMonth(year, month: 12),
            hour: 0, minute: 0, second: 0,
            DateTimeKind.Utc);

        return new DateSpanDto
        {
            StartDate = startDate,
            EndDate = endDate
        };
    }

    private static DateSpanDto GetMonthPeriod(int year, int[]? months)
    {
        ArgumentNullException.ThrowIfNull(months);

        var fromMonth = months[0];

        var toMonth = months.Length == 1
            ? months[0]
            : months[^1];

        var startDate = new DateTime(
            year, fromMonth, day: 1,
            hour: 0, minute: 0, second: 0,
            DateTimeKind.Utc);

        var endDate = new DateTime(
            year, toMonth, day: DateTime.DaysInMonth(year, toMonth),
            hour: 0, minute: 0, second: 0,
            DateTimeKind.Utc);

        return new DateSpanDto
        {
            StartDate = startDate,
            EndDate = endDate
        };
    }
}
