using Modsen.Portal.Equipment.Requests.EquipmentHistory.Requests;

namespace Modsen.Portal.Equipment.UnitTests.UseCases.EquipmentTests.Validators;

public class AssignUserToEquipmentCommandValidatorTests
{
    private readonly AssignUserToEquipmentRequest.Validator _validator = new();

    [Theory]
    [MemberData(nameof(GetInvalidAssignUserToEquipmentCommandParams))]
    public void ValidateWhenCommandIsInvalidShouldReturnInvalidResult(
        int equipmentId,
        DateTime issueDate)
    {
        // Arrange
        const int userId = 1;
        const int approverId = 3;

        var command = new AssignUserToEquipmentRequest.Command(
            equipmentId,
            userId,
            approverId,
            issueDate);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [MemberData(nameof(GetValidAssignUserToEquipmentCommandParams))]
    public void ValidateWhenCommandIsValidShouldReturnValidResult(
        int equipmentId,
        DateTime issueDate)
    {
        // Arrange
        const int userId = 1;
        const int approverId = 3;

        var command = new AssignUserToEquipmentRequest.Command(
            equipmentId,
            userId,
            approverId,
            issueDate);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    public static IEnumerable<object[]> GetInvalidAssignUserToEquipmentCommandParams()
    {
        yield return
        [
            -1,
            new DateTime(2023, 7, 22, 0, 0, 0, DateTimeKind.Utc)
        ];
        yield return
        [
            1,
            new DateTime(2023, 7, 22, 0, 0, 1, DateTimeKind.Utc)
        ];
        yield return
        [
            -1,
            new DateTime(2023, 7, 22, 11, 56, 12, DateTimeKind.Utc)
        ];
    }

    public static IEnumerable<object[]> GetValidAssignUserToEquipmentCommandParams()
    {
        yield return
        [
            1,
            new DateTime(2023, 7, 22, 0, 0, 0, DateTimeKind.Utc)
        ];
        yield return
        [
            1,
            DateTime.UtcNow.Date
        ];
    }
}
