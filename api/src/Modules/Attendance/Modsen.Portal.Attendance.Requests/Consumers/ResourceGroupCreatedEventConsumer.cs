using Modsen.Portal.Attendance.DataAccess.Npgsql;
using Modsen.Portal.Attendance.DataAccess.Npgsql.Entities;
using Modsen.Portal.Core.Communication.ExchangeDefinition;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Identity.Contract.Events;

namespace Modsen.Portal.Attendance.Requests.Consumers;

internal sealed class ResourceGroupCreatedEventConsumer : ResourceGroupEventConsumerBase<ResourceGroupCreatedEvent>
{
    public ResourceGroupCreatedEventConsumer(IConnectionFactory connectionFactory, AttendanceDbContext dbContext)
        : base(connectionFactory, dbContext) { }

    protected override int GetResourceGroupId(ResourceGroupCreatedEvent @event)
    {
        return @event.ResourceGroupId;
    }

    protected override ResourceGroupEntity CreateResourceGroup(ResourceGroupCreatedEvent @event)
    {
        return new ResourceGroupEntity
        {
            ResourceGroupId = @event.ResourceGroupId,
            Name = @event.ResourceGroupName,
        };
    }

    protected override void UpdateResourceGroup(ResourceGroupEntity group, ResourceGroupCreatedEvent @event)
    {
        group.Name = @event.ResourceGroupName;
    }

    internal sealed class ResourceGroupCreatedEventConsumerDefinition() :
        ConsumerDefinitionBase<ResourceGroupCreatedEventConsumer>(
            "group-created-attendance-queue",
            "group-created-exchange");

    internal sealed class RegistrationDefinition : ConsumerConfigurationRegistration<
        ResourceGroupCreatedEventConsumer,
        ResourceGroupCreatedEventConsumerDefinition>;
}
