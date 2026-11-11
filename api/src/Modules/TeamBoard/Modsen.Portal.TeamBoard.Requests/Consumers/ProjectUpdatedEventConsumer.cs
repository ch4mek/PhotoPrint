using MassTransit;
using Modsen.Portal.Core.Communication.ExchangeDefinition;
using Modsen.Portal.Core.Database;
using Modsen.Portal.TeamBoard.Requests.Core.DbQueries;
using Modsen.Portal.TimeTrack.Contract.Events;

namespace Modsen.Portal.TeamBoard.Requests.Consumers;

internal sealed class ProjectUpdatedEventConsumer : IConsumer<ProjectUpdatedEvent>
{
    private readonly IUpdateTeamBoardProjectDbQuery _updateTeamBoardProjectDbQuery;
    private readonly IConnectionFactory _connectionFactory;

    public ProjectUpdatedEventConsumer(IUpdateTeamBoardProjectDbQuery updateTeamBoardProjectDbQuery,
        IConnectionFactory connectionFactory)
    {
        _updateTeamBoardProjectDbQuery = updateTeamBoardProjectDbQuery;
        _connectionFactory = connectionFactory;
    }

    public async Task Consume(ConsumeContext<ProjectUpdatedEvent> context)
    {
        var @event = context.Message;

        await _updateTeamBoardProjectDbQuery.ExecuteAsync(new UpdateTeamBoardProjectDbRequest(@event.ProjectId, @event.IsActive,
            @event.ProjectName, @event.ClientName, @event.UpdatedById, @event.IsPublic, @event.Color), context.CancellationToken);
        await _connectionFactory.CommitCurrentTransaction(context.CancellationToken);
    }

    internal sealed class ProjectUpdatedEventConsumerDefinition() :
        ConsumerDefinitionBase<ProjectUpdatedEventConsumer>(
            "project-updated-teamboard-queue",
            "project-updated-exchange");

    internal sealed class RegistrationDefinition : ConsumerConfigurationRegistration<
        ProjectUpdatedEventConsumer,
        ProjectUpdatedEventConsumerDefinition>;
}
