using Modsen.Portal.Core.Communication;
using Modsen.Portal.Core.Communication.ExchangeDefinition;

namespace Modsen.Portal.Identity.Contract.Events;

public class DepartmentSyncEvent : IIntegrationEvent
{
    public required string EventId { get; init; }
    public required string DepartmentId { get; init; }
    public required string DepartmentShortName { get; init; }
    public required string DepartmentFullName { get; init; }
    public required int DepartmentOrderWeight { get; init; }
    public required int DepartmentReportingToRoleId { get; init; }
    public required string DepartmentReportingToRoleName { get; init; }
}

internal sealed class DepartmentSyncExchangeDefinition : ExchangeDefinitionBase<DepartmentSyncEvent>
{
    public override string ExchangeName => "department-sync-exchange";
}
