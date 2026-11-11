using MassTransit;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Communication.ExchangeDefinition;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Finance.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.Contract.Events;

namespace Modsen.Portal.Finance.Requests.Consumers;

internal class ProjectUpdatedEventConsumer : IConsumer<ProjectUpdatedEvent>
{
    private readonly IConnectionFactory _connectionFactory;
    private readonly IFinanceDbContext _dbContext;

    public ProjectUpdatedEventConsumer(IConnectionFactory connectionFactory, IFinanceDbContext dbContext)
    {
        _connectionFactory = connectionFactory;
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<ProjectUpdatedEvent> context)
    {
        var @event = context.Message;

        var project = await _dbContext.FinanceProjects
                .FirstOrDefaultAsync(project => project.Id == @event.ProjectId, context.CancellationToken)
            ?? throw new EntityNotFoundException(PortalMessages.ProjectNotFound);

        project.ProjectName = @event.ProjectName;
        project.ClientId = @event.ClientId;
        project.Color = @event.Color;
        project.IsActive = @event.IsActive;
        project.IsPublic = @event.IsPublic;

        await _dbContext.SaveChangesAsync(context.CancellationToken);

        await _connectionFactory.CommitCurrentTransaction(context.CancellationToken);
    }

    internal sealed class ProjectUpdatedEventConsumerDefinition() :
        ConsumerDefinitionBase<ProjectUpdatedEventConsumer>(
            "project-updated-finance-queue",
            "project-updated-exchange");

    internal sealed class RegistrationDefinition : ConsumerConfigurationRegistration<
        ProjectUpdatedEventConsumer,
        ProjectUpdatedEventConsumerDefinition>;
}
