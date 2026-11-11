using MassTransit;
using Modsen.Portal.Booking.Requests.Core.CoreDto;
using Modsen.Portal.Booking.Requests.Core.DbQueries;
using Modsen.Portal.Core.Communication.ExchangeDefinition;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Identity.Contract.Events;

namespace Modsen.Portal.Booking.Requests.Consumers;

internal sealed class UserSyncEventConsumer : IConsumer<UserSyncEvent>
{
    private readonly ICreateOrUpdateUserDbQuery _createOrUpdateUserDbQuery;
    private readonly IConnectionFactory _connectionFactory;

    public UserSyncEventConsumer(ICreateOrUpdateUserDbQuery createOrUpdateUserDbQuery, IConnectionFactory connectionFactory)
    {
        _createOrUpdateUserDbQuery = createOrUpdateUserDbQuery;
        _connectionFactory = connectionFactory;
    }

    public async Task Consume(ConsumeContext<UserSyncEvent> context)
    {
        var user = new CreateOrUpdateUserCoreDto
        {
            UserId = context.Message.UserId,
            FirstName = context.Message.FirstName,
            LastName = context.Message.LastName,
            FirstNameRu = context.Message.FirstNameRu,
            LastNameRu = context.Message.LastNameRu,
            InitiatorId = context.Message.InitiatorId,
            Email = context.Message.Email,
            DepartmentId = context.Message.DepartmentId,
            ResourceGroupId = context.Message.ResourceGroupId,
            Position = context.Message.Position,
        };

        await _createOrUpdateUserDbQuery.Execute(user, context.CancellationToken);

        await _connectionFactory.CommitCurrentTransaction(context.CancellationToken);
    }

    internal sealed class UserSyncEventConsumerDefinition() :
        ConsumerDefinitionBase<UserSyncEventConsumer>("user-sync-booking-queue", "user-sync-exchange");

    internal sealed class RegistrationDefinition : ConsumerConfigurationRegistration<
        UserSyncEventConsumer,
        UserSyncEventConsumerDefinition>;
}
