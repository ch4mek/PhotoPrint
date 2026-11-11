using Modsen.Portal.Equipment.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.Equipment.Requests.Dto;

internal class UserEquipmentDto
{
    public string? EquipmentTypeId { get; set; }
    public string? EquipmentTypeName { get; set; }
    public required string EquipmentName { get; set; }
    public required string SerialNumber { get; set; }
    public string? InventoryNumber { get; set; }
    public DateTime IssueDate { get; set; }
    public DateTime? ReturnDate { get; set; }

    public static UserEquipmentDto Of(EquipmentAssignEntity source)
    {
        return new UserEquipmentDto
        {
            EquipmentTypeId = source.Equipment?.TypeId,
            EquipmentTypeName = source.Equipment?.Type?.FullName,
            EquipmentName = source.Equipment!.Name,
            SerialNumber = source.Equipment.SerialNumber,
            InventoryNumber = source.Equipment?.InventoryNumber,
            IssueDate = source.IssueDate,
            ReturnDate = source.ReturnDate,
        };
    }
}
