using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;

namespace Modsen.Portal.Booking.Requests.Workplaces.Core.BusinessRules;

internal interface ICanCurrentUserDeleteBooking
{
    (bool canDelete, string? message) CanDelete(CanCurrentUserDeleteBookingRequest request);
}

internal sealed class CanCurrentUserDeleteBooking : ICanCurrentUserDeleteBooking
{
    private readonly IAuthenticatedUserContext _authenticatedUserContext;

    public CanCurrentUserDeleteBooking(IAuthenticatedUserContext authenticatedUserContext)
    {
        _authenticatedUserContext = authenticatedUserContext;
    }

    public (bool canDelete, string? message) CanDelete(CanCurrentUserDeleteBookingRequest request)
    {
        var userId = _authenticatedUserContext.UserId;

        switch (_authenticatedUserContext.GetRoles())
        {
            case var roles when (roles.IsEmployee() || roles.IsSystemAdministrator()) && request.BookingUserId != userId:
                return (false, $"User with id = {userId} does not have permissions to delete booking of other Users!");
            case var roles when roles.IsHeadOfDepartment() && request.BookingUserDepartmentId != _authenticatedUserContext.DepartmentId && !request.IsForGuestBooking:
                return (false, $"User with id = {userId} does not have permissions to delete booking of Users from other department!");
            case var roles when roles.IsGroupManager() && request.BookingUserRecourseGroup != _authenticatedUserContext.ResourceGroupId && !request.IsForGuestBooking:
                return (false, $"User with id = {userId} does not have permissions to delete booking of Users from other resource group!");
        }

        var isChief = _authenticatedUserContext.GetRoles().IsCeo() ||
            _authenticatedUserContext.GetRoles().IsCto() ||
            _authenticatedUserContext.GetRoles().IsHeadOfDepartment() ||
            _authenticatedUserContext.GetRoles().IsGroupManager();

        if (request.IsBlockBooking && !isChief)
        {
            return (false, $"User with id = {userId} does not have permissions to delete block booking!");
        }

        return (true, null);
    }
}

internal sealed record CanCurrentUserDeleteBookingRequest(
    int BookingUserId,
    int? BookingUserRecourseGroup,
    string? BookingUserDepartmentId,
    bool IsForGuestBooking,
    bool IsBlockBooking);
