using Modsen.Portal.Merch.Requests.Kits.Requests.RequestDto;

namespace Modsen.Portal.Merch.UnitTests.Kits.Requests.RequestDto;

public class CreateKitDtoValidatorTests
{
    private readonly CreateKitDto.Validator _validator = new();

    [Fact]
    public void ValidateWhenMoreThan10CompoundsShouldReturnFailure()
    {
        //Arrange
        var requestDto = new CreateKitDto(
            "name",
            "fileId",
            "description",
            new List<AddKitCompoundDto>()
            {
                new(1, 2),
                new(2, 2),
                new(3, 2),
                new(4, 2),
                new(5, 2),
                new(6, 2),
                new(7, 2),
                new(8, 2),
                new(9, 2),
                new(10, 2),
                new(11, 2),
            },
            new List<KitMinAmountDto>() { new(1, 2) });

        //Act
        var result = _validator.Validate(requestDto);

        //Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Select(error => error.PropertyName).Should().Contain("Compounds.Count");

    }

    [Fact]
    public void ValidateWhenLessThan1CompoundShouldReturnFailure()
    {
        //Arrange
        var requestDto = new CreateKitDto(
            "name",
            "fileId",
            "description",
            new List<AddKitCompoundDto>(),
            new List<KitMinAmountDto>() { new(1, 2) });

        //Act
        var result = _validator.Validate(requestDto);

        //Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Select(error => error.PropertyName).Should().Contain("Compounds.Count");

    }

    [Fact]
    public void ValidateWhenCompoundAmountMoreThan9999ShouldReturnFailure()
    {
        //Arrange
        var requestDto = new CreateKitDto(
            "name",
            "fileId",
            "description",
            new List<AddKitCompoundDto>()
            {
                new(1, 9999 + 1),
            },
            new List<KitMinAmountDto>() { new(1, 2) });

        //Act
        var result = _validator.Validate(requestDto);

        //Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Select(error => error.PropertyName).Should().Contain("Compounds[0]");

    }

    [Fact]
    public void ValidateWhenCompoundsDuplicatesShouldReturnFailure()
    {
        //Arrange
        var requestDto = new CreateKitDto(
            "name",
            "fileId",
            "description",
            new List<AddKitCompoundDto>()
            {
                new(1, 1),
                new(1, 2),
            },
            new List<KitMinAmountDto>() { new(1, 2) });

        //Act
        var result = _validator.Validate(requestDto);

        //Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Select(error => error.PropertyName).Should().Contain("Compounds");
        result.Errors.Select(error => error.ErrorMessage).Should().Contain("Compounds cannot be repeated");

    }

    [Fact]
    public void ValidateWhenMinAmountsInvalidShouldReturnFailure()
    {
        //Arrange
        var requestDto = new CreateKitDto(
            "name",
            "fileId",
            "description",
            new List<AddKitCompoundDto>()
            {
                new(1, 1),
                new(2, 2),
            },
            new List<KitMinAmountDto>() { new(1, 9999 + 1) });

        //Act
        var result = _validator.Validate(requestDto);

        //Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Select(error => error.PropertyName).Should().Contain("MinAmounts[0]");

    }
}
