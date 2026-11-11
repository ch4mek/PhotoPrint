using FluentValidation;
using Modsen.Portal.Core.UseCases.ValidationRules;

namespace Modsen.Portal.Merch.Requests.Items.Requests.RequestDto;

internal sealed record SizeMinAmountDto(
    string SizeId,
    int StorageId,
    int MinAmount)
{
    internal sealed class Validator : AbstractValidator<SizeMinAmountDto>
    {
        public Validator()
        {
            RuleFor(x => x.MinAmount)
                .Digits()
                .WithMessage("You can enter value from 0 to 9999");
        }
    }
}
