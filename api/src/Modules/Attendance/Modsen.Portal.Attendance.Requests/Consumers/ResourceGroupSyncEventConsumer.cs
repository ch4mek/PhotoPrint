using Modsen.Portal.Attendance.DataAccess.Npgsql;
using Modsen.Portal.Attendance.DataAccess.Npgsql.Entities;
using Modsen.Portal.Core.Communication.ExchangeDefinition;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Identity.Contract.Events;

namespace Modsen.Portal.Attendance.Requests.Consumers;

internal sealed class ResourceGroupSyncEventConsumer : ResourceGroupEventConsumerBase<ResourceGroupSyncEvent>
{
    public ResourceGroupSyncEventConsumer(IConnectionFactory connectionFactory, AttendanceDbContext dbContext)
        : base(connectionFactory, dbContext) { }

    protected override int GetResourceGroupId(ResourceGroupSyncEvent @event)
    {
        return @event.ResourceGroupId;
    }

    protected override ResourceGroupEntity CreateResourceGroup(ResourceGroupSyncEvent @event)
    {
        return new ResourceGroupEntity
        {
            ResourceGroupId = @event.ResourceGroupId,
            Name = @event.ResourceGroupName,
        };
    }

    protected override void UpdateResourceGroup(ResourceGroupEntity group, ResourceGroupSyncEvent @event)
    {
        group.Name = @event.ResourceGroupName;
    }

    internal sealed class ResourceGroupSyncEventConsumerDefinition() :
        ConsumerDefinitionBase<ResourceGroupSyncEventConsumer>(
            "group-sync-attendance-queue",
            "group-sync-exchange");

    internal sealed class RegistrationDefinition : ConsumerConfigurationRegistration<
        ResourceGroupSyncEventConsumer,
        ResourceGroupSyncEventConsumerDefinition>;
}
