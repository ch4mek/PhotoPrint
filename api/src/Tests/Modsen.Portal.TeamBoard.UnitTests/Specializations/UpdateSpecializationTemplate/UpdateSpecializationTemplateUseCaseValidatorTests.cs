using Modsen.Portal.TeamBoard.Requests.Skills.SpecializationTemplates.Requests;
using Modsen.Portal.TeamBoard.Requests.Skills.SpecializationTemplates.Requests.RequestDto;

namespace Modsen.Portal.TeamBoard.UnitTests.Specializations.UpdateSpecializationTemplate;

public class UpdateSpecializationTemplateUseCaseValidatorTests
{
    private readonly UpdateSpecializationTemplateRequest.Validator _validator = new();

    [Theory]
    [MemberData(nameof(InvalidData))]
    private void Validate_WhenRequestIsInvalid_ShouldReturnInvalidResult(
        int specializationId,
        string name,
        string message,
        string[] departmentsIds,
        List<SpecializationTemplateHardSkillDto> hardSkills)
    {
        // Act
        var updateDto = new UpdateSpecializationTemplateDto(
            specializationId,
            name,
            departmentsIds,
            hardSkills);
        var result = _validator.Validate(new UpdateSpecializationTemplateRequest.Command(updateDto));

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Any(x => x.ErrorMessage == message).Should().BeTrue();
    }

    [Theory]
    [MemberData(nameof(ValidData))]
    private void Validate_WhenRequestIsValid_ShouldReturnValidResult(
        int specializationId,
        string name,
        string[] departmentsIds,
        List<SpecializationTemplateHardSkillDto> hardSkills)
    {
        // Act
        var updateDto = new UpdateSpecializationTemplateDto(
            specializationId,
            name,
            departmentsIds,
            hardSkills);
        var result = _validator.Validate(new UpdateSpecializationTemplateRequest.Command(updateDto));

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [MemberData(nameof(InvalidLength))]
    private void Validate_WhenLengthIsInvalid_ShouldReturnInValidResult(
        int specializationId,
        string name,
        string[] departmentsIds,
        List<SpecializationTemplateHardSkillDto> hardSkills)
    {
        // Act
        var updateDto = new UpdateSpecializationTemplateDto(
            specializationId,
            name,
            departmentsIds,
            hardSkills);

        var result = _validator.Validate(new UpdateSpecializationTemplateRequest.Command(updateDto));

        // Assert
        result.IsValid.Should().BeFalse();
    }

    public static IEnumerable<object[]> ValidData =>
        new List<object[]>
        {
            new object[]
            {
                1,
                "name",
                new string[2] { "Java", "React" },
                new List<SpecializationTemplateHardSkillDto>
                {
                    new() { Id = 1, Name = "Windows" },
                    new() { Id = 2, Name = "Linux" }
                }
            },
            new object[]
            {
                1,
                "name123##@",
                new string[1] { "Java" },
                new List<SpecializationTemplateHardSkillDto>
                {
                    new() { Id = 1, Name = "Windows" }
                }
            }
        };

    public static IEnumerable<object?[]> InvalidData =>
        new List<object[]>
        {
            new object[] { 1, "     ", "The string cannot contain only spaces or/and punctuation symbols", new string[1], new List<SpecializationTemplateHardSkillDto>() },
            new object[] { 1, "*TEST*", "The string must contain only letters of the Russian and English alphabets, uppercase and lowercase letters, hyphens, spaces, punctuation marks, numbers and the following characters: + @ # ( ) [ ] { } < >", new string[1], new List<SpecializationTemplateHardSkillDto>() },
            new object[] { 1, "--------", "The string cannot contain only spaces or/and punctuation symbols", null!, null! },
            new object[] { null, "name", "This field is required", null!, new List<SpecializationTemplateHardSkillDto>() }
        };

    public static IEnumerable<object[]> InvalidLength =>
        new List<object[]>
        {
            new object[]
            {
                1,
                "a",
                new string[2] { "Java", "React" },
                new List<SpecializationTemplateHardSkillDto>
                {
                    new() { Id = 1, Name = "Windows" },
                    new() { Id = 2, Name = "Linux" }
                }
            },
            new object[]
            {
                1,
                new string('a', 251),
                new string[1] { "Java" },
                new List<SpecializationTemplateHardSkillDto>
                {
                    new() { Id = 1, Name = "Windows" }
                }
            }
        };
}
