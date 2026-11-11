using Modsen.Portal.Core;
using Modsen.Portal.Core.Equipment;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.Equipment.Requests.Dto;

namespace Modsen.Portal.Equipment.Controllers.Contracts.Output;

public class EquipmentOutput
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Location { get; set; }
    public required string Status { get; set; }
    public required string SerialNumber { get; set; }
    public string? InventoryNumber { get; set; }
    public decimal PurchasePrice { get; set; }
    public required string PurchaseCurrency { get; set; }
    public required string PurchasePriceAndCurrency { get; set; }
    public decimal PurchasePriceUsd { get; set; }
    public DateTime PurchaseDate { get; set; }
    public required string PurchasePlace { get; set; }
    public DateTime GuaranteeDate { get; set; }
    public required string Characteristics { get; set; }
    public string? Comment { get; set; }
    public string? InvoiceNumber { get; set; }

    public string? EquipmentTypeId { get; set; }
    public LocationType? Storage { get; set; }
    public string? RepairPlace { get; set; }

    public string? JiraTask { get; set; }

    public required ApproverOutput? Approver { get; set; }
    public EquipmentHolderOutput? ActiveHolder { get; set; }
    public EquipmentTypeOutput? Type { get; set; }

    internal static EquipmentOutput Of(EquipmentCoreDto source)
    {
        return new EquipmentOutput()
        {
            Id = source.Id,
            Name = source.Name,
            Location = source.Location.GetDescription(),

            Status = source.BrokenStatus != EquipmentBrokenStatus.None
                ? source.BrokenStatus.GetDescription()
                : GetActiveHolderStatus(),

            SerialNumber = source.SerialNumber,
            InventoryNumber = source.InventoryNumber,

            PurchasePrice = source.PurchasePrice,
            PurchaseCurrency = source.PurchaseCurrency.GetDescription(),
            PurchasePriceAndCurrency = $"{source.PurchasePrice} {source.PurchaseCurrency.GetDescription()}",
            PurchasePriceUsd = source.PurchasePriceUsd,
            PurchaseDate = source.PurchaseDate,
            PurchasePlace = source.PurchasePlace,

            GuaranteeDate = source.GuaranteeDate,
            Characteristics = source.Characteristics,
            Comment = source.Comment,
            InvoiceNumber = source.InvoiceNumber,

            EquipmentTypeId = source.TypeId,
            Storage = source.Storage,
            RepairPlace = source.RepairPlace,

            JiraTask = source.JiraTask,

            Approver = ApproverOutput.Of(source.Approver),
            ActiveHolder = EquipmentHolderOutput.Of(source.ActiveHolder),
            Type = EquipmentTypeOutput.Of(source.Type),
        };

        string GetActiveHolderStatus()
        {
            return source.ActiveHolder == null
                ? EquipmentAvailabilityType.Available.GetDescription()
                : EquipmentAvailabilityType.Issued.GetDescription();
        }
    }
}
