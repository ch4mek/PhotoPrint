namespace Modsen.Portal.Equipment.Requests.Equipment.Core.CoreDto;

public class EquipmentsToExcelExportCoreDto
{
    public IEnumerable<EquipmentToExcelExportCoreDto> Equipments { get; set; } = [];
    public bool ShouldPrintPurchaseDetails { get; set; }
    public string? SummaryReportWorksheet { get; init; }
    public int BaseFontSize { get; init; }
    public int DefaultColumnWidthInSymbols { get; init; }
    public required string DefaultDateFormat { get; init; }
    public bool ShouldPrintDepartment { get; set; }
}
