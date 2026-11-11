using Modsen.Portal.Core.Communication;
using Modsen.Portal.Core.Communication.ExchangeDefinition;

namespace Modsen.Portal.Identity.Contract.Events;

public class ResourceGroupCreatedEvent : IIntegrationEvent
{
    public required string EventId { get; init; }
    public required int ResourceGroupId { get; init; }
    public required string ResourceGroupName { get; init; }
    public required bool IsActive { get; set; }
}

internal sealed class ResourceGroupCreatedExchangeDefinition : ExchangeDefinitionBase<ResourceGroupCreatedEvent>
{
    public override string ExchangeName => "group-created-exchange";
}
