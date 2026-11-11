using FluentValidation;
using Modsen.Portal.Core.TeamBoard;
using Modsen.Portal.Core.UseCases.ValidationRules;
using Modsen.Portal.Core.Utils.Extensions;

namespace Modsen.Portal.TeamBoard.Requests.Education.Requests.RequestDto;

public class CreateUserEducationDto
{
    public int UserId { get; set; }
    public EducationLevel EducationLevel { get; set; }
    public required string EducationalInstitution { get; set; }
    public required string Faculty { get; set; }
    public required string Specialization { get; set; }
    public StudyForm StudyForm { get; set; }
    public int GraduationYear { get; set; }
    public string? Comment { get; set; }

    internal sealed class Validator : AbstractValidator<CreateUserEducationDto>
    {
        public Validator()
        {
            RuleFor(x => x.EducationLevel).IsInEnum();

            RuleFor(x => x.EducationalInstitution).ShortTextType();

            RuleFor(x => x.EducationalInstitution)
                .NotOnlyPunctuationMarks()
                .WithMessage("Please enter a valid educational institution");

            RuleFor(x => x.Faculty).ShortTextType();

            RuleFor(x => x.Faculty)
                .NotOnlyPunctuationMarks()
                .WithMessage("Please enter a valid faculty");

            RuleFor(x => x.Specialization).ShortTextType();

            RuleFor(x => x.Specialization)
                .NotOnlyPunctuationMarks()
                .WithMessage("Please enter a valid specialization");

            RuleFor(x => x.StudyForm).IsInEnum();

            RuleFor(x => x.GraduationYear).PositiveDigits()
                .WithMessage("Invalid graduation year format");

            RuleFor(x => x.Comment)
                .LongTextType()
                .When(x => !x.Comment.IsNullOrEmpty());

            When(x => x.Comment is not null, () =>
            {
                RuleFor(x => x.Comment)
                    .NotOnlyPunctuationMarks()
                    .WithMessage("Please enter a valid comment");
            });
        }
    }
}
