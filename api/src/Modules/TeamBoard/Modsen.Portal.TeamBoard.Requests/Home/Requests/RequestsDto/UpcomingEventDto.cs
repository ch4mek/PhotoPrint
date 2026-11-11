using System.ComponentModel;
using Modsen.Portal.Core.Inquiries;

namespace Modsen.Portal.TeamBoard.Requests.Home.Requests.RequestsDto;

internal sealed record UpcomingEventDto
{
    public UpcomingEventType Type { get; init; }
    public string? Name { get; init; }
    public DateOnly StartDate   { get; init; }
    public DateOnly? EndDate { get; init; }
    public string? Status { get; init; }
    public string? PerformanceReviewDateType { get; init; }
    public int? InquiryId { get; init; }
    public InquiryType? InquiryType { get; init; }
}

internal enum UpcomingEventType
{
    [Description("Absence")]
    Absence,
    [Description("Performance review")]
    PerformanceReview,
    [Description("Development plan")]
    DevelopmentPlan,
    [Description("Contract")]
    Contract
}
