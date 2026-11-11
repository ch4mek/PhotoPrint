using Modsen.Portal.Core;
using Modsen.Portal.Core.TimeTrack;
using Modsen.Portal.TimeTrack.UseCases.Reports.Requests.GetTrackedTimeByStimulsoft;
using Modsen.Portal.TimeTrack.UseCases.Services;

namespace Modsen.Portal.TimeTrack.UnitTests.Services;

public class PeriodServiceTests
{
    private readonly PeriodService _periodService = new();

    [Theory]
    [InlineData(2013)]
    [InlineData(2015)]
    [InlineData(2023)]
    public void IsFilterForEntireYearWhenOnlyYearIsSpecifiedShouldReturnTrue(int year)
    {
        // Arrange
        var request = new GetTotalTrackedTimeToExportRequest.Query(
            year,
            MonthOfYear: [],
            WeekOfYear: null,
            DepartmentId: null,
            UserId: null,
            ResourceGroupId: null,
            Format: It.IsAny<DocumentType>(),
            ReportName: It.IsAny<string>(),
            TrackedTimeType.Fact);

        // Act
        var actualResult = _periodService.IsFilterForEntireYear(request);

        // Assert
        actualResult.Should().BeTrue();
    }

    [Theory]
    [InlineData(2013, new[] { 1, 2, 3 })]
    [InlineData(2015, new[] { 4, 5, 6 })]
    [InlineData(2016, new[] { 8, 9, 10 })]
    public void IsFilterForSelectedQuarterWhenYearAndThreeSequentMonthsAreSpecifiedShouldReturnTrue(
        int year,
        int[] monthsOfYear)
    {
        // Arrange
        var request = new GetTotalTrackedTimeToExportRequest.Query(
            year,
            monthsOfYear,
            WeekOfYear: null,
            DepartmentId: null,
            UserId: null,
            ResourceGroupId: null,
            Format: It.IsAny<DocumentType>(),
            ReportName: It.IsAny<string>(),
            TrackedTimeType.Fact);

        // Act
        var actualResult = _periodService.IsFilterForSelectedQuarter(request);

        // Assert
        actualResult.Should().BeTrue();
    }

    [Theory]
    [InlineData(2013, new[] { 1 })]
    [InlineData(2015, new[] { 4 })]
    [InlineData(2016, new[] { 8 })]
    public void IsFilterForOneSelectedMonthWhenYearAndOneMonthAreSpecifiedShouldReturnTrue(
        int year,
        int[] monthsOfYear)
    {
        // Arrange
        var request = new GetTotalTrackedTimeToExportRequest.Query(
            year,
            monthsOfYear,
            WeekOfYear: null,
            DepartmentId: null,
            UserId: null,
            ResourceGroupId: null,
            Format: It.IsAny<DocumentType>(),
            ReportName: It.IsAny<string>(),
            TrackedTimeType.Fact);

        // Act
        var actualResult = _periodService.IsFilterForOneSelectedMonth(request);

        // Assert
        actualResult.Should().BeTrue();
    }

    [Theory]
    [InlineData(2013, new[] { 1, 3, 8 })]
    [InlineData(2015, new[] { 4, 9, 6 })]
    public void IsFilterForSelectedQuarterWhenYearAndThreeNotSequentMonthsAreSpecifiedShouldReturnFalse(
        int year,
        int[] monthsOfYear)
    {
        // Arrange
        var request = new GetTotalTrackedTimeToExportRequest.Query(
            year,
            monthsOfYear,
            WeekOfYear: null,
            DepartmentId: null,
            UserId: null,
            ResourceGroupId: null,
            Format: It.IsAny<DocumentType>(),
            ReportName: It.IsAny<string>(),
            TrackedTimeType.Fact);

        // Act
        var actualResult = _periodService.IsFilterForSelectedQuarter(request);

        // Assert
        actualResult.Should().BeFalse();
    }
}
