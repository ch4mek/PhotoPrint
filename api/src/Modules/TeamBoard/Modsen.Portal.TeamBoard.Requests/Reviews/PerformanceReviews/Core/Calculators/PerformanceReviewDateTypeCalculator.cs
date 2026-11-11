using Modsen.Portal.Core.TeamBoard;
using Modsen.Portal.Core.Utils.Extensions;

namespace Modsen.Portal.TeamBoard.Requests.Reviews.PerformanceReviews.Core.Calculators;

internal interface IPerformanceReviewDateTypeCalculator
{
    string Calculate(DateTime date, int overdueDaysCount, int upcomingIminentBorderDaysCount);
}

internal sealed class PerformanceReviewDateTypeCalculator : IPerformanceReviewDateTypeCalculator
{
    public string Calculate(DateTime date, int overdueDaysCount, int upcomingIminentBorderDaysCount)
    {
        var currentDate = DateTime.Now.Date;
        var daysDifference = (date.Date - currentDate).Days;

        if (daysDifference > overdueDaysCount)
        {
            return PerformanceReviewDateType.Future.GetDescription();
        }

        if (daysDifference <= overdueDaysCount && daysDifference > upcomingIminentBorderDaysCount)
        {
            return PerformanceReviewDateType.Upcoming.GetDescription();
        }

        if (daysDifference <= upcomingIminentBorderDaysCount && daysDifference >= (-1) * overdueDaysCount)
        {
            return PerformanceReviewDateType.Imminent.GetDescription();
        }

        return PerformanceReviewDateType.OverdueMoreThanNDays.GetDescription();
    }
}
