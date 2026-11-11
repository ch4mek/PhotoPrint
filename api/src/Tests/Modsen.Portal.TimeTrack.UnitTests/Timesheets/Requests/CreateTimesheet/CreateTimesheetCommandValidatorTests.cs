using Modsen.Portal.TimeTrack.UseCases.Timesheets.Requests;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Requests.RequestDto;

namespace Modsen.Portal.TimeTrack.UnitTests.Timesheets.Requests.CreateTimesheet;

public class CreateTimesheetCommandValidatorTests
{
    [Theory]
    [MemberData(nameof(GetInvalidCreateTimesheetManagerCommandParameters))]
    public void ValidateFactWhenDataIsInvalidShouldReturnInvalidResult(string taskName, int trackedInMinutes)
    {
        // Arrange
        var validator = new CreateOrUpdateTimesheet.Validator();
        var command = new CreateOrUpdateTimesheet.Command(new UpsertFactDto
        {
            TaskName = taskName,
            FactTrackedInMinutes = trackedInMinutes
        });

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
    }
    [Theory]
    [MemberData(nameof(GetInvalidCreateTimesheetManagerCommandParameters))]
    public void ValidateBillableWhenDataIsInvalidShouldReturnInvalidResult(string taskName, int trackedInMinutes)
    {
        // Arrange
        var validator = new CreateOrUpdateTimesheet.Validator();
        var command = new CreateOrUpdateTimesheet.Command(new UpsertBillableDto
        {
            TaskName = taskName,
            BillableTrackedInMinutes = trackedInMinutes
        });

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [MemberData(nameof(GetValidCreateTimesheetManagerCommandParameters))]
    public void ValidateFactWhenDataIsValidShouldReturnValidResult(string taskName, int trackedInMinutes)
    {
        // Arrange
        var validator = new CreateOrUpdateTimesheet.Validator();
        var command = new CreateOrUpdateTimesheet.Command(new UpsertFactDto
        {
            TaskName = taskName,
            FactTrackedInMinutes = trackedInMinutes,
        });

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [MemberData(nameof(GetValidCreateTimesheetManagerCommandParameters))]
    public void ValidateBillableWhenDataIsValidShouldReturnValidResult(string taskName, int trackedInMinutes)
    {
        // Arrange
        var validator = new CreateOrUpdateTimesheet.Validator();
        var command = new CreateOrUpdateTimesheet.Command(new UpsertBillableDto
        {
            TaskName = taskName,
            BillableTrackedInMinutes = trackedInMinutes,
        });

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    public static IEnumerable<object?[]> GetInvalidCreateTimesheetManagerCommandParameters()
    {
        yield return [" ", 180];
        yield return ["        ", 180];
        yield return ["", -1];
        yield return ["TimeTracker", -1];
        yield return ["John Doe", -1];
        yield return ["a", 180];
        yield return [new string('y', 251), 180];
    }

    public static IEnumerable<object?[]> GetValidCreateTimesheetManagerCommandParameters()
    {
        yield return ["", 180];
        yield return ["aa", 180];
        yield return ["TimeTracker", 30];
        yield return ["test", 15];
        yield return ["John Doe", 30];
        yield return [new string('y', 250), 180];
    }
}
