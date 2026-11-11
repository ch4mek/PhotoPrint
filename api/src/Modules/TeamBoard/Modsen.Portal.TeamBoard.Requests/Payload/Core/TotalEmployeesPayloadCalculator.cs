using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Services;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.Requests.Payload.Core.Calculators;
using Modsen.Portal.TeamBoard.Requests.Payload.Core.CoreDto;
using Modsen.Portal.TeamBoard.Requests.Payload.KpiColors.Requests.RequestDto;

namespace Modsen.Portal.TeamBoard.Requests.Payload.Core;

internal interface ITotalEmployeesPayloadCalculator
{
    Task<PayloadTotalHoursCoreDto[]> Calculate(PayloadCoreDto payload, CancellationToken cancellationToken);
}

internal class TotalEmployeesPayloadCalculator : ITotalEmployeesPayloadCalculator
{
    private readonly IClockService _clockService;
    private readonly ITeamBoardDbContext _dbContext;
    private readonly IKpiColorCalculator _kpiColorCalculator;
    public TotalEmployeesPayloadCalculator(IClockService clockService,
        IKpiColorCalculator kpiColorCalculator,
        ITeamBoardDbContext dbContext)
    {
        _kpiColorCalculator = kpiColorCalculator;
        _dbContext = dbContext;
        _clockService = clockService;
    }


    public async Task<PayloadTotalHoursCoreDto[]> Calculate(PayloadCoreDto payload, CancellationToken cancellationToken)
    {
        var currentMonth = _clockService.UtcNow.Month;
        var colorsDbResponse = await _dbContext.KpiColors.ToListAsync(cancellationToken);

        var colors = colorsDbResponse.Select(
            color => new KpiColorDto
            {
                Id = color.Id,
                Name = color.Name,
                RightIntervalBorder = color.RightIntervalBorder
            }).ToList();

        var headTotals = payload.Users
            .Where(user => user.IsHeadOfDepartment)
            .SelectMany(x => x.HourDetails)
            .Where(head => head.HasActivity)
            .GroupBy(x => x.MonthOfYear)
            .Select(group => new KeyValuePair<int, KpiTimesDto>(group.Key, new KpiTimesDto
            {
                ActiveHeadsCount = group.Count(),
                TotalUserTime = group.Sum(x => x.TotalUserTime),
                TrackedBillableTime = group.Sum(x => x.TrackedBillableInHours),
                HeadNotWorkingCount = group.Count(x => x.TotalUserTime == 0),
            }))
            .ToDictionary();

        var totals = payload.Users
            .SelectMany(x => x.HourDetails)
            .Where(x => x.HasActivity)
            .GroupBy(x => x.MonthOfYear)
            .Select(group =>
            {
                var totalPlanned = currentMonth >= group.Key
                    ? group.Sum(x => x.RequiredHours)
                    : group.Sum(x => x.PlannedWorkloadInHours);
                var totalFactBillableInHours = group.Sum(x => x.TrackedBillableInHours);

                var lessKpiTime = new KpiTimesDto();
                if (headTotals.TryGetValue(group.Key, out var value))
                {
                    lessKpiTime = value;
                }

                var totalTimePerUsers = group.Sum(user => user.TotalUserTime) - lessKpiTime.TotalUserTime;

                var kpi = (totalTimePerUsers) == 0
                    ? 0
                    : totalFactBillableInHours / totalTimePerUsers * 100m;

                var color = _kpiColorCalculator.GetKpiColor(kpi, colors);

                var usersCount = group.Count() - lessKpiTime.ActiveHeadsCount;

                if (usersCount < 0)
                {
                    usersCount = 0;
                }

                return new PayloadTotalHoursCoreDto
                {
                    MonthOfYear = group.Key,
                    TotalFactBillableInHours = totalFactBillableInHours,
                    TotalPlannedWorkloadInHours = totalPlanned,
                    TotalActualFte = group.Sum(userHours => userHours.ActualFte),
                    TotalPlannedFte = group.Sum(userHours => userHours.PlannedFte),
                    TotalKpi = Round(kpi),
                    Color = color,
                    TotalTimePerUsers = totalTimePerUsers,
                    UsersCount = usersCount
                };
            })
            .OrderBy(x => x.MonthOfYear)
            .ToArray();

        return totals;
    }

    private static decimal Round(decimal val)
    {
        return Math.Round(val, 2);
    }
}
