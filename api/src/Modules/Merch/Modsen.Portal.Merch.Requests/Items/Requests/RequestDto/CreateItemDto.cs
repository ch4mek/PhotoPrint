using FluentValidation;
using Modsen.Portal.Core.UseCases.ValidationRules;
using Modsen.Portal.Core.Utils.Extensions;

namespace Modsen.Portal.Merch.Requests.Items.Requests.RequestDto;

internal sealed record CreateItemDto(
    string Name,
    string? Supplier,
    string? Description,
    string? PhotoFileId,
    bool IsClothing,
    List<SizeMinAmountDto> SizeMinAmounts)
{
    internal sealed class Validator : AbstractValidator<CreateItemDto>
    {
        public Validator()
        {
            RuleFor(dto => dto.Name).ShortTextType();

            When(dto => !dto.Supplier.IsNullOrEmpty(),
                () => RuleFor(dto => dto.Supplier).ShortTextType());

            When(dto => !dto.Description.IsNullOrEmpty(),
                () => RuleFor(dto => dto.Description).LongTextType());

            When(dto => !dto.PhotoFileId.IsNullOrEmpty(),
                () => RuleFor(dto => dto.PhotoFileId).MaximumLength(32));

            RuleFor(dto => dto.SizeMinAmounts)
                .ForEach(collection => collection.SetValidator(new SizeMinAmountDto.Validator()));
        }
    }
}
