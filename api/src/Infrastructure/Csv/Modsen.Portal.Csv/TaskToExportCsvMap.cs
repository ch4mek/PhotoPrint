using System.Globalization;
using CsvHelper.Configuration;
using Modsen.Portal.TimeTrack.UseCases.Queries.Reports.Exporters.Csv.Models;

namespace Modsen.Portal.Csv;

internal sealed class TaskToExportCsvMap : ClassMap<TaskToCsvExportModel>
{
    public TaskToExportCsvMap(string defaultDateFormat)
    {
        AutoMap(CultureInfo.InvariantCulture);
        Map(m => m.TimeInMinutes)
            .Name("Time (h)")
            .Convert(t =>
        {
            var interval = TimeSpan.FromMinutes(t.Value.TimeInMinutes);
            return $"{interval.Days * 24 + interval.Hours}:{interval.Minutes:d2}";
        });
        Map(m => m.Date)
            .Convert(t => t.Value.Date.ToString(defaultDateFormat, CultureInfo.InvariantCulture));
    }
}
