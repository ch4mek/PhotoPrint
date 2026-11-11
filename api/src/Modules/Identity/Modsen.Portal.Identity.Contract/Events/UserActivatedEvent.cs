using Modsen.Portal.Core.Communication;
using Modsen.Portal.Core.Communication.ExchangeDefinition;

namespace Modsen.Portal.Identity.Contract.Events;

public class UserActivatedEvent : IIntegrationEvent
{
    public required string EventId { get; init; }
    public required int UserId { get; init; }
    public required int ActivatedById { get; init; }
}

internal sealed class UserActivatedExchangeDefinition : ExchangeDefinitionBase<UserActivatedEvent>
{
    public override string ExchangeName => "user-activated-exchange";
}
