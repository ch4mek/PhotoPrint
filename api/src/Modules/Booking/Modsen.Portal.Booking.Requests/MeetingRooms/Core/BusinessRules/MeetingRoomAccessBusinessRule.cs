using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Booking.DataAccess.Npgsql;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;

namespace Modsen.Portal.Booking.Requests.MeetingRooms.Core.BusinessRules;

internal interface IMeetingRoomAccessBusinessRule
{
    Task VerifyCurrentUserCanSeeCollisionForUserId(int userId, CancellationToken token);
    Task VerifyCurrentUserCanDeleteEntryForUserId(int userId, CancellationToken token);
    Task VerifyCurrentUserCanBookForUserId(int userId, CancellationToken token);
}

internal class MeetingRoomAccessBusinessRule : IMeetingRoomAccessBusinessRule
{
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly IBookingDbContext _bookingDbContext;

    public MeetingRoomAccessBusinessRule(
        IAuthenticatedUserContext authenticatedUserContext,
        IBookingDbContext bookingDbContext)
    {
        _authenticatedUserContext = authenticatedUserContext;
        _bookingDbContext = bookingDbContext;
    }

    public Task VerifyCurrentUserCanSeeCollisionForUserId(int userId, CancellationToken token)
    {
        return InternalCheck(userId,
            employeeErrorMessage: PortalMessages.CannotCheckCollisionsForAnotherUser,
            headOfDepartmentErrorMessage: PortalMessages.HeadCannotCheckCollisionsForUserFromOtherDepartment,
            groupManagerErrorMessage: PortalMessages.GroupManagerCannotCheckCollisionsForUserFromOtherGroup,
            token);
    }

    public Task VerifyCurrentUserCanDeleteEntryForUserId(int userId, CancellationToken token)
    {
        return InternalCheck(userId, 
            employeeErrorMessage: PortalMessages.CannotDeleteMeetingRoomEntryForAnotherUser,
            headOfDepartmentErrorMessage: PortalMessages.HeadCannotDeleteMeetingRoomEntryForUserFromOtherDepartment,
            groupManagerErrorMessage: PortalMessages.GroupManagerCannotDeleteMeetingRoomEntryForUserFromOtherGroup,
            token);
    }

    public Task VerifyCurrentUserCanBookForUserId(int userId, CancellationToken token)
    {
        return InternalCheck(userId, 
            employeeErrorMessage: PortalMessages.CannotBookMeetingRoomForAnotherUser,
            headOfDepartmentErrorMessage: PortalMessages.HeadCantBookMeetingRoomForUserFromOtherDepartment,
            groupManagerErrorMessage: PortalMessages.GroupManagerCantBookMeetingRoomForUserFromOtherGroup,
            token);
    }

    private async Task InternalCheck(int userId, string employeeErrorMessage, string headOfDepartmentErrorMessage,
        string groupManagerErrorMessage, CancellationToken cancellationToken)
    {
        if ((_authenticatedUserContext.GetRoles().IsEmployee() || _authenticatedUserContext.GetRoles().IsSystemAdministrator()) &&
            _authenticatedUserContext.UserId != userId)
        {
            throw new AppException(employeeErrorMessage);
        }

        if (_authenticatedUserContext.GetRoles().IsHeadOfDepartment() &&
            _authenticatedUserContext.UserId != userId)
        {
            var isUserInDepartment = await _bookingDbContext
                .BookingUsers
                .Where(x => x.UserId == userId)
                .Where(x => x.DepartmentId == _authenticatedUserContext.DepartmentId)
                .AnyAsync(cancellationToken);
            if (!isUserInDepartment)
            {
                throw new AppException(headOfDepartmentErrorMessage);
            }
        }

        if (_authenticatedUserContext.GetRoles().IsGroupManager() &&
            _authenticatedUserContext.UserId != userId)
        {
            var isUserInGroup = await _bookingDbContext
                .BookingUsers
                .Where(x => x.UserId == userId)
                .Where(x => x.ResourceGroupId == _authenticatedUserContext.ResourceGroupId)
                .AnyAsync(cancellationToken);
            if (!isUserInGroup)
            {
                throw new AppException(groupManagerErrorMessage);
            }
        }
    }
}
