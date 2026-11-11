using Modsen.Portal.Core.Export;
using Modsen.Portal.TimeTrack.UseCases.Queries.Reports.Exporters.Excel.Models.Projects;

namespace Modsen.Portal.TimeTrack.UseCases.Queries.Reports.Exporters.Excel;

public interface IProjectToExcelAccountExporter : IExcelExporter<ProjectWithDatesToExportModel>;
