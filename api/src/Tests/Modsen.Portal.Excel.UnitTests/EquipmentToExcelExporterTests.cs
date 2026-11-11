using System.Globalization;
using ClosedXML.Excel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.Equipment;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.Core.Utils.Modules;
using Modsen.Portal.Equipment.Requests.Equipment.Core.CoreDto;
using Modsen.Portal.Equipment.Requests.Equipment.Core.Exporters;
using Modsen.Portal.Equipment.Requests.EquipmentPortalSettings.Dto;

namespace Modsen.Portal.Excel.UnitTests;

public class EquipmentToExcelExporterTests
{
    private readonly IServiceScope _scope;
    private readonly EquipmentPortalSettingsDto _settingsDto;

    public EquipmentToExcelExporterTests()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                 { "FontNames:DefaultFontName", "DejaVu Sans" }
            })
            .Build();
        var serviceCollection = new ServiceCollection();
        var rootModule = new RegistrationModuleAggregate(configuration);

        rootModule.AddModule<ExcelModule>();
        rootModule.LoadRoot(serviceCollection);

        _scope = serviceCollection.BuildServiceProvider().CreateScope();

        _settingsDto = new EquipmentPortalSettingsDto
        {
            SummaryReportWorksheet = "Summary Report",
            BaseFontSize = 10,
            EquipmentsFileName = "Equipment",
            DefaultColumnWidthInSymbols = 30,
            DefaultDateFormat = "dd/MM/yyyy",
            FileExportDateFormat = "dd.MM.yyyy"
        };
    }

    [Fact]
    public async void ExportWhenEquipmentsProvidedShouldConvertAllToExcel()
    {
        var equipments = new List<EquipmentToExcelExportCoreDto>
        {
            new()
            {
                Id = 1,
                Name = "TestName",
                Type = "Mobile Phone",
                SerialNumber = "00000000001",
                InventoryNumber = "INV-001",
                PurchasePriceUsd = 3000,
                PurchaseDate = DateTime.Today.Date,
                PurchasePlace = "Minsk",
                GuaranteeDate = DateTime.Today.AddYears(1).Date,
                Characteristics = "Test characteristics",
                Location = "Belarus",
                Comment = "Test comment",
                InvoiceNumber = "fsdqAOAPS",
                Approver = new ApproverToExcelExportCoreDto
                {
                    ApproverId = 1,
                    ApproverFullName = "Test Approver"
                },
                ActiveHolder = new EquipmentHolderToExcelExportCoreDto
                {
                    Id = 2,
                    DepartmentFullName = "Test",
                    FullName = "Test Holder"
                },
                Availability = EquipmentAvailabilityType.Issued.GetDescription()
            }
        };

        var exporter = _scope.ServiceProvider.GetRequiredService<IEquipmentToExcelExporter>();

        var expectedTable = new List<string>
        {
            "Id", "Availability", "Invoice number", "Serial number", "Inventory number", "Name", "Type", "Price, $", "User",
            "Department", "Equipment characteristics", "Purchase place", "Approver", "Purchase date", "Storage",
            "Guarantee date", "Location", "Repair place", "Notes",
            "1", "Issued", "fsdqAOAPS", "00000000001", "INV-001", "TestName",
            "Mobile Phone", "3,000.00", "Test Holder", "Test", "Test characteristics", "Minsk", "Test Approver",
            DateTime.Today.Date.ToString(_settingsDto.DefaultDateFormat), "",
            DateTime.Today.AddYears(1).Date.ToString(_settingsDto.DefaultDateFormat),
            "Belarus", "", "Test comment"
        };

        var result = await exporter.Export(new EquipmentsToExcelExportCoreDto
        {
            Equipments = equipments,
            ShouldPrintPurchaseDetails = true,
            ShouldPrintDepartment = true,
            SummaryReportWorksheet = _settingsDto.SummaryReportWorksheet,
            BaseFontSize = _settingsDto.BaseFontSize,
            DefaultColumnWidthInSymbols = _settingsDto.DefaultColumnWidthInSymbols,
            DefaultDateFormat = _settingsDto.DefaultDateFormat
        }, CancellationToken.None);

        using var workbook = new XLWorkbook(new MemoryStream(result));

        workbook.TryGetWorksheet(_settingsDto.SummaryReportWorksheet, out var worksheet).Should().BeTrue();

        worksheet.CellsUsed().Should().HaveCount(expectedTable.Count(el => !el.IsNullOrEmpty()));

        worksheet
            .Cells("A1:S2")
            .Select(cell => cell.Value.IsNumber
                ? cell.Value.GetNumber().ToString(".00", CultureInfo.InvariantCulture)
                : cell.Value.ToString(CultureInfo.InvariantCulture)
            )
            .Should()
            .BeEquivalentTo(expectedTable, config => config.WithStrictOrdering());
    }

    [Fact]
    public async Task ExportWhenEquipmentsNotProvidedShouldConvertAndReturnOnlyLabelsAsync()
    {
        var equipments = new List<EquipmentToExcelExportCoreDto>();

        var exporter = _scope.ServiceProvider.GetRequiredService<IEquipmentToExcelExporter>();

        var expectedTable = new List<string>
        {
            "Id", "Availability", "Invoice number", "Serial number", "Inventory number", "Name",
            "Type", "Price, $", "User", "Department", "Equipment characteristics",
            "Purchase place", "Approver", "Purchase date", "Storage", "Guarantee date", "Location", "Repair place", "Notes"
        };

        var result = await exporter.Export(new EquipmentsToExcelExportCoreDto
        {
            Equipments = equipments,
            ShouldPrintDepartment = true,
            ShouldPrintPurchaseDetails = true,
            SummaryReportWorksheet = _settingsDto.SummaryReportWorksheet,
            BaseFontSize = _settingsDto.BaseFontSize,
            DefaultColumnWidthInSymbols = _settingsDto.DefaultColumnWidthInSymbols,
            DefaultDateFormat = _settingsDto.DefaultDateFormat
        }, CancellationToken.None);

        using var workbook = new XLWorkbook(new MemoryStream(result));

        workbook.TryGetWorksheet(_settingsDto.SummaryReportWorksheet, out var worksheet).Should().BeTrue();

        worksheet.CellsUsed().Should().HaveCount(expectedTable.Count(el => !el.IsNullOrEmpty()));

        worksheet
            .Cells("A1:S1")
            .Select(cell => cell.Value.IsNumber
                ? cell.Value.GetNumber().ToString(".00", CultureInfo.InvariantCulture)
                : cell.Value.ToString(CultureInfo.InvariantCulture)
            )
            .Should()
            .BeEquivalentTo(expectedTable, config => config.WithStrictOrdering());
    }
}
