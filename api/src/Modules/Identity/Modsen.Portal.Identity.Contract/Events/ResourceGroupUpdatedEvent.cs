using Modsen.Portal.Core.Communication;
using Modsen.Portal.Core.Communication.ExchangeDefinition;

namespace Modsen.Portal.Identity.Contract.Events;

public class ResourceGroupUpdatedEvent : IIntegrationEvent
{
    public required string EventId { get; init; }
    public required int ResourceGroupId { get; init; }
    public required string ResourceGroupName { get; init; }
    public required bool IsActive { get; set; }
}

internal sealed class ResourceGroupUpdatedExchangeDefinition : ExchangeDefinitionBase<ResourceGroupUpdatedEvent>
{
    public override string ExchangeName => "group-updated-exchange";
}
