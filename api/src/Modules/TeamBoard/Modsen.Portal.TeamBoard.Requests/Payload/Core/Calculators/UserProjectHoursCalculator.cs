using Modsen.Portal.Identity.Contract.Models;
using Modsen.Portal.TeamBoard.Requests.Payload.Core.CoreDto;

namespace Modsen.Portal.TeamBoard.Requests.Payload.Core.Calculators;

internal interface IUserProjectHoursCalculator
{
    PayloadUserProjectHoursCoreDto CalculateUserProjectHoursForSingleAssign(CalculateUserProjectHoursForSingleAssignRequest request);

    PayloadUserProjectHoursCoreDto CalculateUserProjectHoursForGroupAssigns(
        IGrouping<int, (DateOnly UserOnProjectFrom, DateOnly? UserOnProjectTo, PayloadUserProjectHoursCoreDto UserProjectHours)> group,
        int workingHoursInMonth);
}

internal sealed class UserProjectHoursCalculator : IUserProjectHoursCalculator
{
    private readonly IWorkingDaysCalculator _workingDaysCalculator;

    public UserProjectHoursCalculator(
        IWorkingDaysCalculator workingDaysCalculator)
    {
        _workingDaysCalculator = workingDaysCalculator;
    }

    public PayloadUserProjectHoursCoreDto CalculateUserProjectHoursForGroupAssigns(
        IGrouping<int, (DateOnly UserOnProjectFrom, DateOnly? UserOnProjectTo, PayloadUserProjectHoursCoreDto UserProjectHours)> group,
        int workingHoursInMonth)
    {
        var dataByAssign = group
            .Select(g => new
            {
                g.UserProjectHours.RequiredHours,
                g.UserProjectHours.PlannedFte,
                g.UserProjectHours.ActualFte,
                g.UserProjectHours.TrackedBillableInHours,
                g.UserOnProjectFrom,
                g.UserOnProjectTo
            })
            .Distinct()
            .GroupBy(g => new { g.UserOnProjectFrom, g.UserOnProjectTo })
            .Select(g => g.Where(assign => assign.TrackedBillableInHours > 0 || g.Count() == 1))
            .Select(g => new
            {
                RequiredHours = Round(g.Sum(assign => assign.RequiredHours)),
                TrackedBillableInHours = Round(g.Sum(assign => assign.TrackedBillableInHours))
            })
            .ToArray();

        var requiredHours = dataByAssign.Sum(assign => assign.RequiredHours);
        var trackedBillableInHours = dataByAssign.Sum(assign => assign.TrackedBillableInHours);

        return new PayloadUserProjectHoursCoreDto
        {
            ProjectId = group.Key,
            RequiredHours = requiredHours,
            PlannedFte = workingHoursInMonth > 0 ? Round(requiredHours / workingHoursInMonth) : 0m,
            ActualFte = workingHoursInMonth > 0 ? Round(trackedBillableInHours / workingHoursInMonth) : 0m,
            TrackedBillableInHours = trackedBillableInHours,
        };
    }

    public PayloadUserProjectHoursCoreDto CalculateUserProjectHoursForSingleAssign(
        CalculateUserProjectHoursForSingleAssignRequest request)
    {
        var monthStart = new DateOnly(request.Year, request.Month, 1);
        var monthEnd = new DateOnly(request.Year, request.Month, DateTime.DaysInMonth(request.Year, request.Month));

        var start = request.OnProjectFrom > monthStart
            ? request.OnProjectFrom
            : monthStart;
        var end = (request.OnProjectTo ?? monthEnd) < monthEnd
            ? request.OnProjectTo!.Value
            : monthEnd;

        if (start > end)
        {
            return new PayloadUserProjectHoursCoreDto
            {
                ProjectId = request.ProjectId
            };
        }

        var sUser = 0;

        for (var date = start; date <= end; date = date.AddDays(1))
        {
            if (!_workingDaysCalculator.IsWorkingDay(date))
            {
                continue;
            }

            if (request.DateRanges != null && !IsInAnyRange(date, request.DateRanges))
            {
                continue;
            }

            sUser++;
        }

        var s = (decimal)sUser / request.WorkingDaysInMonth;

        var requiredHours = Round(request.WorkingHoursInMonth * request.Fte * s);

        return new PayloadUserProjectHoursCoreDto
        {
            ProjectId = request.ProjectId,
            RequiredHours = requiredHours,
            PlannedFte = request.WorkingHoursInMonth > 0 ? Round(requiredHours / request.WorkingHoursInMonth) : 0m,
            ActualFte = request.WorkingHoursInMonth > 0 ? Round(request.TrackedBillable / request.WorkingHoursInMonth) : 0m,
            TrackedBillableInHours = request.TrackedBillable,
        };
    }

    private static bool IsInAnyRange(DateOnly day, DateRangeDto[] ranges)
    {
        for (var i = 0; i < ranges.Length; i++)
        {
            var r = ranges[i];
            if (r.Start <= day && r.End >= day)
            {
                return true;
            }
        }

        return false;
    }

    private static decimal Round(decimal value)
    {
        return Math.Round(value, 2);
    }
}

internal sealed record CalculateUserProjectHoursForSingleAssignRequest(
    int ProjectId,
    int Year,
    int Month,
    int WorkingDaysInMonth,
    int WorkingHoursInMonth,
    DateOnly OnProjectFrom,
    DateOnly? OnProjectTo,
    decimal Fte,
    decimal TrackedBillable,
    DateRangeDto[]? DateRanges);
