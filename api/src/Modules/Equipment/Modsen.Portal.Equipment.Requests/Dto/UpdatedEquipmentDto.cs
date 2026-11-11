using Modsen.Portal.Core;
using Modsen.Portal.Core.Equipment;
using Modsen.Portal.Equipment.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.Equipment.Requests.Dto;

internal record UpdatedEquipmentDto(
    int Id,
    string Name,
    string TypeId,
    EquipmentLocationType Location,
    EquipmentBrokenStatus BrokenStatus,
    string SerialNumber,
    string? InventoryNumber,
    decimal PurchasePrice,
    MoneyCurrencyType PurchaseCurrency,
    decimal PurchasePriceUsd,
    DateTime PurchaseDate,
    string PurchasePlace,
    DateTime GuaranteeDate,
    string Characteristics,
    string? Comment,
    string? InvoiceNumber,
    LocationType Storage,
    string? RepairPlace,
    int ApproverId,
    string JiraTask)
{
    public static UpdatedEquipmentDto Of(EquipmentEntity source)
    {
        return new UpdatedEquipmentDto(
            source.Id,
            source.Name,
            source.TypeId!,
            source.Location,
            source.BrokenStatus,
            source.SerialNumber,
            source.InventoryNumber,
            source.PurchasePrice,
            source.PurchaseCurrency,
            source.PurchasePriceUsd,
            source.PurchaseDate,
            source.PurchasePlace,
            source.GuaranteeDate,
            source.Characteristics,
            source.Comment,
            source.InvoiceNumber,
            source.Storage!.Value,
            source.RepairPlace,
            source.ApproverId!.Value,
            source.JiraTask!
        );
    }
}
