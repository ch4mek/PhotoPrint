using Modsen.Portal.Core;
using Modsen.Portal.Core.Communication;
using Modsen.Portal.Core.Communication.ExchangeDefinition;

namespace Modsen.Portal.Identity.Contract.Events;

public record UserRegisteredLocationUpdatedEvent : IIntegrationEvent
{
    public required string EventId { get; init; }
    public required int UserId { get; init; }
    public required int RegisteredLocationId { get; set; }
    public required string? CurrentLocation { get; init; }
    public LocationType Location { get; set; }
}

internal sealed class UserRegisteredLocationUpdatedExchangeDefinition : ExchangeDefinitionBase<UserRegisteredLocationUpdatedEvent>
{
    public override string ExchangeName => "user-registered-location-updated-exchange";
}
