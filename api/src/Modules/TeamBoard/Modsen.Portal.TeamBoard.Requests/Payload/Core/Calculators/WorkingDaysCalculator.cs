namespace Modsen.Portal.TeamBoard.Requests.Payload.Core.Calculators;

internal interface IWorkingDaysCalculator
{
    Dictionary<int, int> GetWorkingDaysInMonth(int year);

    int GetWorkingDays(int year, int month, int startDay, int endDay);

    bool IsWorkingDay(DateOnly day);
}

internal sealed class WorkingDaysCalculator : IWorkingDaysCalculator
{
    public Dictionary<int, int> GetWorkingDaysInMonth(int year)
    {
        var workingDaysInMonth = new Dictionary<int, int>();

        for (var month = 1; month <= 12; month++)
        {
            workingDaysInMonth.Add(month, GetWorkingDays(year, month, 1, DateTime.DaysInMonth(year, month)));
        }

        return workingDaysInMonth;
    }

    public int GetWorkingDays(int year, int month, int startDay, int endDay)
    {
        var workingDays = 0;

        for (var day = new DateOnly(year, month, startDay); day <= new DateOnly(year, month, endDay); day = day.AddDays(1))
        {
            if (IsWorkingDay(day))
            {
                workingDays++;
            }
        }

        return workingDays;
    }

    public bool IsWorkingDay(DateOnly day)
    {
        return day.DayOfWeek != DayOfWeek.Saturday && day.DayOfWeek != DayOfWeek.Sunday;
    }
}
