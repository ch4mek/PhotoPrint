using Modsen.Portal.Core;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.TimeTrack.UseCases.Reports.Requests;
using Modsen.Portal.TimeTrack.UseCases.Services.Contracts;

namespace Modsen.Portal.TimeTrack.UseCases.Services;

internal class PeriodService : IPeriodService
{
    public bool IsFilterForEntireYear(IPeriodRequest period)
    {
        return period is
        {
            Year: not null,
            MonthOfYear.Length: 0,
            WeekOfYear: null
        };
    }

    public bool IsFilterForSelectedQuarter(IPeriodRequest period)
    {
        return period is
        {
            Year: not null,
            MonthOfYear.Length: PortalSharedConstants.CountOfMonthInQuarter
        }
        && period.MonthOfYear.IsConsecutive();
    }

    public bool IsFilterForOneSelectedMonth(IPeriodRequest period)
    {
        return period is
        {
            Year: not null,
            MonthOfYear.Length: 1
        };
    }
}
