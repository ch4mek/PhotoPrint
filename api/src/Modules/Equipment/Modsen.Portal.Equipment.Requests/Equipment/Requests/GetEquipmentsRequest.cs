using FluentValidation;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.OData;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.UseCases.ValidationRules;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.Equipment.DataAccess.Npgsql;
using Modsen.Portal.Equipment.DataAccess.Npgsql.OData;
using Modsen.Portal.Equipment.Requests.Core.BusinessRules;

namespace Modsen.Portal.Equipment.Requests.Equipment.Requests;

internal sealed class GetEquipmentsRequest
    : IQueryHandler<GetEquipmentsRequest.Query, PageableListOfItems<OdpEquipment>>
{
    private readonly ICanAccessUnassignedEquipmentBusinessRule _canAccessUnassignedEquipment;
    private readonly IAuthenticatedUserContext _authenticatedUser;
    private readonly IEquipmentDbContext _equipmentDbContext;

    public GetEquipmentsRequest(ICanAccessUnassignedEquipmentBusinessRule canAccessUnassignedEquipment,
        IAuthenticatedUserContext authenticatedUser,
        IEquipmentDbContext equipmentDbContext)
    {
        _canAccessUnassignedEquipment = canAccessUnassignedEquipment;
        _equipmentDbContext = equipmentDbContext;
        _authenticatedUser = authenticatedUser;
    }

    public async Task<PageableListOfItems<OdpEquipment>> Handle(Query query, CancellationToken cancellationToken)
    {
        var canAccessUnassignedEquipment = _canAccessUnassignedEquipment.IsCorrespondsToBusiness(_authenticatedUser);

        return await GetPageableEquipments(query, canAccessUnassignedEquipment, cancellationToken);
    }

    private async Task<PageableListOfItems<OdpEquipment>> GetPageableEquipments(Query request,
        bool canAccessUnassignedEquipment, CancellationToken cancellationToken)
    {
        var query = _equipmentDbContext.Equipments
            .Include(x => x.Approver)
            .Include(x => x.Assigns)
            .AsSplitQuery()
            .Select(OdpEquipment.Map)
            .AsQueryable();

        var canManageAll = _authenticatedUser.HasClaim(EquipmentClaimTypeNames.ManageAllEquipment.Name);

        if (!canManageAll && !canAccessUnassignedEquipment)
        {
            query = FilterWithDepartmentAndResourceGroup(query);
        }

        var queryOptionsWithoutPagination = request.QueryOptions.RebuildWithoutPagination();

        var result = await request.QueryOptions
            .ApplyTo(query)
            .Cast<OdpEquipment>()
            .ToListAsync(cancellationToken);

        var totalCount = await queryOptionsWithoutPagination
            .ApplyTo(query)
            .OfType<OdpEquipment>()
            .CountAsync(cancellationToken);

        var pageableEquipments = result.ToPageable(request.QueryOptions, totalCount);
        var isHeadOrGroupManager = _authenticatedUser.GetRoles().IsHeadOfDepartment() || _authenticatedUser.GetRoles().IsGroupManager();
        if (!canManageAll && isHeadOrGroupManager)
        {
            HideEquipmentsPrice(pageableEquipments);
        }

        return pageableEquipments;
    }

    private IQueryable<OdpEquipment> FilterWithDepartmentAndResourceGroup(IQueryable<OdpEquipment> query)
    {
        var departmentId = _authenticatedUser.DepartmentId;
        var resourceGroupId = _authenticatedUser.GetRoles().IsGroupManager() ? _authenticatedUser.ResourceGroupId : null;

        query = query.Where(x => x.ActiveHolderId != null);

        if (departmentId is not null)
        {
            query = query.Where(x => x.ActiveHolderDepartmentId == departmentId);
        }

        if (resourceGroupId is not null)
        {
            query = query.Where(x => x.ActiveHolderResourceGroupId == resourceGroupId);
        }

        return query;
    }

    private static void HideEquipmentsPrice(PageableListOfItems<OdpEquipment> pageableEquipments)
    {
        pageableEquipments.Items.ToList().ForEach(e =>
        {
            e.PurchasePlace = string.Empty;
            e.PurchasePriceUsd = 0;
            e.PurchasePrice = 0;
        });
    }

    internal sealed record Query(ODataQueryOptions<OdpEquipment> QueryOptions)
        : IQuery<PageableListOfItems<OdpEquipment>>, IEquipmentRequest;

    internal sealed class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.QueryOptions).SetValidator(new ODataQueryOptionsValidator());
            RuleFor(x => x.QueryOptions.RawValues).DivisiblePaginationRule();
        }
    }
}
