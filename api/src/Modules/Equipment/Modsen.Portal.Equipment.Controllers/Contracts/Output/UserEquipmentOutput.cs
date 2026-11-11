namespace Modsen.Portal.Equipment.Controllers.Contracts.Output;

public class UserEquipmentOutput
{
    public string? EquipmentTypeId { get; set; }
    public string? EquipmentTypeName { get; set; }
    public required string EquipmentName { get; set; }
    public string? SerialNumber { get; set; }
    public string? InventoryNumber { get; set; }
    public DateTime? IssueDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public bool IsAssigned { get; set; }
}
