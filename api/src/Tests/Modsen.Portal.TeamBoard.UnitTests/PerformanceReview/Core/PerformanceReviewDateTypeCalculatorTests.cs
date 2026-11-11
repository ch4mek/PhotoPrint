using Modsen.Portal.Core.TeamBoard;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.TeamBoard.Requests.Reviews.PerformanceReviews.Core.Calculators;

namespace Modsen.Portal.TeamBoard.UnitTests.PerformanceReview.Core;

public class PerformanceReviewDateTypeCalculatorTests
{
    private readonly PerformanceReviewDateTypeCalculator _calculator = new();

    [Fact]
    public void CalculateWhenReviewDatesDiffenerceIsMoreThanOverdueReturnsFutureStatus()
    {
        //Arrange
        const int overdueDaysCount = 7;
        const int upcomingIminentBorderDaysCount = 3;

        var reviewDate = DateTime.Today.AddDays(overdueDaysCount + 5);

        //Act
        var status = _calculator.Calculate(reviewDate, overdueDaysCount, upcomingIminentBorderDaysCount);

        //Assert
        status.Should().Be(PerformanceReviewDateType.Future.GetDescription());

    }

    [Fact]
    public void CalculateWhenReviewDatesDiffenerceIsLessThanOverdueAndMoreThanIminentBorderReturnsUpcomingStatus()
    {
        //Arrange
        const int overdueDaysCount = 7;
        const int upcomingIminentBorderDaysCount = 3;

        var date = DateTime.Today.AddDays(upcomingIminentBorderDaysCount + 1);

        //Act
        var status = _calculator.Calculate(date, overdueDaysCount, upcomingIminentBorderDaysCount);

        //Assert
        status.Should().Be(PerformanceReviewDateType.Upcoming.GetDescription());

    }

    [Fact]
    public void CalculateWhenReviewDatesDiffenerceIsLessThanIminentBorderAndInFutureReturnsImminentStatus()
    {
        //Arrange
        const int overdueDaysCount = 7;
        const int upcomingIminentBorderDaysCount = 3;

        var date = DateTime.Today.AddDays(upcomingIminentBorderDaysCount - 1);

        //Act
        var status = _calculator.Calculate(date, overdueDaysCount, upcomingIminentBorderDaysCount);

        //Assert
        status.Should().Be(PerformanceReviewDateType.Imminent.GetDescription());

    }

    [Fact]
    public void CalculateWhenReviewDatesDiffenerceIsLessThanIminentBorderAndInPastReturnsImminentStatus()
    {
        //Arrange
        const int overdueDaysCount = 7;
        const int upcomingIminentBorderDaysCount = 3;

        var date = DateTime.Today.AddDays(-upcomingIminentBorderDaysCount - 1);

        //Act
        var status = _calculator.Calculate(date, overdueDaysCount, upcomingIminentBorderDaysCount);

        //Assert
        status.Should().Be(PerformanceReviewDateType.Imminent.GetDescription());

    }

    [Fact]
    public void CalculateWhenReviewDatesDiffenerceIsMoreThanIminentBorderAndInPastReturnsOverdueStatus()
    {
        //Arrange
        const int overdueDaysCount = 7;
        const int upcomingIminentBorderDaysCount = 3;

        var date = DateTime.Today.AddDays(-overdueDaysCount - 1);

        //Act
        var status = _calculator.Calculate(date, overdueDaysCount, upcomingIminentBorderDaysCount);

        //Assert
        status.Should().Be(PerformanceReviewDateType.OverdueMoreThanNDays.GetDescription());

    }
}
