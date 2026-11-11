using Bogus;
using Modsen.Portal.TimeTrack.UseCases.Projects.Requests;

namespace Modsen.Portal.TimeTrack.UnitTests.UseCases.Validators;

public class CreateProjectCommandValidatorTests
{
    private readonly CreateProjectRequest.Validator _validator = new();

    private readonly Faker<CreateProjectRequest.Command> _createProjectCommandFaker = new();

    public CreateProjectCommandValidatorTests()
    {
        _createProjectCommandFaker.CustomInstantiator(x => new CreateProjectRequest.Command(
            Name: "TestFirstName",
            Color: x.Internet.Color(),
            ManagerId: null,
            ClientId: 1,
            IsPublic: false,
            IsBillable: true,
            TechStack: null,
            Industry: null,
            CloudServices: null,
            Architecture: null
        ));
    }

    [Theory]
    [MemberData(nameof(GetInvalidCreateProjectParams))]
    public void ValidateWhenCommandIsInvalidShouldReturnInvalidResult(string name, string color)
    {
        // Arrange
        var command = _createProjectCommandFaker.Clone()
            .RuleFor(x => x.Name, name)
            .RuleFor(x => x.Color, color)
            .Generate();

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [MemberData(nameof(GetValidCreateProjectParams))]
    public void ValidateWhenCommandIsValidShouldReturnValidResult(string name, string color)
    {
        // Arrange
        var command = _createProjectCommandFaker.Clone()
            .RuleFor(x => x.Name, name)
            .RuleFor(x => x.Color, color)
            .Generate();

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    public static IEnumerable<object?[]> GetInvalidCreateProjectParams()
    {
        yield return [string.Empty, "#fff"];
        yield return [null, "#123456"];
        yield return ["A", null];
        yield return [new string('l', 251), "#000"];
        yield return ["Test project 1", "#aa"];
        yield return ["Test project 2", "eee"];
        yield return ["Test project 3", "777#"];
        yield return ["Test project 4", "123456"];
        yield return ["Test project 5", "*234567"];
        yield return ["Test project 6", "#efg"];
        yield return ["Test project 7", "#"];
        yield return ["T", "%abc"];
    }

    public static IEnumerable<object?[]> GetValidCreateProjectParams()
    {
        yield return ["AB", "#fff"];
        yield return ["Brackets Example {}<>()[]", "#fff"];
        yield return [new string('y', 250), "#AE5FFF"];
        yield return ["TimeTracker", "#000000"];
    }
}
