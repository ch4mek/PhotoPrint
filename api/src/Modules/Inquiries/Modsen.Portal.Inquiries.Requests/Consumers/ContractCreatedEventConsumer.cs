using MassTransit;
using Modsen.Portal.Core.Communication.ExchangeDefinition;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Inquiries.Requests.Agreements.Core;
using Modsen.Portal.TeamBoard.Contract.Events;

namespace Modsen.Portal.Inquiries.Requests.Consumers;

internal sealed class ContractCreatedEventConsumer : IConsumer<ContractCreatedEvent>
{
    private readonly IConnectionFactory _connectionFactory;
    private readonly IChangedContractTransactionComposer _changedContractTransactionComposer;

    public ContractCreatedEventConsumer(
        IConnectionFactory connectionFactory,
        IChangedContractTransactionComposer changedContractTransactionComposer)
    {
        _connectionFactory = connectionFactory;
        _changedContractTransactionComposer = changedContractTransactionComposer;
    }

    public async Task Consume(ConsumeContext<ContractCreatedEvent> context)
    {
        var @event = context.Message;

        await _changedContractTransactionComposer.Compose(
            @event.UserId,
            @event.ContractFrom,
            context.CancellationToken);

        await _connectionFactory.CommitCurrentTransaction(context.CancellationToken);
    }

    internal sealed class ContractCreatedEventConsumerDefinition() :
        ConsumerDefinitionBase<ContractCreatedEventConsumer>("contract-created-inquiries-queue", "contract-created-exchange");

    internal sealed class RegistrationDefinition : ConsumerConfigurationRegistration<
        ContractCreatedEventConsumer,
        ContractCreatedEventConsumerDefinition>;

}
