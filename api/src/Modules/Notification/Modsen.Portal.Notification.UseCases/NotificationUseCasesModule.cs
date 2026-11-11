using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.Communication.ExchangeDefinition;
using Modsen.Portal.Core.Utils.Modules;
using Modsen.Portal.Notification.UseCases.Consumers;
using Modsen.Portal.Notification.UseCases.Core;
using Modsen.Portal.Notification.UseCases.Core.Composers;
using Modsen.Portal.Notification.UseCases.Requests.Queries;

namespace Modsen.Portal.Notification.UseCases;

public sealed class NotificationUseCasesModule : RegistrationModule
{
    public override void Load(IServiceCollection services)
    {
        services.AddScoped<ICanUserInteractWithNotificationsBusinessRule, CanUserInteractWithNotificationsBusinessRule>();
        services.AddScoped<IMarkNotificationsAsSentDbQuery, MarkNotificationsAsSentDbQuery>();
        services.AddScoped<INotificationSender, NotificationSender>();
        services.AddScoped<INotificationComposer, NotificationComposer>();
        services.AddScoped<IInquiryTeamsNotificationComposer, InquiryTeamsNotificationComposer>();
    }

    public override IEnumerable<IConsumerConfigurationRegistration> GetConsumerRegistrationConfigurations()
    {
        yield return new DevelopmentPlanDeletedEventConsumer.RegistrationDefinition();
        yield return new NotificationsCreatedEventConsumer.RegistrationDefinition();
        yield return new UserCreatedEventConsumer.RegistrationDefinition();
        yield return new UserUpdatedEventConsumer.RegistrationDefinition();
        yield return new UserSyncEventConsumer.RegistrationDefinition();
    }
}
