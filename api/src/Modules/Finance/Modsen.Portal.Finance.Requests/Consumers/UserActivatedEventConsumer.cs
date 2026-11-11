using MassTransit;
using Modsen.Portal.Core.Communication.ExchangeDefinition;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Finance.Requests.Core.DbQueries;
using Modsen.Portal.Identity.Contract.Events;

namespace Modsen.Portal.Finance.Requests.Consumers;

internal class UserActivatedEventConsumer : IConsumer<UserActivatedEvent>
{
    private readonly IUpdateIsActiveForUserDbQuery _updateIsActiveForUserDbQuery;
    private readonly IConnectionFactory _connectionFactory;

    public UserActivatedEventConsumer(
        IUpdateIsActiveForUserDbQuery updateIsActiveForUserDbQuery,
        IConnectionFactory connectionFactory)
    {
        _updateIsActiveForUserDbQuery = updateIsActiveForUserDbQuery;
        _connectionFactory = connectionFactory;
    }

    public async Task Consume(ConsumeContext<UserActivatedEvent> context)
    {
        var @event = context.Message;

        await _updateIsActiveForUserDbQuery.Set(@event.UserId, isActive: true, context.CancellationToken);
        await _connectionFactory.CommitCurrentTransaction(context.CancellationToken);
    }

    internal sealed class UserActivatedEventConsumerDefinition() :
        ConsumerDefinitionBase<UserActivatedEventConsumer>("user-activated-finance-queue", "user-activated-exchange");

    internal sealed class RegistrationDefinition : ConsumerConfigurationRegistration<
        UserActivatedEventConsumer,
        UserActivatedEventConsumerDefinition>;
}
