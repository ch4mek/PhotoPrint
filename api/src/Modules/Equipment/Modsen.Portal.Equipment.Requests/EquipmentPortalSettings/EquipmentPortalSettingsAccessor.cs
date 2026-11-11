using Modsen.Portal.Core.Services.PortalSettings;
using Modsen.Portal.Core.SettingNames;
using Modsen.Portal.Equipment.Requests.EquipmentPortalSettings.Dto;

namespace Modsen.Portal.Equipment.Requests.EquipmentPortalSettings;

public interface IEquipmentPortalSettingsAccessor
{
    Task<EquipmentPortalSettingsDto> GetSettings(CancellationToken token);
}

public class EquipmentPortalSettingsAccessor :
    BasePortalSettingsAccessor<EquipmentPortalSettingsDto>,
    IEquipmentPortalSettingsAccessor
{
    public EquipmentPortalSettingsAccessor(IPortalSettingsAccessor portalSettingsAccessor)
        : base(portalSettingsAccessor) { }

    protected override string[] GetSettingNames()
    {
        return
        [
            ExcelSettingNames.SummaryReportWorksheet,
            ExcelSettingNames.BaseFontSize,
            ExcelSettingNames.EquipmentsFileName,
            ExcelSettingNames.DefaultColumnWidthInSymbols,
            EquipmentSettingNames.FileExportDateFormat,
            EquipmentSettingNames.DefaultDateFormat
        ];
    }

    protected override EquipmentPortalSettingsDto CreateSettingsModel(IReadOnlyCollection<PortalSettingValue> settings)
    {
        return new EquipmentPortalSettingsDto
        {
            SummaryReportWorksheet = Get(ExcelSettingNames.SummaryReportWorksheet).GetString(),
            BaseFontSize = Get(ExcelSettingNames.BaseFontSize).GetInt(),
            EquipmentsFileName = Get(ExcelSettingNames.EquipmentsFileName).GetString(),
            DefaultColumnWidthInSymbols = Get(ExcelSettingNames.DefaultColumnWidthInSymbols).GetInt(),
            FileExportDateFormat = Get(EquipmentSettingNames.FileExportDateFormat).GetString(),
            DefaultDateFormat = Get(EquipmentSettingNames.DefaultDateFormat).GetString()
        };
    }
}
