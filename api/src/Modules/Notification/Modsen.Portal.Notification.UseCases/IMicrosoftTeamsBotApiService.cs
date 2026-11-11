namespace Modsen.Portal.Notification.UseCases;

public interface IMicrosoftTeamsBotApiService
{
    Task SendNotificationAsync(string jsonModel, CancellationToken cancellationToken = default);
}
