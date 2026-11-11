using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.TimeTrack;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TimeTrack.UseCases.Core.DbQueries;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.DbQueries;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Requests.RequestDto;

namespace Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.Validators;

internal interface IUpdateSubmitStatusValidator
{
    Task ValidateAsync(UpdateUserSubmitStatusDto dto, CancellationToken cancellationToken);
}

internal sealed class UpdateSubmitStatusValidator : IUpdateSubmitStatusValidator
{
    private readonly IGetTimesheetSubmitStatusDbQuery _getTimesheetSubmitStatusDbQuery;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly IGetTimeTrackUserDbQuery _getTimeTrackUserDbQuery;

    public UpdateSubmitStatusValidator(
        IGetTimesheetSubmitStatusDbQuery getTimesheetSubmitStatusDbQuery,
        IAuthenticatedUserContext authenticatedUserContext,
        IGetTimeTrackUserDbQuery getTimeTrackUserDbQuery)
    {
        _getTimesheetSubmitStatusDbQuery = getTimesheetSubmitStatusDbQuery;
        _authenticatedUserContext = authenticatedUserContext;
        _getTimeTrackUserDbQuery = getTimeTrackUserDbQuery;
    }

    public async Task ValidateAsync(UpdateUserSubmitStatusDto dto, CancellationToken cancellationToken)
    {
        var roles = _authenticatedUserContext.GetRoles();

        ValidateExecutiveStatus(roles, dto.Status);
        ValidateEmployeeOrSystemAdminStatus(roles, dto);

        var user = await _getTimeTrackUserDbQuery.ExecuteAsync(dto.UserId, cancellationToken)
            ?? throw new InvalidDataAppException(PortalMessages.UserNotFound);

        ValidateHeadOfDepartment(roles, dto.Status, user);
        ValidateGroupManager(roles, dto.Status, user);

        var submitStatus = await _getTimesheetSubmitStatusDbQuery.ExecuteAsync(userId: user.UserId,
            new DateOnly(year: dto.Year, month: dto.Month, day: 1),
            cancellationToken);

        ValidateSubmitStatusTransitions(submitStatus, dto.Status);
        ValidateRole(roles);
    }

    private static void ValidateExecutiveStatus(IReadOnlyCollection<string> roles, TimesheetSubmitStatusType status)
    {
        if ((roles.IsCeo() || roles.IsCto()) && status == TimesheetSubmitStatusType.UnderReview)
        {
            throw new ActionNotAllowedException(PortalMessages.CurrentUserCanSendOnlyApprovedOrRejectedStatuses);
        }
    }

    private void ValidateEmployeeOrSystemAdminStatus(IReadOnlyCollection<string> roles, UpdateUserSubmitStatusDto dto)
    {
        if (!roles.IsEmployee() && !roles.IsSystemAdministrator())
        {
            return;
        }

        if (dto.Status != TimesheetSubmitStatusType.UnderReview)
        {
            throw new ActionNotAllowedException(PortalMessages.CurrentUserCanOnlySendUnderReviewStatus);
        }

        if (_authenticatedUserContext.UserId != dto.UserId)
        {
            throw new ActionNotAllowedException(PortalMessages.CannotChangeOtherUsersTimesheetStatus);
        }
    }

    private void ValidateHeadOfDepartment(IReadOnlyCollection<string> roles, TimesheetSubmitStatusType status, TimeTrackUser user)
    {
        if (!roles.IsHeadOfDepartment())
        {
            return;
        }

        if (status == TimesheetSubmitStatusType.Approved || status == TimesheetSubmitStatusType.Rejected)
        {
            if (user.DepartmentId != _authenticatedUserContext.DepartmentId)
            {
                throw new ActionNotAllowedException(PortalMessages.HeadCannotUpdateStatusForUserFromOtherDepartment);
            }
        }
        else if (status == TimesheetSubmitStatusType.UnderReview)
        {
            throw new ActionNotAllowedException(PortalMessages.CurrentUserCanSendOnlyApprovedOrRejectedStatuses);
        }
    }

    private void ValidateGroupManager(IReadOnlyCollection<string> roles, TimesheetSubmitStatusType status, TimeTrackUser user)
    {
        if (!roles.IsGroupManager())
        {
            return;
        }

        if (status == TimesheetSubmitStatusType.Approved || status == TimesheetSubmitStatusType.Rejected)
        {
            if (user.ResourceGroupId != _authenticatedUserContext.ResourceGroupId)
            {
                throw new ActionNotAllowedException(PortalMessages.GmCannotUpdateStatusForUserFromOtherGroup);
            }
        }
        else if (status == TimesheetSubmitStatusType.UnderReview)
        {
            throw new ActionNotAllowedException(PortalMessages.CurrentUserCanSendOnlyApprovedOrRejectedStatuses);
        }
    }

    private static void ValidateSubmitStatusTransitions(TimesheetSubmitStatusType? submitStatus, TimesheetSubmitStatusType newStatus)
    {
        if (submitStatus is null)
        {
            if (newStatus == TimesheetSubmitStatusType.Rejected)
            {
                throw new InvalidDataAppException(PortalMessages.TimesheetCannotBeRejectedBeforeUnderReviewStatus);
            }
            return;
        }

        if (newStatus == submitStatus)
        {
            throw new InvalidDataAppException(PortalMessages.StatusHasAlreadyBeenSent);
        }

        if (submitStatus == TimesheetSubmitStatusType.Approved && newStatus == TimesheetSubmitStatusType.UnderReview)
        {
            throw new InvalidDataAppException(PortalMessages.MonthHasAlreadyBeenApproved);
        }
    }

    private static void ValidateRole(IReadOnlyCollection<string> roles)
    {
        var isAllowedRole = roles.IsCto() || roles.IsCeo() || roles.IsHeadOfDepartment() ||
            roles.IsGroupManager() || roles.IsEmployee() || roles.IsSystemAdministrator();

        if (!isAllowedRole)
        {
            throw new InvalidDataAppException(PortalMessages.ThereIsNoAppropriateRole);
        }
    }
}
