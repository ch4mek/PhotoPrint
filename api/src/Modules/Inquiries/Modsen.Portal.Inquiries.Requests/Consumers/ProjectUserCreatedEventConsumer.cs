using MassTransit;
using Modsen.Portal.Core.Communication.ExchangeDefinition;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Inquiries.Requests.InquiryAccepts.Core.Resolvers;
using Modsen.Portal.TimeTrack.Contract.Events;

namespace Modsen.Portal.Inquiries.Requests.Consumers;

internal sealed class ProjectUserCreatedEventConsumer : IConsumer<ProjectUserCreatedEvent>
{
    private readonly IUpdateUserAcceptsResolver _acceptsResolver;

    public ProjectUserCreatedEventConsumer(IUpdateUserAcceptsResolver acceptsResolver)
    {
        _acceptsResolver = acceptsResolver;
    }

    public async Task Consume(ConsumeContext<ProjectUserCreatedEvent> context)
    {
        var @event = context.Message;

        if (@event.UserPosition.IsIntern())
        {
            return;
        }

        await _acceptsResolver.ResolveAsync(@event.UserId, context.CancellationToken);
    }

    internal sealed class ProjectUserCreatedEventConsumerDefinition() :
        ConsumerDefinitionBase<ProjectUserCreatedEventConsumer>(
            "project-user-created-inquiries-queue",
            "project-user-created-exchange");

    internal sealed class RegistrationDefinition : ConsumerConfigurationRegistration<
        ProjectUserCreatedEventConsumer,
        ProjectUserCreatedEventConsumerDefinition>;
}
