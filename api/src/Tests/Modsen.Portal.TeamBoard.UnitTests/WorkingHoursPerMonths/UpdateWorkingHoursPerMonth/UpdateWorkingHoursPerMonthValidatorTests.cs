using FluentValidation;
using Modsen.Portal.TeamBoard.Requests.Calendar.Requests;

namespace Modsen.Portal.TeamBoard.UnitTests.WorkingHoursPerMonths.UpdateWorkingHoursPerMonth;

public class UpdateWorkingHoursPerMonthValidatorTests
{
    private readonly UpdateWorkingHoursPerMonthRequest.Validator _validator = new();

    [Theory]
    [MemberData(nameof(InvalidData))]
    public async Task ValidateWhenRequestIsInvalidShouldThrowValidationException(int id, int hours)
    {
        // Arrange
        var command = new UpdateWorkingHoursPerMonthRequest.Command(id, hours);

        // Act
        var result = () => _validator.ValidateAndThrowAsync(command);

        // Assert
        await result.Should().ThrowAsync<ValidationException>();
    }

    [Theory]
    [MemberData(nameof(ValidData))]
    public async Task ValidateWhenRequestIsValidShouldNotThrowValidationException(int id, int hours)
    {
        // Arrange
        var command = new UpdateWorkingHoursPerMonthRequest.Command(id, hours);

        // Act
        var result = () => _validator.ValidateAndThrowAsync(command);

        // Assert
        await result.Should().NotThrowAsync<ValidationException>();
    }

    public static IEnumerable<object[]> InvalidData =>
        new List<object[]>
        {
            new object[] {1, -12},
            new object[] {2, -15},
            new object[] {2, -1},
            new object[] {2, 249}
        };

    public static IEnumerable<object[]> ValidData =>
        new List<object[]>
        {
            new object[] {1, 12},
            new object[] {1, 140},
            new object[] {1, 0},
            new object[] {1, 248}
        };
}
