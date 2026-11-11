namespace Modsen.Portal.TeamBoard.Requests.Reviews.PerformanceReviews.Requests.RequestDto;

internal record PerformanceReviewDto
{
    public int UserId { get; set; }
    public DateTime? Date { get; set; }
}
