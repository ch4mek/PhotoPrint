namespace Modsen.Portal.Core.Authentication.Jwt;

public sealed class JwtAuthenticationOptions
{
    public required string SecurityKey { get; set; }
    public required string Issuer { get; set; }
    public required string Audience { get; set; }
    public int TokenLifetimeInMinutes { get; set; }
}
