using Kontur.Api.Data;
using Kontur.Api.Infrastructure;
using Kontur.Core.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Kontur.Api.Modules.Identity;

public record LoginRequest(string Login, string Password);

[ApiController]
[Route("auth")]
public class AuthController(KonturDbContext db, TokenService tokens, AuditService audit, ICurrentUser me)
    : ControllerBase
{
    /// <summary>Вход по тестовой учётной записи, выдача JWT с ролью и scope (ОТ-02).</summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Login) || string.IsNullOrWhiteSpace(req.Password))
            throw new ApiException(400, "Логин и пароль обязательны");

        var user = await db.Users.FirstOrDefaultAsync(u => u.Login == req.Login.Trim().ToLowerInvariant());
        if (user is null || user.PasswordHash != Auth.Hash(user.Login, req.Password))
        {
            await audit.LogRawAsync("", req.Login, "login", "auth", req.Login, "denied", "неверный логин или пароль");
            throw new ApiException(401, "Неверный логин или пароль");
        }

        await audit.LogRawAsync(user.Id, user.Login, "login", "auth", user.Id, "success");
        return Ok(new
        {
            token = tokens.Issue(user),
            user = new { user.Id, user.Login, user.FullName, role = user.Role, departmentScope = user.DepartmentScope },
        });
    }

    [HttpGet("me")]
    [Authorize]
    public IActionResult Me() => Ok(new { me.Id, me.Login, me.Role, scope = me.Scope });
}
