namespace Modsen.Portal.Identity.Contract.Models;

public class UserWithManagersDto
{
    public int UserId { get; set; }
    public int? ResourceGroupId { get; set; }
    public string? DepartmentId { get; set; }
    public int HeadOfDepartmentUserId { get; set; }
    public int ChiefId { get; set; }
    public int GroupManagerId { get; set; }
}
