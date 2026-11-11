using System.Linq.Expressions;
using Modsen.Portal.Core.TeamBoard;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.TeamBoard.DataAccess.Npgsql.OData;

public class OdpUserEducation
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public EducationLevel EducationLevel { get; set; }
    public required string EducationalInstitution { get; set; }
    public required string Faculty { get; set; }
    public required string Specialization { get; set; }
    public StudyForm StudyForm { get; set; }
    public int GraduationYear { get; set; }
    public string? Comment { get; set; }

    public int EducationLevelOrderWeight { get; set; }
    public int StudyFormOrderWeight { get; set; }

    internal static readonly Expression<Func<UserEducationEntity, OdpUserEducation>> MapUserEducation =
        x => new OdpUserEducation
        {
            Id = x.Id,
            UserId = x.UserId,
            EducationLevel = x.EducationLevel,
            EducationalInstitution = x.EducationalInstitution,
            Faculty = x.Faculty,
            Specialization = x.Specialization,
            StudyForm = x.StudyForm,
            GraduationYear = x.GraduationYear,
            Comment = x.Comment,
            EducationLevelOrderWeight = x.EducationLevelOrderWeight,
            StudyFormOrderWeight = x.StudyFormOrderWeight
        };
}
