using Modsen.Portal.Core.Communication;
using Modsen.Portal.Core.Communication.ExchangeDefinition;

namespace Modsen.Portal.Notification.Contract.Events;

public class DevelopmentPlanDeletedEvent : IIntegrationEvent
{
    public required string EventId { get; init; }
    public required int DevelopmentPlanId { get; init; }
    public required IReadOnlyCollection<int> UserIds { get; set; }
}

internal sealed class UserDeactivatedExchangeDefinition : ExchangeDefinitionBase<DevelopmentPlanDeletedEvent>
{
    public override string ExchangeName => "development-plan-deleted-exchange";
}

