using Modsen.Portal.Core.Communication;
using Modsen.Portal.Core.Communication.ExchangeDefinition;

namespace Modsen.Portal.TimeTrack.Contract.Events;

public class ProjectUpdatedEvent : IIntegrationEvent
{
    public required string EventId { get; init; }
    public required int ClientId { get; init; }
    public required int ProjectId { get; init; }
    public required string ClientName { get; init; }
    public required string ProjectName { get; init; }
    public required int? UpdatedById { get; init; }
    public required bool IsActive { get; init; }
    public required bool IsPublic { get;init; }
    public required int? OldManagerUserId { get; init; }
    public required int? NewManagerUserId { get; init; }
    public string? Color { get; set; }
}
internal sealed class ProjectUpdatedExchangeDefinition : ExchangeDefinitionBase<ProjectUpdatedEvent>
{
    public override string ExchangeName => "project-updated-exchange";
}
