using Modsen.Portal.TeamBoard.Requests.Payload.Core.Calculators;
using Modsen.Portal.TeamBoard.Requests.Payload.Core.CoreDto;

namespace Modsen.Portal.TeamBoard.UnitTests.Payload.Core.Calculators;

public class WorkingHoursInMonthCalculatorTests
{
    private readonly WorkingHoursInMonthCalculator _workingHoursInMonthCalculator = new();

    [Fact]
    public void HandleWhenDateToIsNullShouldReturnCorrectHours()
    {
        // Arrange
        var year = 2025;
        var month = 4;
        var defaultCountryId = 1;

        var registeredLocationHistories = new RegisteredLocationHistoryCoreDto[]
        {
            new()
            {
                RegisteredLocationId = 2,
                DateFrom = new DateOnly(2025, 1, 1),
                DateTo = null
            }
        };

        var calendarCountryHourDetails = new CalendarCountryHourDetailsCoreDto[]
        {
            new()
            {
                CountryId = 2,
                Hours = 10,
            }
        };

        var request = new WorkingHoursInMonthCalculatorRequest(
            registeredLocationHistories,
            calendarCountryHourDetails,
            defaultCountryId,
            year,
            month);

        // Act
        var result = _workingHoursInMonthCalculator.CalculateWorkingHoursInMonth(request);

        // Assert
        result.Should().Be(10);
    }

    [Fact]
    public void HandleWhenDateIsNotInIntervalShouldReturnDefaultHours()
    {
        // Arrange
        var year = 2025;
        var month = 4;
        var defaultCountryId = 1;

        var registeredLocationHistories = new RegisteredLocationHistoryCoreDto[]
        {
            new()
            {
                RegisteredLocationId = 2,
                DateFrom = new DateOnly(2025, 1, 1),
                DateTo = new DateOnly(2025, 3, 1)
            }
        };

        var calendarCountryHourDetails = new CalendarCountryHourDetailsCoreDto[]
        {
            new()
            {
                CountryId = 2,
                Hours = 10,
            },
             new()
            {
                CountryId = 1,
                Hours = 20,
            }
        };

        var request = new WorkingHoursInMonthCalculatorRequest(
            registeredLocationHistories,
            calendarCountryHourDetails,
            defaultCountryId,
            year,
            month);

        // Act
        var result = _workingHoursInMonthCalculator.CalculateWorkingHoursInMonth(request);

        // Assert
        result.Should().Be(20);
    }

    [Fact]
    public void HandleWhenTwoRegistryLocationsInOneIntravalShouldReturnFirstHours()
    {
        // Arrange
        var year = 2025;
        var month = 3;
        var defaultCountryId = 1;

        var registeredLocationHistories = new RegisteredLocationHistoryCoreDto[]
        {
            new()
            {
                RegisteredLocationId = 2,
                DateFrom = new DateOnly(2025, 1, 1),
                DateTo = new DateOnly(2025, 3, 1)
            },
            new()
            {
                RegisteredLocationId = 2,
                DateFrom = new DateOnly(2025, 3, 2),
                DateTo = null
            }
        };

        var calendarCountryHourDetails = new CalendarCountryHourDetailsCoreDto[]
        {
            new()
            {
                CountryId = 2,
                Hours = 10,
            },
             new()
            {
                CountryId = 1,
                Hours = 20,
            }
        };

        var request = new WorkingHoursInMonthCalculatorRequest(
            registeredLocationHistories,
            calendarCountryHourDetails,
            defaultCountryId,
            year,
            month);

        // Act
        var result = _workingHoursInMonthCalculator.CalculateWorkingHoursInMonth(request);

        // Assert
        result.Should().Be(10);
    }
}
