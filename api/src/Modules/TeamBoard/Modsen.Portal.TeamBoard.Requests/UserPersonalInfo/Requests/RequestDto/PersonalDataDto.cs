using Modsen.Portal.Core.TeamBoard;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.TeamBoard.Requests.UserPersonalInfo.Requests.RequestDto;

internal class PersonalDataDto
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? FirstNameRu { get; set; }
    public string? LastNameRu { get; set; }
    public string? Email { get; set; }
    public string? PersonalEmail { get; set; }
    public string? PersonalEmailSecond { get; set; }
    public string? PersonalPhone { get; set; }
    public string? Telegram { get; set; }
    public string? LinkedIn { get; set; }
    public MilitaryServiceStatus? MilitaryServiceStatus { get; set; }
    public bool? IsGraduateJobPlacement { get; set; }
    public string? EnglishLevelId { get; set; }
    public string? Position { get; set; }
    public DateOnly? DateOfBirth { get; set; }

    public static PersonalDataDto Of(TeamBoardUser source)
    {
        return new PersonalDataDto
        {
            FirstName = source.FirstName,
            LastName = source.LastName,
            FirstNameRu = source.FirstNameRu,
            LastNameRu = source.LastNameRu,
            Email = source.Email,
            PersonalEmail = source.PersonalEmail,
            PersonalEmailSecond = source.PersonalEmailSecond,
            PersonalPhone = source.PersonalPhone,
            Telegram = source.Telegram,
            LinkedIn = source.LinkedIn,
            MilitaryServiceStatus = source.MilitaryServiceStatus,
            IsGraduateJobPlacement = source.IsGraduateJobPlacement,
            EnglishLevelId = source.EnglishLevelId,
            DateOfBirth = source.DateOfBirth,
            Position = source.Position,
        };
    }
}
