namespace Modsen.Portal.Identity.Requests.ResourceGroups.Requests.RequestDtos;

internal class GroupDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int? GroupManagerId { get; set; }
    public string? ManagerFirstName { get; set; }
    public string? ManagerLastName { get; set; }
    public required string DepartmentId { get; set; }
}
