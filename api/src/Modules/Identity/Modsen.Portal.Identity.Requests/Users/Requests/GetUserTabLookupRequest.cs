using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.Requests.Users.Core;
using Modsen.Portal.Identity.Requests.Users.Core.CoreDto;
using Modsen.Portal.Identity.Requests.Users.Requests.RequestDto;
using Modsen.Portal.TimeTrack.Contract;

namespace Modsen.Portal.Identity.Requests.Users.Requests;

internal sealed class GetUserTabLookupRequest : IQueryHandler<GetUserTabLookupRequest.Query, List<UserLookupDto>>
{
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly IIdentityDbContext _identityDbContext;
    private readonly IUserLookupLoader _userTabLookupLoader;
    private readonly ITimeTrackContract _timeTrackContract;

    public GetUserTabLookupRequest(
        IAuthenticatedUserContext authenticatedUserContext,
        IUserLookupLoader userTabLookupLoader,
        IIdentityDbContext identityDbContext,
        ITimeTrackContract timeTrackContract)
    {
        _authenticatedUserContext = authenticatedUserContext;
        _identityDbContext = identityDbContext;
        _timeTrackContract = timeTrackContract;
        _userTabLookupLoader = userTabLookupLoader;
    }

    public async Task<List<UserLookupDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        string? departmentId = null;
        int? resourceGroupId = null;
        IReadOnlyCollection<int>? coordinatorManagedUserIds = null;

        if (!_authenticatedUserContext.HasClaim(HrClaimTypeNames.ManageHrModule.Name) &&
            !_authenticatedUserContext.HasClaim(AccountantClaimTypeNames.ManageAccountant.Name))
        {
            if (_authenticatedUserContext.HasClaim(CoordinatorClaimTypeNames.ManageProjectCoordinator.Name))
            {
                coordinatorManagedUserIds = await _timeTrackContract.GetUserIdsByProjectManagerAsync(_authenticatedUserContext.GetRequiredUserId(), cancellationToken);
            }

            if (_authenticatedUserContext.GetRoles().IsHeadOfDepartment())
            {
                departmentId = request.Roles.Count == 0
                    || !await _identityDbContext.Roles
                        .Where(x => request.Roles.Contains(x.Name!))
                        .Select(x => x.IsCompanyManager)
                        .AllAsync(isManager => isManager, cancellationToken)
                        ? _authenticatedUserContext.DepartmentId
                        : null;
            }

            if (_authenticatedUserContext.GetRoles().IsGroupManager())
            {
                resourceGroupId = _authenticatedUserContext.ResourceGroupId;
            }
        }

        var excludedUsers = new List<int>{ _authenticatedUserContext.GetRequiredUserId() };

        if (_authenticatedUserContext.GetRoles().IsCto())
        {
            var ceo = await _identityDbContext.UserRoles
                .Where(x => x.Role!.Name == PortalRoleNames.Ceo)
                .Select(x => x.UserId)
                .ToListAsync(cancellationToken);
            excludedUsers.AddRange(ceo);
        }

        var loaderRequestDto = new UserLookupLoaderRequestDto()
        {
            KeyWord = request.KeyWord,
            IsActive = request.IsActive,
            DepartmentId = departmentId,
            ResourceGroupId = resourceGroupId,
            Roles = request.Roles,
            UserIds = coordinatorManagedUserIds,
            ExcludedUsers = excludedUsers,
        };  

        var users = await _userTabLookupLoader.LoadAsync(loaderRequestDto, cancellationToken);

        return users;
    }

    internal sealed record Query(string? KeyWord, bool? IsActive, List<string> Roles) : IQuery<List<UserLookupDto>>;
}
