using MassTransit;
using Modsen.Portal.Core.Communication.ExchangeDefinition;
using Modsen.Portal.Core.Database;
using Modsen.Portal.TeamBoard.Requests.Core.CoreDto;
using Modsen.Portal.TeamBoard.Requests.Core.DbQueries;
using Modsen.Portal.TimeTrack.Contract.Events;

namespace Modsen.Portal.TeamBoard.Requests.Consumers;

internal sealed class ProjectUserUpdatedEventConsumer : IConsumer<ProjectUserUpdatedEvent>
{
    private readonly IUpdateProjectUserDbQuery _updateProjectUserDbQuery;
    private readonly IConnectionFactory _connectionFactory;

    public ProjectUserUpdatedEventConsumer(IUpdateProjectUserDbQuery updateProjectUserDbQuery,
        IConnectionFactory connectionFactory)
    {
        _updateProjectUserDbQuery = updateProjectUserDbQuery;
        _connectionFactory = connectionFactory;
    }

    public async Task Consume(ConsumeContext<ProjectUserUpdatedEvent> context)
    {
        var @event = context.Message;

        await _updateProjectUserDbQuery.ExecuteAsync(new UpdateProjectUserCoreDto(
            @event.ProjectUserId,
            @event.UserId,
            @event.ProjectId,
            @event.IsTimeOnProjectBillable,
            @event.Fte,
            @event.UserOnProjectSeniorityLevelId,
            @event.UserOnProjectFrom,
            @event.UserOnProjectTo),
        context.CancellationToken);

        await _connectionFactory.CommitCurrentTransaction(context.CancellationToken);
    }

    internal sealed class ProjectUserUpdatedEventConsumerDefinition() :
        ConsumerDefinitionBase<ProjectUserUpdatedEventConsumer>(
            "project-user-updated-teamboard-queue",
            "project-user-updated-exchange");

    internal sealed class RegistrationDefinition : ConsumerConfigurationRegistration<
        ProjectUserUpdatedEventConsumer,
        ProjectUserUpdatedEventConsumerDefinition>;
}
