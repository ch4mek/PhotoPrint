using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Equipment.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Equipment.Requests.Core.DbQueries;
using Modsen.Portal.Equipment.Requests.Dto;

namespace Modsen.Portal.Equipment.Requests.Equipment.Requests;

internal sealed class GetEquipmentByIdRequest : IQueryHandler<GetEquipmentByIdRequest.Query, EquipmentCoreDto>
{
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly IGetEquipmentByIdDbQuery _getEquipmentByIdDbQuery;

    public GetEquipmentByIdRequest(IAuthenticatedUserContext authenticatedUserContext,
        IGetEquipmentByIdDbQuery getEquipmentByIdDbQuery)
    {
        _authenticatedUserContext = authenticatedUserContext;
        _getEquipmentByIdDbQuery = getEquipmentByIdDbQuery;
    }

    public async Task<EquipmentCoreDto> Handle(Query request, CancellationToken cancellationToken)
    {
        var equipment = await _getEquipmentByIdDbQuery.ExecuteAsync(request.Id, cancellationToken)
                ?? throw new EntityNotFoundException(PortalMessages.EquipmentNotFound);

        var activeHolder = equipment.Assigns
            .Where(equipmentAssign => equipmentAssign is { IsDeleted: false, ReturnDate: null })
            .MinBy(equipmentAssign => equipmentAssign.IssueDate);

        CheckIfUserHasAccess(activeHolder);

        var equipmentDto = EquipmentCoreDto.Of(equipment);
        var hasManageAllEquipmentClaim = _authenticatedUserContext.HasClaim(EquipmentClaimTypeNames.ManageAllEquipment.Name);
        var roles = _authenticatedUserContext.GetRoles();
        var hasNoAccessToPrice = roles.IsHeadOfDepartment() || roles.IsGroupManager() || roles.IsEmployee();
        if (hasNoAccessToPrice && !hasManageAllEquipmentClaim)
        {
            equipmentDto.PurchasePlace = string.Empty;
            equipmentDto.PurchasePriceUsd = 0;
            equipmentDto.PurchasePrice = 0;
        }

        if (activeHolder is not null)
        {
            equipmentDto.ActiveHolder = EquipmentHolderDto.Of(activeHolder);
        }

        return equipmentDto;
    }

    private void CheckIfUserHasAccess(EquipmentAssignEntity? activeHolder)
    {
        if (_authenticatedUserContext.HasClaim(EquipmentClaimTypeNames.ManageAllEquipment.Name))
        {
            return;
        }

        var roles = _authenticatedUserContext.GetRoles();
        var isHeadFromAnotherDepartment = roles.IsHeadOfDepartment() && (activeHolder is null ||
                activeHolder.User!.DepartmentId is null ||
                activeHolder.User.DepartmentId != _authenticatedUserContext.DepartmentId);

        if (isHeadFromAnotherDepartment)
        {
            throw new ActionNotAllowedException(PortalMessages.HeadCannotSeeOtherDepartmentEquipment);
        }

        var isGroupManagerFromAnotherGroup = roles.IsGroupManager() && (activeHolder is null ||
                activeHolder.User!.ResourceGroupId != _authenticatedUserContext.ResourceGroupId);
        if (isGroupManagerFromAnotherGroup)
        {
            throw new ActionNotAllowedException(PortalMessages.GroupManagerCannotSeeOtherResourceGroupEquipment);
        }
    }

    internal sealed record Query(int Id) : IQuery<EquipmentCoreDto>, IEquipmentRequest;

}
