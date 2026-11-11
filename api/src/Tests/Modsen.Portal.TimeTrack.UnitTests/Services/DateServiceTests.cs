using Modsen.Portal.Core;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Core.TimeTrack;
using Modsen.Portal.TimeTrack.UseCases.Dtos.Reports;
using Modsen.Portal.TimeTrack.UseCases.Reports.Requests.GetTrackedTimeByStimulsoft;
using Modsen.Portal.TimeTrack.UseCases.Services;
using Modsen.Portal.TimeTrack.UseCases.Services.Contracts;

namespace Modsen.Portal.TimeTrack.UnitTests.Services;

public class DateServiceTests
{
    private readonly Mock<IClockService> _clock = new();
    private readonly Mock<IPeriodService> _periodService = new();
    private readonly DateService _dateService;

    public DateServiceTests()
    {
        _dateService = new DateService(_clock.Object, _periodService.Object);
    }

    [Fact]
    public void GetDateSpanWhenOnlyYearIsSpecifiedShouldReturnYearDateSpan()
    {
        // Arrange
        var request = new GetTotalTrackedTimeToExportRequest.Query(
            Year: 2023,
            MonthOfYear: [],
            WeekOfYear: null,
            DepartmentId: null,
            UserId: null,
            ResourceGroupId: null,
            Format: It.IsAny<DocumentType>(),
            ReportName: It.IsAny<string>(),
            TrackedTimeType.Fact);

        var expectedResult = new DateSpanDto
        {
            StartDate = new DateTime(
                year: 2023, month: 1, day: 1,
                hour: 0, minute: 0, second: 0,
                DateTimeKind.Utc),

            EndDate = new DateTime(
                year: 2023, month: 12, day: DateTime.DaysInMonth(2023, month: 12),
                hour: 0, minute: 0, second: 0,
                DateTimeKind.Utc)
        };

        _periodService.Setup(x => x.IsFilterForEntireYear(request))
            .Returns(true);

        // Act
        var actualResult = _dateService.GetDateSpan(request);

        // Assert
        actualResult.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public void GetDateSpanWhenYearAndOneMonthAreSpecifiedShouldReturnOneMonthDateSpan()
    {
        // Arrange
        var request = new GetTotalTrackedTimeToExportRequest.Query(
            Year: 2023,
            MonthOfYear: [1],
            WeekOfYear: null,
            DepartmentId: null,
            UserId: null,
            ResourceGroupId: null,
            Format: It.IsAny<DocumentType>(),
            ReportName: It.IsAny<string>(),
            TrackedTimeType.Fact);

        var expectedResult = new DateSpanDto
        {
            StartDate = new DateTime(
                year: 2023, month: 1, day: 1,
                hour: 0, minute: 0, second: 0,
                DateTimeKind.Utc),

            EndDate = new DateTime(
                year: 2023, month: 1, day: DateTime.DaysInMonth(year: 2023, month: 1),
                hour: 0, minute: 0, second: 0,
                DateTimeKind.Utc)
        };

        _periodService.Setup(x => x.IsFilterForOneSelectedMonth(request))
            .Returns(true);

        // Act
        var actualResult = _dateService.GetDateSpan(request);

        // Assert
        actualResult.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public void GetDateSpanWhenYearAndThreeSequentMonthsAreSpecifiedShouldReturnQuarterDateSpan()
    {
        // Arrange
        var request = new GetTotalTrackedTimeToExportRequest.Query(
            Year: 2023,
            MonthOfYear: [3, 4, 5],
            WeekOfYear: null,
            DepartmentId: null,
            UserId: null,
            ResourceGroupId: null,
            Format: It.IsAny<DocumentType>(),
            ReportName: It.IsAny<string>(),
            TrackedTimeType.Fact);

        var expectedResult = new DateSpanDto
        {
            StartDate = new DateTime(
                year: 2023, month: 3, day: 1,
                hour: 0, minute: 0, second: 0,
                DateTimeKind.Utc),

            EndDate = new DateTime(
                year: 2023, month: 5, day: DateTime.DaysInMonth(year: 2023, month: 5),
                hour: 0, minute: 0, second: 0,
                DateTimeKind.Utc)
        };

        _periodService.Setup(x => x.IsFilterForSelectedQuarter(request))
            .Returns(true);

        // Act
        var actualResult = _dateService.GetDateSpan(request);

        // Assert
        actualResult.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public void GetDateSpanWhenYearIsNotSpecifiedShouldReturnDateSpanFromMinValueToNow()
    {
        // Arrange
        var request = new GetTotalTrackedTimeToExportRequest.Query(
            Year: null,
            MonthOfYear: [],
            WeekOfYear: null,
            DepartmentId: null,
            UserId: null,
            ResourceGroupId: null,
            Format: It.IsAny<DocumentType>(),
            ReportName: It.IsAny<string>(),
            TrackedTimeType.Fact);

        var nowDateTime = DateTime.UtcNow;
        _clock.Setup(clock => clock.UtcNow).Returns(nowDateTime);

        var expectedResult = new DateSpanDto
        {
            StartDate = DateTime.MinValue,
            EndDate = nowDateTime.Date
        };

        // Act
        var actualResult = _dateService.GetDateSpan(request);

        // Assert
        actualResult.Should().BeEquivalentTo(expectedResult);
    }
}
