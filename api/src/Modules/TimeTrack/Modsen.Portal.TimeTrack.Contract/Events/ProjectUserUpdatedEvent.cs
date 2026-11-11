using Modsen.Portal.Core.Communication;
using Modsen.Portal.Core.Communication.ExchangeDefinition;

namespace Modsen.Portal.TimeTrack.Contract.Events;

public class ProjectUserUpdatedEvent : IIntegrationEvent
{
    public required string EventId { get; init; }
    public required int ProjectUserId { get; init; }
    public required int ProjectId { get; init; }
    public required int UserId { get; init; }
    public string? UserPosition { get; init; }
    public required bool IsTimeOnProjectBillable { get; init; }
    public required decimal Fte { get; init; }
    public required string UserOnProjectSeniorityLevelId { get; init; }
    public required DateOnly UserOnProjectFrom { get; init; }
    public required DateOnly? UserOnProjectTo { get; init; }
}

internal sealed class ProjectUserUpdatedExchangeDefinition : ExchangeDefinitionBase<ProjectUserUpdatedEvent>
{
    public override string ExchangeName => "project-user-updated-exchange";
}
