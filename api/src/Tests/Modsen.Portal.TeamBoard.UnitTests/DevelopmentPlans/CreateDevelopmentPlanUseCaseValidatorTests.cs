using Bogus;
using Modsen.Portal.TeamBoard.Requests.Reviews.DevelopmentPlans.Requests;

namespace Modsen.Portal.TeamBoard.UnitTests.DevelopmentPlans;

public class CreateDevelopmentPlanUseCaseValidatorTests
{
    private readonly CreateDevelopmentPlanRequest.Validator _validator = new();
    private readonly Faker<CreateDevelopmentPlanRequest.Command> _updatePersonalDataUseCaseCommandFaker = new();

    public CreateDevelopmentPlanUseCaseValidatorTests()
    {
        _updatePersonalDataUseCaseCommandFaker.CustomInstantiator(x => new CreateDevelopmentPlanRequest.Command(
            StartDate: DateOnly.Parse("2024-11-19"),
            DeadlineDate: DateOnly.Parse("2024-11-20"),
            Summary: "",
            Notes: "",
            UserId: 1210,
            Source: null,
            ConversationId: null
        ));
    }

    [Fact]
    public void ValidateWhenRequestWhenSummaryContainsLessThanOneSymbolShouldReturnInvalidResult()
    {
        //Arrange
        var command = _updatePersonalDataUseCaseCommandFaker.Generate() with
        {
            Summary = "h",
            Notes = "test"
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void ValidateWhenRequestWhenSummaryContainsMoreThanThousandSymbolShouldReturnInvalidResult()
    {
        //Arrange
        var command = _updatePersonalDataUseCaseCommandFaker.Generate() with
        {
            Summary = new string('a', 1001),
            Notes = "test"
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void ValidateWhenRequestWhenNotesContainsLessThanOneSymbolShouldReturnInvalidResult()
    {
        //Arrange
        var command = _updatePersonalDataUseCaseCommandFaker.Generate() with
        {
            Summary = "hasdf",
            Notes = "t"
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void ValidateWhenRequestWhenNotesContainsMoreThanThousandSymbolShouldReturnInvalidResult()
    {
        //Arrange
        var command = _updatePersonalDataUseCaseCommandFaker.Generate() with
        {
            Summary = "asdf",
            Notes = new string('s', 1001)
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void ValidateWhenRequestWhenNotesContainsMoreThanThousandSymbolShouldReturnValidResult()
    {
        //Arrange
        var command = _updatePersonalDataUseCaseCommandFaker.Generate() with
        {
            Summary = "summary",
            Notes = "some notes"
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void ValidateWhenRequestWhenNotesNullShouldReturnValidResult()
    {
        //Arrange
        var command = _updatePersonalDataUseCaseCommandFaker.Generate() with
        {
            Summary = "summary",
            Notes = null
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void ValidateWhenRequestWhenNotesEmptyShouldReturnValidResult()
    {
        //Arrange
        var command = _updatePersonalDataUseCaseCommandFaker.Generate() with
        {
            Summary = "summary",
            Notes = ""
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }
}
