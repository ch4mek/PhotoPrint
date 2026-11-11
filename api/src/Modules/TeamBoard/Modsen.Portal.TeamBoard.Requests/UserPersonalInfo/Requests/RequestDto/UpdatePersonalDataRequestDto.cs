using FluentValidation;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.TeamBoard;
using Modsen.Portal.Core.UseCases.ValidationRules;
using Modsen.Portal.Core.Utils.Extensions;

namespace Modsen.Portal.TeamBoard.Requests.UserPersonalInfo.Requests.RequestDto;

internal sealed record UpdatePersonalDataRequestDto(
    int UserId,
    string FirstName,
    string LastName,
    string? FirstNameRu,
    string? LastNameRu,
    string? PatronymicRu,
    int? GroupId,
    string? DepartmentId,
    string? CorporateEmail,
    string? PersonalEmail,
    string? PersonalEmailSecond,
    string? PersonalPhone,
    string? Telegram,
    string? LinkedIn,
    string EnglishLevel,
    DateOnly? DateOfBirth,
    string Position,
    MilitaryServiceStatus? MilitaryServiceStatus,
    bool? IsGraduatedJobPlacement)
{
    internal sealed class Validator : AbstractValidator<UpdatePersonalDataRequestDto>
    {
        public Validator()
        {
            RuleFor(x => x.FirstNameRu)
                .NotNull()
                .CyrilicAlphabeticWithHyphensSpaces()
                .SpacesOrPunctuationSymbolsRule();

            RuleFor(x => x.LastNameRu)
                .NotNull()
                .CyrilicAlphabeticWithHyphensSpaces()
                .SpacesOrPunctuationSymbolsRule();

            RuleFor(x => x.PatronymicRu)
                .NotNull()
                .CyrilicAlphabeticWithHyphensSpaces()
                .SpacesOrPunctuationSymbolsRule();

            RuleFor(x => x.DateOfBirth)
                .LessThan(DateOnly.FromDateTime(DateTime.Now))
                .NotNull();

            When(x => !x.PersonalEmail.IsNullOrEmpty(), () =>
                RuleFor(x => x.PersonalEmail)
                    .EmailRuleWithCustomLength(PortalSharedConstants.EmailType2MinLength,
                        PortalSharedConstants.EmailMaxLength)
                    .WithMessage("Invalid email address format"));

            When(x => !x.PersonalEmailSecond.IsNullOrEmpty(), () =>
                RuleFor(x => x.PersonalEmailSecond)
                    .EmailRuleWithCustomLength(PortalSharedConstants.EmailType2MinLength,
                        PortalSharedConstants.EmailMaxLength)
                    .WithMessage("Invalid email address format"));

            When(x => !x.Telegram.IsNullOrEmpty(), () =>
                RuleFor(x => x.Telegram)
                    .ShortTextType()
                    .WithMessage("Invalid telegram format"));

            When(x => !x.LinkedIn.IsNullOrEmpty(), () =>
                RuleFor(x => x.LinkedIn)
                    .Link()
                    .LinkRule());

            When(x => !x.PersonalPhone.IsNullOrEmpty(), () =>
            {
                RuleFor(x => x.PersonalPhone)
                    .NotEmpty()
                    .PhoneNumberType()
                    .WithMessage("Invalid phone number format");
            });

            RuleFor(x => x.EnglishLevel)
                .NotNull()
                .NotEmpty();

            RuleFor(x => x.Position)
                .Must(position => position.IsEmployee() || position.IsHeadOfDepartment()
                    || position.IsGroupManager() || position.IsSystemAdministrator()
                    || position.IsIntern())
                .WithMessage("Invalid position value");
        }
    }
}
