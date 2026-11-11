using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.Communication.ExchangeDefinition;
using Modsen.Portal.Core.Utils.Modules;
using Modsen.Portal.Finance.Requests.Consumers;
using Modsen.Portal.Finance.Requests.Core.Builders;
using Modsen.Portal.Finance.Requests.Core.Converters;
using Modsen.Portal.Finance.Requests.Core.DbQueries;
using Modsen.Portal.Finance.Requests.ExchangeRates;
using Modsen.Portal.Finance.Requests.Expenses;
using Modsen.Portal.Finance.Requests.Managements;
using Modsen.Portal.Finance.Requests.ProjectRates;
using Modsen.Portal.Finance.Requests.ProjectRates.Core.Composers;
using Modsen.Portal.Finance.Requests.ProjectRates.Core.Encryptor;
using Modsen.Portal.Finance.Requests.ProjectRates.Core.Loaders;

namespace Modsen.Portal.Finance.Requests;

public sealed class FinanceRequestsModule : RegistrationModule
{
    public override void Load(IServiceCollection services)
    {
        AddModule<ExpensesModule>();
        AddModule<ProjectRateModule>();
        AddModule<ExchangeRatesModule>();
        AddModule<ManagementsModule>();

        base.Load(services);

        services.AddScoped<IProjectUserLoader, ProjectUserLoader>();

        services.AddScoped<IFinanceClientsAndUsersQueryBuilder, FinanceClientsAndUsersQueryBuilder>();
        services.AddScoped<IGetProjectRatesQueryBuilder, GetProjectRatesQueryBuilder>();

        services.AddScoped<IProjectRateComposer, ProjectRateComposer>();

        services.AddScoped<ICreateOrUpdateUserDbQuery, CreateOrUpdateUserDbQuery>();
        services.AddScoped<IUpdateIsActiveForUserDbQuery, UpdateIsActiveForUserDbQuery>();

        services.AddScoped<IProjectRateEncryptor, ProjectRateEncryptor>();

        services.AddScoped<ICurrencyConverter, CurrencyConverter>();
    }

    public override IEnumerable<IConsumerConfigurationRegistration> GetConsumerRegistrationConfigurations()
    {
        yield return new UserActivatedEventConsumer.RegistrationDefinition();
        yield return new UserCreatedEventConsumer.RegistrationDefinition();
        yield return new UserDeactivatedEventConsumer.RegistrationDefinition();
        yield return new UserSyncEventConsumer.RegistrationDefinition();
        yield return new UserUpdatedEventConsumer.RegistrationDefinition();
        yield return new DepartmentSyncEventConsumer.RegistrationDefinition();
        yield return new ProjectCreatedEventConsumer.RegistrationDefinition();
        yield return new ProjectUpdatedEventConsumer.RegistrationDefinition();
        yield return new ClientCreatedEventConsumer.RegistrationDefinition();
        yield return new ClientUpdatedEventConsumer.RegistrationDefinition();
        yield return new ProjectUserCreatedEventConsumer.RegistrationDefinition();
        yield return new ProjectUserUpdatedEventConsumer.RegistrationDefinition();
    }
}
