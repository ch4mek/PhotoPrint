using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.Utils.Modules;
using Modsen.Portal.Identity.Requests.ResourceGroups.Core.BusinessRules;
using Modsen.Portal.Identity.Requests.ResourceGroups.Core.Resolvers;

namespace Modsen.Portal.Identity.Requests.ResourceGroups;

internal sealed class ResourceGroupsModule : RegistrationModule
{
    public override void Load(IServiceCollection services)
    {
        services.AddScoped<ICanGroupMemberBeAddedBusinessRule, CanGroupMemberBeAddedBusinessRule>();
        services.AddScoped<ICanUserUpdateGroupBusinessRule, CanUserUpdateGroupBusinessRule>();
        services.AddScoped<IIsNewGroupManagerValidBusinessRule, IsNewGroupManagerValidBusinessRule>();
        services.AddScoped<ICanGroupMemberBeRemovedBusinessRule, CanGroupMemberBeRemovedBusinessRule>();
        services.AddScoped<IUpdateGroupManagerResolver, UpdateGroupManagerResolver>();
    }
}
