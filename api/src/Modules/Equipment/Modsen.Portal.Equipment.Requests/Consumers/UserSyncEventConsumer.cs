using MassTransit;
using Modsen.Portal.Core.Communication.ExchangeDefinition;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Equipment.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Equipment.Requests.Core.DbQueries;
using Modsen.Portal.Identity.Contract.Events;

namespace Modsen.Portal.Equipment.Requests.Consumers;

internal sealed class UserSyncEventConsumer : IConsumer<UserSyncEvent>
{
    private readonly IUserCreatedOrUpdatedDbQuery _userCreatedOrUpdatedDbQuery;
    private readonly IConnectionFactory _connectionFactory;

    public UserSyncEventConsumer(IConnectionFactory connectionFactory,
        IUserCreatedOrUpdatedDbQuery userCreatedOrUpdatedDbQuery)
    {
        _connectionFactory = connectionFactory;
        _userCreatedOrUpdatedDbQuery = userCreatedOrUpdatedDbQuery;
    }

    public async Task Consume(ConsumeContext<UserSyncEvent> context)
    {
        var @event = context.Message;

        if (@event.Position.IsIntern())
        {
            return;
        }

        var equipmentUser = new EquipmentUser
        {
            UserId = @event.UserId,
            FirstName = @event.FirstName,
            LastName = @event.LastName,
            FirstNameRu = @event.FirstNameRu,
            LastNameRu = @event.LastNameRu,
            DepartmentId = @event.DepartmentId,
            Email = @event.Email,
            ResourceGroupId = @event.ResourceGroupId,
            Position = @event.Position
        };

        await _userCreatedOrUpdatedDbQuery.ExecuteAsync(equipmentUser, context.CancellationToken);
        await _connectionFactory.CommitCurrentTransaction(context.CancellationToken);
    }

    internal sealed class UserSyncEventConsumerDefinition() :
        ConsumerDefinitionBase<UserSyncEventConsumer>("user-sync-equipment-queue", "user-sync-exchange");

    internal sealed class RegistrationDefinition : ConsumerConfigurationRegistration<
        UserSyncEventConsumer,
        UserSyncEventConsumerDefinition>;
}
