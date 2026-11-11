using MassTransit;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Communication.ExchangeDefinition;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.Contract.Events;

namespace Modsen.Portal.Identity.Requests.Consumers;

internal sealed class ProjectDeactivatedEventConsumer : IConsumer<ProjectDeactivatedEvent>
{
    private readonly IConnectionFactory _connectionFactory;
    private readonly IIdentityDbContext _identityDbContext;

    public ProjectDeactivatedEventConsumer(
        IIdentityDbContext identityDbContext,
        IConnectionFactory connectionFactory)
    {
        _identityDbContext = identityDbContext;
        _connectionFactory = connectionFactory;
    }

    public async Task Consume(ConsumeContext<ProjectDeactivatedEvent> context)
    {
        var @event = context.Message;
        var resourceType = nameof(PermissionResourceType.Project);

        await _identityDbContext.PermissionAssignments
            .Where(p => p.ResourceId == @event.ProjectId)
            .Where(p => p.ResourceType == resourceType)
            .ExecuteDeleteAsync(context.CancellationToken);
        await _identityDbContext.SaveChanges(context.CancellationToken);
        await _connectionFactory.CommitCurrentTransaction(context.CancellationToken);
    }

    internal sealed class ProjectDeactivatedEventConsumerDefinition() :
        ConsumerDefinitionBase<ProjectDeactivatedEventConsumer>("project-deactivated-identity-queue", "project-deactivated-exchange");

    internal sealed class RegistrationDefinition : ConsumerConfigurationRegistration<
        ProjectDeactivatedEventConsumer,
        ProjectDeactivatedEventConsumerDefinition>;
}
