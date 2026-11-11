using System.Linq.Expressions;
using Modsen.Portal.Equipment.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.Equipment.DataAccess.Npgsql.OData;

public class OdpAssignmentsHistory
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int EquipmentId { get; set; }
    public string? UserDepartmentId { get; set; }
    public int? UserResourceGroupId { get; set; }
    public string? UserFirstName { get; set; }
    public string? UserLastName { get; set; }
    public string? UserFirstNameRu { get; set; }
    public string? UserLastNameRu { get; set; }
    public string? UserFullName { get; set; }
    public string? UserFullNameRu { get; set; }
    public required string EquipmentName { get; set; }
    public string? EquipmentTypeId { get; set; }
    public string? SerialNumber { get; set; }
    public string? InventoryNumber { get; set; }
    public DateTime IssueDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public bool IsAssigned { get; set; }

    internal static readonly Expression<Func<EquipmentAssignEntity, OdpAssignmentsHistory>> Map =
        x => new OdpAssignmentsHistory
        {
            Id = x.Id,
            UserId = x.User!.UserId,
            EquipmentId = x.EquipmentId,
            UserDepartmentId = x.User.DepartmentId,
            UserResourceGroupId = x.User.ResourceGroupId,

            UserFirstName = x.User.FirstName,
            UserLastName = x.User.LastName,
            UserFullName = x.User.LastName + ' ' + x.User.FirstName,

            UserFirstNameRu = x.User.FirstNameRu,
            UserLastNameRu = x.User.LastNameRu,
            UserFullNameRu = x.User.LastNameRu != null && x.User.FirstNameRu != null
                ? x.User.LastNameRu + ' ' + x.User.FirstNameRu
                : null,

            EquipmentName = x.Equipment!.Name,
            EquipmentTypeId = x.Equipment!.TypeId,
            SerialNumber = x.Equipment.SerialNumber,
            InventoryNumber = x.Equipment.InventoryNumber,
            IssueDate = x.IssueDate,
            ReturnDate = x.ReturnDate,
            IsAssigned = !(x.IsDeleted ||
                    x.ReturnDate <= DateTime.UtcNow ||
                    x.ReturnDate < x.Equipment.Assigns
                        .Where(a => !a.IsDeleted && a.ReturnDate != null)
                        .Max(a => a.ReturnDate))
        };
}
