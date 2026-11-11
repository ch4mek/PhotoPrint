using System.Globalization;
using CsvHelper;
using Modsen.Portal.TimeTrack.UseCases.Queries.Reports.Exporters.Csv;
using Modsen.Portal.TimeTrack.UseCases.Queries.Reports.Exporters.Csv.Models;

namespace Modsen.Portal.Csv;

internal sealed class ReportToCsvExporter : IReportToCsvExporter
{
    public Task<byte[]> Export(TasksToCsvExportModel model, CancellationToken token)
    {
        using var stream = new MemoryStream();
        using (var writer = new StreamWriter(stream))
        {
            using var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture);

            var csvMap = new TaskToExportCsvMap(model.DateFormat);

            csvWriter.Context.RegisterClassMap(csvMap);
            csvWriter.WriteRecords(model.Tasks);
        }

        return Task.FromResult(stream.ToArray());
    }
}
