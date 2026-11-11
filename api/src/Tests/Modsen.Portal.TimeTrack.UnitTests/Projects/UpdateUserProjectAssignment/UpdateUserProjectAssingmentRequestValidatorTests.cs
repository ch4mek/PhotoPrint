using Modsen.Portal.TimeTrack.UseCases.Projects.Requests;

namespace Modsen.Portal.TimeTrack.UnitTests.Projects.UpdateUserProjectAssignment;

public class UpdateUserProjectAssingmentRequestValidatorTests
{
    private readonly UpdateUserProjectAssignmentRequest.Validator _validator = new();
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    
    [Theory]
    [MemberData(nameof(ValidData))]
    public async Task ValidateWhenRequestIsValidShouldReturnValid(int id, DateOnly userOnProjectFrom, DateOnly? userOnProjectTo)
    {
        // Arrange
        var command = new UpdateUserProjectAssignmentRequest.Command(id,
            UserId: 1,
            ProjectId: 1,
            IsTimeOnProjectBillable: true,
            Fte: 0.5m,
            UserOnProjectSeniorityLevelId: "MiddleL2",
            userOnProjectFrom, userOnProjectTo);

        // Act
        var result = await _validator.ValidateAsync(command, _cancellationToken);

        // Assert
        result.IsValid.Should().BeTrue();
    }
    
    [Theory]
    [MemberData(nameof(InvalidData))]
    public async Task ValidateWhenRequestIsInvalidShouldReturnInvalid(int id, DateOnly userOnProjectFrom, DateOnly? userOnProjectTo)
    {
        // Arrange
        var command = new UpdateUserProjectAssignmentRequest.Command(id,
            UserId: 0,
            ProjectId: 0,
            IsTimeOnProjectBillable: true,
            Fte: 0.5m,
            UserOnProjectSeniorityLevelId: "MiddleL2",
            userOnProjectFrom, userOnProjectTo);

        // Act
        var result = await _validator.ValidateAsync(command, _cancellationToken);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    public static IEnumerable<object?[]> ValidData =>
        new List<object?[]>
        {
            new object?[] {1, new DateOnly(year: 2024, month: 6, day: 20), new DateOnly(year: 2024, month: 6, day: 24)},
            new object?[] {1, new DateOnly(year: 2024, month: 6, day: 20), null}
        };

    public static IEnumerable<object?[]> InvalidData =>
        new List<object?[]>
        {
            new object?[] {1, null, null},
            new object?[] {null, null, null}
        };
}
