namespace Modsen.Portal.TimeTrack.DataAccess.Abstractions.Clients;

public interface IGetClientNameByClientIdDbQuery
{
    Task<string?> GetClientNameByClientIdAsync(int clientId, CancellationToken cancellationToken);
}
