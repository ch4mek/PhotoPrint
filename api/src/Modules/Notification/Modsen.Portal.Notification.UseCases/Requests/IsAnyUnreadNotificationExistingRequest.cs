using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Notification;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Notification.DataAccess.Npgsql;

namespace Modsen.Portal.Notification.UseCases.Requests;

internal sealed class IsAnyUnreadNotificationExistingRequest
    : IQueryHandler<IsAnyUnreadNotificationExistingRequest.Query, IsAnyUnreadNotificationExistingRequest.Response>
{
    private readonly INotificationDbContext _notificationDbContext;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;

    public IsAnyUnreadNotificationExistingRequest(
        INotificationDbContext notificationDbContext,
        IAuthenticatedUserContext authenticatedUserContext)
    {
        _notificationDbContext = notificationDbContext;
        _authenticatedUserContext = authenticatedUserContext;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        if (_authenticatedUserContext.UserId != request.UserId)
        {
            throw new ActionNotAllowedException("You can't get another user's notifications");
        }

        return new Response(await IsAnyUnreadUserNotification(request, cancellationToken));
    }

    private Task<bool> IsAnyUnreadUserNotification(Query query, CancellationToken cancellationToken)
    {
        return _notificationDbContext.Notifications
            .Where(n => n.NotificationChannel == NotificationChannels.SignalR
                || n.NotificationChannel == NotificationChannels.WebNotification)
            .AnyAsync(notification =>
            notification.UserId == query.UserId &&
            !notification.Read &&
            !notification.IsDeleted && (
                query.NotificationTypes == null ||
                query.NotificationTypes.Count == 0 ||
                query.NotificationTypes.Contains(notification.NotificationType)), cancellationToken);
    }

    internal sealed record Query(int UserId, List<NotificationTypes>? NotificationTypes) : IQuery<Response>;

    internal record Response(bool IsAnyUnreadNotificationExisting);
}
