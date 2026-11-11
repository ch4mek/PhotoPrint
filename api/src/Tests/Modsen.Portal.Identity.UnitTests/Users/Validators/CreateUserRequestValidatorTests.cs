using Bogus;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.TeamBoard;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.Identity.Requests.Users.Requests;
using Modsen.Portal.Identity.UnitTests.Users.Validators.TheoryDataProviders.Email;

namespace Modsen.Portal.Identity.UnitTests.Users.Validators;

public class CreateUserRequestValidatorTests
{
    private readonly CreateUserRequest.Validator _createUserValidator = new();

    private readonly Faker<CreateUserRequest.Command> _faker = new();

    public CreateUserRequestValidatorTests()
    {
        _faker.CustomInstantiator(x => new CreateUserRequest.Command(
            "Test name",
            "Test surname",
            "Тестовое имя",
            "Тестовая фамилия",
            "Тестовое отчество",
            "#fff",
            CountryId: 1,
            CurrentLocation: "Minsk",
            RegisteredLocationId: PortalSharedConstants.DefaultRegisteredLocationId,
            $"{Guid.CreateVersion7().ToAlphabetic()}@modsen-software.com",
            null, null, null, null, null,
            PortalRoleNames.Employee,
            LocationType.Minsk,
            "Js",
            1,
            MilitaryServiceStatus.Fit,
            null,
            ContractDateFrom: DateOnly.FromDateTime(DateTime.Today),
            ContractDateTo: DateOnly.FromDateTime(DateTime.Today).AddYears(1),
            HireDate: DateOnly.FromDateTime(DateTime.Today),
            DateOfBirth: new DateOnly(2000, 2, 2))
        );
    }

    [Theory]
    [ClassData(typeof(InvalidEmailsProvider))]
    public void ValidateWhenEmailIsInvalidShouldReturnFalse(string email)
    {
        var command = _faker.Clone().RuleFor(x => x.Email, email)
            .Generate();

        var result = _createUserValidator.Validate(command);

        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [ClassData(typeof(ValidEmailsProvider))]
    public void ValidateWhenEmailIsValidShouldReturnTrue(string email)
    {
        var command = _faker.Clone()
            .RuleFor(x => x.Email, _ => email)
            .Generate();

        var result = _createUserValidator.Validate(command);

        result.Errors.Should().BeEmpty();
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void ValidateWhenColorIsNotHexColorShouldReturnFalse()
    {
        var command = _faker.Clone()
            .RuleFor(x => x.Color, _ => "notHexColor")
            .Generate();

        var result = _createUserValidator.Validate(command);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void ValidateWhenFirstNameContainsOnlySpacesShouldReturnFalse()
    {
        var command = _faker.Clone()
            .RuleFor(x => x.FirstName, _ => "     ")
            .Generate();

        var result = _createUserValidator.Validate(command);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void ValidateWhenDepartmentIdIsNullShouldReturnTrue()
    {
        var command = _faker.Clone()
            .RuleFor(x => x.DepartmentId, _ => null)
            .Generate();

        var result = _createUserValidator.Validate(command);

        result.IsValid.Should().BeTrue();
    }
}
