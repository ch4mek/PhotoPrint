using Modsen.Portal.TimeTrack.UseCases.Timesheets.Requests;

namespace Modsen.Portal.TimeTrack.UnitTests.Timesheets.Requests.DeleteTimesheet;

public class DeleteTimesheetCommandValidatorTests
{
    private readonly DeleteTimesheetRequest.DeleteTimesheetCommandValidator _validator = new();

    [Fact]
    public void ValidateShouldReturnFalseWhenIdIsLessThanZero()
    {
        // Arrange
        const int timesheetId = -13;

        var command = new DeleteTimesheetRequest.Command(timesheetId, true);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void ValidateShouldReturnFalseWhenIdIsEqualToZero()
    {
        // Arrange
        const int timesheetId = 0;

        var command = new DeleteTimesheetRequest.Command(timesheetId, true);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void ValidateShouldReturnTrueWhenIdIsGreaterThanZero()
    {
        // Arrange
        const int timesheetId = 13;

        var command = new DeleteTimesheetRequest.Command(timesheetId, true);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }
}
