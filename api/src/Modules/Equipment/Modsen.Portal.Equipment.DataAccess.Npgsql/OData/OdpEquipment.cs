using System.Linq.Expressions;
using Modsen.Portal.Core.Equipment;
using Modsen.Portal.Equipment.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.Equipment.DataAccess.Npgsql.OData;

public class OdpEquipment
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public EquipmentLocationType Location { get; set; }
    public required string Status { get; set; }
    public required string SerialNumber { get; set; }
    public string? InventoryNumber { get; set; }
    public decimal PurchasePrice { get; set; }
    public required string PurchaseCurrency { get; set; }
    public decimal PurchasePriceUsd { get; set; }
    public DateTime PurchaseDate { get; set; }
    public required string PurchasePlace { get; set; }
    public DateTime GuaranteeDate { get; set; }
    public required string Characteristics { get; set; }
    public string? Comment { get; set; }
    public string? InvoiceNumber { get; set; }

    public int? ApproverId { get; set; }
    public string? ApproverFullName { get; set; }
    public string? ApproverFullNameRu { get; set; }
    public string? ApproverFirstName { get; set; }
    public string? ApproverLastName { get; set; }

    public string? ApproverFirstNameRu { get; set; }
    public string? ApproverLastNameRu { get; set; }
    public string? EquipmentTypeId { get; set; }
    public string? EquipmentTypeShortName { get; set; }
    public string? EquipmentTypeFullName { get; set; }

    public int? ActiveHolderId { get; set; }
    public string? ActiveHolderFullName { get; set; }
    public string? ActiveHolderFirstName { get; set; }
    public string? ActiveHolderLastName { get; set; }

    public string? ActiveHolderFullNameRu { get; set; }
    public string? ActiveHolderFirstNameRu { get; set; }
    public string? ActiveHolderLastNameRu { get; set; }
    public string? ActiveHolderDepartmentId { get; set; }
    public DateTime? ActiveHolderIssueDate { get; set; }
    public DateTime? ActiveHolderReturnDate { get; set; }
    public int? ActiveHolderResourceGroupId { get; set; }
    public string? Storage { get; set; }
    public string? RepairPlace { get; set; }
    public string? JiraTask { get; set; }

    
    internal static readonly Expression<Func<EquipmentEntity, OdpEquipment>> Map =
        x => new OdpEquipment
        {
                Id = x.Id,
                Name = x.Name,
                Location = x.Location,
                Status = x.BrokenStatus == EquipmentBrokenStatus.None
                    ? (x.Assigns
                        .Any(a => !a.IsDeleted && (a.ReturnDate == null || a.ReturnDate > DateTime.UtcNow))
                        ? "Issued"
                        : "Available")
                    : x.BrokenStatus.ToString(),
                SerialNumber = x.SerialNumber,
                InventoryNumber = x.InventoryNumber,
                PurchasePrice = x.PurchasePrice,
                PurchaseCurrency = x.PurchaseCurrency.ToString(),
                PurchasePriceUsd = x.PurchasePriceUsd,
                PurchaseDate = x.PurchaseDate,
                PurchasePlace = x.PurchasePlace,
                GuaranteeDate = x.GuaranteeDate,
                Characteristics = x.Characteristics,
                Comment = x.Comment,
                InvoiceNumber = x.InvoiceNumber,

                ApproverId = x.ApproverId,
                ApproverFullName = x.Approver != null ? x.Approver.LastName + ' ' + x.Approver.FirstName : null,
                ApproverFullNameRu = x.Approver != null && x.Approver.LastNameRu != null && x.Approver.FirstNameRu != null
                    ? x.Approver.LastNameRu + ' ' + x.Approver.FirstNameRu
                    : null,
                ApproverFirstName = x.Approver != null ? x.Approver.FirstName : null,
                ApproverLastName = x.Approver != null ? x.Approver.LastName : null,
                ApproverFirstNameRu = x.Approver != null ? x.Approver.FirstNameRu : null,
                ApproverLastNameRu = x.Approver != null ? x.Approver.LastNameRu : null,

                EquipmentTypeId = x.TypeId,
                EquipmentTypeShortName = x.Type != null ? x.Type.ShortName : null,
                EquipmentTypeFullName = x.Type != null ? x.Type.FullName : null,

                ActiveHolderId = x.Assigns
                    .Where(a => !a.IsDeleted && (a.ReturnDate == null || a.ReturnDate > DateTime.UtcNow))
                    .OrderByDescending(a => a.IssueDate)
                    .Select(a => (int?)a.AssignedToUserId)
                    .FirstOrDefault(),

                ActiveHolderFullName = x.Assigns
                    .Where(a => !a.IsDeleted && (a.ReturnDate == null || a.ReturnDate > DateTime.UtcNow))
                    .OrderByDescending(a => a.IssueDate)
                    .Select(a => a.User != null ? a.User.LastName + ' ' + a.User.FirstName : null)
                    .FirstOrDefault(),

                ActiveHolderFirstName = x.Assigns
                    .Where(a => !a.IsDeleted && (a.ReturnDate == null || a.ReturnDate > DateTime.UtcNow))
                    .OrderByDescending(a => a.IssueDate)
                    .Select(a => a.User != null ? a.User.FirstName : null)
                    .FirstOrDefault(),

                ActiveHolderLastName = x.Assigns
                    .Where(a => !a.IsDeleted && (a.ReturnDate == null || a.ReturnDate > DateTime.UtcNow))
                    .OrderByDescending(a => a.IssueDate)
                    .Select(a => a.User != null ? a.User.LastName : null)
                    .FirstOrDefault(),

                ActiveHolderFullNameRu = x.Assigns
                    .Where(a => !a.IsDeleted && (a.ReturnDate == null || a.ReturnDate > DateTime.UtcNow))
                    .OrderByDescending(a => a.IssueDate)
                    .Select(a => a.User != null && a.User.LastNameRu != null && a.User.FirstNameRu != null
                        ? a.User.LastNameRu + ' ' + a.User.FirstNameRu
                        : null)
                    .FirstOrDefault(),

                ActiveHolderFirstNameRu = x.Assigns
                    .Where(a => !a.IsDeleted && (a.ReturnDate == null || a.ReturnDate > DateTime.UtcNow))
                    .OrderByDescending(a => a.IssueDate)
                    .Select(a => a.User != null ? a.User.FirstNameRu : null)
                    .FirstOrDefault(),

                ActiveHolderLastNameRu = x.Assigns
                    .Where(a => !a.IsDeleted && (a.ReturnDate == null || a.ReturnDate > DateTime.UtcNow))
                    .OrderByDescending(a => a.IssueDate)
                    .Select(a => a.User != null ? a.User.LastNameRu : null)
                    .FirstOrDefault(),

                ActiveHolderDepartmentId = x.Assigns
                    .Where(a => !a.IsDeleted && (a.ReturnDate == null || a.ReturnDate > DateTime.UtcNow))
                    .OrderByDescending(a => a.IssueDate)
                    .Select(a => a.User != null ? a.User.DepartmentId : null)
                    .FirstOrDefault(),

                ActiveHolderIssueDate = x.Assigns
                    .Where(a => !a.IsDeleted && (a.ReturnDate == null || a.ReturnDate > DateTime.UtcNow))
                    .OrderByDescending(a => a.IssueDate)
                    .Select(a => (DateTime?)a.IssueDate)
                    .FirstOrDefault(),

                ActiveHolderReturnDate = x.Assigns
                    .Where(a => !a.IsDeleted && (a.ReturnDate == null || a.ReturnDate > DateTime.UtcNow))
                    .OrderByDescending(a => a.IssueDate)
                    .Select(a => a.ReturnDate)
                    .FirstOrDefault(),

                ActiveHolderResourceGroupId = x.Assigns
                    .Where(a => !a.IsDeleted && (a.ReturnDate == null || a.ReturnDate > DateTime.UtcNow))
                    .OrderByDescending(a => a.IssueDate)
                    .Select(a => a.User != null ? a.User.ResourceGroupId : null)
                    .FirstOrDefault(),

                Storage = x.Storage != null ? x.Storage.ToString() : null,
                RepairPlace = x.RepairPlace,
                JiraTask = x.JiraTask
        };
 
}
