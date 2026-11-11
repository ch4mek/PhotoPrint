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

namespace Modsen.Portal.Equipment.Requests.EquipmentHistory.Requests;

internal sealed class GetPageableAssignmentsHistoryRequest :
    IQueryHandler<GetPageableAssignmentsHistoryRequest.Query, PageableListOfItems<OdpAssignmentsHistory>>
{
    private readonly ICanAccessUnassignedEquipmentBusinessRule _canAccessUnassignedEquipment;
    private readonly IAuthenticatedUserContext _authenticatedUser;
    private readonly IEquipmentDbContext _equipmentDbContext;


    public GetPageableAssignmentsHistoryRequest(ICanAccessUnassignedEquipmentBusinessRule canAccessUnassignedEquipment,
        IAuthenticatedUserContext authenticatedUser,
        IEquipmentDbContext equipmentDbContext)
    {
        _canAccessUnassignedEquipment = canAccessUnassignedEquipment;
        _authenticatedUser = authenticatedUser;
        _equipmentDbContext = equipmentDbContext;
    }

    public async Task<PageableListOfItems<OdpAssignmentsHistory>> Handle(Query request, CancellationToken cancellationToken)
    {
        var queryOptionsWithoutPagination = request.QueryOptions.RebuildWithoutPagination();
        var query = GetFilteredQuery();

        var result = await request.QueryOptions
            .ApplyTo(query)
            .Cast<OdpAssignmentsHistory>()
            .ToListAsync(cancellationToken);

        var queryWithoutPagination = queryOptionsWithoutPagination
            .ApplyTo(query)
            .OfType<OdpAssignmentsHistory>();

        var totalCount = await queryWithoutPagination.CountAsync(cancellationToken);

        return result.ToPageable(request.QueryOptions, totalCount);
    }
    private IQueryable<OdpAssignmentsHistory> GetFilteredQuery()
    {
        var canAccessUnassignedEquipment = _canAccessUnassignedEquipment.IsCorrespondsToBusiness(_authenticatedUser);
        var hasManageAllEquipmentClaim = _authenticatedUser.HasClaim(EquipmentClaimTypeNames.ManageAllEquipment.Name);

        var departmentId = _authenticatedUser.DepartmentId;
        var resourceGroupId = _authenticatedUser.GetRoles().IsGroupManager() ? _authenticatedUser.ResourceGroupId : null;

        var query = _equipmentDbContext.EquipmentAssigns
            .Select(OdpAssignmentsHistory.Map)
            .AsQueryable();

        if (!canAccessUnassignedEquipment && !hasManageAllEquipmentClaim)
        {
            if (departmentId is not null)
            {
                query = query.Where(history => history.UserDepartmentId == departmentId);
            }

            if (resourceGroupId is not null)
            {
                query = query.Where(history => history.UserResourceGroupId == resourceGroupId);
            }
        }

        return query;
    }

    internal sealed record Query(ODataQueryOptions<OdpAssignmentsHistory> QueryOptions)
        : IQuery<PageableListOfItems<OdpAssignmentsHistory>>;

    internal sealed class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.QueryOptions).SetValidator(new ODataQueryOptionsValidator());
            RuleFor(x => x.QueryOptions.RawValues).DivisiblePaginationRule();
        }
    }
}
