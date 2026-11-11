using FluentValidation;
using Modsen.Portal.Core.Cv;
using Modsen.Portal.Core.UseCases.ValidationRules;
using Modsen.Portal.Cv.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.Cv.Requests.Cvs.Requests.RequestDto;

internal record CreateLanguageDto
{
    public required string Name { get; set; }
    public CvLanguageLevel Level { get; set; }

    public LanguageEntity ToEntity(int? userId, DateTime now)
    {
        return new LanguageEntity
        {
            Name = Name,
            Level = Level,
            CreatedById = userId,
            CreatedAtUtc = now
        };
    }

    internal sealed class Validator : AbstractValidator<CreateLanguageDto>
    {
        public Validator()
        {
            RuleFor(x => x.Name)
                .NotNull()
                .NotEmpty()
                .AlphabeticWithHyphensSpaces();

            RuleFor(x => x.Level)
                .NotNull()
                .IsInEnum();
        }
    }
}
