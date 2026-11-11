using MassTransit;
using Modsen.Portal.Core.Communication.ExchangeDefinition;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Inquiries.Requests.Agreements.Core;
using Modsen.Portal.TeamBoard.Contract.Events;

namespace Modsen.Portal.Inquiries.Requests.Consumers;

internal sealed class ContractUpdatedEventConsumer : IConsumer<ContractUpdatedEvent>
{
    private readonly IConnectionFactory _connectionFactory;
    private readonly IChangedContractTransactionComposer _changedContractTransactionComposer;

    public ContractUpdatedEventConsumer(
        IConnectionFactory connectionFactory,
        IChangedContractTransactionComposer changedContractTransactionComposer)
    {
        _connectionFactory = connectionFactory;
        _changedContractTransactionComposer = changedContractTransactionComposer;
    }

    public async Task Consume(ConsumeContext<ContractUpdatedEvent> context)
    {
        var @event = context.Message;

        await _changedContractTransactionComposer.Compose(
            @event.UserId,
            @event.ContractFrom,
            context.CancellationToken);

        await _connectionFactory.CommitCurrentTransaction(context.CancellationToken); 
    }

    internal sealed class ContractUpdatedEventConsumerDefinition() :
        ConsumerDefinitionBase<ContractUpdatedEventConsumer>("contract-updated-inquiries-queue", "contract-updated-exchange");

    internal sealed class RegistrationDefinition : ConsumerConfigurationRegistration<
        ContractUpdatedEventConsumer,
        ContractUpdatedEventConsumerDefinition>;
}
