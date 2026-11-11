using Modsen.Portal.Core.Communication;
using Modsen.Portal.Core.Communication.ExchangeDefinition;

namespace Modsen.Portal.TimeTrack.Contract.Events;

public class ProjectDeactivatedEvent : IIntegrationEvent
{
    public required string EventId { get; init; }
    public required string ProjectId { get; init; }
}

internal sealed class ProjectDeactivatedExchangeDefinition : ExchangeDefinitionBase<ProjectDeactivatedEvent>
{
    public override string ExchangeName => "project-deactivated-exchange";
}
