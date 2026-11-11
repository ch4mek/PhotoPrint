using Modsen.Portal.Core;
using Modsen.Portal.Core.Equipment;

namespace Modsen.Portal.Equipment.Requests.Dto;

internal class EquipmentCoreDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string SerialNumber { get; set; }
    public string? InventoryNumber { get; set; }
    public string? TypeId { get; set; }
    public required EquipmentBrokenStatus BrokenStatus { get; set; }
    public decimal PurchasePrice { get; set; }
    public MoneyCurrencyType PurchaseCurrency { get; set; }
    public decimal PurchasePriceUsd { get; set; }
    public EquipmentLocationType Location { get; set; }
    public DateTime PurchaseDate { get; set; }
    public required string PurchasePlace { get; set; }
    public DateTime GuaranteeDate { get; set; }
    public required string Characteristics { get; set; }
    public string? Comment { get; set; }
    public string? InvoiceNumber { get; set; }
    public LocationType? Storage { get; set; }
    public string? RepairPlace { get; set; }
    public string? JiraTask { get; set; }

    public ApproverDto? Approver { get; set; }
    public EquipmentHolderDto? ActiveHolder { get; set; }
    public EquipmentTypeDto? Type { get; set; }

    public static EquipmentCoreDto Of(DataAccess.Npgsql.DatabaseEntities.EquipmentEntity source)
    {
        return new EquipmentCoreDto
        {
            Id = source.Id,
            Name = source.Name,
            SerialNumber = source.SerialNumber,
            InventoryNumber = source.InventoryNumber,
            TypeId = source.TypeId,
            BrokenStatus = source.BrokenStatus,
            PurchasePrice = source.PurchasePrice,
            PurchaseCurrency = source.PurchaseCurrency,
            PurchasePriceUsd = source.PurchasePriceUsd,
            Location = source.Location,
            PurchaseDate = source.PurchaseDate,
            PurchasePlace = source.PurchasePlace,
            GuaranteeDate = source.GuaranteeDate,
            Characteristics = source.Characteristics,
            Comment = source.Comment,
            InvoiceNumber = source.InvoiceNumber,
            Storage = source.Storage,
            RepairPlace = source.RepairPlace,
            JiraTask = source.JiraTask,

            Approver = ApproverDto.Of(source.Approver),
            Type = EquipmentTypeDto.Of(source.Type),
        };
    }
}
