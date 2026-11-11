namespace Modsen.Portal.TimeTrack.UseCases.Projects.Requests.RequestDtos;

public class UserProjectLookupDto
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public required string ProjectName { get; set; }
    public bool? IsTimeOnProjectBillable { get; set; }
    public string? ClientName { get; set; }
    public DateOnly UserOnProjectFrom { get; set; }
    public DateOnly? UserOnProjectTo { get; set; }
}
