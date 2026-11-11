using System.Globalization;
using Modsen.Portal.TeamBoard.Requests.Payload.Core.Calculators;

namespace Modsen.Portal.TeamBoard.UnitTests.Payload.Core.Calculators;

public class UserProjectHoursCalculatorTests
{
    private readonly UserProjectHoursCalculator _userProjectHoursCalculator;
    private readonly WorkingDaysCalculator _workingDaysCalculator = new();

    public UserProjectHoursCalculatorTests()
    {
        _userProjectHoursCalculator = new(_workingDaysCalculator);
    }


    [Theory]
    [InlineData(200, 2, 2025, 0.5, "2025-01-1", "2025-03-01", 20, 10, 100, 0.5, 0.05)]
    [InlineData(200, 2, 2025, 0.5, "2025-01-1", null, 20, 10, 100, 0.5, 0.05)]
    [InlineData(200, 2, 2025, 0.5, "2024-02-1", null, 20, 20, 100, 0.5, 0.1)]
    [InlineData(200, 2, 2025, 0.5, "2024-02-1", "2026-02-01", 20, 20, 100, 0.5, 0.1)]
    [InlineData(200, 2, 2025, 1, "2025-02-10", "2025-03-01", 20, 40, 150, 0.75, 0.2)]
    [InlineData(200, 2, 2025, 1, "2025-01-1", "2025-02-10", 20, 40, 60, 0.3, 0.2)]
    public void HandleCalculateUserProjectHoursForSingleAssignShouldReturnCorrectData(
        int workingHoursInMonth,
        int month,
        int year,
        decimal fte,
        string onProjectFromInput,
        string? onProjectToInput,
        int workingDaysInMonth,
        decimal trackedBillable,
        decimal expectedRequiredHours,
        decimal expectedPlannedFte,
        decimal expectedActualFte)
    {
        // Arrange
        var onProjectFrom = DateOnly.Parse(onProjectFromInput, CultureInfo.InvariantCulture);
        DateOnly? onProjectTo = null;

        if (onProjectToInput is not null)
        {
            onProjectTo = DateOnly.Parse(onProjectToInput, CultureInfo.InvariantCulture);
        }

        var projectId = 1;

        var request = new CalculateUserProjectHoursForSingleAssignRequest(
            projectId,
            year,
            month,
            workingDaysInMonth,
            workingHoursInMonth,
            onProjectFrom,
            onProjectTo,
            fte,
            trackedBillable,
            null);

        // Act
        var result = _userProjectHoursCalculator.CalculateUserProjectHoursForSingleAssign(request);

        // Assert

        result.RequiredHours.Should().Be(expectedRequiredHours);
        result.PlannedFte.Should().Be(expectedPlannedFte);
        result.ActualFte.Should().Be(expectedActualFte);
    }
}
