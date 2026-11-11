using Modsen.Portal.TimeTrack.UseCases.Timesheets.Requests;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Requests.RequestDto;

namespace Modsen.Portal.TimeTrack.UnitTests.Timesheets.Requests.UpdateTrackedTime;

public class UpdateTrackedTimeCommandValidatorTests
{
    private readonly CreateOrUpdateTimesheet.Validator _validator = new();

    [Theory]
    [MemberData(nameof(GetInvalidUpdateTrackedTimeParams))]
    public void ValidateWhenCommandFactIsInvalidShouldReturnInvalidResult(
        int trackedTimeInMinutes)
    {
        // Arrange
        var command = new CreateOrUpdateTimesheet.Command(
            new UpsertFactDto
            {
                TaskName = Guid.CreateVersion7().ToString(),
                FactTrackedInMinutes = trackedTimeInMinutes,
            });

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [MemberData(nameof(GetInvalidUpdateTrackedTimeParams))]
    public void ValidateWhenCommandBillableIsInvalidShouldReturnInvalidResult(
        int trackedTimeInMinutes)
    {
        // Arrange
        var command = new CreateOrUpdateTimesheet.Command(
            new UpsertBillableDto
            {
                TaskName = Guid.CreateVersion7().ToString(),
                BillableTrackedInMinutes = trackedTimeInMinutes,
            });

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [MemberData(nameof(GetValidUpdateTrackedTimeParams))]
    public void ValidateWhenCommandFactIsValidShouldReturnValidResult(
        int trackedTimeInMinutes)
    {
        // Arrange
        var command = new CreateOrUpdateTimesheet.Command(
            new UpsertFactDto
            {
                FactTrackedInMinutes = trackedTimeInMinutes,
                TaskName = "Test 1"
            });

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [MemberData(nameof(GetValidUpdateTrackedTimeParams))]
    public void ValidateWhenCommandBillableIsValidShouldReturnValidResult(
        int trackedTimeInMinutes)
    {
        // Arrange
        var command = new CreateOrUpdateTimesheet.Command(
            new UpsertFactDto
            {
                FactTrackedInMinutes = trackedTimeInMinutes,
                TaskName = "Test 1"
            });

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }


    public static IEnumerable<object?[]> GetInvalidUpdateTrackedTimeParams()
    {
        yield return [-2];
        yield return [-1];
    }

    public static IEnumerable<object?[]> GetValidUpdateTrackedTimeParams()
    {
        yield return [14];
        yield return [1];
        yield return [100];
    }

}
