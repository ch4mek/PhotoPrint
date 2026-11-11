using Modsen.Portal.Certificate.UseCases.Dto;
using Modsen.Portal.Certificate.UseCases.Requests;
using Modsen.Portal.Core.Certificate;

namespace Modsen.Portal.Certificate.UnitTests.UseCases.Validators;

public class CreateCertificateCommandValidatorTests
{
    [Theory]
    [InlineData("123 ~!@#$%^&()_+{}[]:;',.<>?=/\\")]
    [InlineData("123recep&ient/")]
    [InlineData("345346")]
    public void ValidateWhenRecepientIsInvalidShouldReturnInvalidResult(string recepient)
    {
        var validator = new CreateCertificateCommandUseCase.CreateCertificateCommandValidator();

        var command = new CreateCertificateCommandUseCase.Command(new CreateCertificateDto
        {
            Recipient = recepient,
            Technology = string.Empty
        });

        var result = validator.Validate(command);

        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [InlineData("recepient-test")]
    [InlineData("Recepient Test")]
    public void ValidateWhenRecepientIsValidShouldReturnValidResult(string recepient)
    {
        var validator = new CreateCertificateCommandUseCase.CreateCertificateCommandValidator();

        var command = new CreateCertificateCommandUseCase.Command(new CreateCertificateDto
        {
            Recipient = recepient,
            Technology = string.Empty
        });

        var result = validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("Js", null)]
    [InlineData("Net", JsStackType.React)]
    public void ValidateWhenDataIsInvalidShouldReturnInvalidResult(string departmentId, JsStackType? stack)
    {
        var validator = new CreateCertificateCommandUseCase.CreateCertificateCommandValidator();

        var command = new CreateCertificateCommandUseCase.Command(new CreateCertificateDto
        {
            Stack = stack,
            Technology = departmentId
        });

        var result = validator.Validate(command);

        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [InlineData("Js", JsStackType.React)]
    [InlineData("Net", null)]
    public void ValidateWhenDataIsValidShouldReturnValidResult(string departmentId, JsStackType? stack)
    {
        var validator = new CreateCertificateCommandUseCase.CreateCertificateCommandValidator();

        var command = new CreateCertificateCommandUseCase.Command(new CreateCertificateDto
        {
            Stack = stack,
            Technology = departmentId
        });

        var result = validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }
}
