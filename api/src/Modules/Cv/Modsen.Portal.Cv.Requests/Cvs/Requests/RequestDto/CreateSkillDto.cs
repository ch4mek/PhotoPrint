using FluentValidation;
using Modsen.Portal.Core.Cv;
using Modsen.Portal.Core.UseCases.ValidationRules;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.Cv.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.Cv.Requests.Cvs.Requests.RequestDto;

internal record CreateSkillDto
{
    public string? Name { get; set; }
    public CvSkillLevel? Level { get; set; }
    public int? ExperienceInYears { get; set; }
    public int? LastUsedYear { get; set; }

    public SkillEntity ToEntity(int? userId, DateTime now, int index)
    {
        return new SkillEntity
        {
            Name = Name,
            Level = Level,
            ExperienceInYears = ExperienceInYears,
            LastUsedYear = LastUsedYear,
            OrderWeight = index * 100
        };
    }

    internal sealed class Validator : AbstractValidator<CreateSkillDto>
    {
        public Validator()
        {
            When(x => !x.Name.IsNullOrEmpty(), () =>
                RuleFor(x => x.Name)
                    .AlphabeticWithHyphensSpaces());

            When(x => x.Level is not null, () =>
                RuleFor(x => x.Level)
                    .IsInEnum());

            When(x => x.ExperienceInYears is not null, () =>
                RuleFor(x => x.ExperienceInYears!.Value)
                    .PositiveDigits());

            When(x => x.LastUsedYear is not null, () =>
                RuleFor(x => x.LastUsedYear!.Value)
                    .PositiveDigits());
        }
    }
}
