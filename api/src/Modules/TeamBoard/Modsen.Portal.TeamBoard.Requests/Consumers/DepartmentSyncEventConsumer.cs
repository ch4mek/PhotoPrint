using MassTransit;
using Modsen.Portal.Core.Communication.ExchangeDefinition;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Identity.Contract.Events;
using Modsen.Portal.TeamBoard.Requests.Core.DbQueries;

namespace Modsen.Portal.TeamBoard.Requests.Consumers;

internal sealed class DepartmentSyncEventConsumer : IConsumer<DepartmentSyncEvent>
{
    private readonly ISyncDepartmentDbQuery _syncDepartmentDbQuery;
    private readonly IConnectionFactory _connectionFactory;

    public DepartmentSyncEventConsumer(ISyncDepartmentDbQuery syncDepartmentDbQuery, IConnectionFactory connectionFactory)
    {
        _syncDepartmentDbQuery = syncDepartmentDbQuery;
        _connectionFactory = connectionFactory;
    }

    public async Task Consume(ConsumeContext<DepartmentSyncEvent> context)
    {
        var @event = context.Message;
        await _syncDepartmentDbQuery.ExecuteAsync(
            new SyncDepartmentDbQueryRequest
            {
                DepartmentId = @event.DepartmentId,
                FullName = @event.DepartmentFullName,
                DepartmentOrderWeight = @event.DepartmentOrderWeight,
                ShortName = @event.DepartmentShortName,
                DepartmentReportingToRoleName = @event.DepartmentReportingToRoleName,
            }, context.CancellationToken);
        await _connectionFactory.CommitCurrentTransaction(context.CancellationToken);
    }

    internal sealed class DepartmentSyncEventConsumerDefinition() :
        ConsumerDefinitionBase<DepartmentSyncEventConsumer>(
            "department-sync-teamboard-queue",
            "department-sync-exchange");

    internal sealed class RegistrationDefinition : ConsumerConfigurationRegistration<
        DepartmentSyncEventConsumer,
        DepartmentSyncEventConsumerDefinition>;
}
