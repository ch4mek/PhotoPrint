using Modsen.Portal.Core.TeamBoard;
using Modsen.Portal.TeamBoard.Requests.UserPersonalInfo.Core.CoreDto;

namespace Modsen.Portal.TeamBoard.Requests.UserPersonalInfo.Requests.RequestDto;

internal class PersonalInfoDto
{
    public string? FullName { get; set; }
    public string? FullNameRu { get; set; }

    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? FirstNameRu { get; set; }
    public string? LastNameRu { get; set; }
    public string? PatronymicRu { get; set; }

    public string? Email { get; set; }
    public string? PersonalEmail { get; set; }
    public string? PersonalEmailSecond { get; set; }
    public string? PersonalPhone { get; set; }
    public string? Telegram { get; set; }
    public string? LinkedIn { get; set; }
    public string? Position { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public string? Location { get; set; }
    public string? Seniority { get; set; }
    public string? PrimarySpecialization { get; set; }
    public string? DepartmentId { get; set; }
    public string? DepartmentFullName { get; set; }
    public int? ResourceGroupId { get; set; }
    public string? ResourceGroupName { get; set; }
    public string? EnglishLevel { get; set; }
    public string? Comments { get; set; }
    public string? Color { get; set; }
    public string? AvatarFileId { get; set; }
    public string? SoftSkillAverageSeniority { get; set; }
    public string? GeneralSeniority { get; set; }
    public string? CurrentSalary { get; set; }
    public DateTime? UpcomingPerformanceReview { get; set; }
    public string? PerformanceReviewDateStatus { get; set; }
    public string? RegisteredLocation { get; set; }
    public int? RegisteredLocationId { get; set; }
    public string? CurrentLocation { get; set; }
    public bool? NeedProject { get; set; }
    public MilitaryServiceStatus? MilitaryServiceStatus { get; set; }
    public bool? IsGraduateJobPlacement { get; set; }

    public static PersonalInfoDto Of(PersonalInfoCoreDto source, string? softSkillAverageSeniority,
        string? generalSeniority)
    {
        return new PersonalInfoDto
        {
            FullName = source.FullName,
            FullNameRu = source.FullNameRu,
            FirstName = source.FirstName,
            LastName = source.LastName,
            FirstNameRu = source.FirstNameRu,
            LastNameRu = source.LastNameRu,
            PatronymicRu = source.PatronymicRu,

            Email = source.Email,
            PersonalEmail = source.PersonalEmail,
            PersonalEmailSecond = source.PersonalEmailSecond,
            PersonalPhone = source.PersonalPhone,
            Telegram = source.Telegram,
            LinkedIn = source.LinkedIn,
            Position = source.Position,
            DateOfBirth = source.DateOfBirth,
            Location = source.Location,
            Seniority = source.SeniorityLevel,
            PrimarySpecialization = source.PrimarySpecialization,
            DepartmentId = source.DepartmentId,
            DepartmentFullName = source.DepartmentFullName,
            ResourceGroupId = source.ResourceGroupId,
            ResourceGroupName = source.ResourceGroupName,
            EnglishLevel = source.EnglishLevel,
            Comments = source.Comments,
            AvatarFileId = source.AvatarFileId,
            Color = source.Color,
            NeedProject = source.NeedProject,
            SoftSkillAverageSeniority = softSkillAverageSeniority,
            GeneralSeniority = generalSeniority,
            RegisteredLocation = source.RegisteredLocation,
            RegisteredLocationId = source.RegisteredLocationId,
            CurrentLocation = source.CurrentLocation,
            MilitaryServiceStatus = source.MilitaryServiceStatus,
            IsGraduateJobPlacement = source.IsGraduateJobPlacement
        };
    }
}
