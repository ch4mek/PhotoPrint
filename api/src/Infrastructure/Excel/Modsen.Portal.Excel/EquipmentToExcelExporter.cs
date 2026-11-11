using System.Globalization;
using ClosedXML.Excel;
using Modsen.Portal.Equipment.Requests.Equipment.Core.CoreDto;
using Modsen.Portal.Equipment.Requests.Equipment.Core.Exporters;

namespace Modsen.Portal.Excel;

public sealed class EquipmentToExcelExporter : IEquipmentToExcelExporter
{
    private int _currentRow;
    private int _currentColumn;
    private readonly int _startRow = 1;
    private readonly int _startColumn = 1;
    private string[] _headerColumnsNames = [];

    private const int s_dateFormatId = 14;

    public Task<byte[]> Export(EquipmentsToExcelExportCoreDto coreDto, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(coreDto);

        using var workbook = new XLWorkbook();
        var sheet = workbook.AddWorksheet(coreDto.SummaryReportWorksheet);

        _currentRow = _startRow;
        _currentColumn = _startColumn;

        SetHeaderColumnsNames(coreDto.ShouldPrintPurchaseDetails, coreDto.ShouldPrintDepartment);

        PrintHeader(sheet);

        foreach (var equipment in coreDto.Equipments)
        {
            PrintEquipment(sheet, equipment, coreDto.ShouldPrintPurchaseDetails, coreDto.ShouldPrintDepartment, coreDto.DefaultDateFormat);
        }

        SetNumberColumnsStyle(coreDto.ShouldPrintPurchaseDetails, sheet);

        SetBaseStyle(sheet, coreDto);

        using var stream = new MemoryStream();

        workbook.SaveAs(stream);

        return Task.FromResult(stream.ToArray());
    }

    private void SetHeaderColumnsNames(bool shouldPrintPurchaseDetails, bool shouldPrintDepartment)
    {
        var headers = new List<string>
        {
            "Id",
            "Availability",
            "Invoice number",
            "Serial number",
            "Inventory number",
            "Name",
            "Type"
        };
        if (shouldPrintPurchaseDetails)
        {
            headers.Add("Price, $");
        }
        headers.Add("User");
        if (shouldPrintDepartment)
        {
            headers.Add("Department");
        }
        headers.Add("Equipment characteristics");
        if (shouldPrintPurchaseDetails)
        {
            headers.Add("Purchase place");
        }
        headers.Add("Approver");
        headers.Add("Purchase date");
        headers.Add("Storage");
        headers.Add("Guarantee date");
        headers.Add("Location");
        headers.Add("Repair place");
        headers.Add("Notes");

        _headerColumnsNames = headers.ToArray();
    }

    private static void SetNumberColumnsStyle(bool shouldPrintPurchaseDetails, IXLWorksheet sheet)
    {
        if (shouldPrintPurchaseDetails)
        {
            sheet.Column(12).Style.NumberFormat.SetNumberFormatId(s_dateFormatId);
            sheet.Column(13).Style.NumberFormat.SetNumberFormatId(s_dateFormatId);
            sheet.Column(6).Style.NumberFormat.Format = "#,##0.00";
        }
        else
        {
            sheet.Column(10).Style.NumberFormat.SetNumberFormatId(s_dateFormatId);
            sheet.Column(11).Style.NumberFormat.SetNumberFormatId(s_dateFormatId);
        }
    }

    private void PrintHeader(IXLWorksheet sheet)
    {
        sheet
            .FirstCell()
            .InsertData(_headerColumnsNames, true);

        sheet.FirstRow().Style.Font.SetBold();

        _currentRow++;
    }

    private void PrintEquipment(IXLWorksheet sheet, EquipmentToExcelExportCoreDto equipment,
        bool shouldPrintPurchaseDetails, bool shouldPrintDepartment, string defaultDateFormat)
    {
        var data = new List<string?>
        {
            equipment.Id.ToString(CultureInfo.InvariantCulture),
            equipment.Availability,
            equipment.InvoiceNumber,
            equipment.SerialNumber,
            equipment.InventoryNumber,
            equipment.Name,
            equipment.Type
        };

        if (shouldPrintPurchaseDetails)
        {
            data.Add(equipment.PurchasePriceUsd.ToString("#,##0.00", CultureInfo.InvariantCulture));
        }

        data.Add(equipment.ActiveHolder?.FullName);

        if (shouldPrintDepartment)
        {
            data.Add(equipment.ActiveHolder?.DepartmentFullName);
        }

        data.Add(equipment.Characteristics);
        if (shouldPrintPurchaseDetails)
        {
            data.Add(equipment.PurchasePlace);
        }

        data.Add(equipment.Approver?.ApproverFullName);
        data.Add(equipment.PurchaseDate.ToString(defaultDateFormat, CultureInfo.InvariantCulture));
        data.Add(equipment.Storage);
        data.Add(equipment.GuaranteeDate.ToString(defaultDateFormat, CultureInfo.InvariantCulture));
        data.Add(equipment.Location);
        data.Add(equipment.RepairPlace);
        data.Add(equipment.Comment);

        sheet.Cell(_currentRow, _currentColumn).InsertData(data.ToArray(), true);

        _currentRow++;
    }

    private static void SetBaseStyle(IXLWorksheet sheet, EquipmentsToExcelExportCoreDto coreDto)
    {
        sheet.Style.Font.SetFontSize(coreDto.BaseFontSize);

        sheet.CellsUsed().Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
        sheet.CellsUsed().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
        sheet.CellsUsed().Style.Alignment.WrapText = true;
        sheet.Columns().Width = coreDto.DefaultColumnWidthInSymbols;
    }
}
