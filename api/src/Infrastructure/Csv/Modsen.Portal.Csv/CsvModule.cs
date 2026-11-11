using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.Utils.Modules;
using Modsen.Portal.TimeTrack.UseCases.Queries.Reports.Exporters.Csv;

namespace Modsen.Portal.Csv;

public sealed class CsvModule : RegistrationModule
{
    public override void Load(IServiceCollection services)
    {
        services.AddScoped<IReportToCsvExporter, ReportToCsvExporter>();
    }
}
