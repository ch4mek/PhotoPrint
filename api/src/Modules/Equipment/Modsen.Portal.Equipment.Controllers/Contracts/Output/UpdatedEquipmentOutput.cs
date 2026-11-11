using Modsen.Portal.Core;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.Equipment.Requests.Dto;

namespace Modsen.Portal.Equipment.Controllers.Contracts.Output;

internal record UpdatedEquipmentOutput(
    int Id,
    string Name,
    string EquipmentTypeId,
    string Location,
    string BrokenStatus,
    string SerialNumber,
    string? InventoryNumber,
    decimal PurchasePrice,
    string PurchaseCurrency,
    string PurchasePriceAndCurrency,
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
    public static UpdatedEquipmentOutput Of(UpdatedEquipmentDto equipmentDto)
    {
        return new UpdatedEquipmentOutput(
            equipmentDto.Id,
            equipmentDto.Name,
            equipmentDto.TypeId,
            equipmentDto.Location.ToString(),
            equipmentDto.BrokenStatus.GetDescription(),
            equipmentDto.SerialNumber,
            equipmentDto.InventoryNumber,
            equipmentDto.PurchasePrice,
            equipmentDto.PurchaseCurrency.ToString(),
            $"{equipmentDto.PurchasePrice} {equipmentDto.PurchaseCurrency.GetDescription()}",
            equipmentDto.PurchasePriceUsd,
            equipmentDto.PurchaseDate,
            equipmentDto.PurchasePlace,
            equipmentDto.GuaranteeDate,
            equipmentDto.Characteristics,
            equipmentDto.Comment,
            equipmentDto.InvoiceNumber,
            equipmentDto.Storage,
            equipmentDto.RepairPlace,
            equipmentDto.ApproverId,
            equipmentDto.JiraTask);
    }
}
