using Modsen.Portal.Identity.Contract.Models;
using Modsen.Portal.TeamBoard.Requests.Payload.Core.CoreDto;

namespace Modsen.Portal.TeamBoard.Requests.Payload.Core.Calculators;

internal interface IUserActivityMonthsCalculator
{
    public List<int> Calculate(ICollection<ContractDatesDto> contractDates,
        DateOnly? hireDate, DateOnly careerStartedAt, int year, DateRangeDto[]? dateRangeDtos);
}

internal class UserActivityMonthsCalculator : IUserActivityMonthsCalculator
{
    public List<int> Calculate(ICollection<ContractDatesDto> contractDates,
        DateOnly? hireDate, DateOnly careerStartedAt, int year, DateRangeDto[]? dateRangeDtos)
    {
        var activityStart = GetActivityStart(hireDate, careerStartedAt);
        var yearStart = new DateOnly(year, 1, 1);
        var yearEnd = new DateOnly(year, 12, 31);

        if (contractDates.Count == 0)
        {
            return CalculateForNonContract(activityStart, yearEnd, yearStart, dateRangeDtos);
        }

        return CalculateForContract(contractDates, activityStart, yearEnd, yearStart, dateRangeDtos);
    }

    private static List<int> CalculateForContract(ICollection<ContractDatesDto> contractDates, DateOnly activityStart, DateOnly yearEnd,
        DateOnly yearStart, DateRangeDto[]? ranges)
    {
        var activeMonths = new HashSet<DateOnly>();
        foreach (var contract in contractDates)
        {
            var start = contract.ContractFrom < activityStart ? activityStart : contract.ContractFrom;
            var end = contract.FireDate ?? contract.ContractTo ?? yearEnd;

            if (end < yearStart || start > yearEnd)
            {
                continue;
            }

            if (start < yearStart)
            {
                start = yearStart;
            }

            if (end > yearEnd)
            {
                end = yearEnd;
            }

            var current = new DateOnly(start.Year, start.Month, 1);
            var last = new DateOnly(end.Year, end.Month, 1);

            while (current <= last)
            {
                var monthStart = current;
                var monthEnd = new DateOnly(current.Year, current.Month, DateTime.DaysInMonth(current.Year, current.Month));

                if (ranges == null || MonthInRanges(monthStart, monthEnd, ranges))
                {
                    activeMonths.Add(current);
                }
                current = current.AddMonths(1);
            }
        }

        return activeMonths.OrderBy(d => d).Select(d => d.Month).ToList();
    }

    private static List<int> CalculateForNonContract(DateOnly activityStart, DateOnly yearEnd, DateOnly yearStart, DateRangeDto[]? ranges)
    {
        var activeMonths = new HashSet<DateOnly>();
        if (activityStart <= yearEnd)
        {
            var start = activityStart < yearStart ? yearStart : activityStart;
            var end = yearEnd;

            var current = new DateOnly(start.Year, start.Month, 1);
            var last = new DateOnly(end.Year, end.Month, 1);

            while (current <= last)
            {
                var monthStart = current;
                var monthEnd = new DateOnly(current.Year, current.Month, DateTime.DaysInMonth(current.Year, current.Month));

                if (ranges == null || MonthInRanges(monthStart, monthEnd, ranges))
                {
                    activeMonths.Add(current);
                }

                current = current.AddMonths(1);
            }
        }

        return activeMonths.OrderBy(d => d).Select(d => d.Month).ToList();
    }

    private static DateOnly GetActivityStart(DateOnly? hireDate, DateOnly careerStartedAt)
    {
        if (!hireDate.HasValue)
        {
            return careerStartedAt;
        }

        return hireDate.Value < careerStartedAt
            ? careerStartedAt
            : hireDate.Value;
    }

    private static bool MonthInRanges(DateOnly monthStart, DateOnly monthEnd, DateRangeDto[] ranges)
    {
        for (var i = 0; i < ranges.Length; i++)
        {
            var r = ranges[i];

            if (r.Start <= monthEnd && r.End >= monthStart)
            {
                return true;
            }
        }

        return false;
    }
}
