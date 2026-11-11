using Modsen.Portal.TeamBoard.Requests.Payload.Core.CoreDto;

namespace Modsen.Portal.TeamBoard.Requests.Payload.Core.Calculators;

internal interface IWorkingHoursInMonthCalculator
{
    int CalculateWorkingHoursInMonth(WorkingHoursInMonthCalculatorRequest request);
}

internal sealed class WorkingHoursInMonthCalculator : IWorkingHoursInMonthCalculator
{
    public int CalculateWorkingHoursInMonth(WorkingHoursInMonthCalculatorRequest request)
    {
        var lastDayOfMonth = new DateOnly(request.Year, request.Month, DateTime.DaysInMonth(request.Year, request.Month));

        var registeredLocationId = request.RegisteredLocationHistories
            .Where(x => x.DateFrom <= lastDayOfMonth)
            .Where(x => x.DateTo == null || x.DateTo >= lastDayOfMonth)
            .Select(x => x.RegisteredLocationId)
            .FirstOrDefault();

        var countryCalendar = request.CalendarCountryHourDetails.FirstOrDefault(x => x.CountryId == registeredLocationId);

        if (countryCalendar is null || countryCalendar.Hours <= 0)
        {
            countryCalendar = request.CalendarCountryHourDetails.First(x => x.CountryId == request.DefaultCountryId);
        }

        return countryCalendar.Hours;
    }
}

internal sealed record WorkingHoursInMonthCalculatorRequest(
    IReadOnlyCollection<RegisteredLocationHistoryCoreDto> RegisteredLocationHistories,
    IReadOnlyCollection<CalendarCountryHourDetailsCoreDto> CalendarCountryHourDetails,
    int DefaultCountryId,
    int Year,
    int Month);
