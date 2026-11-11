using Modsen.Portal.Core.Exceptions;

namespace Modsen.Portal.Core.Services.PortalSettings;

public abstract class BasePortalSettingsAccessor<TSettings> where TSettings : class, new()
{
    private readonly IPortalSettingsAccessor _portalSettingsAccessor;

    private IReadOnlyCollection<PortalSettingValue>? _settingValues;
    private TSettings? _settings;

    protected BasePortalSettingsAccessor(IPortalSettingsAccessor portalSettingsAccessor)
    {
        _portalSettingsAccessor = portalSettingsAccessor;
    }

    protected abstract string[] GetSettingNames();

    public async Task<TSettings> GetSettings(CancellationToken token)
    {
        _settingValues ??= await _portalSettingsAccessor.GetByNames(GetSettingNames(), token);
        return _settings ??= CreateSettingsModel(_settingValues);
    }

    protected abstract TSettings CreateSettingsModel(IReadOnlyCollection<PortalSettingValue> settings);

    protected PortalSettingValue Get(string name)
    {
        return _settingValues?.FirstOrDefault(x => x.Name == name) ??
               throw new AppException($"Portal setting '{name}' is not configured at all.");
    }
}

