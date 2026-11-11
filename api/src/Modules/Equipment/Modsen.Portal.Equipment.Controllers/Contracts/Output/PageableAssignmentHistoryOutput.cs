using Modsen.Portal.Core.Utils;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.Equipment.DataAccess.Npgsql.OData;

namespace Modsen.Portal.Equipment.Controllers.Contracts.Output;

internal class PageableAssignmentHistoryOutput : PageableListOfItems<AssignementOutput>
{
    public static PageableAssignmentHistoryOutput Of(PageableListOfItems<OdpAssignmentsHistory> pageableEquipments)
    {
        var equipmentItemsOutput = pageableEquipments
            .Items.Select(x => new AssignementOutput
            {
                Id = x.Id,
                EquipmentId = x.EquipmentId,
                UserId = x.UserId,
                IsAssigned = x.IsAssigned,
                IssueDate = x.IssueDate,
                ReturnDate = x.ReturnDate,
                UserFirstName = x.UserFirstName,
                UserFirstNameRu = x.UserFirstNameRu,
                UserLastNameRu = x.UserLastNameRu,
                UserLastName = x.UserLastName,
                UserFullName = x.UserFullName,
                UserFullNameRu = x.UserFullNameRu.IsNullOrEmpty() ? null : x.UserFullNameRu,
                UserDepartmentId = x.UserDepartmentId,
                UserResourceGroupId = x.UserResourceGroupId,
                EquipmentName = x.EquipmentName,
                EquipmentTypeId = x.EquipmentTypeId,
                SerialNumber = x.SerialNumber,
                InventoryNumber = x.InventoryNumber,
            }).ToList();

        return new PageableAssignmentHistoryOutput
        {
            Items = equipmentItemsOutput,
            Page = pageableEquipments.Page,
            PageSize = pageableEquipments.PageSize,
            Total = pageableEquipments.Total,
        };
    }
}
