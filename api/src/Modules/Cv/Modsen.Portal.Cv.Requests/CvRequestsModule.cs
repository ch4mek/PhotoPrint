using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.Communication.ExchangeDefinition;
using Modsen.Portal.Core.Utils.Modules;
using Modsen.Portal.Cv.Requests.Consumers;
using Modsen.Portal.Cv.Requests.Core.DbQueries;
using Modsen.Portal.Cv.Requests.Cvs;

namespace Modsen.Portal.Cv.Requests;

public sealed class CvRequestsModule : RegistrationModule
{
    public override void Load(IServiceCollection services)
    {
        AddModule<CvsModule>();

        base.Load(services);

        services.AddScoped<ICreateOrUpdateUserDbQuery, CreateOrUpdateUserDbQuery>();
    }


    public override IEnumerable<IConsumerConfigurationRegistration> GetConsumerRegistrationConfigurations()
    {
        yield return new UserActivatedEventConsumer.RegistrationDefinition();
        yield return new UserCreatedEventConsumer.RegistrationDefinition();
        yield return new UserDeactivatedEventConsumer.RegistrationDefinition();
        yield return new UserSyncEventConsumer.RegistrationDefinition();
        yield return new UserUpdatedEventConsumer.RegistrationDefinition();
        yield return new ClientCreatedEventConsumer.RegistrationDefinition();
        yield return new ClientUpdatedEventConsumer.RegistrationDefinition();
        yield return new ProjectCreatedEventConsumer.RegistrationDefinition();
        yield return new ProjectUpdatedEventConsumer.RegistrationDefinition();
    }
}
