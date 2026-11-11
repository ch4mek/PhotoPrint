using System.Security.Claims;

namespace Modsen.Portal.Core.Authentication.Jwt;

public interface ITokenService
{
    TokenModel GenerateToken(UserTokenDto user);
    (bool IsValid, Claim[] Claims) GetClaimsFromToken(string token);
}
