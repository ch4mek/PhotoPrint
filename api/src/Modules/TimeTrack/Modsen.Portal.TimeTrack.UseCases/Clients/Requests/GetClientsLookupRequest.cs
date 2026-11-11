using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Identity.Contract;
using Modsen.Portal.TimeTrack.UseCases.Clients.Core.Loaders;

namespace Modsen.Portal.TimeTrack.UseCases.Clients.Requests;

internal sealed class GetClientsLookupRequest : IQueryHandler<GetClientsLookupRequest.Query, List<ClientLookupResponse>>
{
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly IClientLookupLoader _clientLookupLoader;
    private readonly IIdentityContract _identityContract;

    public GetClientsLookupRequest(
        IAuthenticatedUserContext authenticatedUserContext,
        IClientLookupLoader clientLookupLoader,
        IIdentityContract identityContract)
    {
        _authenticatedUserContext = authenticatedUserContext;
        _clientLookupLoader = clientLookupLoader;
        _identityContract = identityContract;
    }

    public async Task<List<ClientLookupResponse>> Handle(Query request, CancellationToken cancellationToken)
    {
        if (_authenticatedUserContext.HasClaim(StaffingClaimTypeNames.ViewProjectExits.Name)
            && !_authenticatedUserContext.HasClaim(StaffingClaimTypeNames.ManageAllProjectExits.Name)
            && !_authenticatedUserContext.IsCeo()
            && !_authenticatedUserContext.IsCto()
            && !_authenticatedUserContext.IsHeadOfDepartment()
            && !_authenticatedUserContext.IsGroupManager())
        {
            return [];
        }

        var forceAll = _authenticatedUserContext.HasClaim(StaffingClaimTypeNames.ManageAllProjectExits.Name);

        if (!forceAll)
        {
            await ValidateRequest(request, cancellationToken);
        }

        return await _clientLookupLoader.LoadAsync(new(
            _authenticatedUserContext.GetRoles(),
            request.KeyWord,
            request.IsActive,
            request.OnlyManagedClients,
            _authenticatedUserContext.GetRequiredUserId(),
            _authenticatedUserContext.ResourceGroupId,
            _authenticatedUserContext.DepartmentId,
            forceAll), cancellationToken);
    }

    private async Task ValidateRequest(Query request, CancellationToken cancellationToken)
    {
        if (!_authenticatedUserContext.GetRoles().IsEmployee() &&
            !_authenticatedUserContext.GetRoles().IsHeadOfDepartment() &&
            request.OnlyManagedClients != null)
        {
            throw new ActionNotAllowedException(PortalMessages.OnlyManagedUsersUsageRestriction);
        }

        if (_authenticatedUserContext.GetRoles().IsEmployee() &&
            request.OnlyManagedClients.HasValue &&
            request.OnlyManagedClients.Value)
        {
            var canUserBeManager = await _identityContract
                .CanUserBeProjectManagerByDepartment(_authenticatedUserContext.GetRequiredUserId(), cancellationToken);

            if (!canUserBeManager ||
                request.OnlyManagedClients is null ||
                request.OnlyManagedClients == false)
            {
                throw new ActionNotAllowedException(PortalMessages.UserCantBeManager);
            }
        }
    }

    internal sealed record Query(string? KeyWord, bool? IsActive, bool? OnlyManagedClients)
        : IQuery<List<ClientLookupResponse>>, ITimeTrackRequest;
}
