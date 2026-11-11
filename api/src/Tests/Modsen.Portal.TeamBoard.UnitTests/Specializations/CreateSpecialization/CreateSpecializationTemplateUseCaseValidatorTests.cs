using Modsen.Portal.TeamBoard.Requests.Skills.SpecializationTemplates.Requests;
using Modsen.Portal.TeamBoard.Requests.Skills.SpecializationTemplates.Requests.RequestDto;

namespace Modsen.Portal.TeamBoard.UnitTests.Specializations.CreateSpecialization;

public class CreateSpecializationTemplateUseCaseValidatorTests
{
    private readonly CreateSpecializationTemplateRequest.Validator _validator = new();

    [Theory]
    [MemberData(nameof(InvalidData))]
    public void ValidateWhenRequestIsInvalidShouldReturnInvalidResult(string departmentId, string name, string message)
    {
        // Act
        var departmentIds = new List<string> { departmentId };
        var hardSkillIds = new List<int> { };

        var requestDto = new CreateSpecializationTemplateDto(name, departmentIds, hardSkillIds);

        var result = _validator.Validate(new CreateSpecializationTemplateRequest.Command(requestDto));

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Any(x => x.ErrorMessage == message).Should().BeTrue();
    }

    [Theory]
    [MemberData(nameof(ValidData))]
    public void ValidateWhenRequestIsValidShouldReturnValidResult(string departmentId, string name)
    {
        // Act
        var departmentIds = new List<string> { departmentId };
        var hardSkillIds = new List<int> { };

        var requestDto = new CreateSpecializationTemplateDto(name, departmentIds, hardSkillIds);

        var result = _validator.Validate(new CreateSpecializationTemplateRequest.Command(requestDto));

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [MemberData(nameof(InvalidLength))]
    public void ValidateWhenLengthIsInvalidShouldReturnValidResult(string departmentId, string name)
    {
        // Act
        var departmentIds = new List<string> { departmentId };
        var hardSkillIds = new List<int> { };

        var requestDto = new CreateSpecializationTemplateDto(name, departmentIds, hardSkillIds);

        var result = _validator.Validate(new CreateSpecializationTemplateRequest.Command(requestDto));

        // Assert
        result.IsValid.Should().BeFalse();
    }

    public static IEnumerable<object[]> ValidData =>
        new List<object[]>
        {
            new object[] { "Net", "name" },
            new object[] { "Net", "name123##@" }
        };

    public static IEnumerable<object?[]> InvalidData =>
        new List<object[]>
        {
            new object[] { "Net", "     ", "The string cannot contain only spaces or/and punctuation symbols" },
            new object[] { "Net", "*TEST*", "The string must contain only letters of the Russian and English alphabets, uppercase and lowercase letters, hyphens, spaces, punctuation marks, numbers and the following characters: + @ # ( ) [ ] { } < >" },
            new object[] { "Net", "--------", "The string cannot contain only spaces or/and punctuation symbols" }
        };

    public static IEnumerable<object[]> InvalidLength =>
        new List<object[]>
        {
            new object[] { "Net", "a" },
            new object[] { "Net", new string('a', 251) }
        };
}
