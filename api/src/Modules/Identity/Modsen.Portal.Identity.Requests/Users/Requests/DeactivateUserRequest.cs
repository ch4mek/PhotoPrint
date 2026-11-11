using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Booking.Contract;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Communication;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Equipment.Contract;
using Modsen.Portal.Identity.Contract.Events;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.Requests.Core;
using Modsen.Portal.Identity.Requests.Dto;
using Modsen.Portal.Inquiries.Contract;
using Modsen.Portal.TimeTrack.Contract;

namespace Modsen.Portal.Identity.Requests.Users.Requests;

internal sealed class DeactivateUserRequest : ICommandHandler<DeactivateUserRequest.Command, UserDto>
{
    private readonly IIdentityDbContext _identityDbContext;
    private readonly IUserLoader _userLoader;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly ITimeTrackContract _timeTrackContract;
    private readonly IEquipmentContract _equipmentContract;
    private readonly IBookingContract _bookingContract;
    private readonly ICommunicationBus _communicationBus;
    private readonly IAuthenticationTokenContext _authenticationTokenContext;
    private readonly IInquiriesContract _inquiriesContract;

    public DeactivateUserRequest(
        IAuthenticatedUserContext authenticatedUserContext,
        ITimeTrackContract timeTrackContract,
        IEquipmentContract equipmentContract,
        IBookingContract bookingContract,
        ICommunicationBus communicationBus,
        IIdentityDbContext identityDbContext,
        IUserLoader userLoader,
        IAuthenticationTokenContext authenticationTokenContext,
        IInquiriesContract inquiriesContract)
    {
        _authenticatedUserContext = authenticatedUserContext;
        _timeTrackContract = timeTrackContract;
        _equipmentContract = equipmentContract;
        _bookingContract = bookingContract;
        _communicationBus = communicationBus;
        _userLoader = userLoader;
        _identityDbContext = identityDbContext;
        _authenticationTokenContext = authenticationTokenContext;
        _inquiriesContract = inquiriesContract;
    }

    public async Task<UserDto> Handle(Command request, CancellationToken cancellationToken)
    {
        var userToDeactivate = await _userLoader.LoadAsync(request.UserId, cancellationToken);

        CheckAccessAsync(userToDeactivate);

        await CheckUserIsNotAssignedToAnyProject(userToDeactivate.Id, cancellationToken);
        await CheckEquipmentUserCanBeDeactivated(userToDeactivate.Id, cancellationToken);
        await CheckBookingUserCanBeDeactivated(userToDeactivate.Id, cancellationToken);
        await CheckUserIsAssignedToProjectAsPmAsync(userToDeactivate.Id, cancellationToken);
        await CheckUserIsInResourceGroup(userToDeactivate, cancellationToken);
        await CheckUserIsApproverOrSubstituteOfRequests(userToDeactivate.Id, cancellationToken);

        await _identityDbContext.Users
            .Where(u => u.Id == userToDeactivate.Id)
            .ExecuteUpdateAsync(setters =>
                setters.SetProperty(u => u.IsActive, false),
                cancellationToken);

        await _identityDbContext.PermissionAssignments
            .Where(p => p.UserId == userToDeactivate.Id)
            .ExecuteDeleteAsync(cancellationToken);

        var notification = new UserDeactivatedEvent
        {
            EventId = Guid.CreateVersion7().ToString(),
            UserId = userToDeactivate.Id,
            DeactivatedById = _authenticatedUserContext.GetRequiredUserId()
        };

        await _communicationBus.PublishAsync(notification, cancellationToken);

        await _authenticationTokenContext.RevokeAllUserTokens(userToDeactivate.Id, reason: RevokeReasons.UserDeactivated, cancellationToken);

        return await _userLoader.LoadAsync(userToDeactivate.Id, cancellationToken);
    }

    private void CheckAccessAsync(UserDto userToDeactivate)
    {
        CheckSelfDeactivation(userToDeactivate);

        var currentUserCanDeactivateByClaim = _authenticatedUserContext.HasClaim(HrClaimTypeNames.ManageHrModule.Name);
        if (currentUserCanDeactivateByClaim)
        {
            return;
        }

        CheckUserIsNotInRole(userToDeactivate, PortalRoleNames.Ceo);

        var currenUserRole = _authenticatedUserContext.GetRoles();

        if (currenUserRole.IsHeadOfDepartment())
        {
            CheckUserIsFromCurrentDepartment(userToDeactivate);
        }

        if (currenUserRole.IsSystemAdministrator())
        {
            CheckUserIsEmployeeOrIntern(userToDeactivate);
        }

        if (currenUserRole.IsEmployee() || currenUserRole.IsGroupManager() || currenUserRole.IsIntern())
        {
            throw new ActionNotAllowedException("You cannot deactivate user");
        }
    }

    private static void CheckUserIsEmployeeOrIntern(UserDto user)
    {
        if (!user.RoleNames.IsEmployee() && !user.RoleNames.IsIntern())
        {
            throw new ActionNotAllowedException(PortalMessages.SysAdminCanActivateOnlyEmployeeOrInternRole);
        }
    }

    private async Task CheckEquipmentUserCanBeDeactivated(int userId, CancellationToken cancellationToken)
    {
        if (!await _equipmentContract.CanEquipmentUserBeDeactivated(userId, cancellationToken))
        {
            throw new ActionNotAllowedException(PortalMessages.UserStillHasAssignedEquipment);
        }
    }

    private async Task CheckBookingUserCanBeDeactivated(int userId, CancellationToken cancellationToken)
    {
        if (!await _bookingContract.CanBookingUserBeDeactivated(userId, cancellationToken))
        {
            throw new ActionNotAllowedException(PortalMessages.UserStillHasScheduledDesk);
        }
    }

    private void CheckSelfDeactivation(UserDto user)
    {
        if (user.Id == _authenticatedUserContext.UserId)
        {
            throw new ActionNotAllowedException(PortalMessages.UserCannotDeactivateHimself);
        }
    }

    private void CheckUserIsNotInRole(UserDto user, string role)
    {
        if (user.Position == role)
        {
            throw new ActionNotAllowedException($"{string.Join(" ", _authenticatedUserContext.GetRoles())} cannot deactivate {role}");
        }
    }

    private void CheckUserIsFromCurrentDepartment(UserDto user)
    {
        if (user.DepartmentId != _authenticatedUserContext.DepartmentId)
        {
            throw new ActionNotAllowedException(PortalMessages.HeadCanDeactivateUsersOnlyFromHisDepartment);
        }
    }

    private async Task CheckUserIsInResourceGroup(UserDto user, CancellationToken cancellationToken)
    {
        if ((user.Position.IsEmployee() || user.Position.IsIntern()) && user.ResourceGroupId != null)
        {
            throw new ActionNotAllowedException(PortalMessages.UserIsInGroup);
        }

        if (user.Position.IsGroupManager() && user.ResourceGroupId != null)
        {
            var group = await _identityDbContext.ResourceGroups
                .Where(group => group.Id == user.ResourceGroupId && group.IsActive)
                .FirstOrDefaultAsync(cancellationToken);

            if (group != null)
            {
                throw new ActionNotAllowedException(PortalMessages.UserIsInGroup);
            }
        }
    }

    private async Task CheckUserIsApproverOrSubstituteOfRequests(int userId, CancellationToken cancellationToken)
    {
        var isUserApproverOrSubstitute = await _inquiriesContract.IsUserApproverOrSubstitute(userId, cancellationToken);

        if (isUserApproverOrSubstitute)
        {
            throw new ActionNotAllowedException(PortalMessages.UserIsApproverOrSubstitute);
        }
    }

    private async Task CheckUserIsNotAssignedToAnyProject(int userId, CancellationToken cancellationToken)
    {
        var projects = await _timeTrackContract.GetProjectsByUserId(userId, cancellationToken);

        if (projects.Count > 0)
        {
            throw new ActionNotAllowedException(PortalMessages.UserIsStillAssignedToProjects);
        }
    }

    private async Task CheckUserIsAssignedToProjectAsPmAsync(int userId, CancellationToken cancellationToken)
    {
        var isUserProjectManager = await _timeTrackContract.IsUserAssignedToProjectAsPmAsync(userId, cancellationToken);

        if (isUserProjectManager)
        {
            throw new ActionNotAllowedException(PortalMessages.UserIsProjectManager);
        }
    }

    internal sealed record Command(int UserId) : ICommand<UserDto>, ITransactionalRequest, IIdentityRequest;
}
