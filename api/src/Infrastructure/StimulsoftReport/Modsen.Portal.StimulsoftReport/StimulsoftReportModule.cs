using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.Core.Utils.Modules;
using Modsen.Portal.StimulsoftReport.Contract;
using Stimulsoft.Base;

namespace Modsen.Portal.StimulsoftReport;

public sealed class StimulsoftReportModule : RegistrationModule
{
    public override void Load(IServiceCollection services)
    {
        var licenseKey = Configuration.GetValue<string>("AppSettings:StimulsoftSettings:LicenseKey");
        if (!licenseKey.IsNullOrWhiteSpace())
        {
            StiLicense.Key = licenseKey;
        }

        services.AddScoped<IReportExporter, StimulsoftReportExporter>();
    }
}
