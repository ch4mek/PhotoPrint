using System.Globalization;
using Modsen.Portal.TeamBoard.Requests.Payload.Core.BusinessRules;

namespace Modsen.Portal.TeamBoard.UnitTests.Payload.Core.BusinessRule;

public class IsUserAssignedBusinessRuleTests
{
    private readonly IsUserAssignedBusinessRule _isUserAssignedBusinessRule = new();

    [Theory]
    [InlineData("2025-01-01", null, 4, 2025, true)]
    [InlineData("2025-01-01", "2025-05-01", 4, 2025, true)]
    [InlineData("2024-04-01", null, 4, 2025, true)]
    [InlineData("2025-05-01", null, 4, 2025, false)]
    [InlineData("2025-05-01", "2025-07-02", 4, 2025, false)]
    [InlineData("2025-01-01", "2025-02-01", 4, 2025, false)]
    [InlineData("2025-05-01", "2025-01-02", 4, 2025, false)]
    public void HandleIsUserAssignedShouldReturnCorrectData(string userOnProjectFrom, string? userOnProjectTo, int month, int year, bool expectedResult)
    {
        // Arrange
        var onProjectFrom = DateOnly.Parse(userOnProjectFrom, CultureInfo.InvariantCulture);
        DateOnly? onProjectTo = null;

        if (userOnProjectTo is not null)
        {
            onProjectTo = DateOnly.Parse(userOnProjectTo, CultureInfo.InvariantCulture);
        }

        // Act
        var result = _isUserAssignedBusinessRule.IsUserAssigned(onProjectFrom, onProjectTo, month, year);

        // Assert

        result.Should().Be(expectedResult);
    }
}
