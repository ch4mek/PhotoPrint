using System.Globalization;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Communication.ExchangeDefinition;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Cv.DataAccess.Npgsql;
using Modsen.Portal.Cv.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TimeTrack.Contract.Events;

namespace Modsen.Portal.Cv.Requests.Consumers;

internal sealed class ProjectCreatedEventConsumer : IConsumer<ProjectCreatedEvent>
{
    private readonly IConnectionFactory _connectionFactory;
    private readonly ICvDbContext _dbContext;

    public ProjectCreatedEventConsumer(IConnectionFactory connectionFactory, ICvDbContext dbContext)
    {
        _connectionFactory = connectionFactory;
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<ProjectCreatedEvent> context)
    {
        var @event = context.Message;

        var projectById = await _dbContext.CvProjects
            .FirstOrDefaultAsync(
                p => p.ProjectId == @event.ProjectId.ToString(CultureInfo.InvariantCulture),
                context.CancellationToken);

        if (projectById is null)
        {
            var projectByName = await _dbContext.CvProjects
                .Include(p => p.Cvs)
                .FirstOrDefaultAsync(
                    p => p.Name == @event.ProjectName,
                    context.CancellationToken);

            if (projectByName is null)
            {
                var newProject = new CvProjectEntity
                {
                    ProjectId = @event.ProjectId.ToString(CultureInfo.InvariantCulture),
                    Name = @event.ProjectName,
                    IsActive = @event.IsActive,
                };

                _dbContext.CvProjects.Add(newProject);
            }
            else
            {
                var newProject = new CvProjectEntity
                {
                    ProjectId = @event.ProjectId.ToString(CultureInfo.InvariantCulture),
                    Name = @event.ProjectName,
                    IsActive = @event.IsActive,
                    Cvs = projectByName.Cvs,
                };

                _dbContext.CvProjects.Remove(projectByName);
                _dbContext.CvProjects.Add(newProject);
            }

            await _dbContext.SaveChangesAsync(context.CancellationToken);
            await _connectionFactory.CommitCurrentTransaction(context.CancellationToken);
        }
    }

    internal sealed class ProjectCreatedEventConsumerDefinition() :
        ConsumerDefinitionBase<ProjectCreatedEventConsumer>("project-created-cv-queue", "project-created-exchange");

    internal sealed class RegistrationDefinition : ConsumerConfigurationRegistration<
        ProjectCreatedEventConsumer,
        ProjectCreatedEventConsumerDefinition>;
}
