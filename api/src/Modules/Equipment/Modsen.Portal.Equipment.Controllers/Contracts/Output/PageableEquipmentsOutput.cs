using Modsen.Portal.Core;
using Modsen.Portal.Core.Equipment;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.Equipment.DataAccess.Npgsql.OData;

namespace Modsen.Portal.Equipment.Controllers.Contracts.Output;

public class PageableEquipmentsOutput : PageableListOfItems<EquipmentOutput>
{
    internal static PageableEquipmentsOutput Of(PageableListOfItems<OdpEquipment> source)
    {
        var equipmentItemsOutput = source
            .Items.Select(x =>
            {
                var locationDescription = x.Location.GetDescription();
                var purchaseCurrencyDescription = Enum.Parse<MoneyCurrencyType>(x.PurchaseCurrency, ignoreCase: true).GetDescription();
                var statusDescription = Enum.Parse<EquipmentAvailabilityType>(x.Status).GetDescription();

                return new EquipmentOutput
                {
                    Id = x.Id,
                    Name = x.Name,
                    EquipmentTypeId = x.EquipmentTypeId,
                    Status = statusDescription,
                    Location = locationDescription,
                    SerialNumber = x.SerialNumber,
                    InventoryNumber = x.InventoryNumber,
                    PurchasePrice = x.PurchasePrice,
                    PurchaseCurrency = purchaseCurrencyDescription,
                    PurchasePriceAndCurrency =
                        $"{x.PurchasePrice} {purchaseCurrencyDescription}",
                    PurchasePriceUsd = x.PurchasePriceUsd,
                    PurchaseDate = x.PurchaseDate,
                    PurchasePlace = x.PurchasePlace,
                    GuaranteeDate = x.GuaranteeDate,
                    Characteristics = x.Characteristics,
                    Comment = x.Comment,
                    InvoiceNumber = x.InvoiceNumber,
                    Storage = x.Storage.IsNullOrEmpty() ? null : Enum.Parse<LocationType>(x.Storage),
                    RepairPlace = x.RepairPlace,
                    JiraTask = x.JiraTask,
                    Approver = new ApproverOutput
                    {
                        Id = x.ApproverId!.Value,
                        FullName = x.ApproverFullName!,
                        FullNameRu = x.ApproverFullNameRu.GetValueOrNullIfEmpty()
                    },
                    ActiveHolder = x is { ActiveHolderId: not null, ActiveHolderIssueDate: not null }
                        ? new EquipmentHolderOutput
                        {
                            Id = x.ActiveHolderId.Value,
                            FullName = x.ActiveHolderFullName!,
                            DepartmentId = x.ActiveHolderDepartmentId,
                            IssueDate = x.ActiveHolderIssueDate.Value,
                            ReturnDate = x.ActiveHolderReturnDate,
                            ApproverId = x.ApproverId,
                            ApproverFullName = x.ApproverFullName,
                            ApproverFullNameRu = x.ApproverFullNameRu.GetValueOrNullIfEmpty()
                        }
                        : null,
                    Type = new EquipmentTypeOutput(x.EquipmentTypeId!, x.EquipmentTypeShortName!, x.EquipmentTypeFullName!)
                };
            }).ToList();

        return new PageableEquipmentsOutput
        {
            Items = equipmentItemsOutput,
            Page = source.Page,
            PageSize = source.PageSize,
            Total = source.Total,
        };
    }
}
