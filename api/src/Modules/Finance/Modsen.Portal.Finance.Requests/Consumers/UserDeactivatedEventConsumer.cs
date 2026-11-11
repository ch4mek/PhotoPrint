using MassTransit;
using Modsen.Portal.Core.Communication.ExchangeDefinition;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Finance.Requests.Core.DbQueries;
using Modsen.Portal.Identity.Contract.Events;

namespace Modsen.Portal.Finance.Requests.Consumers;

internal class UserDeactivatedEventConsumer : IConsumer<UserDeactivatedEvent>
{
    private readonly IUpdateIsActiveForUserDbQuery _updateIsActiveForUserDbQuery;
    private readonly IConnectionFactory _connectionFactory;

    public UserDeactivatedEventConsumer(
        IUpdateIsActiveForUserDbQuery updateIsActiveForUserDbQuery,
        IConnectionFactory connectionFactory)
    {
        _updateIsActiveForUserDbQuery = updateIsActiveForUserDbQuery;
        _connectionFactory = connectionFactory;
    }

    public async Task Consume(ConsumeContext<UserDeactivatedEvent> context)
    {
        var @event = context.Message;

        await _updateIsActiveForUserDbQuery.Set(@event.UserId, isActive: false, context.CancellationToken);
        await _connectionFactory.CommitCurrentTransaction(context.CancellationToken);
    }

    internal sealed class UserDeactivatedEventConsumerDefinition() :
        ConsumerDefinitionBase<UserDeactivatedEventConsumer>("user-deactivated-finance-queue",
            "user-deactivated-exchange");

    internal sealed class RegistrationDefinition : ConsumerConfigurationRegistration<
        UserDeactivatedEventConsumer,
        UserDeactivatedEventConsumerDefinition>;
}
