using MassTransit;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Communication.ExchangeDefinition;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Identity.Contract.Events;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.TeamBoard.Requests.Consumers;

internal sealed class ResourceGroupSyncEventConsumer : IConsumer<ResourceGroupSyncEvent>
{
    private readonly ITeamBoardDbContext _teamBoardDbContext;
    private readonly IConnectionFactory _connectionFactory;

    public ResourceGroupSyncEventConsumer(
        ITeamBoardDbContext teamBoardDbContext,
        IConnectionFactory connectionFactory)
    {
        _teamBoardDbContext = teamBoardDbContext;
        _connectionFactory = connectionFactory;
    }

    public async Task Consume(ConsumeContext<ResourceGroupSyncEvent> context)
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
            };
            _teamBoardDbContext.TeamBoardResourceGroups.Add(group);
        }
        else
        {
            group.ResourceGroupName = @event.ResourceGroupName;
            _teamBoardDbContext.TeamBoardResourceGroups.Update(group);
        }

        await _teamBoardDbContext.SaveChangesAsync(context.CancellationToken);
        await _connectionFactory.CommitCurrentTransaction(context.CancellationToken);
    }

    internal sealed class ResourceGroupSyncEventConsumerDefinition() :
        ConsumerDefinitionBase<ResourceGroupSyncEventConsumer>("group-sync-teamboard-queue", "group-sync-exchange");

    internal sealed class RegistrationDefinition : ConsumerConfigurationRegistration<
        ResourceGroupSyncEventConsumer,
        ResourceGroupSyncEventConsumerDefinition>;
}
