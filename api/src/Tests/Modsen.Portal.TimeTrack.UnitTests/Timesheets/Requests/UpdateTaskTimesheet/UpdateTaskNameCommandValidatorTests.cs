using Modsen.Portal.TimeTrack.UseCases.Timesheets.Requests;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Requests.RequestDto;

namespace Modsen.Portal.TimeTrack.UnitTests.Timesheets.Requests.UpdateTaskTimesheet;

public class UpdateTaskNameCommandValidatorTests
{
    [Theory]
    [MemberData(nameof(GetInvalidTaskNameParameters))]
    public void ValidateWhenDataIsInvalidShouldReturnInvalidResult(string taskName, string oldTaskName)
    {
        // Arrange
        var validator = new UpdateTaskNameRequest.Validator();

        var command = new UpdateTaskNameRequest.Command(new UpdateTaskNameDto
        {
            UserId = 2,
            ProjectId = 1,
            EndDate = new DateOnly(2022, 5, 2),
            StartDate = new DateOnly(2022, 3, 22),
            OldTaskName = oldTaskName,
            TaskName = taskName
        });

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [MemberData(nameof(GetValidTaskNameParameters))]
    public void ValidateWhenDataIsValidShouldReturnValidResult(string taskName, string oldTaskName)
    {
        // Arrange
        var validator = new UpdateTaskNameRequest.Validator();

        var command = new UpdateTaskNameRequest.Command(new UpdateTaskNameDto
        {
            UserId = 2,
            ProjectId = 1,
            EndDate = new DateOnly(2022, 5, 2),
            StartDate = new DateOnly(2022, 3, 22),
            OldTaskName = oldTaskName,
            TaskName = taskName
        });

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    public static IEnumerable<object?[]> GetInvalidTaskNameParameters()
    {
        yield return ["        ", "abcde"];
        yield return [new string('y', 251), "abcde"];
        yield return ["a", "abcde"];
        yield return ["abcde", "a"];
        yield return ["abcde", new string('y', 251)];
        yield return ["abcde", "        "];
    }

    public static IEnumerable<object?[]> GetValidTaskNameParameters()
    {
        yield return ["abcde", "Thomas Shelby"];
        yield return ["abcde", "ab"];
        yield return ["abcde", new string('y', 250)];
        yield return ["abcde", ""];
    }
}
