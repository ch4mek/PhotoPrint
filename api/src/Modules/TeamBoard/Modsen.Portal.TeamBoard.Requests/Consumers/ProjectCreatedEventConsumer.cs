using MassTransit;
using Modsen.Portal.Core.Communication.ExchangeDefinition;
using Modsen.Portal.Core.Database;
using Modsen.Portal.TeamBoard.Requests.Core.Composers;
using Modsen.Portal.TeamBoard.Requests.Core.CoreDto;
using Modsen.Portal.TimeTrack.Contract.Events;

namespace Modsen.Portal.TeamBoard.Requests.Consumers;

internal sealed class ProjectCreatedEventConsumer : IConsumer<ProjectCreatedEvent>
{
    private readonly ITeamBoardProjectComposer _teamBoardProjectComposer;
    private readonly IConnectionFactory _connectionFactory;

    public ProjectCreatedEventConsumer(
        ITeamBoardProjectComposer teamBoardProjectComposer,
        IConnectionFactory connectionFactory)
    {
        _teamBoardProjectComposer = teamBoardProjectComposer;
        _connectionFactory = connectionFactory;
    }

    public async Task Consume(ConsumeContext<ProjectCreatedEvent> context)
    {
        var @event = context.Message;
        var coreDto = new CreateProjectCoreDto(@event.ProjectId, @event.IsActive, @event.ProjectName,
            @event.ClientName, CreatorId: null, @event.IsPublic, @event.Color);

        await _teamBoardProjectComposer.ComposeAsync(coreDto, context.CancellationToken);
        await _connectionFactory.CommitCurrentTransaction(context.CancellationToken);
    }

    internal sealed class ProjectCreatedEventConsumerDefinition() :
        ConsumerDefinitionBase<ProjectCreatedEventConsumer>(
            "project-created-teamboard-queue",
            "project-created-exchange");

    internal sealed class RegistrationDefinition : ConsumerConfigurationRegistration<
        ProjectCreatedEventConsumer,
        ProjectCreatedEventConsumerDefinition>;
}
