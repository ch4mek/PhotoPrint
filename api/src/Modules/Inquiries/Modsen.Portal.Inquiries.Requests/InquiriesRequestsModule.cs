using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.Communication.ExchangeDefinition;
using Modsen.Portal.Core.Utils.Modules;
using Modsen.Portal.Inquiries.Contract;
using Modsen.Portal.Inquiries.Requests.Agreements;
using Modsen.Portal.Inquiries.Requests.Assigns;
using Modsen.Portal.Inquiries.Requests.Calendar;
using Modsen.Portal.Inquiries.Requests.Comments;
using Modsen.Portal.Inquiries.Requests.Consumers;
using Modsen.Portal.Inquiries.Requests.Core;
using Modsen.Portal.Inquiries.Requests.Core.BusinessRules;
using Modsen.Portal.Inquiries.Requests.Files;
using Modsen.Portal.Inquiries.Requests.Inquiries;
using Modsen.Portal.Inquiries.Requests.InquiryAccepts;
using Modsen.Portal.Inquiries.Requests.InquiryTemplates;

namespace Modsen.Portal.Inquiries.Requests;

public sealed class InquiriesRequestsModule : RegistrationModule
{
    public override void Load(IServiceCollection services)
    {
        AddModule<InquiryTemplatesModule>();
        AddModule<InquiriesModule>();
        AddModule<CommentsModule>();
        AddModule<CalendarModule>();
        AddModule<InquiryAcceptsModule>();
        AddModule<FilesModule>();
        AddModule<AgreementsModule>();
        AddModule<InquiriesAssignsModule>();

        base.Load(services);

        services.AddScoped<ICanInquiriesUserBeRetrievedBusinessRule, CanInquiriesUserBeRetrievedBusinessRule>();
        services.AddScoped<IHasUserAccessBusinessRule, HasUserAccessBusinessRule>();
        services.AddScoped<IInquiriesContract, InquiriesContract>();
        services.AddScoped<IUserLoader, UserLoader>();
        services.AddScoped<INotificationSender, NotificationSender>();
    }


    public override IEnumerable<IConsumerConfigurationRegistration> GetConsumerRegistrationConfigurations()
    {
        yield return new UserActivatedEventConsumer.RegistrationDefinition();
        yield return new UserCreatedEventConsumer.RegistrationDefinition();
        yield return new UserDeactivatedEventConsumer.RegistrationDefinition();
        yield return new UserSyncEventConsumer.RegistrationDefinition();
        yield return new UserUpdatedEventConsumer.RegistrationDefinition();
        yield return new UserRegisteredLocationUpdatedEventConsumer.RegistrationDefinition();
        yield return new ProjectUpdatedEventConsumer.RegistrationDefinition();
        yield return new ProjectUserCreatedEventConsumer.RegistrationDefinition();
        yield return new ProjectUserUpdatedEventConsumer.RegistrationDefinition();
        yield return new ContractCreatedEventConsumer.RegistrationDefinition();
        yield return new ContractUpdatedEventConsumer.RegistrationDefinition();
    }
}
