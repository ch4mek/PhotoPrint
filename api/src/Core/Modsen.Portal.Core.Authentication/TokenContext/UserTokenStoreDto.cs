namespace Modsen.Portal.Core.Authentication.TokenContext;

public sealed class UserTokenStoreDto
{
    public required string AccessToken { get; init; }
    public required IReadOnlyCollection<string> IssuedForAddresses { get; init; }
    public required DateTime? ValidTillUtc { get; init; }
}
