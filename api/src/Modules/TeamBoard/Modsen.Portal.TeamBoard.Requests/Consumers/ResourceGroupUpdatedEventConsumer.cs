using MassTransit;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Communication.ExchangeDefinition;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Identity.Contract.Events;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.TeamBoard.Requests.Consumers;

internal sealed class ResourceGroupUpdatedEventConsumer : IConsumer<ResourceGroupUpdatedEvent>
{
    private readonly ITeamBoardDbContext _teamBoardDbContext;
    private readonly IConnectionFactory _connectionFactory;

    public ResourceGroupUpdatedEventConsumer(
        ITeamBoardDbContext teamBoardDbContext,
        IConnectionFactory connectionFactory)
    {
        _teamBoardDbContext = teamBoardDbContext;
        _connectionFactory = connectionFactory;
    }

    public async Task Consume(ConsumeContext<ResourceGroupUpdatedEvent> context)
    {
        var @event = context.Message;
        var group = await _teamBoardDbContext.TeamBoardResourceGroups
            .FirstOrDefaultAsync(x => x.ResourceGroupId == @event.ResourceGroupId, context.CancellationToken);

        if (group == null)
        {
            group = new TeamBoardResourceGroup
            {
                ResourceGroupId = @event.ResourceGroupId,
                ResourceGroupName = @event.ResourceGroupName,
                IsActive = @event.IsActive,
            };
            _teamBoardDbContext.TeamBoardResourceGroups.Add(group);
        }
        else
        {
            group.IsActive = @event.IsActive;
            group.ResourceGroupName = @event.ResourceGroupName;
            _teamBoardDbContext.TeamBoardResourceGroups.Update(group);
        }

        await _teamBoardDbContext.SaveChangesAsync(context.CancellationToken);
        await _connectionFactory.CommitCurrentTransaction(context.CancellationToken);
    }

    internal sealed class ResourceGroupUpdatedEventConsumerDefinition() :
        ConsumerDefinitionBase<ResourceGroupUpdatedEventConsumer>("group-updated-teamboard-queue",
            "group-updated-exchange");

    internal sealed class RegistrationDefinition : ConsumerConfigurationRegistration<
        ResourceGroupUpdatedEventConsumer,
        ResourceGroupUpdatedEventConsumerDefinition>;
}
