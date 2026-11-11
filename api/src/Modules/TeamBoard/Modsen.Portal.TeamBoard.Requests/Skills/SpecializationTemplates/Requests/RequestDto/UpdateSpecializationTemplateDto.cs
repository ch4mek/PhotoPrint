using FluentValidation;
using Modsen.Portal.Core.UseCases.ValidationRules;

namespace Modsen.Portal.TeamBoard.Requests.Skills.SpecializationTemplates.Requests.RequestDto;

internal sealed record UpdateSpecializationTemplateDto(
    int Id,
    string Name,
    IReadOnlyCollection<string> DepartmentIds,
    IReadOnlyCollection<SpecializationTemplateHardSkillDto>? HardSkills)
{
    internal sealed class Validator : AbstractValidator<UpdateSpecializationTemplateDto>
    {
        public Validator()
        {
            RuleFor(x => x.Id)
                .NotNull()
                .NotEqual(0)
                .WithMessage("This field is required");
            RuleFor(x => x.Name)
                .NotNull()
                .WithMessage("This field is required")
                .SpacesOrPunctuationSymbolsRule()
                .AlphanumericWithSpecialChars();
        }
    }
}
