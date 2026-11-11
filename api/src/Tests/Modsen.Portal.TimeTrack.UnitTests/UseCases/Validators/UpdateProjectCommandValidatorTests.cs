using Modsen.Portal.TimeTrack.UseCases.Projects.Requests;

namespace Modsen.Portal.TimeTrack.UnitTests.UseCases.Validators;

public class ValidatorTests
{
    private readonly UpdateProjectRequest.Validator _validator = new();

    [Theory]
    [MemberData(nameof(GetInvalidCreateProjectParams))]
    public void ValidateWhenCommandIsInvalidShouldReturnInvalidResult(string name, string color)
    {
        // Arrange
        var command = new UpdateProjectRequest.Command(
            Id: 1,
            Name: name,
            Color: color,
            ClientId: 1,
            ManagerId: null,
            IsPublic: true,
            IsBillable: false,
            TechStack: null,
            Industry: null,
            CloudServices: null,
            Architecture: null
        );

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
        var command = new UpdateProjectRequest.Command(
            Id: 1,
            Name: name,
            Color: color,
            ClientId: 1,
            ManagerId: null,
            IsPublic: true,
            IsBillable: false,
            TechStack: null,
            Industry: null,
            CloudServices: null,
            Architecture: null
        );

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
        yield return ["    ", "#fFf"];
        yield return [new string('p', 64), "%abc"];
    }

    public static IEnumerable<object?[]> GetValidCreateProjectParams()
    {
        yield return ["AB", "#fff"];
        yield return [new string('y', 250), "#AE5FFF"];
        yield return ["TimeTracker", "#000000"];
        yield return ["Brackets Example {}<>()[]", "#000000"];
    }
}
