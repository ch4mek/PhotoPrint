using FluentValidation;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Notification;
using Modsen.Portal.Core.OData;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.UseCases.ValidationRules;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.Notification.DataAccess.Npgsql;
using Modsen.Portal.Notification.DataAccess.Npgsql.Entities;
using Modsen.Portal.Notification.UseCases.Requests.Queries;

namespace Modsen.Portal.Notification.UseCases.Requests;

internal sealed class GetOdpNotificationsRequest : IQueryHandler<GetOdpNotificationsRequest.Query, PageableListOfItems<OdpNotification>>
{
    private readonly INotificationDbContext _notificationDbContext;
    private readonly IMarkNotificationsAsSentDbQuery _markNotificationsAsSentDbQuery;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;

    public GetOdpNotificationsRequest(
        INotificationDbContext notificationDbContext,
        IAuthenticatedUserContext authenticatedUserContext,
        IMarkNotificationsAsSentDbQuery markNotificationsAsSentDbQuery)
    {
        _notificationDbContext = notificationDbContext;
        _authenticatedUserContext = authenticatedUserContext;
        _markNotificationsAsSentDbQuery = markNotificationsAsSentDbQuery;
    }

    public async Task<PageableListOfItems<OdpNotification>> Handle(Query request, CancellationToken cancellationToken)
    {
        if (_authenticatedUserContext.UserId != request.UserId)
        {
            throw new ActionNotAllowedException("You can't get another user's notifications");
        }

        var notifications = await GetOdataNotifications(request.UserId, request.QueryOptions, cancellationToken);
        var notificationIds = notifications.Items.Select(notification => notification.Id).ToArray();

        await _markNotificationsAsSentDbQuery.ExecuteAsync(notificationIds, cancellationToken);

        return notifications;
    }

    private async Task<PageableListOfItems<OdpNotification>> GetOdataNotifications(int userId,
        ODataQueryOptions<OdpNotification> queryOptions, CancellationToken cancellationToken)
    {
        var query = _notificationDbContext.Notifications
            .AsNoTracking()
            .Where(notification => notification.UserId == userId)
            .Where(n => n.NotificationChannel == NotificationChannels.SignalR ||
                n.NotificationChannel == NotificationChannels.WebNotification)
            .AsQueryable()
            .Select(OdpNotification.Map);

        var total = await queryOptions.RebuildWithoutPagination()
            .ApplyTo(query)
            .OfType<OdpNotification>()
            .CountAsync(cancellationToken);

        if (total == 0)
        {
            return EmptyPageableListOfItems<OdpNotification>.Value;
        }

        var result = await queryOptions
            .ApplyTo(query)
            .Cast<OdpNotification>()
            .ToListAsync(cancellationToken);

        return result.ToPageable(queryOptions, total);
    }

    internal sealed record Query(int UserId, ODataQueryOptions<OdpNotification> QueryOptions) : IQuery<PageableListOfItems<OdpNotification>>,
        ITransactionalRequest;

    internal sealed class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(query => query.UserId).GreaterThan(0);

            RuleFor(x => x.QueryOptions).SetValidator(new ODataQueryOptionsValidator());
            RuleFor(x => x.QueryOptions.RawValues).PaginationRule();
        }
    }
}
