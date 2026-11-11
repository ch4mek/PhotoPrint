using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Modsen.Portal.Core.Authentication.Jwt;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Core.Utils.Extensions;

namespace Modsen.Portal.Core.Authentication.TokenContext;

public interface IAuthenticatedTokenStore
{
    Task Revoke(string token, int userId, string reason, CancellationToken cancellationToken);
    Task<bool> IsRevoked(string token, CancellationToken cancellationToken);
    Task<string?> GetRevokeReason(string token, CancellationToken cancellationToken);
    Task<List<UserTokenStoreDto>> GetTokens(int userId, CancellationToken cancellationToken);
    Task Save(int userId, IReadOnlyCollection<UserTokenStoreDto> userTokens, CancellationToken cancellationToken);
}

internal sealed class AuthenticatedTokenStore : IAuthenticatedTokenStore
{
    private readonly string _revokedMarker = "revoked";

    private readonly IClockService _clockService;
    private readonly IPublicTokenMaskProvider _publicTokenMaskProvider;
    private readonly IDistributedCache _distributedCache;
    private readonly ILogger<AuthenticatedTokenStore> _logger;
    private readonly TimeSpan _tokenLifeTime;

    public AuthenticatedTokenStore(
        IClockService clockService,
        IPublicTokenMaskProvider publicTokenMaskProvider,
        IDistributedCache distributedCache,
        ILogger<AuthenticatedTokenStore> logger,
        JwtAuthenticationOptions authenticationOptions)
    {
        _clockService = clockService;
        _publicTokenMaskProvider = publicTokenMaskProvider;
        _distributedCache = distributedCache;
        _logger = logger;
        _tokenLifeTime = TimeSpan.FromMinutes(authenticationOptions.TokenLifetimeInMinutes);
    }

    public async Task Revoke(string token, int userId, string reason, CancellationToken cancellationToken)
    {
        var maskedToken = _publicTokenMaskProvider.GetMaskedToken(token);

        _logger.LogWarning("Token {MaskedToken} has been revoked for user {UserId}. Reason: {Reason}", maskedToken, userId, reason);

        await _distributedCache.RemoveAsync(token, cancellationToken);
        await _distributedCache.SetStringAsync(token, _revokedMarker, new DistributedCacheEntryOptions
        {
            SlidingExpiration = _tokenLifeTime,
        }, cancellationToken);

        var revokeReasonCacheKey = GetRevokeReasonCacheKey(token);
        await _distributedCache.RemoveAsync(revokeReasonCacheKey, cancellationToken);
        await _distributedCache.SetStringAsync(revokeReasonCacheKey, reason, new DistributedCacheEntryOptions
        {
            SlidingExpiration = _tokenLifeTime,
        }, cancellationToken);
    }

    public async Task<bool> IsRevoked(string token, CancellationToken cancellationToken)
    {
        var cacheValue = await _distributedCache.GetStringAsync(token, cancellationToken);
        return cacheValue == _revokedMarker;
    }

    public Task<string?> GetRevokeReason(string token, CancellationToken cancellationToken)
    {
        var revokeReasonCacheKey = GetRevokeReasonCacheKey(token);
        return _distributedCache.GetStringAsync(revokeReasonCacheKey, cancellationToken);
    }

    public async Task<List<UserTokenStoreDto>> GetTokens(int userId, CancellationToken cancellationToken)
    {
        var cacheKey = GetUserTokensCacheKey(userId);
        var cacheValue = await _distributedCache.GetStringAsync(cacheKey, cancellationToken);
        var tokens = cacheValue.IsNullOrEmpty()
            ? []
            : JsonSerializer.Deserialize<UserTokenStoreDto[]>(cacheValue) ?? [];
        var nowUtc = _clockService.UtcNow;

        return tokens
            .Where(x => x.ValidTillUtc.HasValue)
            .Where(x => x.ValidTillUtc >= nowUtc)
            .ToList();
    }

    public async Task Save(int userId, IReadOnlyCollection<UserTokenStoreDto> userTokens, CancellationToken cancellationToken)
    {
        var cacheKey = GetUserTokensCacheKey(userId);
        var cacheValue = JsonSerializer.Serialize(userTokens.ToArray());

        await _distributedCache.RemoveAsync(cacheKey, cancellationToken);
        await _distributedCache.SetStringAsync(cacheKey, cacheValue, new DistributedCacheEntryOptions
        {
            SlidingExpiration = _tokenLifeTime
        }, cancellationToken);
    }

    private static string GetUserTokensCacheKey(int userId)
    {
        return $"user_tokens_{userId}";
    }

    private static string GetRevokeReasonCacheKey(string token)
    {
        var hash = Hash(token);
        return $"rvk_rsn_{hash}";
    }

    private static string Hash(string input)
    {
        var inputBytes = Encoding.UTF8.GetBytes(input);
        var inputHash = SHA256.HashData(inputBytes);
        return Convert.ToHexString(inputHash);
    }
}
