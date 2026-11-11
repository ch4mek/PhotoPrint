using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Timesheets;
using Modsen.Portal.TimeTrack.UseCases.BusinessRules;

namespace Modsen.Portal.TimeTrack.UnitTests.BusinessRules;

public class CanUserProjectBeDecreasedBusinessRuleTests
{
    private readonly Mock<IGetTimesheetByProjectIdAndUserIdAndDateIntervalDbQuery>
        _getTimesheetByProjectIdAndUserIdAndDateIntervalDbQueryMock = new();

    private readonly ICanUserProjectBeDecreasedBusinessRule _businessRule;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    
    public CanUserProjectBeDecreasedBusinessRuleTests()
    {
        _businessRule = new CanUserProjectBeDecreasedBusinessRule(
            _getTimesheetByProjectIdAndUserIdAndDateIntervalDbQueryMock.Object);
    }

    //10.06 - 20.06 => 13.06 – 18.06
    [Theory]
    [MemberData(nameof(ReturnSuccess))]
    public async Task IsCorrespondsToBusinessWhenTrackDatesAreNotLostShouldReturnSuccess(
        object timesheets)
    {
        // Arrange
        var castedTimesheets = (List<GetTimesheetByProjectIdAndUserIdAndDateIntervalDbResponse>)timesheets;

        _getTimesheetByProjectIdAndUserIdAndDateIntervalDbQueryMock.Setup(x => x.ExecuteAsync(
                It.IsAny<GetTimesheetByProjectIdAndUserIdAndDateIntervalDbRequest>(), _cancellationToken))
            .ReturnsAsync(castedTimesheets);

        var userOnProjectFromBefore = new DateOnly(year: 2024, month: 6, day: 10);
        var userOnProjectToBefore = new DateOnly(year: 2024, month: 6, day: 20);
        var userOnProjectFrom = new DateOnly(year: 2024, month: 6, day: 13);
        var userOnProjectTo = new DateOnly(year: 2024, month: 6, day: 18);

        // Act
        var result = await _businessRule.IsCorrespondsToBusiness(new CanUserProjectBeDecreasedModel(
                ProjectId: 1, UserId: 1, userOnProjectFromBefore, userOnProjectToBefore, userOnProjectFrom, userOnProjectTo),
            _cancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }
    
    [Theory]
    [MemberData(nameof(ReturnFail))]
    public async Task IsCorrespondsToBusinessWhenTrackDatesAreLostShouldReturnFail(
        object timesheets)
    {
        // Arrange
        var castedTimesheets = (List<GetTimesheetByProjectIdAndUserIdAndDateIntervalDbResponse>)timesheets;

        _getTimesheetByProjectIdAndUserIdAndDateIntervalDbQueryMock.Setup(x => x.ExecuteAsync(
                It.IsAny<GetTimesheetByProjectIdAndUserIdAndDateIntervalDbRequest>(), _cancellationToken))
            .ReturnsAsync(castedTimesheets);

        var userOnProjectFromBefore = new DateOnly(year: 2024, month: 6, day: 10);
        var userOnProjectToBefore = new DateOnly(year: 2024, month: 6, day: 20);
        var userOnProjectFrom = new DateOnly(year: 2024, month: 6, day: 13);
        var userOnProjectTo = new DateOnly(year: 2024, month: 6, day: 18);

        // Act
        var result = await _businessRule.IsCorrespondsToBusiness(new CanUserProjectBeDecreasedModel(
                ProjectId: 1, UserId: 1, userOnProjectFromBefore, userOnProjectToBefore, userOnProjectFrom, userOnProjectTo),
            _cancellationToken);

        // Assert
        result.IsSuccess.Should().BeFalse();
    }


    public static IEnumerable<object[]> ReturnSuccess =>
        new List<object[]>
        {
            new object[]
            {
                new List<GetTimesheetByProjectIdAndUserIdAndDateIntervalDbResponse>
                {
                    new (ProjectId: 1, UserId: 1, ReportForDate: new DateOnly(year: 2024, month: 6, day: 14)),
                    new (ProjectId: 1, UserId: 1, ReportForDate: new DateOnly(year: 2024, month: 6, day: 15)),
                    new (ProjectId: 1, UserId: 1, ReportForDate: new DateOnly(year: 2024, month: 6, day: 16))
                }
            },
            new object[] { new List<GetTimesheetByProjectIdAndUserIdAndDateIntervalDbResponse>() },
            new object[]
            {
                new List<GetTimesheetByProjectIdAndUserIdAndDateIntervalDbResponse>
                {
                    new (ProjectId: 1, UserId: 1, ReportForDate: new DateOnly(year: 2024, month: 6, day: 13)),
                    new (ProjectId: 1, UserId: 1, ReportForDate: new DateOnly(year: 2024, month: 6, day: 15)),
                    new (ProjectId: 1, UserId: 1, ReportForDate: new DateOnly(year: 2024, month: 6, day: 18))
                }
            }
        };

    public static IEnumerable<object[]> ReturnFail =>
        new List<object[]>
        {
            new object[]
            {
                new List<GetTimesheetByProjectIdAndUserIdAndDateIntervalDbResponse>
                {
                    new (ProjectId: 1, UserId: 1, ReportForDate: new DateOnly(year: 2024, month: 6, day: 11)),
                    new (ProjectId: 1, UserId: 1, ReportForDate: new DateOnly(year: 2024, month: 6, day: 15)),
                    new (ProjectId: 1, UserId: 1, ReportForDate: new DateOnly(year: 2024, month: 6, day: 16))
                }
            },
            new object[]
            {
                new List<GetTimesheetByProjectIdAndUserIdAndDateIntervalDbResponse>
                {
                    new (ProjectId: 1, UserId: 1, ReportForDate: new DateOnly(year: 2024, month: 6, day: 11)),
                    new (ProjectId: 1, UserId: 1, ReportForDate: new DateOnly(year: 2024, month: 6, day: 15)),
                    new (ProjectId: 1, UserId: 1, ReportForDate: new DateOnly(year: 2024, month: 6, day: 19))
                }
            },
            new object[]
            {
                new List<GetTimesheetByProjectIdAndUserIdAndDateIntervalDbResponse>
                {
                    new (ProjectId: 1, UserId: 1, ReportForDate: new DateOnly(year: 2024, month: 6, day: 13)),
                    new (ProjectId: 1, UserId: 1, ReportForDate: new DateOnly(year: 2024, month: 6, day: 15)),
                    new (ProjectId: 1, UserId: 1, ReportForDate: new DateOnly(year: 2024, month: 6, day: 19))
                }
            }
        };
}
