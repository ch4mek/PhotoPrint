namespace Modsen.Portal.Equipment.Requests.EquipmentPortalSettings.Dto;

public class EquipmentPortalSettingsDto
{
    public string? SummaryReportWorksheet { get; init; }
    public int BaseFontSize { get; init; }
    public string? EquipmentsFileName { get; init; }
    public int DefaultColumnWidthInSymbols { get; init; }
    public string? FileExportDateFormat { get; init; }

    public string? DefaultDateFormat { get; init; }
}
