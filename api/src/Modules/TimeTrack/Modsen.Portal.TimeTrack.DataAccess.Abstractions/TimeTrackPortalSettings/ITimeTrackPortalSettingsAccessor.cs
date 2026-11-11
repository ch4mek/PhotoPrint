namespace Modsen.Portal.TimeTrack.DataAccess.Abstractions.TimeTrackPortalSettings;

public interface ITimeTrackPortalSettingsAccessor
{
    Task<TimeTrackPortalSettingsDto> GetSettings(CancellationToken token);
}
