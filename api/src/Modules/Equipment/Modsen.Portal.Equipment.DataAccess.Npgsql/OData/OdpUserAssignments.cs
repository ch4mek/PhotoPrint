using System.Linq.Expressions;
using Modsen.Portal.Equipment.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.Equipment.DataAccess.Npgsql.OData;

public class OdpUserAssignments
{
    public int AssignedToUserId { get; set; }
    public string? EquipmentTypeId { get; set; }
    public string? EquipmentTypeName { get; set; }
    public required string EquipmentName { get; set; }
    public string? SerialNumber { get; set; }
    public string? InventoryNumber { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime IssueDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public bool IsAssigned { get; set; }

    internal static readonly Expression<Func<EquipmentAssignEntity, OdpUserAssignments>> Map =
        x => new OdpUserAssignments
        {
            AssignedToUserId = x.AssignedToUserId,
            EquipmentTypeId = x.Equipment!.TypeId,
            EquipmentTypeName = x.Equipment!.Type!.FullName,
            SerialNumber = x.Equipment.SerialNumber,
            InventoryNumber = x.Equipment.InventoryNumber,
            IssueDate = x.IssueDate,
            ReturnDate = x.ReturnDate,
            IsAssigned = !(x.IsDeleted ||
                    x.ReturnDate <= DateTime.UtcNow ||
                    x.ReturnDate < x.Equipment.Assigns
                        .Where(a => !a.IsDeleted && a.ReturnDate != null)
                        .Max(a => a.ReturnDate)),
            IsDeleted = x.IsDeleted,
            EquipmentName = x.Equipment.Name,
        };
}
