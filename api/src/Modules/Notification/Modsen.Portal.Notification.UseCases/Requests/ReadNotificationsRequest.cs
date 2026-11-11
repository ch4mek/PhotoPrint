using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Notification.DataAccess.Npgsql;
using Modsen.Portal.Notification.UseCases.Core;

namespace Modsen.Portal.Notification.UseCases.Requests;

internal sealed class ReadNotificationsRequest : ICommandHandler<ReadNotificationsRequest.Command, ReadNotificationsRequest.Response>
{
    private readonly ICanUserInteractWithNotificationsBusinessRule _canUserInteractWithNotificationsBusinessRule;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly INotificationDbContext _notificationDbContext;

    public ReadNotificationsRequest(
        ICanUserInteractWithNotificationsBusinessRule canUserInteractWithNotifications,
        IAuthenticatedUserContext authenticatedUserContext,
        INotificationDbContext notificationDbContext)
    {
        _canUserInteractWithNotificationsBusinessRule = canUserInteractWithNotifications;
        _authenticatedUserContext = authenticatedUserContext;
        _notificationDbContext = notificationDbContext;
    }

    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        var areNotificationsExisting = await AreNotificationsExisting(request.NotificationIds, cancellationToken);
        if (!areNotificationsExisting)
        {
            throw new EntityNotFoundException("Notifications haven't been found");
        }

        var model = new CanUserInteractWithNotificationsModel(_authenticatedUserContext.GetRequiredUserId(), request.NotificationIds);

        var businessRuleResponse = await _canUserInteractWithNotificationsBusinessRule
            .IsCorrespondsToBusiness(model, cancellationToken);

        if (!businessRuleResponse.IsSuccess)
        {
            throw new ActionNotAllowedException(businessRuleResponse.Reason ?? string.Empty);
        }

        var updatedCount = await ReadNotifications(request.NotificationIds, cancellationToken);

        return new Response(updatedCount);
    }

    private async Task<int> ReadNotifications(List<int> notificationIds, CancellationToken cancellationToken)
    {
        var updatedCount = await _notificationDbContext.Notifications
            .Where(notification => notificationIds.Contains(notification.Id))
            .ExecuteUpdateAsync(setter => setter.SetProperty(notification => notification.Read, true), cancellationToken);

        return updatedCount;
    }

    private async Task<bool> AreNotificationsExisting(List<int> notificationIds, CancellationToken cancellationToken)
    {
        var count = await _notificationDbContext.Notifications
            .CountAsync(notification => notificationIds.Contains(notification.Id), cancellationToken);

        return count == notificationIds.Count;
    }

    internal sealed record Command(List<int> NotificationIds) : ICommand<Response>, ITransactionalRequest;

    internal record Response(int UpdatedCount);

    internal sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(query => query.NotificationIds).NotEmpty();
        }
    }
}
