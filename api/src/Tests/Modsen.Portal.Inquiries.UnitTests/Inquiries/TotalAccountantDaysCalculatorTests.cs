using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.CoreDto;


namespace Modsen.Portal.Inquiries.UnitTests.Inquiries;

public class TotalAccountantDaysCalculatorTests
{
    private readonly ITotalAccountantDaysCalculator _totalAccountantDaysCalculator = new TotalAccountantDaysCalculator();

    private readonly List<(DateType, DateOnly)> _inquiryDates =
    [
        (DateType.WorkingDay, new DateOnly(year: 2025, month: 12, day: 11)),
        (DateType.WorkingDay, new DateOnly(year: 2025, month: 12, day: 12)),
        (DateType.Saturday, new DateOnly(year: 2025, month: 12, day: 13)),
        (DateType.Sunday, new DateOnly(year: 2025, month: 12, day: 14)),
        (DateType.Holiday, new DateOnly(year: 2025, month: 12, day: 15)),
        (DateType.Holiday, new DateOnly(year: 2025, month: 12, day: 20)) //Saturday holiday
    ];

    [Fact]
    public void CalculateTotalAccoundantDaysWithoutHolidaysShouldReturnTotalDaysAndTotalAccoundantDaysWithoutHolidays()
    {
        // Arrange
        var includeHolidays = false;
        var includeWeekends = true;

        var expected = new TotalAccoundantDaysCoreDto(
            TotalDays: 6,
            TotalAccoundantDays: 4);

        // Act
        var days = _totalAccountantDaysCalculator.Calculate(includeHolidays, includeWeekends, _inquiryDates);

        // Assert
        days.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void CalculateTotalAccoundantDaysWithoutWeekendsShouldReturnTotalDaysAndTotalAccoundantDaysWithoutWeekends()
    {
        // Arrange
        var includeHolidays = true;
        var includeWeekends = false;

        var expected = new TotalAccoundantDaysCoreDto(
            TotalDays: 6,
            TotalAccoundantDays: 3);

        // Act
        var days = _totalAccountantDaysCalculator.Calculate(includeHolidays, includeWeekends, _inquiryDates);

        // Assert
        days.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void CalculateTotalAccoundantDaysWithoutWeekendsAndHolidaysShouldReturnTotalDaysAndTotalAccoundantDaysWithoutWeekendsAndHolidays()
    {
        // Arrange
        var includeHolidays = false;
        var includeWeekends = false;

        var expected = new TotalAccoundantDaysCoreDto(
            TotalDays: 6,
            TotalAccoundantDays: 2);

        // Act
        var days = _totalAccountantDaysCalculator.Calculate(includeHolidays, includeWeekends, _inquiryDates);

        // Assert
        days.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void CalculateTotalAccoundantDaysWithWeekendsAndHolidaysShouldReturnTotalDaysAndTotalAccoundantDaysWithWeekendsAndHolidays()
    {
        // Arrange
        var includeHolidays = true;
        var includeWeekends = true;

        var expected = new TotalAccoundantDaysCoreDto(
            TotalDays: 6,
            TotalAccoundantDays: 6);

        // Act
        var days = _totalAccountantDaysCalculator.Calculate(includeHolidays, includeWeekends, _inquiryDates);

        // Assert
        days.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void CalculateTotalAccoundantDaysWithEmptyListShouldReturnTotalDaysAndTotalAccoundantDaysEqualsZero()
    {
        // Arrange
        var includeHolidays = false;
        var includeWeekends = false;

        var expected = new TotalAccoundantDaysCoreDto(
            TotalDays: 0,
            TotalAccoundantDays: 0);

        // Act
        var days = _totalAccountantDaysCalculator.Calculate(includeHolidays, includeWeekends, []);

        // Assert
        days.Should().BeEquivalentTo(expected);
    }
}
