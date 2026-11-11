using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Notification;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Notification.DataAccess.Npgsql;

namespace Modsen.Portal.Notification.UseCases.Requests;

internal sealed class DeleteNotificationsByUserIdRequest :
    ICommandHandler<DeleteNotificationsByUserIdRequest.Command, DeleteNotificationsByUserIdRequest.Response>
{
    private readonly INotificationDbContext _notificationDbContext;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;

    public DeleteNotificationsByUserIdRequest(
        INotificationDbContext notificationDbContext,
        IAuthenticatedUserContext authenticatedUserContext)
    {
        _notificationDbContext = notificationDbContext;
        _authenticatedUserContext = authenticatedUserContext;
    }

    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        if (_authenticatedUserContext.UserId != request.UserId)
        {
            throw new ActionNotAllowedException("You can't delete another user's notifications");
        }

        var deletedCount = await _notificationDbContext.Notifications
            .Where(notification => notification.UserId == request.UserId && !notification.IsDeleted &&
                (request.NotificationTypes == null || request.NotificationTypes.Count == 0 ||
                    request.NotificationTypes.Contains(notification.NotificationType)))
            .ExecuteUpdateAsync(setter => setter.SetProperty(notification => notification.IsDeleted, true),
                cancellationToken);

        return new Response(deletedCount);
    }

    internal sealed record Command(int UserId, List<NotificationTypes>? NotificationTypes) : ICommand<Response>,
        ITransactionalRequest;

    internal record Response(int DeletedCount);

    internal sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(query => query.UserId).GreaterThan(0);
        }
    }
}
