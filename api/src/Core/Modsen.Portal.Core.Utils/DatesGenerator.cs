namespace Modsen.Portal.Core.Utils;

public static class DatesGenerator
{
    public static IEnumerable<DateTime> GetDatesRangeTillDate(DateTime fromDate, DateTime toDate, int step = 1)
    {
        if (fromDate.Date > toDate.Date)
        {
            throw new ArgumentOutOfRangeException(nameof(fromDate), "fromDate grater than toDate");
        }

        if (step < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(step), "step cant be less than 1");
        }

        return Generate();

        IEnumerable<DateTime> Generate()
        {
            do
            {
                yield return fromDate.Date;

                fromDate = fromDate.AddDays(step);
            } while (fromDate.Date <= toDate.Date);
        }
    }

    public static IEnumerable<DateOnly> GetDatesRangeTillDate(DateOnly fromDate, DateOnly toDate, int step = 1)
    {
        if (fromDate > toDate)
        {
            throw new ArgumentOutOfRangeException(nameof(fromDate), "fromDate grater than toDate");
        }

        if (step < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(step), "step cant be less than 1");
        }

        return Generate();

        IEnumerable<DateOnly> Generate()
        {
            do
            {
                yield return fromDate;

                fromDate = fromDate.AddDays(step);
            } while (fromDate <= toDate);
        }
    }
}
