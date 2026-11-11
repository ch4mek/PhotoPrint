using Modsen.Portal.Core.TeamBoard;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.OData;

namespace Modsen.Portal.TeamBoard.DataAccess.Npgsql.Mappers;

public interface IPageableOdpTeamboardUserToPageableOdpUserPerformanceReviewDtoMapper
{
    PageableListOfItems<OdpUserPerformanceReviewDto> Map(PageableListOfItems<OdpTeamBoardUser> pageableTeamBoardUsers, int overdueDaysCount);
}

internal class PageableOdpTeamboardUserToPageableOdpUserPerformanceReviewDtoMapper : IPageableOdpTeamboardUserToPageableOdpUserPerformanceReviewDtoMapper
{
    public PageableListOfItems<OdpUserPerformanceReviewDto> Map(PageableListOfItems<OdpTeamBoardUser> pageableTeamBoardUsers, int overdueDaysCount)
    {
        return new PageableListOfItems<OdpUserPerformanceReviewDto>
        {
            Items = pageableTeamBoardUsers.Items
                .Select(x => new OdpUserPerformanceReviewDto
                {
                   Id = x.Id,
                   FullName = x.FullName,
                   DepartmentId = x.DepartmentId,
                   Dates = GetPerformanceReviewDates(pageableTeamBoardUsers.Items.ToList(), x.Id, overdueDaysCount)
                })
                .ToList(),
            Page = pageableTeamBoardUsers.Page,
            PageSize = pageableTeamBoardUsers.PageSize,
            Total = pageableTeamBoardUsers.Total
        };
    }

    private static List<PerformanceReviewDateDto> GetPerformanceReviewDates(List<OdpTeamBoardUser> teamBoardUsers, int id, int overdueDaysCount)
    {
        var dates = teamBoardUsers.Where(y => y.Id == id)
            .SelectMany(x => x.OdpUserPerformanceReviews.Select(p => new { p.Id, p.InterviewDateFromUtc, p.Status, p.ConversationId }));

        dates = dates.OrderByDescending(x => x.InterviewDateFromUtc);

        return dates.Select(x => new PerformanceReviewDateDto
        {
            PerformanceReviewId = x.Id,
            Date = x.InterviewDateFromUtc!.Value.Date,
            ConversationId = x.ConversationId,
            PerformanceReviewDateType = x.Status == UserPerformanceReviewStatus.Passed
                ? PerformanceReviewDateType.Passed
                : GetPerformanceReviewDateType(x.InterviewDateFromUtc.Value.Date, overdueDaysCount)
        }).ToList();
    }

    private static PerformanceReviewDateType GetPerformanceReviewDateType(DateTime date, int overdueDaysCount)
    {
        var currentDate = DateTime.Now.Date;
        var daysDifference = (date.Date - currentDate).Days;
        var upcomingIminentBorderDaysCount = 3;

        if (daysDifference > overdueDaysCount)
        {
            return PerformanceReviewDateType.Future;
        }
        else if (daysDifference <= overdueDaysCount && daysDifference > upcomingIminentBorderDaysCount)
        {
            return PerformanceReviewDateType.Upcoming;
        }
        else if (daysDifference <= upcomingIminentBorderDaysCount && daysDifference >= (-1)*overdueDaysCount)
        {
            return PerformanceReviewDateType.Imminent;
        }
        else
        {
            return PerformanceReviewDateType.OverdueMoreThanNDays;
        }
    }
}
