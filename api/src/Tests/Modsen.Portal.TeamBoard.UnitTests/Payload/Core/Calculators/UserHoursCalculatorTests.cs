using Modsen.Portal.TeamBoard.Requests.Payload.Core.Calculators;
using Modsen.Portal.TeamBoard.Requests.Payload.Core.CoreDto;

namespace Modsen.Portal.TeamBoard.UnitTests.Payload.Core.Calculators;

public class UserHoursCalculatorTests
{
    private readonly UserHoursCalculator _userHoursCalculator = new(new WorkingDaysCalculator());

    [Fact]
    public void HandleCalculateUserHoursShouldReturnCorrectValue()
    {
        // Arrange
        var payloadProjectDetails = new PayloadUserProjectHoursCoreDto[]
        {
            new()
            {
                ProjectId = 1,
                ActualFte = 0.5m,
                PlannedFte = 0.2m,
                TrackedBillableInHours = 10,
                RequiredHours = 30,
            },
            new()
            {
                ProjectId = 1,
                ActualFte = 0.5m,
                PlannedFte = 0.2m,
                TrackedBillableInHours = 10,
                RequiredHours = 30,
            },
        };

        var month = 3;
        var year = 2026;
        var plannedFte = 0.8m;
        var userTotalTimePerMonth = new Dictionary<int, int>()
        {
            { month, 100 }
        };

        var activeMonths = new List<int> { 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
        var workingHoursPerMonth = 168;

        // Act

        var result = _userHoursCalculator.CalculateUserHours(new UserHourCalculatorDto(
            payloadProjectDetails,
            month,
            year,
            activeMonths,
            plannedFte,
            workingHoursPerMonth,
            userTotalTimePerMonth,
            null));

        // Assert

        result.MonthOfYear.Should().Be(month);
        result.ActualFte.Should().Be(1);
        result.PlannedFte.Should().Be(plannedFte);
        result.PlannedWorkloadInHours.Should().Be(134.4m);
        result.RequiredHours.Should().Be(60);
        result.TrackedBillableInHours.Should().Be(20);
        result.TotalUserTime.Should().Be(100);
        result.HasActivity.Should().BeTrue();
    }

    [Fact]
    public void HandleWhenUserHasStartActivityDateInFutureMonthShouldReturnHasActivityFalse()
    {
        // Arrange
        var payloadProjectDetails = new PayloadUserProjectHoursCoreDto[]
        {
            new()
            {
                ProjectId = 1,
                ActualFte = 0.5m,
                PlannedFte = 0.2m,
                TrackedBillableInHours = 10,
                RequiredHours = 30,
            },
            new()
            {
                ProjectId = 1,
                ActualFte = 0.5m,
                PlannedFte = 0.2m,
                TrackedBillableInHours = 10,
                RequiredHours = 30,
            },
        };

        var month = 1;
        var year = 2026;
        var plannedFte = 0.8m;
        var userTotalTimePerMonth = new Dictionary<int, int>()
        {
            { month, 100 }
        };

        var activeMonths = new List<int> { 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
        var workingHoursPerMonth = 168;

        // Act

        var result = _userHoursCalculator.CalculateUserHours(new UserHourCalculatorDto(
            payloadProjectDetails,
            month,
            year,
            activeMonths,
            plannedFte,
            workingHoursPerMonth,
            userTotalTimePerMonth,
            null));

        // Assert
        result.HasActivity.Should().BeFalse();
    }

    [Fact]
    public void HandleWhenUserHasStartActivityDateInCurrentMonthShouldReturnHasActivityTrue()
    {
        // Arrange
        var payloadProjectDetails = new PayloadUserProjectHoursCoreDto[]
        {
            new()
            {
                ProjectId = 1,
                ActualFte = 0.5m,
                PlannedFte = 0.2m,
                TrackedBillableInHours = 10,
                RequiredHours = 30,
            },
            new()
            {
                ProjectId = 1,
                ActualFte = 0.5m,
                PlannedFte = 0.2m,
                TrackedBillableInHours = 10,
                RequiredHours = 30,
            },
        };

        var month = 1;
        var year = 2026;
        var plannedFte = 0.8m;
        var userTotalTimePerMonth = new Dictionary<int, int>()
        {
            { month, 100 }
        };

        var activeMonths = new List<int> { 1, 10, 11, 12 };
        var workingHoursPerMonth = 168;

        // Act

        var result = _userHoursCalculator.CalculateUserHours(new UserHourCalculatorDto(
            payloadProjectDetails,
            month,
            year,
            activeMonths,
            plannedFte,
            workingHoursPerMonth,
            userTotalTimePerMonth,
            null));

        // Assert
        result.HasActivity.Should().BeTrue();
    }
}
