using Modsen.Portal.Core.Utils;
using Modsen.Portal.Equipment.Requests.EquipmentPortalSettings.Dto;
using Modsen.Portal.Equipment.Requests.Users.Requests.RequestDtos;

namespace Modsen.Portal.Equipment.Controllers.Contracts.Output;

public class PageableUserEquipmentOutput : PageableListOfItems<UserEquipmentOutput>
{
    internal static PageableUserEquipmentOutput Of(PageableUserEquipmentDto pageableUserEquipments, EquipmentPortalSettingsDto settings)
    {
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(pageableUserEquipments);

        var output = pageableUserEquipments
            .Items.Select(
                x => new UserEquipmentOutput
                {
                    EquipmentTypeId = x.EquipmentTypeId,
                    EquipmentTypeName = x.EquipmentTypeName,
                    EquipmentName = x.EquipmentName,
                    SerialNumber = x.SerialNumber,
                    InventoryNumber = x.InventoryNumber,
                    IssueDate = x.IssueDate,
                    ReturnDate = x.ReturnDate,
                    IsAssigned = x.IsAssigned
                }).ToList();

        return new PageableUserEquipmentOutput
        {
            Items = output,
            Page = pageableUserEquipments.Page,
            PageSize = pageableUserEquipments.PageSize,
            Total = pageableUserEquipments.Total
        };
    }
}

