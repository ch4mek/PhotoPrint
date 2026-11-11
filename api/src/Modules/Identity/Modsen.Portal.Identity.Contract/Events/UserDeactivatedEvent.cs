using Modsen.Portal.Core.Communication;
using Modsen.Portal.Core.Communication.ExchangeDefinition;

namespace Modsen.Portal.Identity.Contract.Events;

public class UserDeactivatedEvent : IIntegrationEvent
{
    public required string EventId { get; init; }
    public required int UserId { get; init; }
    public required int DeactivatedById { get; init; }
}

internal sealed class UserDeactivatedExchangeDefinition : ExchangeDefinitionBase<UserDeactivatedEvent>
{
    public override string ExchangeName => "user-deactivated-exchange";
}
