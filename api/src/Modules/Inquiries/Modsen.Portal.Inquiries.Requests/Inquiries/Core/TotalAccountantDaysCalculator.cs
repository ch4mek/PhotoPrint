using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.CoreDto;

namespace Modsen.Portal.Inquiries.Requests.Inquiries.Core;

internal interface ITotalAccountantDaysCalculator
{
    TotalAccoundantDaysCoreDto Calculate(
        bool includeHolidays,
        bool includeWeekends,
        List<(DateType DateType, DateOnly Date)> inquiryDays);
}

internal class TotalAccountantDaysCalculator : ITotalAccountantDaysCalculator
{
    public TotalAccoundantDaysCoreDto Calculate(
        bool includeHolidays,
        bool includeWeekends,
        List<(DateType DateType, DateOnly Date)> inquiryDays)
    {
        var totalAccountantDays = inquiryDays.Count;

        if (!includeWeekends)
        {
            totalAccountantDays -= inquiryDays.Count(x =>
                x.DateType is DateType.DayOff or DateType.Saturday or DateType.Sunday);

            if (includeHolidays)
            {
                //if we include holidays, day was a saturday or sunday but became holiday, subtract this day as well
                totalAccountantDays -= inquiryDays.Count(x =>
                    x.Date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday && x.DateType is DateType.Holiday);
            }
        }

        if (!includeHolidays)
        {
            totalAccountantDays -= inquiryDays.Count(x => x.DateType == DateType.Holiday);
        }

        return new TotalAccoundantDaysCoreDto(
            TotalDays: inquiryDays.Count,
            TotalAccoundantDays: totalAccountantDays);
    }
}
