using System.Linq.Expressions;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.TeamBoard.Requests.Reviews.PerformanceReviews.Requests.RequestDto;

internal class PerformanceReviewLookupDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string? UserFirstName { get; set; }
    public string? UserLastName { get; set; }
    public string? UserFirstNameRu { get; set; }
    public string? UserLastNameRu { get;set; }
    public string? UserPosition { get;set; }
    public DateTime Date { get; set; }
    public string? PerformanceReviewDateType { get; set; }

    public static readonly Expression<Func<UserPerformanceReview, PerformanceReviewLookupDto>> Map =
        x => new PerformanceReviewLookupDto
        {
            Id = x.Id,
            UserId = x.UserId,
            UserFirstName = x.User!.FirstName,
            UserLastName = x.User!.LastName,
            UserFirstNameRu = x.User!.FirstNameRu,
            UserLastNameRu = x.User!.LastNameRu,
            Date = x.InterviewDateFromUtc!.Value,
            UserPosition = x.User.Position
        };
}
