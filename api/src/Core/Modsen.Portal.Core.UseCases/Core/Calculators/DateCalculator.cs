namespace Modsen.Portal.Core.UseCases.Core.Calculators;

public interface IDateCalculator
{
    public bool IsWholeMonthsPassed(DateOnly fromDate, DateOnly toDate);

    public bool IsWholeYearsPassed(DateOnly fromDate, DateOnly toDate);

    public bool AreDatesSame(DateOnly fromDate, DateOnly toDate);

    public IReadOnlyCollection<DateOnly> CalculatePassedMonthsDates(DateOnly fromDate, DateOnly toDate);

    public IReadOnlyCollection<DateOnly> CalculatePassedYears(DateOnly fromDate, DateOnly toDate);

    public int GetWholeYears(DateOnly fromDate, DateOnly toDate);
}

public sealed class DateCalculator : IDateCalculator
{
    public int GetWholeYears(DateOnly fromDate, DateOnly toDate)
    {
        var years = 0;

        for (var date = fromDate.AddYears(1); date <= toDate; date = date.AddYears(1))
        {
            years++;
        }

        return years;
    }

    public bool IsWholeMonthsPassed(DateOnly fromDate, DateOnly toDate)
    {
        var monthsPassed = (toDate.Year - fromDate.Year) * 12 + toDate.Month - fromDate.Month;

        if (monthsPassed == 0)
        {
            return false;
        }

        var isDateFromLastDayOfMonth = fromDate.Day == DateTime.DaysInMonth(fromDate.Year, fromDate.Month);
        var isDateToLastDayOfMonth = toDate.Day == DateTime.DaysInMonth(toDate.Year, toDate.Month);

        if (isDateFromLastDayOfMonth)
        {
            return isDateToLastDayOfMonth;
        }

        return (isDateToLastDayOfMonth && toDate.Day < fromDate.Day) || fromDate.AddMonths(monthsPassed) == toDate;
    }

    public bool IsWholeYearsPassed(DateOnly fromDate, DateOnly toDate)
    {
        var yearsPassed = toDate.Year - fromDate.Year;

        if (yearsPassed == 0)
        {
            return false;
        }

        return fromDate.AddYears(yearsPassed) == toDate;
    }

    public bool AreDatesSame(DateOnly fromDate, DateOnly toDate)
    {
        return fromDate == toDate;
    }

    public IReadOnlyCollection<DateOnly> CalculatePassedMonthsDates(DateOnly fromDate, DateOnly toDate)
    {
        var dates = new List<DateOnly>();
        var month = 0;

        while (fromDate.AddMonths(month) <= toDate)
        {
            dates.Add(fromDate.AddMonths(month));
            month++;
        }

        return dates;
    }

    public IReadOnlyCollection<DateOnly> CalculatePassedYears(DateOnly fromDate, DateOnly toDate)
    {
        var dates = new List<DateOnly>();

        while (fromDate <= toDate)
        {
            dates.Add(fromDate);
            fromDate = fromDate.AddYears(1);
        }

        return dates;
    }
}
