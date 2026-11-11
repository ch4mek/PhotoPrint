using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Equipment.DataAccess.Npgsql;
using Modsen.Portal.Equipment.Requests.Core.BusinessRules;
using Modsen.Portal.Equipment.Requests.Equipment.Core.CoreDto;
using Modsen.Portal.Equipment.Requests.Equipment.Requests.RequestDto;
using Modsen.Portal.Identity.Contract;

namespace Modsen.Portal.Equipment.Requests.Equipment.Requests;

internal sealed class GetEquipmentFiltersRequest : IQueryHandler<GetEquipmentFiltersRequest.Query, AvailableFilterOptionsDto>
{
    private readonly ICanAccessUnassignedEquipmentBusinessRule _canAccessUnassignedEquipment;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly IEquipmentDbContext _equipmentDbContext;
    private readonly IIdentityContract _identityContract;

    private CancellationToken _cancellationToken;

    public GetEquipmentFiltersRequest(ICanAccessUnassignedEquipmentBusinessRule canAccessUnassignedEquipment,
        IAuthenticatedUserContext authenticatedUserContext,
        IEquipmentDbContext equipmentDbContext,
        IIdentityContract identityContract)
    {
        _canAccessUnassignedEquipment = canAccessUnassignedEquipment;
        _authenticatedUserContext = authenticatedUserContext;
        _equipmentDbContext = equipmentDbContext;
        _identityContract = identityContract;
    }

    public async Task<AvailableFilterOptionsDto> Handle(Query request, CancellationToken cancellationToken)
    {
        _cancellationToken = cancellationToken;

        var managersIds = await _identityContract.GetManagerIds(cancellationToken);
        var possibleEquipmentApprovers = await GetPossibleEquipmentApproversAsync(managersIds);

        var possibleEquipmentActiveHolders = await GetPossibleEquipmentActiveHoldersAsync();

        return new AvailableFilterOptionsDto
        {
            Approvers = possibleEquipmentApprovers,
            Users = possibleEquipmentActiveHolders
        };
    }

    private Task<List<UserFilterCoreDto>> GetPossibleEquipmentApproversAsync(List<int> managerIds)
    {
        return _equipmentDbContext.EquipmentUsers
            .Where(user => user.IsActive && managerIds.Contains(user.UserId))
            .OrderBy(user => user.UserId)
            .Select(user => new UserFilterCoreDto
                {
                    Id = user.UserId,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    FirstNameRu = user.FirstNameRu ?? string.Empty,
                    LastNameRu = user.LastNameRu ?? string.Empty,
                })
            .ToListAsync(_cancellationToken);
    }

    private Task<List<UserFilterCoreDto>> GetPossibleEquipmentActiveHoldersAsync()
    {
        var canAccessUnassignedEquipment = _canAccessUnassignedEquipment
            .IsCorrespondsToBusiness(_authenticatedUserContext);
        var canManageAll = _authenticatedUserContext.HasClaim(EquipmentClaimTypeNames.ManageAllEquipment.Name);

        var departmentId = _authenticatedUserContext.DepartmentId;
        var resourceGroupId = _authenticatedUserContext.GetRoles().IsGroupManager()
            ? _authenticatedUserContext.ResourceGroupId
            : null;

        var query = _equipmentDbContext.EquipmentUsers.Where(user => user.IsActive);

        if (!canManageAll && !canAccessUnassignedEquipment)
        {
            if(departmentId is not null)
            {
                query = query.Where(equipmentUser => equipmentUser.DepartmentId == departmentId);
            }

            if (resourceGroupId is not null)
            {
                query = query.Where(equipmentUser => equipmentUser.ResourceGroupId == resourceGroupId);
            }
        }

        return query
            .OrderBy(user => user.UserId)
            .Select(user => new UserFilterCoreDto
            {
                Id = user.UserId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                FirstNameRu = user.FirstNameRu ?? string.Empty,
                LastNameRu = user.LastNameRu ?? string.Empty,
            })
            .ToListAsync(_cancellationToken);
    }

    internal sealed record Query : IQuery<AvailableFilterOptionsDto>, IEquipmentRequest;
}
