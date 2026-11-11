using MassTransit;
using Modsen.Portal.Core.Communication.ExchangeDefinition;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Inquiries.Requests.InquiryAccepts.Core.Resolvers;
using Modsen.Portal.TimeTrack.Contract.Events;

namespace Modsen.Portal.Inquiries.Requests.Consumers;

internal sealed class ProjectUserUpdatedEventConsumer : IConsumer<ProjectUserUpdatedEvent>
{
    private readonly IUpdateUserAcceptsResolver _updateUserAcceptsResolver;

    public ProjectUserUpdatedEventConsumer(IUpdateUserAcceptsResolver updateUserAcceptsResolver)
    {
        _updateUserAcceptsResolver = updateUserAcceptsResolver;
    }

    public async Task Consume(ConsumeContext<ProjectUserUpdatedEvent> context)
    {
        var @event = context.Message;

        if (@event.UserPosition.IsIntern())
        {
            return;
        }

        await _updateUserAcceptsResolver.ResolveAsync(@event.UserId, context.CancellationToken);
    }

    internal sealed class ProjectUserUpdatedEventConsumerDefinition() :
        ConsumerDefinitionBase<ProjectUserUpdatedEventConsumer>(
            "project-user-updated-inquiries-queue",
            "project-user-updated-exchange");

    internal sealed class RegistrationDefinition : ConsumerConfigurationRegistration<
        ProjectUserUpdatedEventConsumer,
        ProjectUserUpdatedEventConsumerDefinition>;
}
