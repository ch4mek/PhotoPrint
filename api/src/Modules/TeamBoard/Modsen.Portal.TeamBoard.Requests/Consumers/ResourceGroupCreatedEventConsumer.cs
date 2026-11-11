using MassTransit;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Communication.ExchangeDefinition;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Identity.Contract.Events;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.TeamBoard.Requests.Consumers;

internal sealed class ResourceGroupCreatedEventConsumer : IConsumer<ResourceGroupCreatedEvent>
{
    private readonly ITeamBoardDbContext _teamBoardDbContext;
    private readonly IConnectionFactory _connectionFactory;

    public ResourceGroupCreatedEventConsumer(
        ITeamBoardDbContext teamBoardDbContext,
        IConnectionFactory connectionFactory)
    {
        _teamBoardDbContext = teamBoardDbContext;
        _connectionFactory = connectionFactory;
    }

    public async Task Consume(ConsumeContext<ResourceGroupCreatedEvent> context)
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
                IsActive = @event.IsActive
            };
            _teamBoardDbContext.TeamBoardResourceGroups.Add(group);
        }
        else
        {
            group.ResourceGroupName = @event.ResourceGroupName;
            group.IsActive = @event.IsActive;
            _teamBoardDbContext.TeamBoardResourceGroups.Update(group);
        }

        await _teamBoardDbContext.SaveChangesAsync(context.CancellationToken);
        await _connectionFactory.CommitCurrentTransaction(context.CancellationToken);
    }

    internal sealed class ResourceGroupCreatedEventConsumerDefinition() :
        ConsumerDefinitionBase<ResourceGroupCreatedEventConsumer>(
            "group-created-teamboard-queue",
            "group-created-exchange");

    internal sealed class RegistrationDefinition : ConsumerConfigurationRegistration<
        ResourceGroupCreatedEventConsumer,
        ResourceGroupCreatedEventConsumerDefinition>;
}
