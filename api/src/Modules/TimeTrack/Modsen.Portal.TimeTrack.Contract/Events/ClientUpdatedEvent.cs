using Modsen.Portal.Core.Communication;
using Modsen.Portal.Core.Communication.ExchangeDefinition;

namespace Modsen.Portal.TimeTrack.Contract.Events;

public class ClientUpdatedEvent : IIntegrationEvent
{
    public required string EventId { get; init; }
    public required int ClientId { get; init; }
    public required string ClientName { get; init; }
    public string? ClientLegalEntity { get; init; }
    public required bool IsActive { get; init; }
    public int? SalesManagerId { get; set; }
}

internal sealed class ClientUpdatedExchangeDefinition : ExchangeDefinitionBase<ClientUpdatedEvent>
{
    public override string ExchangeName => "client-updated-exchange";
}
