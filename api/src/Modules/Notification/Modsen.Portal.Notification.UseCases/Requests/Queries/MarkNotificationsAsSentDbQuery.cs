using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Notification.DataAccess.Npgsql;

namespace Modsen.Portal.Notification.UseCases.Requests.Queries;

internal interface IMarkNotificationsAsSentDbQuery : IDbQuery<int[], int>;

internal class MarkNotificationsAsSentDbQuery : IMarkNotificationsAsSentDbQuery
{
    private readonly INotificationDbContext _notificationDbContext;

    public MarkNotificationsAsSentDbQuery(INotificationDbContext notificationDbContext)
    {
        _notificationDbContext = notificationDbContext;
    }

    public Task<int> ExecuteAsync(int[] notificationIds, CancellationToken cancellationToken)
    {
        return _notificationDbContext.Notifications
            .Where(notification => notificationIds.Contains(notification.Id))
            .ExecuteUpdateAsync(
                setter => setter.SetProperty(notification => notification.Sent, true),
                cancellationToken);
    }
}
