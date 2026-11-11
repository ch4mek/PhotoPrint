using Modsen.Portal.Identity.Requests.Users.Requests;

namespace Modsen.Portal.Identity.UnitTests.Users.Validators;

public class UpdateUserSignatureRequestValidatorTests
{
    private readonly UpdateUserSignatureRequest.Validator _validator = new();

    [Theory]
    [MemberData(nameof(GetInvalidFullNames))]
    public void ValidateWhenFullNameIsNotValidShouldReturnInvalidResult(string fullName)
    {
        // Arrange
        var command = new UpdateUserSignatureRequest.Command(
            fullName,
            "Company CEO",
            "375292005894");

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [MemberData(nameof(GetInvalidPhones))]
    public void ValidateWhenPhonesIsNotValidShouldReturnInvalidResult(string phones)
    {
        // Arrange
        var command = new UpdateUserSignatureRequest.Command(
            "test name",
            "Company CEO",
            phones);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [MemberData(nameof(GetInvalidPositions))]
    public void ValidateWhenPositionIsNotValidShouldReturnInvalidResult(string position)
    {
        // Arrange
        var command = new UpdateUserSignatureRequest.Command(
            "test name",
            position,
            "375292005894");

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [MemberData(nameof(GetValidPositions))]
    public void ValidateWhenPositionIsValidShouldReturnValidResult(string position)
    {
        // Arrange
        var command = new UpdateUserSignatureRequest.Command(
            "test name",
            position,
            "+375292005894");

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    public static IEnumerable<object?[]> GetValidPositions()
    {
        yield return ["ПозицияЁё"];
        yield return [@"Позици\я\Ёё"];
    }

    public static IEnumerable<object?[]> GetInvalidPositions()
    {
        yield return [null];
        yield return [string.Empty];
        yield return [new string('X', 251)];
    }

    public static IEnumerable<object?[]> GetInvalidPhones()
    {
        yield return [null];
        yield return [string.Empty];
        yield return [new string('X', 251)];
        yield return ["+3754473450168974"];
    }

    public static IEnumerable<object?[]> GetInvalidFullNames()
    {
        yield return ["招摇的你"];
        yield return [null];
        yield return [string.Empty];
        yield return [new string('X', 501)];
        yield return ["abc"];
        yield return [",.:;'-_()[]{}"];
    }
}
