using Modsen.Portal.TeamBoard.Requests.Skills.SoftSkills.Requests;

namespace Modsen.Portal.TeamBoard.UnitTests.SofSkills.CreateSoftSkill;

public class CreateSoftSkillCommandValidatorTests
{
    private readonly CreateSoftSkillRequest.Validator _validator = new();

    [Theory]
    [MemberData(nameof(Names))]
    public void ValidateWhenRequestIsInvalidShouldReturnInvalidResult(string name, string message)
    {
        // Act
        var result = _validator.Validate(new CreateSoftSkillRequest.Command(name));

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Any(x => x.ErrorMessage == message).Should().BeTrue();
    }

    [Theory]
    [MemberData(nameof(InvalidLength))]
    public void ValidateWhenLengthIsInvalidShouldReturnInvalidResult(string name)
    {
        // Act
        var result = _validator.Validate(new CreateSoftSkillRequest.Command(name));

        // Assert
        result.IsValid.Should().BeFalse();
    }

    public static IEnumerable<object[]> Names =>
        new List<object[]>
        {
            new object[] { null!, "Enter Skill name" },
            new object[] { "*TEST*", "The string must contain only letters of the Russian and English alphabets, uppercase and lowercase letters, hyphens, spaces, punctuation marks, numbers and the following characters: + @ # ( ) [ ] { } < >" },
            new object[] { "     ", "The string cannot contain only spaces or/and punctuation symbols" },
            new object[] { "--------", "The string cannot contain only spaces or/and punctuation symbols" }
        };

    public static IEnumerable<object[]> InvalidLength =>
        new List<object[]>
        {
            new object[] { "a" },
            new object[] { new string('a', 251) }
        };
}
