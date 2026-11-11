using Modsen.Portal.Core.TeamBoard;
using Modsen.Portal.TeamBoard.Requests.UserPersonalInfo.Requests.RequestDto;

namespace Modsen.Portal.TeamBoard.Controllers.Contracts.Output;

internal sealed record PersonalDataOutput(
    string? FirstName,
    string? LastName,
    string? FirstNameRu,
    string? LastNameRu,
    string? Email,
    string? PersonalEmail,
    string? PersonalEmailSecond,
    string? PersonalPhone,
    string? Telegram,
    string? LinkedIn,
    MilitaryServiceStatus? MilitaryServiceStatus,
    bool? IsGraduateJobPlacement,
    string? EnglishLevelId,
    DateOnly? DateOfBirth,
    string? Position)
{
    public static PersonalDataOutput Of(PersonalDataDto source)
    {
        return new PersonalDataOutput(
            FirstName: source.FirstName,
            LastName: source.LastName,
            FirstNameRu: source.FirstNameRu,
            LastNameRu: source.LastNameRu,
            Email: source.Email,
            PersonalEmail: source.PersonalEmail,
            PersonalEmailSecond: source.PersonalEmailSecond,
            PersonalPhone: source.PersonalPhone,
            Telegram: source.Telegram,
            LinkedIn: source.LinkedIn,
            MilitaryServiceStatus: source.MilitaryServiceStatus,
            IsGraduateJobPlacement: source.IsGraduateJobPlacement,
            EnglishLevelId: source.EnglishLevelId,
            DateOfBirth: source.DateOfBirth,
            Position: source.Position);
    }
}
