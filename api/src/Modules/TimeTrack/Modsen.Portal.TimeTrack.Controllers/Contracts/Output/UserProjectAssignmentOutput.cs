namespace Modsen.Portal.TimeTrack.Controllers.Contracts.Output;

public class UserProjectAssignmentOutput
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public int UserId { get; set; }
    public required string ProjectName { get; set; }
    public required string ClientName { get; set; }
    public required string UserFullName { get; set; }
    public DateOnly? UserOnProjectFrom { get; set; }
    public DateOnly? UserOnProjectTo { get; set; }
    public bool IsActive { get; set; }
    public bool IsTimeOnProjectBillable { get; set; }
}
