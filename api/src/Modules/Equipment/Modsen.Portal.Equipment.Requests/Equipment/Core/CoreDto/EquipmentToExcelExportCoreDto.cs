using Modsen.Portal.Core.Equipment;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.Equipment.DataAccess.Npgsql.OData;

namespace Modsen.Portal.Equipment.Requests.Equipment.Core.CoreDto;

public class EquipmentToExcelExportCoreDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Type { get; set; }
    public required string SerialNumber { get; set; }
    public string? InventoryNumber { get; set; }
    public decimal PurchasePriceUsd { get; set; }
    public DateTime PurchaseDate { get; set; }
    public required string PurchasePlace { get; set; }
    public DateTime GuaranteeDate { get; set; }
    public required string Characteristics { get; set; }
    public required string Location { get; set; }
    public string? Comment { get; set; }
    public string? InvoiceNumber { get; set; }
    public string? Availability { get; set; }
    public string? Storage { get; set; }
    public string? RepairPlace { get; set; }
    public ApproverToExcelExportCoreDto? Approver { get; set; }
    public EquipmentHolderToExcelExportCoreDto? ActiveHolder { get; set; }

    public static EquipmentToExcelExportCoreDto Of(OdpEquipment source, IReadOnlyCollection<EquipmentDepartmentCoreDto> departments)
    {
        ArgumentNullException.ThrowIfNull(source);

        return new EquipmentToExcelExportCoreDto
        {
            Id = source.Id,
            Name = source.Name,
            Type = source.EquipmentTypeFullName,
            SerialNumber = source.SerialNumber,
            InventoryNumber = source.InventoryNumber,
            PurchasePriceUsd = source.PurchasePriceUsd,
            PurchaseDate = source.PurchaseDate,
            PurchasePlace = source.PurchasePlace,
            GuaranteeDate = source.GuaranteeDate,
            Characteristics = source.Characteristics,
            Availability = Enum.TryParse<EquipmentAvailabilityType>(source.Status, out var statusEnum)
                ? statusEnum.GetDescription()
                : source.Status,
            Location = source.Location.GetDescription(),
            Comment = source.Comment,
            InvoiceNumber = source.InvoiceNumber,
            Storage = source.Storage,
            RepairPlace = source.RepairPlace,
            Approver = new ApproverToExcelExportCoreDto
            {
                ApproverId = source.ApproverId!.Value,
                ApproverFullName = ToFullName.Forward(source.ApproverFirstName, source.ApproverLastName)
            },
            ActiveHolder = source is { ActiveHolderIssueDate: not null, ActiveHolderId: not null }
                ? new EquipmentHolderToExcelExportCoreDto
                {
                    Id = source.ActiveHolderId,
                    FullName = ToFullName.Forward(source.ActiveHolderFirstName, source.ActiveHolderLastName),
                    DepartmentFullName = departments.FirstOrDefault(departmentCoreDto =>
                        departmentCoreDto.Id == source.ActiveHolderDepartmentId)?.FullName
                }
                : null
        };
    }
}
