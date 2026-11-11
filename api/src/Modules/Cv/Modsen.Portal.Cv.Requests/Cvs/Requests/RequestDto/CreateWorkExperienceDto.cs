using FluentValidation;
using Modsen.Portal.Core.UseCases.ValidationRules;
using Modsen.Portal.Cv.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.Cv.Requests.Cvs.Requests.RequestDto;

internal record CreateWorkExperienceDto
{
    public required string Specialization { get; set; }
    public int ExperienceInYears { get; set; }

    public WorkExperienceEntity ToEntity(int? userId, DateTime now)
    {
        return new WorkExperienceEntity
        {
            Specialization = Specialization,
            ExperienceInYears = ExperienceInYears,
            CreatedById = userId,
            CreatedAtUtc = now
        };
    }

    internal sealed class Validator : AbstractValidator<CreateWorkExperienceDto>
    {
        public Validator()
        {
            RuleFor(x => x.Specialization)
                .NotNull()
                .NotEmpty()
                .ShortTextType();

            RuleFor(x => x.ExperienceInYears)
                .NotNull()
                .NotEmpty()
                .PositiveDigits();
        }
    }
}
