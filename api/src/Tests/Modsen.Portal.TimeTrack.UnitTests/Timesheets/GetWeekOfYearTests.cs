using Modsen.Portal.Core.Utils.Extensions;

namespace Modsen.Portal.TimeTrack.UnitTests.Timesheets;

public class GetWeekOfYearTests
{
    [Theory]
    [InlineData(2023, 1, 1, 1)]
    [InlineData(2023, 1, 2, 2)]
    [InlineData(2023, 12, 31, 53)]
    [InlineData(2020, 12, 31, 53)]
    [InlineData(2024, 2, 29, 9)]
    [InlineData(2023, 6, 15, 25)]
    [InlineData(2023, 1, 8, 2)]
    [InlineData(2023, 1, 9, 3)]
    [InlineData(2024, 12, 30, 53)]
    [InlineData(2025, 1, 1, 1)]
    public void GetWeekOfYearValidInputsReturnsExpectedResult(
        int year, int month, int day, int expectedWeek)
    {
        // Arrange
        var date = new DateOnly(year, month, day);

        // Act
        var actualWeek = date.GetWeekOfYear();

        // Assert
        actualWeek.Should().Be(expectedWeek);
    }
}
