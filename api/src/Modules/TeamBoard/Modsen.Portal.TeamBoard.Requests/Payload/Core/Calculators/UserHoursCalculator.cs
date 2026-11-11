using Modsen.Portal.Identity.Contract.Models;
using Modsen.Portal.TeamBoard.Requests.Payload.Core.CoreDto;

namespace Modsen.Portal.TeamBoard.Requests.Payload.Core.Calculators;

internal interface IUserHoursCalculator
{
    PayloadUserHoursCoreDto CalculateUserHours(UserHourCalculatorDto dto);
}

internal sealed class UserHoursCalculator : IUserHoursCalculator
{
    private readonly IWorkingDaysCalculator _workingDaysCalculator;

    public UserHoursCalculator( IWorkingDaysCalculator workingDaysCalculator)
    {
        _workingDaysCalculator = workingDaysCalculator;
    }

    public PayloadUserHoursCoreDto CalculateUserHours(UserHourCalculatorDto dto)
    {
        var fte = GetFteInDateRanges(dto.Year, dto.Month, dto.PlannedFte, dto.DateRangeDtos);

        return new PayloadUserHoursCoreDto
        {
            MonthOfYear = dto.Month,
            ProjectDetails = dto.PayloadUserProjectDetails.ToArray(),
            RequiredHours = Round(dto.PayloadUserProjectDetails.Sum(x => x.RequiredHours)),
            PlannedWorkloadInHours = dto.WorkingHoursPerMonth * fte,
            PlannedFte = fte,
            ActualFte = dto.PayloadUserProjectDetails.Sum(x => x.ActualFte),
            TrackedBillableInHours = Round(dto.PayloadUserProjectDetails.Sum(x => x.TrackedBillableInHours)),
            TotalUserTime = dto.TotalUserTimeByMonths[dto.Month],
            HasActivity = dto.ActivityMonths.Contains(dto.Month)
        };
    }

    public decimal GetFteInDateRanges(
        int year,
        int month,
        decimal plannedFte, 
        IList<DateRangeDto>? ranges)
    {
        if (ranges == null)
        {
            return plannedFte;
        }

        var monthStart = new DateOnly(year, month, 1);
        var monthEnd = new DateOnly(year, month, DateTime.DaysInMonth(year, month));

        if (ranges.Count == 1)
        {
            var r = ranges[0];

            if (r.Start <= monthStart && r.End >= monthEnd)
            {
                return plannedFte;
            }

            return CalculatePartial(monthStart, monthEnd, plannedFte, r);
        }

        return CalculateMultipleRanges(monthStart, monthEnd, plannedFte, ranges);
    }

    private decimal CalculatePartial(
        DateOnly start,
        DateOnly end,
        decimal plannedFte,
        DateRangeDto r)
    {
        var included = 0;
        var total = 0;

        for (var date = start; date <= end; date = date.AddDays(1))
        {
            if (_workingDaysCalculator.IsWorkingDay(date))
            {
                total++;

                if (date >= r.Start && date <= r.End)
                {
                    included++;
                }
            }
        }

        return total == 0 ? 0 : plannedFte * included / total;
    }


    private decimal CalculateMultipleRanges(
        DateOnly start,
        DateOnly end,
        decimal plannedFte,
        IList<DateRangeDto> ranges)
    {
        var included = 0;
        var total = 0;

        for (var date = start; date <= end; date = date.AddDays(1))
        {
            if (_workingDaysCalculator.IsWorkingDay(date))
            {
                total++;

                for (var i = 0; i < ranges.Count; i++)
                {
                    var r = ranges[i];
                    if (r.Start <= date && r.End >= date)
                    {
                        included++;
                        break;
                    }
                }
            }
        }

        return total == 0 ? 0 : plannedFte * included / total;
    }

    private static decimal Round(decimal value)
    {
        return Math.Round(value, 2);
    }
}

public record UserHourCalculatorDto(
    ICollection<PayloadUserProjectHoursCoreDto> PayloadUserProjectDetails,
    int Month,
    int Year,
    ICollection<int> ActivityMonths,
    decimal PlannedFte,
    int WorkingHoursPerMonth,
    Dictionary<int, int> TotalUserTimeByMonths,
    IList<DateRangeDto>? DateRangeDtos);
