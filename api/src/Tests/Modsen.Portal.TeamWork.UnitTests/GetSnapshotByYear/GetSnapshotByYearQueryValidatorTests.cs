using Modsen.Portal.TeamWork.UseCases.GetSnapshotByYear;

namespace Modsen.Portal.TeamWork.UnitTests.GetSnapshotByYear;

public class GetSnapshotByYearQueryValidatorTests
{
    private readonly GetSnapshotByYearQueryValidator _validator = new();

    [Theory]
    [MemberData(nameof(Years))]
    public void ValidateWhenRequestIsInvalidShouldReturnInvalidResult(int startYear, int endYear)
    {
        // Act
        var result = _validator.Validate(new GetSnapshotByYearQuery(startYear, endYear,
            DepartmentId: null, [], []));

        // Assert
        result.IsValid.Should().BeFalse();
    }

    public static IEnumerable<object[]> Years =>
        new List<object[]>
        {
            new object[] { 0, 2023 },
            new object[] { -1, 2023 },
            new object[] { 2023, 2022 }
        };
}
