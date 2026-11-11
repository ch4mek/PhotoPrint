using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.Communication.ExchangeDefinition;
using Modsen.Portal.Core.Utils.Modules;
using Modsen.Portal.Identity.Contract;
using Modsen.Portal.Identity.Requests.Claims;
using Modsen.Portal.Identity.Requests.Consumers;
using Modsen.Portal.Identity.Requests.Core;
using Modsen.Portal.Identity.Requests.Departments;
using Modsen.Portal.Identity.Requests.IdentityPortalSettings;
using Modsen.Portal.Identity.Requests.Messages.Core.Composers;
using Modsen.Portal.Identity.Requests.Messages.Core.DbQueries;
using Modsen.Portal.Identity.Requests.Messages.Core.Loader;
using Modsen.Portal.Identity.Requests.PermissionAssignments;
using Modsen.Portal.Identity.Requests.ResourceGroups;
using Modsen.Portal.Identity.Requests.Users;

namespace Modsen.Portal.Identity.Requests;

public sealed class IdentityUseCaseModule : RegistrationModule
{
    public override void Load(IServiceCollection services)
    {
        AddModule<UsersModule>();
        AddModule<ClaimsModule>();
        AddModule<DepartmentsModule>();
        AddModule<PermissionAssignmentsModule>();
        AddModule<ResourceGroupsModule>();

        base.Load(services);

        services.AddScoped<IIdentityPortalSettingsAccessor, IdentityPortalSettingsAccessor>();
        services.AddScoped<IUserLoader, UserLoader>();
        services.AddScoped<IUserMessageLoader, UserMessageLoader>();
        services.AddScoped<IGetUserThreadIdDbQuery, GetUserThreadIdDbQuery>();
        services.AddScoped<IUserDataComposer, UserDataComposer>();

        services.AddScoped<IIdentityCrossWriterContract, IdentityCrossWriterContract>();
    }

    public override IEnumerable<IConsumerConfigurationRegistration> GetConsumerRegistrationConfigurations()
    {
        yield return new InquiryTemplateApproversUpdatedEventConsumer.RegistrationDefinition();
        yield return new ProjectDeactivatedEventConsumer.RegistrationDefinition();
    }
}
