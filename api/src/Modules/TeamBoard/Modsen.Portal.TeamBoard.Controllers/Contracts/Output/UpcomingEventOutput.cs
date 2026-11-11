using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.TeamBoard.Requests.Home.Requests.RequestsDto;

namespace Modsen.Portal.TeamBoard.Controllers.Contracts.Output;

internal record UpcomingEventOutput(
    string Type,
    string? Name,
    DateOnly StartDate,
    DateOnly? EndDate,
    string? Status,
    string? PerformanceReviewDateType,
    int? InquiryId,
    InquiryType? InquiryType)
{
    public static UpcomingEventOutput Of(UpcomingEventDto requestDto)
    {
        return new UpcomingEventOutput(
            requestDto.Type.GetDescription(),
            requestDto.Name,
            requestDto.StartDate,
            requestDto.EndDate,
            requestDto.Status,
            requestDto.PerformanceReviewDateType,
            requestDto.InquiryId,
            requestDto.InquiryType);
    }
}
