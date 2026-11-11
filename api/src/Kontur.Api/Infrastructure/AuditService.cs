using Kontur.Api.Data;
using Kontur.Core.Domain;

namespace Kontur.Api.Infrastructure;

/// <summary>Единый журнал действий (ОТ-04): пользователи, ассистент, система, отказы доступа.</summary>
public class AuditService(KonturDbContext db)
{
    public async Task LogAsync(ICurrentUser user, string action, string objectType, string objectId,
        string result, string detail = "", string actorType = "user")
    {
        db.AuditLog.Add(new AuditEntry
        {
            AtUtc = DateTime.UtcNow,
            ActorId = user.Id,
            ActorLogin = user.Login,
            ActorType = actorType,
            Action = action,
            ObjectType = objectType,
            ObjectId = objectId,
            Result = result,
            Detail = detail,
        });
        await db.SaveChangesAsync();
    }

    /// <summary>Для событий вне сессии (попытка логина, системные события).</summary>
    public async Task LogRawAsync(string actorId, string actorLogin, string action, string objectType,
        string objectId, string result, string detail = "", string actorType = "user")
    {
        db.AuditLog.Add(new AuditEntry
        {
            AtUtc = DateTime.UtcNow,
            ActorId = actorId,
            ActorLogin = actorLogin,
            ActorType = actorType,
            Action = action,
            ObjectType = objectType,
            ObjectId = objectId,
            Result = result,
            Detail = detail,
        });
        await db.SaveChangesAsync();
    }
}
