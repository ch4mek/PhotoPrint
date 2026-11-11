using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Identity.Requests.Core;
using Modsen.Portal.Identity.Requests.Dto;
using Modsen.Portal.TimeTrack.Contract;

namespace Modsen.Portal.Identity.Requests.Users.Requests;

internal sealed class GetUserByIdRequest : IQueryHandler<GetUserByIdRequest.Query, UserDto>
{
    private readonly IUserLoader _userLoader;
    private readonly ITimeTrackContract _timeTrackContract;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;

    public GetUserByIdRequest(IUserLoader userLoader, ITimeTrackContract timeTrackContract, IAuthenticatedUserContext authenticatedUserContext)
    {
        _userLoader = userLoader;
        _timeTrackContract = timeTrackContract;
        _authenticatedUserContext = authenticatedUserContext;
    }

    public async Task<UserDto> Handle(Query request, CancellationToken cancellationToken)
    {
        var user = await _userLoader.LoadAsync(request.UserId, cancellationToken);

        if (request.Claims.Any(x => x == CoordinatorClaimTypeNames.ManageProjectCoordinator.Name) &&
            request.Claims.All(x => x != HrClaimTypeNames.ManageHrModule.Name) &&
            !request.Roles.IsCeo() &&
            !request.Roles.IsCto())
        {
            if (request.Roles.IsHeadOfDepartment() && user.DepartmentId == request.DepartmentId)
            {
                return user;
            }

            var coordinatorManagedUserIds = await _timeTrackContract.GetUserIdsByProjectManagerAsync(_authenticatedUserContext.GetRequiredUserId(), cancellationToken);
            if (!coordinatorManagedUserIds.Contains(request.UserId))
            {
                throw new ActionNotAllowedException($"You can't have access to user with id {request.UserId}");
            }

            return user;
        }

        if (request.Claims.All(x =>
                x != HrClaimTypeNames.ManageHrModule.Name &&
                x != CoordinatorClaimTypeNames.ManageProjectCoordinator.Name) &&
            !request.Claims.Contains(AccountantClaimTypeNames.ManageAccountant.Name) &&
            ((!request.Roles.IsSystemAdministrator() && request.DepartmentId != null && user.DepartmentId != request.DepartmentId) ||
            (request.Roles.IsGroupManager() && (user.ResourceGroupId == null || user.ResourceGroupId != request.ResourceGroupId))))
        {
            throw new ActionNotAllowedException($"You can't have access to user with id {request.UserId}");
        }

        return user;
    }

    internal sealed record Query(int UserId, string? DepartmentId, int? ResourceGroupId, IReadOnlyCollection<string> Roles, IReadOnlyCollection<string> Claims)
        : IQuery<UserDto>;
}
