using MassTransit;
using Modsen.Portal.Core.Communication.ExchangeDefinition;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Finance.DataAccess.Npgsql;
using Modsen.Portal.Finance.DataAccess.Npgsql.Entities;
using Modsen.Portal.TimeTrack.Contract.Events;

namespace Modsen.Portal.Finance.Requests.Consumers;

internal class ProjectCreatedEventConsumer : IConsumer<ProjectCreatedEvent>
{
    private readonly IConnectionFactory _connectionFactory;
    private readonly IFinanceDbContext _dbContext;

    public ProjectCreatedEventConsumer(IConnectionFactory connectionFactory, IFinanceDbContext dbContext)
    {
        _connectionFactory = connectionFactory;
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<ProjectCreatedEvent> context)
    {
        var @event = context.Message;

        var newProject = new FinanceProjectEntity
        {
            Id = @event.ProjectId,
            ProjectName = @event.ProjectName,
            ClientId = @event.ClientId,
            Color = @event.Color,
            IsActive = @event.IsActive,
            IsPublic = @event.IsPublic,
        };

        await _dbContext.FinanceProjects.AddAsync(newProject, context.CancellationToken);
        await _dbContext.SaveChangesAsync(context.CancellationToken);

        await _connectionFactory.CommitCurrentTransaction(context.CancellationToken);
    }

    internal sealed class ProjectCreatedEventConsumerDefinition() :
        ConsumerDefinitionBase<ProjectCreatedEventConsumer>(
            "project-created-finance-queue",
            "project-created-exchange");

    internal sealed class RegistrationDefinition : ConsumerConfigurationRegistration<
        ProjectCreatedEventConsumer,
        ProjectCreatedEventConsumerDefinition>;
}

