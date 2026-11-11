namespace Modsen.Portal.TimeTrack.DataAccess.Abstractions.OData;

public class OdpProjectUser
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int ProjectId { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? DepartmentId { get; set; }
    public required string ProjectName { get; set; }
    public required string UserFullName { get; set; }
    public int? ResourceGroupId { get; set; }
    public bool? IsTimeOnProjectBillable { get; set; }
    public required string ClientName { get; set; }
    public DateOnly UserOnProjectFrom { get; set; }
    public DateOnly? UserOnProjectTo { get; set; }
    public bool IsActive { get; set; }
}
