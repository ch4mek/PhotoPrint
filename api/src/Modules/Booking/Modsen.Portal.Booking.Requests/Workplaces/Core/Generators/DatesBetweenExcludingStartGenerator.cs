namespace Modsen.Portal.Booking.Requests.Workplaces.Core.Generators;

internal interface IDatesBetweenExcludingStartGenerator
{
    IEnumerable<DateTime> Generate(DateTime startDate, DateTime finishDate);
}

internal class DatesBetweenExcludingStartGenerator : IDatesBetweenExcludingStartGenerator
{
    public IEnumerable<DateTime> Generate(DateTime startDate, DateTime finishDate)
    {
        if (startDate.Date > finishDate.Date)
        {
            throw new ArgumentException("startDate grater than finishDate", nameof(startDate));
        }

        return GenerateEnumerator(startDate.AddDays(1), finishDate);
    }

    private static IEnumerable<DateTime> GenerateEnumerator(DateTime startDate, DateTime finishDate)
    {
        var currentDate = startDate;
        while (currentDate <= finishDate.Date)
        {
            yield return currentDate;

            currentDate = currentDate.AddDays(1);
        }
    }
}
