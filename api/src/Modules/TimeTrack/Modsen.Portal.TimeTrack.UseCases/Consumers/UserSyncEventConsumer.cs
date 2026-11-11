using MassTransit;
using Modsen.Portal.Core.Communication.ExchangeDefinition;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Identity.Contract.Events;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Users;
using Modsen.Portal.TimeTrack.Domain;

namespace Modsen.Portal.TimeTrack.UseCases.Consumers;

internal sealed class UserSyncEventConsumer : IConsumer<UserSyncEvent>
{
    private readonly ICreateOrUpdateUserDbQuery _createOrUpdateUserDbQuery;
    private readonly IConnectionFactory _connectionFactory;

    public UserSyncEventConsumer(
        ICreateOrUpdateUserDbQuery createOrUpdateUserDbQuery,
        IConnectionFactory connectionFactory)
    {
        _createOrUpdateUserDbQuery = createOrUpdateUserDbQuery;
        _connectionFactory = connectionFactory;
    }

    public async Task Consume(ConsumeContext<UserSyncEvent> context)
    {
        var @event = context.Message;

        var user = new TimeTrackUser
        {
            UserId = @event.UserId,
            FirstName = @event.FirstName,
            LastName = @event.LastName,
            DepartmentId = @event.DepartmentId,
            ResourceGroupId = @event.ResourceGroupId,
            Email = @event.Email,
            Color = @event.Color,
            AvatarFileId = @event.AvatarFileId,
            IsActive = true,
            CreatedById = @event.CreatedById,
        };

        await _createOrUpdateUserDbQuery.Execute(user, @event.Position, context.CancellationToken);
        await _connectionFactory.CommitCurrentTransaction(context.CancellationToken);
    }

    internal sealed class UserSyncEventConsumerDefinition() :
        ConsumerDefinitionBase<UserSyncEventConsumer>("user-sync-timetrack-queue", "user-sync-exchange");

    internal sealed class RegistrationDefinition : ConsumerConfigurationRegistration<
        UserSyncEventConsumer,
        UserSyncEventConsumerDefinition>;
}
