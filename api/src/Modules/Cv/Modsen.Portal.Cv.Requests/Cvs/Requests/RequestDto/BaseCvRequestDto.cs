using FluentValidation;
using Modsen.Portal.Core.UseCases.ValidationRules;
using Modsen.Portal.Core.Utils.Extensions;

namespace Modsen.Portal.Cv.Requests.Cvs.Requests.RequestDto;

internal abstract class BaseCvRequestDto
{
    public required string CvName { get; set; }
    public int UserId { get; set; }
    public required string UserFirstName { get; set; }
    public required string UserLastName { get; set; }
    public string? UserProjectPosition { get; set; }
    public string? UserLocation { get; set; }
    public string? UserEducation { get; set; }
    public string? UserSkills { get; set; }
    public string? UserAbout { get; set; }
    public string? Website { get; set; }
    public bool IsCorporateDesign { get; set; }
    public List<CreateWorkExperienceDto> WorkExperiences { get; set; } = [];
    public List<CreateLanguageDto> Languages { get; set; } = [];
    public List<CreateProjectExperienceDto> ProjectExperiences { get; set; } = [];
    public List<CreateSkillGroupDto> SkillGroups { get; set; } = [];
}

internal sealed class BaseCvRequestDtoValidator : AbstractValidator<BaseCvRequestDto>
{
    public BaseCvRequestDtoValidator()
    {
        RuleFor(x => x.CvName)
            .NotNull()
            .NotEmpty()
            .ShortTextType();

        RuleFor(x => x.UserId)
            .NotNull()
            .NotEmpty();

        RuleFor(x => x.UserFirstName)
            .NotNull()
            .NotEmpty()
            .AlphabeticWithHyphensSpaces();

        RuleFor(x => x.UserLastName)
            .NotNull()
            .NotEmpty()
            .AlphabeticWithHyphensSpaces();

        When(x => !x.UserProjectPosition.IsNullOrEmpty(), () =>
            RuleFor(x => x.UserProjectPosition)
                .AlphabeticWithHyphensSpaces());

        When(x => !x.UserLocation.IsNullOrEmpty(), () =>
            RuleFor(x => x.UserLocation)
                .AlphabeticWithHyphensSpaces());

        When(x => !x.UserEducation.IsNullOrEmpty(), () =>
            RuleFor(x => x.UserEducation)
                .LongTextType1(required: false));

        When(x => !x.UserSkills.IsNullOrEmpty(), () =>
            RuleFor(x => x.UserSkills)
                .LongTextType1(required: false));

        When(x => !x.UserAbout.IsNullOrEmpty(), () =>
            RuleFor(x => x.UserAbout)
                .LongTextType1(required: false));

        When(x => x.IsCorporateDesign, () =>
            RuleFor(x => x.Website)
                .NotNull()
                .NotEmpty()
                .ShortTextType());

        RuleFor(x => x.IsCorporateDesign)
            .NotNull();

        RuleForEach(x => x.WorkExperiences)
            .SetValidator(new CreateWorkExperienceDto.Validator());

        RuleFor(x => x.WorkExperiences)
            .Must(x => x.Count <= 3);

        RuleForEach(x => x.Languages)
            .SetValidator(new CreateLanguageDto.Validator());

        RuleForEach(x => x.ProjectExperiences)
            .SetValidator(new CreateProjectExperienceDto.Validator());

        RuleForEach(x => x.SkillGroups)
            .SetValidator(new CreateSkillGroupDto.Validator());
    }
}
