using Modsen.Portal.Core.TimeTrack;
using Modsen.Portal.TimeTrack.UseCases.Reports.Core;
using Modsen.Portal.TimeTrack.UseCases.Reports.Requests;

namespace Modsen.Portal.TimeTrack.UnitTests.UseCases.Validators;

public class PeriodValidatorTests
{
    private readonly PeriodValidator<GetUsersWithTotalTrackedTimeRequest.Query> _validator = new();

    [Theory]
    [InlineData(-45, new[] { 2 }, 6)]
    [InlineData(null, new[] { 2 }, null)]
    [InlineData(null, null, 25)]
    [InlineData(2023, new[] { 16 }, null)]
    [InlineData(2023, new[] { 5 }, 77)]
    public void ValidateWhenRequestIsInvalidShouldReturnInvalidResult(int? year, int[] monthOfYear, int? weekOfYear)
    {
        // Arrange
        var query = new GetUsersWithTotalTrackedTimeRequest.Query(
            Year: year,
            MonthOfYear: monthOfYear,
            WeekOfYear: weekOfYear,
            DepartmentId: null,
            ResourceGroupId: null,
            ClientId: null,
            ProjectId: null,
            UserId: null,
            TrackedTimeType.Fact);

        // Act
        var result = _validator.Validate(query);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [InlineData(2023, null, null)]
    [InlineData(2023, new[] { 4 }, null)]
    [InlineData(2023, null, 34)]
    public void ValidateWhenRequestIsValidShouldReturnValidResult(int? year, int[] monthOfYear, int? weekOfYear)
    {
        // Arrange
        var query = new GetUsersWithTotalTrackedTimeRequest.Query(
            Year: year,
            MonthOfYear: monthOfYear,
            WeekOfYear: weekOfYear,
            DepartmentId: null,
            ResourceGroupId: null,
            ClientId: null,
            ProjectId: null,
            UserId: null,
            TrackedTimeType.Fact);

        // Act
        var result = _validator.Validate(query);

        // Assert
        result.IsValid.Should().BeTrue();
    }
}
