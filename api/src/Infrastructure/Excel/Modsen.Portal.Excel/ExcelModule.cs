using ClosedXML.Excel;
using ClosedXML.Graphics;
using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.Utils.Modules;
using Modsen.Portal.Equipment.Requests.Equipment.Core.Exporters;
using Modsen.Portal.TimeTrack.UseCases.Queries.Reports.Exporters.Excel;

namespace Modsen.Portal.Excel;

public sealed class ExcelModule : RegistrationModule
{
    public override void Load(IServiceCollection services)
    {
        LoadOptions.DefaultGraphicEngine = new DefaultGraphicEngine(Configuration["FontNames:DefaultFontName"]);

        services.AddTransient<IProjectToExcelExporter, ProjectToExcelExporter>();
        services.AddTransient<IProjectToExcelAccountExporter, ProjectToExcelAccountExporter>();
        services.AddTransient<IClientsToExcelExporter, ClientsToExcelExporter>();
        services.AddTransient<IClientsToExcelAccountExporter, ClientsToExcelAccountExporter>();
        services.AddTransient<IEquipmentToExcelExporter, EquipmentToExcelExporter>();
    }
}
