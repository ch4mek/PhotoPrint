using Modsen.Portal.TeamWork.UseCases.GetDepartmentSnapshotByYear;

namespace Modsen.Portal.TeamWork.UnitTests.GetDepartmentSnapshotByYear;

public class GetDepartmentSnapshotByYearQueryValidatorTests
{
    private readonly GetDepartmentSnapshotByYearQueryValidator _validator = new();

    [Theory]
    [MemberData(nameof(Years))]
    public void ValidateWhenRequestIsInvalidShouldReturnInvalidResult(int startYear, int endYear)
    {
        // Act
        var result = _validator.Validate(new GetDepartmentSnapshotByYearQuery(startYear, endYear,
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
