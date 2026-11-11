using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Services.BusinessRules;
using Modsen.Portal.Notification.DataAccess.Npgsql;

namespace Modsen.Portal.Notification.UseCases.Core;

internal interface ICanUserInteractWithNotificationsBusinessRule : IBusinessRuleWithFailReason<CanUserInteractWithNotificationsModel>;

internal record CanUserInteractWithNotificationsModel(int UserId, List<int> NotificationIds);

internal class CanUserInteractWithNotificationsBusinessRule : ICanUserInteractWithNotificationsBusinessRule
{
    private readonly INotificationDbContext _notificationDbContext;

    public CanUserInteractWithNotificationsBusinessRule(INotificationDbContext notificationDbContext)
    {
        _notificationDbContext = notificationDbContext;
    }

    public async Task<BusinessRuleResult> IsCorrespondsToBusiness(CanUserInteractWithNotificationsModel? model,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(model);
        var isNotificationOfUser = await AreNotificationsBelongingToUser(model, cancellationToken);

        return !isNotificationOfUser
            ? BusinessRuleResult.Fail("The notification doesn't belong to user")
            : BusinessRuleResult.Success();
    }

    private async Task<bool> AreNotificationsBelongingToUser(CanUserInteractWithNotificationsModel model,
        CancellationToken cancellationToken)
    {
        var count = await _notificationDbContext.Notifications
            .Where(notification => notification.UserId == model.UserId)
            .CountAsync(notification => model.NotificationIds.Contains(notification.Id), cancellationToken);
        return count == model.NotificationIds.Count;
    }
}
