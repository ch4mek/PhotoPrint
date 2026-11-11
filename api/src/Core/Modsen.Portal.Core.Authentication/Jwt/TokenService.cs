using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Core.Utils.Extensions;

namespace Modsen.Portal.Core.Authentication.Jwt;

internal sealed class TokenService : ITokenService
{
    private readonly JwtAuthenticationOptions _authenticationOptions;
    private readonly IClockService _clockService;

    public TokenService(
        JwtAuthenticationOptions authenticationOptions,
        IClockService clockService)
    {
        _authenticationOptions = authenticationOptions;
        _clockService = clockService;
    }

    public TokenModel GenerateToken(UserTokenDto user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_authenticationOptions.SecurityKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.NameIdentifier),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Name, user.LastName),
            new(ClaimTypes.GivenName, user.FirstName)
        };

        if (!user.DepartmentId.IsNullOrWhiteSpace())
        {
            claims.Add(new Claim(PortalClaimTypeNames.DepartmentId, user.DepartmentId));
        }

        if (user.Roles.IsGroupManager())
        {
            var resourceGroupId = user.ResourceGroupId ?? -1;
            claims.Add(new Claim(PortalClaimTypeNames.ResourceGroupId, resourceGroupId.ToString(CultureInfo.InvariantCulture)));
        }
        else if (user.ResourceGroupId.HasValue)
        {
            claims.Add(new Claim(PortalClaimTypeNames.ResourceGroupId, user.ResourceGroupId.Value.ToString(CultureInfo.InvariantCulture)));
        }

        claims.AddRange(user.Roles.Select(userRole => new Claim(ClaimTypes.Role, userRole)));
        claims.AddRange(user.Claims);

        var currentDateTime = _clockService.UtcNow;
        var validToUtc = currentDateTime.AddMinutes(_authenticationOptions.TokenLifetimeInMinutes);
        var jwt = new JwtSecurityToken(
            _authenticationOptions.Issuer,
            _authenticationOptions.Audience,
            claims: claims,
            currentDateTime,
            expires: validToUtc,
            signingCredentials: credentials);

        var tokenHandler = new JwtSecurityTokenHandler();

        return new TokenModel(tokenHandler.WriteToken(jwt), validToUtc);
    }

    public (bool IsValid, Claim[] Claims) GetClaimsFromToken(string token)
    {
        var validationParameters = GetTokenValidationParameters();

        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

            var jwt = (JwtSecurityToken)validatedToken;

            return (true, jwt.Claims.ToArray());
        }
        catch (SecurityTokenValidationException)
        {
            return (false, []);
        }
    }

    private TokenValidationParameters GetTokenValidationParameters()
    {
        return new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = _authenticationOptions.Issuer,
            ValidateAudience = true,
            ValidAudience = _authenticationOptions.Audience,
            ValidateLifetime = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_authenticationOptions.SecurityKey)),
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.FromMinutes(5)
        };
    }
}
