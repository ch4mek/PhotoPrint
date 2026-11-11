using FluentValidation;
using Modsen.Portal.Core.UseCases.ValidationRules;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.Cv.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.Cv.Requests.Cvs.Requests.RequestDto;

internal class CreateProjectExperienceDto
{
    public string? Position { get; set; }
    public string? TeamSize { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public bool IsCurrent { get; set; }
    public string? Description { get; set; }
    public string? Responsibilities { get; set; }
    public string? Environment { get; set; }

    public ProjectExperienceEntity ToEntity(int? userId, DateTime now, int index)
    {
        return new ProjectExperienceEntity
        {
            Position = Position,
            TeamSize = TeamSize,
            StartDate = StartDate,
            EndDate = EndDate,
            IsCurrent = IsCurrent,
            Description = Description,
            Responsibilities = Responsibilities,
            Environment = Environment,
            OrderWeight = index * 100,
            CreatedById = userId,
            CreatedAtUtc = now
        };
    }

    internal sealed class Validator : AbstractValidator<CreateProjectExperienceDto>
    {
        public Validator()
        {
            When(x => !x.Position.IsNullOrEmpty(), () =>
                RuleFor(x => x.Position)
                    .AlphabeticWithHyphensSpaces());

            When(x => !x.TeamSize.IsNullOrEmpty(), () =>
                RuleFor(x => x.TeamSize)
                    .Length(1, 250));

            When(x => x.EndDate is not null && x.StartDate is not null, () =>
                RuleFor(x => x.EndDate)
                    .GreaterThan(x => x.StartDate)
                    .When(x => !x.IsCurrent));

            When(x => x.StartDate is not null, () =>
                RuleFor(x => x.StartDate)
                    .LessThan(x => x.EndDate)
                    .When(x => x.EndDate is not null));

            When(x => !x.Description.IsNullOrEmpty(), () =>
                RuleFor(x => x.Description)
                    .LongTextType1(required: false));

            When(x => !x.Responsibilities.IsNullOrEmpty(), () =>
                RuleFor(x => x.Responsibilities)
                    .LongTextType1(required: false));

            When(x => !x.Environment.IsNullOrEmpty(), () =>
                RuleFor(x => x.Environment)
                    .LongTextType1(required: false));
        }
    }
}
