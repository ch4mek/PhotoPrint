namespace Modsen.Portal.Equipment.Controllers.Contracts.Output;

internal class AssignementOutput
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int EquipmentId { get; set; }
    public string? UserDepartmentId { get; set; }
    public int? UserResourceGroupId { get; set; }
    public string? UserFirstName { get; set; }
    public string? UserLastName { get; set; }
    public string? UserFullName { get; set; }

    public string? UserFirstNameRu { get; set; }
    public string? UserLastNameRu { get; set; }
    public string? UserFullNameRu { get; set; }
    public string? EquipmentName { get; set; }
    public string? EquipmentTypeId { get; set; }
    public string? SerialNumber { get; set; }
    public string? InventoryNumber { get; set; }
    public DateTime IssueDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public bool IsAssigned { get; set; }
}
