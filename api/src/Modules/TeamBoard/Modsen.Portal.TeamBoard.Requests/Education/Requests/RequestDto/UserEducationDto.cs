using Modsen.Portal.Core.TeamBoard;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.TeamBoard.Requests.Education.Requests.RequestDto;

internal class UserEducationDto
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

    internal static UserEducationDto Of(UserEducationEntity source)
    {
        return new UserEducationDto
        {
            Id = source.Id,
            UserId = source.UserId,
            EducationLevel = source.EducationLevel,
            EducationalInstitution = source.EducationalInstitution,
            StudyForm = source.StudyForm,
            Faculty = source.Faculty,
            Specialization = source.Specialization,
            GraduationYear = source.GraduationYear,
            Comment = source.Comment
        };
    }
}
