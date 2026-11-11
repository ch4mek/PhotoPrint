using Kontur.Api.Data;
using Kontur.Api.Infrastructure;
using Kontur.Core.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Kontur.Api.Modules.Audit;

/// <summary>Единый журнал действий (ОТ-04). Доступен директору и аналитику.</summary>
[ApiController]
[Route("audit")]
[Authorize]
public class AuditController(KonturDbContext db, ICurrentUser me, AuditService audit) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] string? objectType, [FromQuery] int limit = 200)
    {
        if (me.Role is not ("director" or "analyst"))
        {
            await audit.LogAsync(me, "access.denied", "audit", "audit_log", "denied", $"роль {me.Role} не имеет доступа к журналу");
            throw new ApiException(403, "Журнал доступен директору и аналитику");
        }

        var q = db.AuditLog.AsNoTracking().AsQueryable();
        if (!string.IsNullOrEmpty(objectType)) q = q.Where(a => a.ObjectType == objectType);
        var items = await q.OrderByDescending(a => a.AtUtc).Take(Math.Min(limit, 1000)).ToListAsync();
        return Ok(new { items, total = items.Count });
    }
}
