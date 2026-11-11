using Microsoft.AspNetCore.DataProtection;
using Modsen.Portal.Core.Utils.Modules;
using Modsen.Portal.Identity.DataAccess.Npgsql;

namespace Modsen.Portal.Api.Modules;

internal sealed class DataProtectionModule : RegistrationModule
{
    public override void Load(IServiceCollection services)
    {
        services
            .AddDataProtection()
            .PersistKeysToDbContext<IdentityModuleDbContext>();
    }
}
