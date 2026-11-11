namespace Modsen.Portal.TimeTrack.Domain;

public class ProjectUser
{
    public required int Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public required int UserId { get; set; }
    public string? DepartmentId { get; set; }
    public required int ProjectId { get; set; }
    public string? ProjectName { get; set; }
    public string? ClientName { get; set; }
    public required DateOnly UserOnProjectFrom { get; set; }
    public DateOnly? UserOnProjectTo { get; set; }
    public string? SeniorityLevel { get; set; }
    public required decimal Fte { get; set; }
    public required bool IsTimeOnProjectBillable { get; set; }
}
