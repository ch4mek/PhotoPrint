using Modsen.Portal.Core.Communication;
using Modsen.Portal.Core.Communication.ExchangeDefinition;

namespace Modsen.Portal.Identity.Contract.Events;

public class ResourceGroupSyncEvent : IIntegrationEvent
{
    public required string EventId { get; init; }
    public required int ResourceGroupId { get; init; }
    public required string ResourceGroupName { get; init; }
}

internal sealed class ResourceGroupSyncExchangeDefinition : ExchangeDefinitionBase<ResourceGroupSyncEvent>
{
    public override string ExchangeName => "group-sync-exchange";
}
