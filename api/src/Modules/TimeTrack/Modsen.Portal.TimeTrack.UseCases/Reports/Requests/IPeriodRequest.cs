namespace Modsen.Portal.TimeTrack.UseCases.Reports.Requests;

internal interface IPeriodRequest
{
    int? Year { get; init; }
    int[]? MonthOfYear { get; init; }
    int? WeekOfYear { get; init; }
}
