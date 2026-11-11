using Modsen.Portal.TimeTrack.UseCases.Projects.Requests;

namespace Modsen.Portal.TimeTrack.UnitTests.Projects.CreateUserProjectAssignment;

public class CreateUserProjectAssignmentRequestValidatorTests
{
    private readonly CreateUserProjectAssignmentRequest.Validator _validator = new();
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    [Theory]
    [MemberData(nameof(ValidData))]
    public async Task ValidateWhenRequestIsValidShouldReturnValidResult(int userId, int projectId, string seniorityLevelId,
        decimal fte, DateOnly userOnProjectFrom, DateOnly? userOnProjectTo, bool isTimeOnProjectBillable)
    {
        // Arrange
        var command = new CreateUserProjectAssignmentRequest.Command(userId, projectId, userOnProjectFrom, userOnProjectTo,
            isTimeOnProjectBillable, fte, seniorityLevelId);

        // Act
        var result = await _validator.ValidateAsync(command, _cancellationToken);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [MemberData(nameof(InvalidData))]
    public async Task ValidateWhenRequestIsInvalidShouldReturnInvalidResult(int userId, int projectId, string seniorityLevelId,
        decimal fte, DateOnly userOnProjectFrom, DateOnly? userOnProjectTo, bool isTimeOnProjectBillable)
    {
        // Arrange
        var command = new CreateUserProjectAssignmentRequest.Command(userId, projectId, userOnProjectFrom, userOnProjectTo,
            isTimeOnProjectBillable, fte, seniorityLevelId);

        // Act
        var result = await _validator.ValidateAsync(command, _cancellationToken);

        //Assert
        result.IsValid.Should().BeFalse();
    }

    public static IEnumerable<object?[]> ValidData =>
        new List<object?[]>
        {
            new object?[] { 1, 1, "SeniorL3", 1m, new DateOnly(2024, 12, 12), null, false},
            new object?[] { 2, 2, "SeniorL3", 0.1m, new DateOnly(2024, 12, 12), new DateOnly(2025, 12, 12), true },
            new object?[] { 2, 2, "SeniorL3", 0m, new DateOnly(2024, 12, 12), new DateOnly(2025, 12, 12), true },
            new object?[] { 2, 2, "SeniorL3", 0.0m, new DateOnly(2024, 12, 12), new DateOnly(2025, 12, 12), false }
        };

    public static IEnumerable<object?[]> InvalidData =>
        new List<object?[]>
        {
            new object?[] { 1, null, "", 1.1m, null, new DateOnly(2024, 12, 12), false },
            new object?[] { "2", 2, "SeniorL3", -0.5m, null, null, true }
        };
}
