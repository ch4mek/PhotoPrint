using Modsen.Portal.Core.Communication;
using Modsen.Portal.Core.Communication.ExchangeDefinition;

namespace Modsen.Portal.TimeTrack.Contract.Events;

public class ProjectCreatedEvent : IIntegrationEvent
{
    public required string EventId { get; init; }
    public required int ClientId { get; init; }
    public required int ProjectId { get; init; }
    public required string ClientName { get; init; }
    public required string ProjectName { get; init; }
    public required int? CreatedById { get; init; }
    public required bool IsActive { get; set; }
    public required bool IsPublic { get; init; }
    public string? Color { get; set; }
}

internal sealed class ProjectCreatedExchangeDefinition : ExchangeDefinitionBase<ProjectCreatedEvent>
{
    public override string ExchangeName => "project-created-exchange";
}
