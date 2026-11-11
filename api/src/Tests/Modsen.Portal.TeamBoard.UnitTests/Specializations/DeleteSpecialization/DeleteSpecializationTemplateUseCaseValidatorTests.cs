using Modsen.Portal.TeamBoard.Requests.Skills.SpecializationTemplates.Requests;

namespace Modsen.Portal.TeamBoard.UnitTests.Specializations.DeleteSpecialization;

public class DeleteSpecializationTemplateUseCaseValidatorTests
{
    private readonly DeleteSpecializationTemplateRequest.Validator _validator = new();

    [Theory]
    [MemberData(nameof(InvalidData))]
    public void ValidateWhenRequestIsInvalidShouldReturnInvalidResult(int id, string message)
    {
        // Act
        var result = _validator.Validate(new DeleteSpecializationTemplateRequest.Command(id));

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Any(x => x.ErrorMessage == message).Should().BeTrue();
    }

    [Theory]
    [MemberData(nameof(ValidData))]
    public void ValidateWhenRequestIsValidShouldReturnValidResult(int id)
    {
        // Act
        var result = _validator.Validate(new DeleteSpecializationTemplateRequest.Command(id));

        // Assert
        result.IsValid.Should().BeTrue();
    }

    public static IEnumerable<object[]> ValidData =>
        new List<object[]>
        {
            new object[] { 1 },
            new object[] { 10 }
        };

    public static IEnumerable<object?[]> InvalidData =>
        new List<object?[]>
        {
            new object?[] { 0, "This field is required" },
            new object?[] { null, "This field is required" }
        };
}
