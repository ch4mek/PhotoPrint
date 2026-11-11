using FluentValidation;
using Modsen.Portal.Core.UseCases.ValidationRules;
using Modsen.Portal.Cv.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.Cv.Requests.Cvs.Requests.RequestDto;

internal record CreateSkillGroupDto
{
    public required string Name { get; set; }
    public required string NameRu { get; set; }
    public bool IsCustom { get; set; }
    public List<CreateSkillDto> Skills { get; set; } = [];

    public SkillGroupEntity ToEntity(int? userId, DateTime now, int index)
    {
        return new SkillGroupEntity
        {
            Name = Name,
            NameRu = NameRu,
            IsCustom = IsCustom,
            Skills = Skills
                .Select((x, skillIndex) => x.ToEntity(userId, now, skillIndex))
                .ToList(),
            OrderWeight = index * 100
        };
    }

    internal sealed class Validator : AbstractValidator<CreateSkillGroupDto>
    {
        public Validator()
        {
            RuleFor(x => x.Name)
                .NotNull()
                .NotEmpty()
                .AlphabeticWithHyphensSpaces();

            RuleFor(x => x.NameRu)
                .NotNull()
                .NotEmpty()
                .AlphabeticWithHyphensSpaces();

            RuleFor(x => x.IsCustom)
                .NotNull();

            RuleForEach(x => x.Skills)
                .SetValidator(new CreateSkillDto.Validator());
        }
    }
}
