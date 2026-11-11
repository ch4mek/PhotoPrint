using Modsen.Portal.TimeTrack.UseCases.Clients.Requests;

namespace Modsen.Portal.TimeTrack.UnitTests.UseCases.Validators;

public class UpdateClientCommandValidatorTests
{
    private readonly UpdateClientRequest.Validator _validator = new();

    [Theory]
    [MemberData(nameof(GetInvalidCreateClientParams))]
    public void ValidateWhenCommandIsInvalidShouldReturnInvalidResult(
        string name,
        string? legalEntity,
        string? comments,
        string? color,
        int RegisteredLocation)
    {
        // Arrange
        var command = new UpdateClientRequest.Command(
            Id: 1,
            Name: name,
            LegalEntity: legalEntity,
            Color: color,
            Comments: comments,
            RegisteredLocation: 1,
            SalesManagerId: 2);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [MemberData(nameof(GetValidCreateClientParams))]
    public void ValidateWhenCommandIsValidShouldReturnValidResult(
        string name,
        string? legalEntity,
        string? comments,
        string? color,
        int RegisteredLocation)
    {
        // Arrange
        var command = new UpdateClientRequest.Command(
            Id: 1,
            Name: name,
            LegalEntity: legalEntity,
            Color: color,
            Comments: comments,
            RegisteredLocation: 1,
            SalesManagerId: 2);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    public static IEnumerable<object?[]> GetInvalidCreateClientParams()
    {
        yield return [null, null, "comment", "#0f0", 0];
        yield return [string.Empty, string.Empty, " comment", "#777777", -1];
        yield return ["U", "U", "comment ", "#4D4", 890];
        yield return ["     ", "     ", "com", "#000000", 0];
        yield return [new string('0', 251), new string('0', 251), "", "#123456", 0];
        yield return ["Lorem Ipsum", "Lorem Ipsum", new string('5', 1001), "#000", 999];
        yield return ["Sit Amet", "Sit Amet", "   ", "#FaF", 909];
        yield return ["Pellentesque ultricies", "Pellentesque ultricies", null, "#FF", 0];
        yield return ["Duis mollis", "Duis mollis", "scelerisque", "000000", 0];
    }

    public static IEnumerable<object?[]> GetValidCreateClientParams()
    {
        yield return ["CL", "CL", null, null, 1];
        yield return [new string('y', 250), new string('y', 250), string.Empty, null, 1];
        yield return ["Client", "Client", null, string.Empty, 2];
        yield return ["       g     ", "       g     ", "    0", "#9f9", 1];
        yield return ["TimeTracker", "TimeTracker", "Comment", "     ", 1];
        yield return ["Brackets Example {}<>()[]", "Brackets Example {}<>()[]", "Comment", "     ", 1];
    }
}
