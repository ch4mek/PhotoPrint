namespace Modsen.Portal.Core.Services.PortalSettings;

public interface IPortalSettingsAccessor : IRecyclableCache
{
    Task<string> GetStringValueByName(string settingName, CancellationToken token);
    Task<IReadOnlyCollection<PortalSettingValue>> GetByNames(string[] settingNames, CancellationToken token);
    Task<IReadOnlyCollection<PortalSettingValue>> GetAll(CancellationToken token);
}
