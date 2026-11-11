namespace Modsen.Portal.TeamBoard.Requests.Skills.SpecializationTemplates.Requests.RequestDto;

public class SpecializationTemplateDepartmentDto
{
    public required string DepartmentId { get; set; }
    public bool IsUserFromDepartmentAssigned { get; set; }
}
