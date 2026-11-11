using MassTransit;
using Modsen.Portal.Core.Communication.ExchangeDefinition;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.Requests.InquiryAccepts.Core.Resolvers;
using Modsen.Portal.TimeTrack.Contract.Events;

namespace Modsen.Portal.Inquiries.Requests.Consumers;

internal sealed class ProjectUpdatedEventConsumer : IConsumer<ProjectUpdatedEvent>
{
    private readonly IConnectionFactory _connectionFactory;
    private readonly IInquiriesDbContext _dbContext;
    private readonly IUpdateProjectManagerAcceptResolver _updateProjectManagerAcceptResolver;

    public ProjectUpdatedEventConsumer(
        IConnectionFactory connectionFactory,
        IInquiriesDbContext dbContext,
        IUpdateProjectManagerAcceptResolver updateProjectManagerAcceptResolver)
    {
        _connectionFactory = connectionFactory;
        _dbContext = dbContext;
        _updateProjectManagerAcceptResolver = updateProjectManagerAcceptResolver;
    }

    public async Task Consume(ConsumeContext<ProjectUpdatedEvent> context)
    {
        var @event = context.Message;

        await _updateProjectManagerAcceptResolver.ResolveAsync(
            @event.ProjectId, @event.OldManagerUserId, @event.NewManagerUserId, @event.IsActive,
            context.CancellationToken);

        await _dbContext.SaveChangesAsync(context.CancellationToken);
        await _connectionFactory.CommitCurrentTransaction(context.CancellationToken);
    }

    internal sealed class ProjectUpdatedEventConsumerDefinition() :
        ConsumerDefinitionBase<ProjectUpdatedEventConsumer>("project-updated-inquiries-queue", "project-updated-exchange");

    internal sealed class RegistrationDefinition : ConsumerConfigurationRegistration<
        ProjectUpdatedEventConsumer,
        ProjectUpdatedEventConsumerDefinition>;
}
