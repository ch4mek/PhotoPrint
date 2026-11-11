using Modsen.Portal.Attendance.DataAccess.Npgsql;
using Modsen.Portal.Attendance.DataAccess.Npgsql.Entities;
using Modsen.Portal.Core.Communication.ExchangeDefinition;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Identity.Contract.Events;

namespace Modsen.Portal.Attendance.Requests.Consumers;

internal sealed class ResourceGroupUpdatedEventConsumer : ResourceGroupEventConsumerBase<ResourceGroupUpdatedEvent>
{
    public ResourceGroupUpdatedEventConsumer(IConnectionFactory connectionFactory, AttendanceDbContext dbContext)
        : base(connectionFactory, dbContext) { }

    protected override int GetResourceGroupId(ResourceGroupUpdatedEvent @event)
    {
        return @event.ResourceGroupId;
    }

    protected override ResourceGroupEntity CreateResourceGroup(ResourceGroupUpdatedEvent @event)
    {
        return new ResourceGroupEntity
        {
            ResourceGroupId = @event.ResourceGroupId,
            Name = @event.ResourceGroupName,
        };
    }

    protected override void UpdateResourceGroup(ResourceGroupEntity group, ResourceGroupUpdatedEvent @event)
    {
        group.Name = @event.ResourceGroupName;
    }

    internal sealed class ResourceGroupUpdatedEventConsumerDefinition() :
        ConsumerDefinitionBase<ResourceGroupUpdatedEventConsumer>(
            "group-updated-attendance-queue",
            "group-updated-exchange");

    internal sealed class RegistrationDefinition : ConsumerConfigurationRegistration<
        ResourceGroupUpdatedEventConsumer,
        ResourceGroupUpdatedEventConsumerDefinition>;
}
