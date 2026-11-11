using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Certificate.UseCases.CertificatePortalSettings;
using Modsen.Portal.Core.Utils.Modules;

namespace Modsen.Portal.Certificate.UseCases.Requests;

public sealed class CertificatesUseCasesModule : RegistrationModule
{
    public override void Load(IServiceCollection services)
    {
        services.AddScoped<ICertificatePortalSettingsAccessor, CertificatePortalSettingsAccessor>();
    }
}
