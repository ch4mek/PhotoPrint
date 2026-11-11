using MassTransit;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Communication.ExchangeDefinition;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Identity.Contract.Events;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.TimeTrack.UseCases.Consumers;

internal sealed class ResourceGroupCreatedEventConsumer : IConsumer<ResourceGroupCreatedEvent>
{
    private readonly ITimeTrackDbContext _timeTrackDbContext;
    private readonly IConnectionFactory _connectionFactory;

    public ResourceGroupCreatedEventConsumer(
        ITimeTrackDbContext timeTrackDbContext,
        IConnectionFactory connectionFactory)
    {
        _timeTrackDbContext = timeTrackDbContext;
        _connectionFactory = connectionFactory;
    }

    public async Task Consume(ConsumeContext<ResourceGroupCreatedEvent> context)
    {
        var @event = context.Message;
        var group = await _timeTrackDbContext.TimeTrackResourceGroups
            .FirstOrDefaultAsync(x => x.ResourceGroupId == @event.ResourceGroupId, context.CancellationToken);

        if (group == null)
        {
            group = new TimeTrackResourceGroup
            {
                ResourceGroupId = @event.ResourceGroupId,
                ResourceGroupName = @event.ResourceGroupName,
                IsActive = @event.IsActive
            };
            _timeTrackDbContext.TimeTrackResourceGroups.Add(group);
        }
        else
        {
            group.ResourceGroupName = @event.ResourceGroupName;
            group.IsActive = @event.IsActive;
            _timeTrackDbContext.TimeTrackResourceGroups.Update(group);
        }

        await _timeTrackDbContext.SaveChanges(context.CancellationToken);
        await _connectionFactory.CommitCurrentTransaction(context.CancellationToken);
    }

    internal sealed class ResourceGroupCreatedEventConsumerDefinition() :
        ConsumerDefinitionBase<ResourceGroupCreatedEventConsumer>(
            "group-created-timetrack-queue",
            "group-created-exchange");

    internal sealed class RegistrationDefinition : ConsumerConfigurationRegistration<
        ResourceGroupCreatedEventConsumer,
        ResourceGroupCreatedEventConsumerDefinition>;
}
