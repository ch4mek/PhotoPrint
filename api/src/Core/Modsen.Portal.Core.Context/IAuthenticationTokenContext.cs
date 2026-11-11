namespace Modsen.Portal.Core.Context;

public interface IAuthenticationTokenContext
{
    Task RegisterUserToken(int userId, string token, DateTime validTillUtc, CancellationToken cancellationToken);
    Task RevokeAllUserTokens(int userId, string reason, CancellationToken cancellationToken);
    Task RevokeUserToken(int userId, string token, string reason, CancellationToken cancellationToken);
    Task<bool> IsTokenRevoked(string token, CancellationToken cancellationToken);
    Task<string?> GetRevokeReason(string token, CancellationToken cancellationToken);
}
