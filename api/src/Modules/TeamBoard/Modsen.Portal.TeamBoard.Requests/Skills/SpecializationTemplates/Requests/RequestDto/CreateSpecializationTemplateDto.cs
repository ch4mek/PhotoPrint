using FluentValidation;
using Modsen.Portal.Core.UseCases.ValidationRules;

namespace Modsen.Portal.TeamBoard.Requests.Skills.SpecializationTemplates.Requests.RequestDto;

internal sealed record CreateSpecializationTemplateDto(
    string Name,
    IReadOnlyCollection<string>? DepartmentIds,
    IReadOnlyCollection<int>? HardSkillIds)
{
    internal sealed class Validator : AbstractValidator<CreateSpecializationTemplateDto>
    {
        public Validator()
        {
            RuleFor(x => x.Name)
                .NotNull()
                .WithMessage("This field is required")
                .SpacesOrPunctuationSymbolsRule()
                .AlphanumericWithSpecialChars();
        }
    }
}
