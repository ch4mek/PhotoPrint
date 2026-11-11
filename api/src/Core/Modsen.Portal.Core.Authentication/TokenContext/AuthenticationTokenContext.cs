using Modsen.Portal.Core.Authentication.Jwt;
using Modsen.Portal.Core.Context;

namespace Modsen.Portal.Core.Authentication.TokenContext;

public sealed class AuthenticationTokenContext : IAuthenticationTokenContext
{
    private readonly IHostEnvironmentContext _hostEnvironmentContext;
    private readonly IIpAddressRegionProvider _ipAddressRegionProvider;
    private readonly IRequestIpAddressProvider _requestIpAddressProvider;
    private readonly IAuthenticatedTokenStore _tokenStore;

    public AuthenticationTokenContext(
        IHostEnvironmentContext hostEnvironmentContext,
        IIpAddressRegionProvider ipAddressRegionProvider,
        IRequestIpAddressProvider requestIpAddressProvider,
        IAuthenticatedTokenStore tokenStore,
        JwtAuthenticationOptions authenticationOptions)
    {
        ArgumentNullException.ThrowIfNull(authenticationOptions);

        _hostEnvironmentContext = hostEnvironmentContext;
        _ipAddressRegionProvider = ipAddressRegionProvider;
        _requestIpAddressProvider = requestIpAddressProvider;
        _tokenStore = tokenStore;
    }

    public async Task RegisterUserToken(int userId, string token, DateTime validTillUtc, CancellationToken cancellationToken)
    {
        if (IsRevokeTokenFeatureEnabled())
        {
            var currentAddress = _requestIpAddressProvider.GetAddress();
            var userTokens = await _tokenStore.GetTokens(userId, cancellationToken);
            var tokensToRevoke = userTokens
                .Where(x => !_ipAddressRegionProvider.IsIssuedFor(currentAddress, x.IssuedForAddresses))
                .Select(x => x.AccessToken)
                .ToArray();
            foreach (var tokenToRevoke in tokensToRevoke)
            {
                await _tokenStore.Revoke(tokenToRevoke, userId, reason: RevokeReasons.RegisteredNewToken, cancellationToken);
            }

            userTokens = userTokens
                .Where(x => !tokensToRevoke.Contains(x.AccessToken))
                .ToList();
            userTokens.Add(new UserTokenStoreDto
            {
                AccessToken = token,
                IssuedForAddresses = _ipAddressRegionProvider.GetAddresses(currentAddress),
                ValidTillUtc = validTillUtc,
            });

            await _tokenStore.Save(userId, userTokens, cancellationToken);
        }
    }

    public async Task RevokeAllUserTokens(int userId, string reason, CancellationToken cancellationToken)
    {
        if (IsRevokeTokenFeatureEnabled())
        {
            var userTokens = await _tokenStore.GetTokens(userId, cancellationToken);
            foreach (var tokenToRevoke in userTokens.Select(x => x.AccessToken))
            {
                await _tokenStore.Revoke(tokenToRevoke, userId, reason, cancellationToken);
            }

            await _tokenStore.Save(userId, [], cancellationToken);
        }
    }

    public async Task RevokeUserToken(int userId, string token, string reason, CancellationToken cancellationToken)
    {
        if (IsRevokeTokenFeatureEnabled())
        {
            var userTokens = await _tokenStore.GetTokens(userId, cancellationToken);

            await _tokenStore.Revoke(token, userId, reason, cancellationToken);
            await _tokenStore.Save(userId, userTokens.Where(x => x.AccessToken != token).ToList(), cancellationToken);
        }
    }

    public Task<bool> IsTokenRevoked(string token, CancellationToken cancellationToken)
    {
        if (!IsRevokeTokenFeatureEnabled())
        {
            return Task.FromResult(false);
        }

        return _tokenStore.IsRevoked(token, cancellationToken);
    }

    public Task<string?> GetRevokeReason(string token, CancellationToken cancellationToken)
    {
        if (!IsRevokeTokenFeatureEnabled())
        {
            return Task.FromResult(default(string?));
        }

        return _tokenStore.GetRevokeReason(token, cancellationToken);
    }
    
    private bool IsRevokeTokenFeatureEnabled()
    {
        return _hostEnvironmentContext.IsStagingEnvironment() ||
            _hostEnvironmentContext.IsProductionEnvironment() ||
            _hostEnvironmentContext.IsIntegrationTestEnvironment() ||
            _hostEnvironmentContext.IsFunctionalTestEnvironment();
    }
}
