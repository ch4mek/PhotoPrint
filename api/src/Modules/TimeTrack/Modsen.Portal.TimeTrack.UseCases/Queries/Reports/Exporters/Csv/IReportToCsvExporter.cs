using Modsen.Portal.Core.Export;
using Modsen.Portal.TimeTrack.UseCases.Queries.Reports.Exporters.Csv.Models;

namespace Modsen.Portal.TimeTrack.UseCases.Queries.Reports.Exporters.Csv;

public interface IReportToCsvExporter : ICsvExporter<TasksToCsvExportModel>;
