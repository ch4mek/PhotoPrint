using FluentValidation;
using Modsen.Portal.TeamBoard.Requests.Calendar.Requests;

namespace Modsen.Portal.TeamBoard.UnitTests.WorkingHoursPerMonths.CreateWorkingHoursPerMonth;

public class CreateWorkingHoursPerMonthValidatorTests
{
    private readonly CreateWorkingHoursPerMonthRequest.Validator _validator = new();

    [Theory]
    [MemberData(nameof(InvalidData))]
    public async Task ValidateWhenRequestIsInvalidShouldThrowValidationException(int year, int monthOfYear,
        int countryId, int hours)
    {
        // Arrange
        var command = new CreateWorkingHoursPerMonthRequest.Command(year, monthOfYear, countryId, hours);

        // Act
        var result = () => _validator.ValidateAndThrowAsync(command);

        // Assert
        await result.Should().ThrowAsync<ValidationException>();
    }

    [Theory]
    [MemberData(nameof(ValidData))]
    public async Task ValidateWhenRequestIsValidShouldNotThrowValidationException(int year, int monthOfYear,
        int countryId, int hours)
    {
        // Arrange
        var command = new CreateWorkingHoursPerMonthRequest.Command(year, monthOfYear, countryId, hours);

        // Act
        var result = () => _validator.ValidateAndThrowAsync(command);

        // Assert
        await result.Should().NotThrowAsync<ValidationException>();
    }

    public static IEnumerable<object[]> InvalidData =>
        new List<object[]>
        {
            new object[] {2020, 13, 2, 140},
            new object[] {2019, 12, 2, 140},
            new object[] {2020, 1, 2, -123},
            new object[] {2019, 13, 2, -140},
            new object[] {2019, 13, 2, 0},
            new object[] {2019, 13, 2, 249}
        };

    public static IEnumerable<object[]> ValidData =>
        new List<object[]>
        {
            new object[] {2020, 1, 0, 140},
            new object[] {2021, 2, 1, 140},
            new object[] {2024, 4, 2, 123},
            new object[] {2027, 3, 3, 1},
            new object[] {2027, 3, 3, 248}
        };
}
