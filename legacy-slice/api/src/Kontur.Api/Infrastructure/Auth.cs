using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Kontur.Core.Domain;
using Microsoft.IdentityModel.Tokens;

namespace Kontur.Api.Infrastructure;

/// <summary>Текущий пользователь из JWT (клеймы: sub, login, role, scope).</summary>
public interface ICurrentUser
{
    string Id { get; }
    string Login { get; }
    string Role { get; }
    string Scope { get; }
}

public class HttpCurrentUser(IHttpContextAccessor accessor) : ICurrentUser
{
    private ClaimsPrincipal? Principal => accessor.HttpContext?.User;
    public string Id => Principal?.FindFirst("sub")?.Value ?? "";
    public string Login => Principal?.FindFirst("login")?.Value ?? "";
    public string Role => Principal?.FindFirst("role")?.Value ?? "";
    public string Scope => Principal?.FindFirst("scope")?.Value ?? "*";
}

public static class Auth
{
    private const string Salt = "kontur-demo-salt";

    public static string Hash(string login, string password) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes($"{login}:{password}:{Salt}"))).ToLowerInvariant();
}

public class TokenService(IConfiguration config)
{
    public string Issue(AppUser user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Secret"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var lifetime = int.TryParse(config["Jwt:LifetimeMinutes"], out var m) ? m : 720;
        var token = new JwtSecurityToken(
            issuer: config["Jwt:Issuer"],
            audience: config["Jwt:Audience"],
            claims:
            [
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim("login", user.Login),
                new Claim("role", user.Role),
                new Claim("scope", user.DepartmentScope),
            ],
            expires: DateTime.UtcNow.AddMinutes(lifetime),
            signingCredentials: creds);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
