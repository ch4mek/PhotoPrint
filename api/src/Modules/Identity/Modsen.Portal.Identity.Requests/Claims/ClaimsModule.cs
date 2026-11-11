using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.Utils.Modules;
using Modsen.Portal.Identity.Requests.Claims.Core;

namespace Modsen.Portal.Identity.Requests.Claims
{
    internal sealed class ClaimsModule : RegistrationModule
    {
        public override void Load(IServiceCollection services)
        {
            services.AddScoped<IUserGroupClaimsLoader, UserGroupClaimsLoader>();
            services.AddScoped<IUserClaimsByGroupIdLoader, UserClaimsByGroupIdLoader>();
        }
    }
}
