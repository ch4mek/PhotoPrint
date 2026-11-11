using MassTransit;
using Modsen.Portal.Core.Communication.ExchangeDefinition;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Finance.DataAccess.Npgsql;
using Modsen.Portal.Finance.DataAccess.Npgsql.Entities;
using Modsen.Portal.TimeTrack.Contract.Events;

namespace Modsen.Portal.Finance.Requests.Consumers;

internal class ProjectUserCreatedEventConsumer : IConsumer<ProjectUserCreatedEvent>
{
    private readonly IFinanceDbContext _dbContext;
    private readonly IConnectionFactory _connectionFactory;

    public ProjectUserCreatedEventConsumer(IFinanceDbContext dbContext, IConnectionFactory connectionFactory)
    {
        _dbContext = dbContext;
        _connectionFactory = connectionFactory;
    }

    public async Task Consume(ConsumeContext<ProjectUserCreatedEvent> context)
    {
        var @event = context.Message;

        if (@event.UserPosition.IsIntern())
        {
            return;
        }

        var projectUser = new FinanceProjectUserEntity
        {
            Id = @event.Id,
            ProjectId = @event.ProjectId,
            UserId = @event.UserId,
            UserOnProjectFrom = @event.UserOnProjectFrom,
            UserOnProjectTo = @event.UserOnProjectTo,
            IsTimeOnProjectBillable = @event.IsTimeOnProjectBillable
        };

        await _dbContext.FinanceProjectUsers.AddAsync(projectUser, context.CancellationToken);
        await _dbContext.SaveChangesAsync(context.CancellationToken);

        await _connectionFactory.CommitCurrentTransaction(context.CancellationToken);
    }

    internal sealed class ProjectUserCreatedEventConsumerDefinition() :
        ConsumerDefinitionBase<ProjectUserCreatedEventConsumer>(
            "project-user-created-finance-queue",
            "project-user-created-exchange");

    internal sealed class RegistrationDefinition : ConsumerConfigurationRegistration<
        ProjectUserCreatedEventConsumer,
        ProjectUserCreatedEventConsumerDefinition>;
}
