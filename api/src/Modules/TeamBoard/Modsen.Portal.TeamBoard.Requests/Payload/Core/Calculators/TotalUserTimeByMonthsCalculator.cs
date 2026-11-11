using Modsen.Portal.Identity.Contract.Models;
using Modsen.Portal.TeamBoard.Requests.Payload.Core.CoreDto;

namespace Modsen.Portal.TeamBoard.Requests.Payload.Core.Calculators;

internal interface ITotalUserTimeByMonthsCalculator
{
    Dictionary<int, int> Calculate(TotalUserTimeByMonthsCalculatorRequest request);
}

internal sealed class TotalUserTimeByMonthsCalculator : ITotalUserTimeByMonthsCalculator
{
    private readonly IWorkingDaysCalculator _workingDaysCalculator;

    public TotalUserTimeByMonthsCalculator(IWorkingDaysCalculator workingDaysCalculator)
    {
        _workingDaysCalculator = workingDaysCalculator;
    }

    public Dictionary<int, int> Calculate(TotalUserTimeByMonthsCalculatorRequest request)
    {
        var contractDates = new List<ContractDatesDto>(request.ContractDates);
        if (!request.IsUserHasContracts)
        {
            var fromDate = request.HireDate.HasValue
                ? GetDate(request.HireDate.Value)
                : request.CareerStartedAt;

            contractDates.Add(new()
            {
                ContractFrom = fromDate,
                ContractTo = null,
                FireDate = null,
            });
        }

        return CalculateByContracts(contractDates, request.Year, request.Month, request.WorkingHoursPerMonth, request.CareerStartedAt, request.DepartmentRanges);

        DateOnly GetDate(DateOnly hireDate)
        {
            return hireDate < request.CareerStartedAt ? request.CareerStartedAt : hireDate;
        }
    }

    private Dictionary<int, int> CalculateByContracts(
        List<ContractDatesDto> contractDates,
        int year, int month, int hoursPerMonth,
        DateOnly careerStartedAt,
        DateRangeDto[]? departmentRanges)
    {
        var workingDaysByMonth = Enumerable.Range(1, 12)
            .ToDictionary(m => m, _ => 0);

        foreach (var contract in contractDates)
        {
            var (start, end) = NormalizePeriod(year, careerStartedAt, contract);

            for (var date = start; date <= end; date = date.AddDays(1))
            {
                if (departmentRanges != null && !IsInDepartment(date, departmentRanges))
                {
                    continue;
                }

                if (!_workingDaysCalculator.IsWorkingDay(date))
                {
                    continue;
                }

                workingDaysByMonth[date.Month]++;
            }
        }

        var result = new Dictionary<int, int>();

        for (var m = 1; m <= 12; m++)
        {
            var sUser = workingDaysByMonth[m];

            if (sUser == 0)
            {
                result[m] = 0;
                continue;
            }

            var totalWorkingDaysInMonth = _workingDaysCalculator
                .GetWorkingDays(year, m, 1, DateTime.DaysInMonth(year, m));

            result[m] = sUser == totalWorkingDaysInMonth
                ? (int)Math.Round(hoursPerMonth * (double)sUser / totalWorkingDaysInMonth)
                : sUser * 8;
        }

        return result;
    }

    private static (DateOnly startDate, DateOnly? endDate) NormalizePeriod(int year, DateOnly careerStartedAt, ContractDatesDto contract)
    {
        var endDate = contract.FireDate ?? contract.ContractTo;
        if (endDate is null || endDate.Value.Year > year)
        {
            endDate = new DateOnly(year, 12, DateTime.DaysInMonth(year, 12));
        }

        var startDate = contract.ContractFrom.Year < year ? new DateOnly(year, 1, 1) : contract.ContractFrom;
        if (startDate < careerStartedAt)
        {
            startDate = careerStartedAt;
        }

        return (startDate, endDate);
    }

    private static bool IsInDepartment(DateOnly date, DateRangeDto[] ranges)
    {
        for (var i = 0; i < ranges.Length; i++)
        {
            var r = ranges[i];
            if (r.Start <= date && r.End >= date)
            {
                return true;
            }
        }

        return false;
    }
}

internal sealed record TotalUserTimeByMonthsCalculatorRequest(
    List<ContractDatesDto> ContractDates,
    bool IsUserHasContracts,
    DateOnly? HireDate,
    int Year,
    int Month,
    int WorkingHoursPerMonth,
    DateOnly CareerStartedAt,
    DateRangeDto[]? DepartmentRanges);
