using Modsen.Portal.TeamBoard.Requests.Payload.Core.Calculators;

namespace Modsen.Portal.TeamBoard.UnitTests.Payload.Core.Calculators;

public class WorkingDaysCalculatorTests
{
    private readonly WorkingDaysCalculator _workingDaysCalculator = new();

    [Fact]
    public void HandleGetWorkingDaysInMonthShouldReturnCorrectValue()
    {
        // Arrange
        var year = 2025;
        var expectedResult = new Dictionary<int, int>
        {
            { 1, 23 },
            { 2, 20 },
            { 3, 21 },
            { 4, 22 },
            { 5, 22 },
            { 6, 21 },
            { 7, 23 },
            { 8, 21 },
            { 9, 22 },
            { 10, 23 },
            { 11, 20 },
            { 12, 23 },
        };

        // Act
        var result = _workingDaysCalculator.GetWorkingDaysInMonth(year);

        // Assert
        result.Should().BeEquivalentTo(expectedResult);
    }

    [Theory]
    [InlineData(2025, 1, 1, 31, 23)]
    [InlineData(2025, 1, 3, 4, 1)]
    public void HandleGetWorkingDaysShouldReturnCorrectData(int year, int month, int startDay, int endDay, int expectedResult)
    {
        // Arrange

        // Act
        var result = _workingDaysCalculator.GetWorkingDays(year, month, startDay, endDay);

        // Assert
        result.Should().Be(expectedResult);
    }
}
