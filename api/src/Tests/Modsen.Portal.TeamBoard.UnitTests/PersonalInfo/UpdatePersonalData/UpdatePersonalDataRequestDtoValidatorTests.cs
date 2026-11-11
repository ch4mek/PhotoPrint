using Bogus;
using Modsen.Portal.TeamBoard.Requests.UserPersonalInfo.Requests.RequestDto;

namespace Modsen.Portal.TeamBoard.UnitTests.PersonalInfo.UpdatePersonalData;

public class UpdatePersonalDataRequestDtoValidatorTests
{
    private readonly UpdatePersonalDataRequestDto.Validator _validator = new();
    private readonly Faker<UpdatePersonalDataRequestDto> _updatePersonalDataUseCaseCommandFaker = new();

    public UpdatePersonalDataRequestDtoValidatorTests()
    {
        _updatePersonalDataUseCaseCommandFaker.CustomInstantiator(x => new UpdatePersonalDataRequestDto(
            UserId: 1,
            FirstName: string.Empty,
            LastName: string.Empty,
            FirstNameRu: "Тест",
            LastNameRu: "Тест",
            PatronymicRu: "Тест",
            DepartmentId: string.Empty,
            GroupId: 1,
            CorporateEmail: string.Empty,
            PersonalEmail: string.Empty,
            PersonalEmailSecond: string.Empty,
            Telegram: string.Empty,
            LinkedIn: null,
            EnglishLevel: "A1",
            PersonalPhone: string.Empty,
            DateOfBirth: new DateOnly(2002, 2, 2),
            Position: string.Empty,
            MilitaryServiceStatus: null,
            IsGraduatedJobPlacement: null
        ));
    }

    [Theory]
    [MemberData(nameof(InvalidEmails))]
    public void ValidateWhenRequestIsInvalidShouldReturnInvalidResult(string email)
    {
        //Arrange
        var command = _updatePersonalDataUseCaseCommandFaker.Generate() with
        {
            PersonalEmail = email,
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();

        result.Errors.Where(x => x.PropertyName == "PersonalEmail")
            .Any(x => x.ErrorMessage == "Invalid email address format").Should().BeTrue();
    }

    [Theory]
    [MemberData(nameof(ValidEmails))]
    public void ValidateWhenLengthIsInvalidShouldReturnInvalidResult(string email)
    {
        //Arrange
        var command = _updatePersonalDataUseCaseCommandFaker.Generate() with
        {
            PersonalEmail = email,
            PersonalEmailSecond = "new" + email,
            Telegram = "aaaa",
            Position = "Employee"
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [MemberData(nameof(InvalidTelegrams))]
    public void ValidateWhenRequestTelegramIsInvalidShouldReturnInvalidResult(string telegram)
    {
        //Arrange
        const string email = "example12345674321@company.com";
        var command = _updatePersonalDataUseCaseCommandFaker.Generate() with
        {
            PersonalEmail = email,
            Telegram = telegram
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();

        result.Errors.Where(x => x.PropertyName == "Telegram")
            .Any(x => x.ErrorMessage == "Invalid telegram format").Should().BeTrue();

    }

    [Theory]
    [MemberData(nameof(ValidTelegrams))]
    public void ValidateWhenTelegramIsInvalidShouldReturnInvalidResult(string telegram)
    {
        //Arrange
        const string email = "example12345674321@company.com";
        const string secondEmail = "pexample12345674321@company.com";
        var command = _updatePersonalDataUseCaseCommandFaker.Generate() with
        {
            PersonalEmail = email,
            PersonalEmailSecond = secondEmail,
            Telegram = telegram,
            Position = "Employee"
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [MemberData(nameof(ValidPositions))]
    public void ValidateWhenPositionIsValidShouldReturnValidResult(string position)
    {
        //Arrange
        const string email = "example12345674321@company.com";
        var command = _updatePersonalDataUseCaseCommandFaker.Generate() with
        {
            PersonalEmail = email,
            PersonalEmailSecond = null,
            Telegram = "@test",
            Position = position,
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [MemberData(nameof(InvalidPositions))]
    public void ValidateWhenPositionIsInvalidShouldReturnInvalidResult(string position)
    {
        //Arrange
        const string email = "example12345674321@company.com";
        var command = _updatePersonalDataUseCaseCommandFaker.Generate() with
        {
            PersonalEmail = email,
            PersonalEmailSecond = null,
            Telegram = "@test",
            Position = position,
            DateOfBirth = new DateOnly(2000, 1, 1)
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();

        result.Errors.Where(x => x.PropertyName == "Position")
            .Any(x => x.ErrorMessage == "Invalid position value").Should().BeTrue();
    }

    [Theory]
    [MemberData(nameof(InvalidFirstNameRuValues))]
    public void ValidateWhenFirstNameRuIsInvalidShouldReturnInvalidResult(string? firstNameRu)
    {
        // Arrange
        var command = _updatePersonalDataUseCaseCommandFaker.Generate() with
        {
            FirstNameRu = firstNameRu
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Any(x => x.PropertyName == "FirstNameRu").Should().BeTrue();
    }

    [Theory]
    [MemberData(nameof(InvalidLastNameRuValues))]
    public void ValidateWhenLastNameRuIsInvalidShouldReturnInvalidResult(string? lastNameRu)
    {
        // Arrange
        var command = _updatePersonalDataUseCaseCommandFaker.Generate() with
        {
            LastNameRu = lastNameRu
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Any(x => x.PropertyName == "LastNameRu").Should().BeTrue();
    }

    [Theory]
    [MemberData(nameof(InvalidPatronymicRuValues))]
    public void ValidateWhenPatronymicRuIsInvalidShouldReturnInvalidResult(string? patronymicRu)
    {
        // Arrange
        var command = _updatePersonalDataUseCaseCommandFaker.Generate() with
        {
            PatronymicRu = patronymicRu
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Any(x => x.PropertyName == "PatronymicRu").Should().BeTrue();
    }

    public static IEnumerable<object[]> InvalidFirstNameRuValues =>
        new List<object[]>
        {
            new object[] { null },
            new object[] { "A" },
            new object[] { "Latin" },
            new object[] { new string('А', 251) }
        };

    public static IEnumerable<object[]> InvalidLastNameRuValues =>
        new List<object[]>
        {
            new object[] { null },
            new object[] { "A" },
            new object[] { "Latin" },
            new object[] { new string('А', 251) }
        };

    public static IEnumerable<object[]> InvalidPatronymicRuValues =>
        new List<object[]>
        {
            new object[] { null },
            new object[] { "A" },
            new object[] { "Latin" },
            new object[] { new string('А', 251) }
        };

    public static IEnumerable<object[]> InvalidEmails =>
        new List<object[]>
        {
            new object[] { "example123456789987654321@gmail..com" },
            new object[] { "example123456789987654321@@gmail.com" },
            new object[] { "example123456789987654321@gmail com" }
        };

    public static IEnumerable<object[]> ValidEmails =>
        new List<object[]>
        {
            new object[] { "example123456789987654321@gmail.com" },
            new object[] { "example12345674321@company.com" },
            new object[] { "example12347654321@company-software.com" }
        };

    public static IEnumerable<object[]> ValidTelegrams =>
        new List<object[]>
        {
            new object[] { "@a" },
            new object[] { "gdsyudsfjfds8435932-322411" },
            new object[] { new string('a', 250) },
            new object[] { null }
        };

    public static IEnumerable<object[]> InvalidTelegrams =>
        new List<object[]>
        {
            new object[] { "a" },
            new object[] { new string('a', 251) }
        };

    public static IEnumerable<object[]> ValidPositions =>
        new List<object[]>
        {
            new [] { "HeadOfDepartment" },
            new [] { "Employee" },
            new [] { "SystemAdministrator" },
            new [] { "GroupManager" }
        };

    public static IEnumerable<object[]> InvalidPositions =>
        new List<object[]>
        {
            new [] { "InvalidPosition" },
            new [] { "Manager" },
            new [] { "Director" },
            new [] { string.Empty },
            new [] { "employee" },
            new [] { "ceo" },
            new [] { "CEO" },
            new [] { "CTO" },
        };
}
