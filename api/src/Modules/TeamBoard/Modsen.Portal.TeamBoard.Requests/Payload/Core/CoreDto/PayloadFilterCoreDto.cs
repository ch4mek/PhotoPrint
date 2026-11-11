namespace Modsen.Portal.TeamBoard.Requests.Payload.Core.CoreDto;

public class PayloadFilterCoreDto
{
    public bool? NeedProject { get; set; }
    public string? Comment { get; set; }
    public string? Plan { get; set; }
    public string? SpecializationName { get; set; }
    public string? ProjectName { get; set; }
    public string? UserId { get; set; }
    public string? UserFullName { get; set; }
    public IReadOnlyCollection<string>? DepartmentIds { get; set; }
    public string? ResourceGroupName { get; set; }
}
