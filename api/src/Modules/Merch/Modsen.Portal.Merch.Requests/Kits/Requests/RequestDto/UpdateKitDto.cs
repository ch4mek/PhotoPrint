using FluentValidation;
using Modsen.Portal.Core.UseCases.ValidationRules;

namespace Modsen.Portal.Merch.Requests.Kits.Requests.RequestDto;

internal sealed record UpdateKitDto(
    int Id,
    string Name,
    string? PhotoFileId,
    string? Description,
    IReadOnlyCollection<AddKitCompoundDto> Compounds,
    IReadOnlyCollection<KitMinAmountDto> MinAmounts)
{
    internal sealed class Validator : AbstractValidator<UpdateKitDto>
    {
        public Validator()
        {
            RuleFor(dto => dto.Name)
                .ShortTextType();

            RuleFor(dto => dto.Description)
                .LongTextType()
                .When(dto => dto.Description != null);

            RuleFor(dto => dto.Compounds.Count)
                .GreaterThan(0)
                .LessThanOrEqualTo(10);

            RuleForEach(dto => dto.Compounds)
                .Must(compound => compound.AmountInKit is > 0 and <= 9999);

            RuleFor(dto => dto.Compounds)
                .Must(dtos => dtos.Count == dtos.Select(compound => compound.ItemId).Distinct().Count())
                .WithMessage("Compounds cannot be repeated");

            RuleForEach(dto => dto.MinAmounts)
                .Must(minAmount => minAmount.MinimumAmount is >= 0 and <= 9999);
        }
    }
}
