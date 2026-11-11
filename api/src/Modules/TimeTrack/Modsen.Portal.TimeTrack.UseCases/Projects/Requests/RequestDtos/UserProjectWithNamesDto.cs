using Modsen.Portal.TimeTrack.Domain;

namespace Modsen.Portal.TimeTrack.UseCases.Projects.Requests.RequestDtos;

public class UserProjectWithNamesDto
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public int UserId { get; set; }
    public string? ProjectName { get; set; }
    public required string UserFullName { get; set; }
    public bool IsTimeOnProjectBillable { get; set; }
    public DateOnly UserOnProjectFrom { get; set; }
    public DateOnly? UserOnProjectTo { get; set; }
    public decimal Fte { get; set; }
    public string? SeniorityLevel { get; set; }

    public bool IsAssignEditable { get; set; }

    public static UserProjectWithNamesDto? Of(ProjectUser? source, bool isAssignEditable)
    {
        if (source == null)
        {
            return null;
        }

        return new UserProjectWithNamesDto
        {
            Id = source.Id,
            ProjectId = source.ProjectId,
            UserId = source.UserId,
            ProjectName = source.ProjectName,
            UserFullName = $"{source.LastName} {source.FirstName}",
            UserOnProjectFrom = source.UserOnProjectFrom,
            UserOnProjectTo = source.UserOnProjectTo,
            IsTimeOnProjectBillable = source.IsTimeOnProjectBillable,
            Fte = source.Fte,
            SeniorityLevel = source.SeniorityLevel,
            IsAssignEditable = isAssignEditable,
        };
    }
}
