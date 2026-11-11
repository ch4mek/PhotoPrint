using System.Globalization;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Communication.ExchangeDefinition;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Cv.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.Contract.Events;

namespace Modsen.Portal.Cv.Requests.Consumers;

internal sealed class ProjectUpdatedEventConsumer : IConsumer<ProjectUpdatedEvent>
{
    private readonly IConnectionFactory _connectionFactory;
    private readonly ICvDbContext _dbContext;

    public ProjectUpdatedEventConsumer(IConnectionFactory connectionFactory, ICvDbContext dbContext)
    {
        _connectionFactory = connectionFactory;
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<ProjectUpdatedEvent> context)
    {
        var @event = context.Message;

        var project = await _dbContext.CvProjects
            .FirstOrDefaultAsync(p => p.ProjectId == @event.ProjectId.ToString(CultureInfo.InvariantCulture), context.CancellationToken);

        if (project is not null)
        {
            project.Name = @event.ProjectName;
            project.IsActive = @event.IsActive;            

            _dbContext.CvProjects.Update(project);
            await _dbContext.SaveChangesAsync(context.CancellationToken);

            await _connectionFactory.CommitCurrentTransaction(context.CancellationToken);
        }
    }

    internal sealed class ProjectUpdatedEventConsumerDefinition() :
        ConsumerDefinitionBase<ProjectUpdatedEventConsumer>("project-updated-cv-queue", "project-updated-exchange");

    internal sealed class RegistrationDefinition : ConsumerConfigurationRegistration<
        ProjectUpdatedEventConsumer,
        ProjectUpdatedEventConsumerDefinition>;
}
