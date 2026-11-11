using Modsen.Portal.Core.Cv;
using Modsen.Portal.Cv.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.Cv.Requests.Cvs.Core.CoreDto;

internal record CvCoreDto
{
    public int Id { get; set; }
    public required string CvName { get; set; }

    public int UserId { get; set; }
    public string? ProjectId { get; set; }
    public string? ClientId { get; set; }

    public CvStatus Status { get; set; }
    public int StatusOrderWeight { get; set; }
    public bool IsVisibleToUser { get; set; }

    public required string UserFirstName { get; set; }
    public required string UserLastName { get; set; }
    public string? UserProjectPosition { get; set; }
    public string? UserLocation { get; set; }
    public string? UserEducation { get; set; }
    public string? UserSkills { get; set; }
    public string? UserAbout { get; set; }

    public string? Website { get; set; }
    public bool IsCorporateDesign { get; set; }

    public int? CreatedById { get; set; }
    public DateTime CreatedAtUtc { get; set; }

    public List<WorkExperienceCoreDto> WorkExperiences { get; set; } = [];
    public List<LanguageCoreDto> Languages { get; set; } = [];
    public List<ProjectExperienceCoreDto> ProjectExperiences { get; set; } = [];
    public List<SkillGroupCoreDto> SkillGroups { get; set; } = [];

    public static CvCoreDto Of(CvEntity entity)
    {
        return new CvCoreDto
        {
            Id = entity.Id,
            CvName = entity.CvName,
            UserId = entity.UserId,
            ProjectId = entity.ProjectId,
            ClientId = entity.ClientId,
            Status = entity.Status,
            StatusOrderWeight = entity.StatusOrderWeight,
            IsVisibleToUser = entity.IsVisibleToUser,
            UserFirstName = entity.UserFirstName,
            UserLastName = entity.UserLastName,
            UserProjectPosition = entity.UserProjectPosition,
            UserLocation = entity.UserLocation,
            UserEducation = entity.UserEducation,
            UserSkills = entity.UserSkills,
            UserAbout = entity.UserAbout,
            Website = entity.Website,
            IsCorporateDesign = entity.IsCorporateDesign,
            CreatedById = entity.CreatedById,
            CreatedAtUtc = entity.CreatedAtUtc,
            WorkExperiences = entity.WorkExperiences.Select(WorkExperienceCoreDto.Of).ToList(),
            Languages = entity.Languages.Select(LanguageCoreDto.Of).ToList(),
            ProjectExperiences = entity.ProjectExperiences.Select(ProjectExperienceCoreDto.Of).ToList(),
            SkillGroups = entity.SkillGroups.Select(SkillGroupCoreDto.Of).ToList()
        };
    }
}
