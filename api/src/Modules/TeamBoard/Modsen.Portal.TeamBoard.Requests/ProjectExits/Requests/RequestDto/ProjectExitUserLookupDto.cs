namespace Modsen.Portal.TeamBoard.Requests.ProjectExits.Requests.RequestDto;

public class ProjectExitUserLookupDto
{
    public int Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? FirstNameRu { get; set; }
    public string? LastNameRu { get; set; }
    public string? DepartmentId { get; set; }
    public string? ResourceGroupName { get; set; }
    public string? Color { get; set; }
    public string? AvatarFileId { get; set; }
    public bool IsActive { get; set; }
    public int? ResourceGroupId { get; set; }
}
