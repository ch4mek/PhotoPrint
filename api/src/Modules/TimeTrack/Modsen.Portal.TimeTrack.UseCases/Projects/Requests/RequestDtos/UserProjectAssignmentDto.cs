namespace Modsen.Portal.TimeTrack.UseCases.Projects.Requests.RequestDtos;

internal class UserProjectAssignmentDto
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public int UserId { get; set; }
    public required string UserFullName { get; set; }
    public required string ProjectName { get; set; }
    public required string ClientName { get; set; }
    public DateOnly? UserOnProjectFrom { get; set; }
    public DateOnly? UserOnProjectTo { get; set; }
    public bool IsActive { get; set; }
    public bool IsTimeOnProjectBillable { get; set; }
}
