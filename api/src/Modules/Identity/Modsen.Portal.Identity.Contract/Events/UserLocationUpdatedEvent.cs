using Modsen.Portal.Core;
using Modsen.Portal.Core.Communication;
using Modsen.Portal.Core.Communication.ExchangeDefinition;

namespace Modsen.Portal.Identity.Contract.Events;

public record UserLocationUpdatedEvent : IIntegrationEvent
{
    public required string EventId { get; init; }
    public required int UserId { get; init; }
    public required LocationType Location { get; set; }
}

internal sealed class UserLocationUpdatedExchangeDefinition : ExchangeDefinitionBase<UserLocationUpdatedEvent>
{
    public override string ExchangeName => "user-location-updated-exchange";
}
