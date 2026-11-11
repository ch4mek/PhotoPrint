using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Notification.DataAccess.Npgsql;
using Modsen.Portal.Notification.UseCases.Core;

namespace Modsen.Portal.Notification.UseCases.Requests;

internal sealed class DeleteNotificationRequest : ICommandHandler<DeleteNotificationRequest.Command, DeleteNotificationRequest.Response>
{
    private readonly ICanUserInteractWithNotificationsBusinessRule _canUserInteractWithNotifications;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly INotificationDbContext _notificationDbContext;

    public DeleteNotificationRequest(
        ICanUserInteractWithNotificationsBusinessRule canUserInteractWithNotifications,
        IAuthenticatedUserContext authenticatedUserContext,
        INotificationDbContext notificationDbContext)
    {
        _canUserInteractWithNotifications = canUserInteractWithNotifications;
        _authenticatedUserContext = authenticatedUserContext;
        _notificationDbContext = notificationDbContext;
    }

    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        var notification = await _notificationDbContext.Notifications
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (notification == null)
        {
            throw new EntityNotFoundException("The notification hasn't been found");
        }

        var model = new CanUserInteractWithNotificationsModel(_authenticatedUserContext.GetRequiredUserId(), [request.Id]);

        var businessRuleResponse = await _canUserInteractWithNotifications.IsCorrespondsToBusiness(model, cancellationToken);
        if (!businessRuleResponse.IsSuccess)
        {
            throw new ActionNotAllowedException(businessRuleResponse.Reason ?? string.Empty);
        }

        notification.IsDeleted = true;

        await _notificationDbContext.SaveChanges(cancellationToken);

        return new Response(notification.Id, notification.UserId);
    }

    internal sealed record Command(int Id) : ICommand<Response>, ITransactionalRequest;

    internal record Response(int Id, int UserId);

    internal sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(query => query.Id).GreaterThan(0);
        }
    }
}
