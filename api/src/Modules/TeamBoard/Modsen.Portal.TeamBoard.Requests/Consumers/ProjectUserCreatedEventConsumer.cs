using MassTransit;
using Modsen.Portal.Core.Communication.ExchangeDefinition;
using Modsen.Portal.Core.Database;
using Modsen.Portal.TeamBoard.Requests.Core.Composers;
using Modsen.Portal.TeamBoard.Requests.Core.CoreDto;
using Modsen.Portal.TimeTrack.Contract.Events;

namespace Modsen.Portal.TeamBoard.Requests.Consumers;

internal sealed class ProjectUserCreatedEventConsumer : IConsumer<ProjectUserCreatedEvent>
{
    private readonly IProjectUserComposer _projectUserComposer;
    private readonly IConnectionFactory _connectionFactory;

    public ProjectUserCreatedEventConsumer(IProjectUserComposer projectsUserComposer,
        IConnectionFactory connectionFactory)
    {
        _projectUserComposer = projectsUserComposer;
        _connectionFactory = connectionFactory;
    }

    public async Task Consume(ConsumeContext<ProjectUserCreatedEvent> context)
    {
        var @event = context.Message;
        var dbRequest = new CreateProjectUserCoreDto(@event.Id, @event.UserId, @event.ProjectId,
            @event.UserOnProjectFrom, @event.UserOnProjectTo, @event.IsTimeOnProjectBillable,
            @event.Fte, @event.UserOnProjectSeniorityLevelId);

        await _projectUserComposer.ComposeAsync(dbRequest, context.CancellationToken);
        await _connectionFactory.CommitCurrentTransaction(context.CancellationToken);
    }

    internal sealed class ProjectUserCreatedEventConsumerDefinition() :
        ConsumerDefinitionBase<ProjectUserCreatedEventConsumer>(
            "project-user-created-teamboard-queue",
            "project-user-created-exchange");

    internal sealed class RegistrationDefinition : ConsumerConfigurationRegistration<
        ProjectUserCreatedEventConsumer,
        ProjectUserCreatedEventConsumerDefinition>;
}
