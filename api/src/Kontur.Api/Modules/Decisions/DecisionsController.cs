using Kontur.Api.Data;
using Kontur.Api.Infrastructure;
using Kontur.Api.Modules.Analytics;
using Kontur.Core.Domain;
using Kontur.Core.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Kontur.Api.Modules.Decisions;

public record DecisionCreateRequest(
    string? Title, string Problem, string? Impact, string? Cause, string Action,
    string OwnerId, string? OwnerName, DateTime? Deadline, int? Priority,
    string? KpiCode, string? RefEntity);

public record DecisionPatchRequest(string? Status, string? Action, string? OwnerId, DateTime? Deadline);

/// <summary>Реестр решений: карточка → владелец/срок → повторная проверка эффекта (УП-04, слайд 5 ТЗ).</summary>
[ApiController]
[Route("decisions")]
[Authorize]
public class DecisionsController(KonturDbContext db, KpiEngine kpi, AuditService audit, ICurrentUser me)
    : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] string? status, [FromQuery] bool? overdue)
    {
        var q = db.Decisions.AsNoTracking().AsQueryable();
        if (!string.IsNullOrEmpty(status)) q = q.Where(d => d.Status == status);
        if (overdue == true)
            q = q.Where(d => d.Deadline < DateTime.UtcNow && (d.Status == "new" || d.Status == "in_progress" || d.Status == "done"));
        var items = await q.OrderByDescending(d => d.CreatedAtUtc).ToListAsync();
        return Ok(new { items, total = items.Count });
    }

    [HttpPost]
    public async Task<IActionResult> Create(DecisionCreateRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Problem) || string.IsNullOrWhiteSpace(req.Action)
            || string.IsNullOrWhiteSpace(req.OwnerId) || req.Deadline is null)
            throw new ApiException(400, "Обязательные поля: problem, action, ownerId, deadline");

        // фиксируем точку отсчёта KPI (окно как в Пульсе — 30 дней) для повторной проверки
        double? factAtCreation = null;
        if (!string.IsNullOrEmpty(req.KpiCode) && KpiEngine.Defs.ContainsKey(req.KpiCode))
            factAtCreation = Math.Round(await kpi.ComputeAsync(req.KpiCode,
                DateTime.UtcNow.AddDays(-30), DateTime.UtcNow.AddDays(1)), 2);

        var d = new Decision
        {
            Id = $"dec-{Guid.NewGuid().ToString()[..8]}",
            Title = string.IsNullOrWhiteSpace(req.Title) ? req.Problem[..Math.Min(60, req.Problem.Length)] : req.Title!,
            Problem = req.Problem,
            Impact = req.Impact ?? "",
            Cause = req.Cause ?? "",
            ActionPlan = req.Action,
            OwnerId = req.OwnerId,
            OwnerName = req.OwnerName ?? "",
            Deadline = req.Deadline!.Value,
            Priority = req.Priority ?? 3,
            Status = "new",
            KpiCode = req.KpiCode ?? "",
            RefEntity = req.RefEntity ?? "",
            FactAtCreation = factAtCreation,
            CreatedAtUtc = DateTime.UtcNow,
            CreatedBy = me.Id,
        };
        db.Decisions.Add(d);
        await db.SaveChangesAsync();
        await audit.LogAsync(me, "decision.created", "decision", d.Id, "success", d.Title);
        return StatusCode(201, d);
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> Patch(string id, DecisionPatchRequest req)
    {
        var d = await db.Decisions.FirstOrDefaultAsync(x => x.Id == id)
                ?? throw new ApiException(404, $"Решение {id} не найдено");

        var changes = new List<string>();
        if (!string.IsNullOrEmpty(req.Status) && req.Status != d.Status)
        {
            if (!new[] { "new", "in_progress", "done", "verified", "failed" }.Contains(req.Status))
                throw new ApiException(400, $"Недопустимый статус: {req.Status}");
            changes.Add($"status: {d.Status} → {req.Status}");
            d.Status = req.Status;
        }
        if (!string.IsNullOrEmpty(req.Action) && req.Action != d.ActionPlan) { changes.Add("изменено действие"); d.ActionPlan = req.Action; }
        if (!string.IsNullOrEmpty(req.OwnerId) && req.OwnerId != d.OwnerId) { changes.Add($"владелец: {d.OwnerId} → {req.OwnerId}"); d.OwnerId = req.OwnerId; d.OwnerName = ""; }
        if (req.Deadline is { } dl && dl != d.Deadline) { changes.Add($"срок: {d.Deadline:yyyy-MM-dd} → {dl:yyyy-MM-dd}"); d.Deadline = dl; }

        if (changes.Count == 0) throw new ApiException(400, "Нет изменений");
        await db.SaveChangesAsync();
        await audit.LogAsync(me, "decision.updated", "decision", d.Id, "success", string.Join("; ", changes));
        return Ok(d);
    }

    /// <summary>Повторная проверка: сравниваем KPI «до» и «после» — главный критерий заказчика.</summary>
    [HttpPost("{id}/verify")]
    public async Task<IActionResult> Verify(string id)
    {
        var d = await db.Decisions.FirstOrDefaultAsync(x => x.Id == id)
                ?? throw new ApiException(404, $"Решение {id} не найдено");
        if (string.IsNullOrEmpty(d.KpiCode) || d.FactAtCreation is null)
            return Ok(new { decisionId = id, outcome = "insufficient_data", note = "решение не привязано к KPI" });

        var now = Math.Round(await kpi.ComputeAsync(d.KpiCode,
            DateTime.UtcNow.AddDays(-30), DateTime.UtcNow.AddDays(1)), 2);
        var def = KpiEngine.Defs[d.KpiCode];
        var improved = def.HigherIsBetter ? now > d.FactAtCreation : now < d.FactAtCreation;

        d.FactAtVerify = now;
        d.Status = improved ? "verified" : "failed";
        await db.SaveChangesAsync();
        await audit.LogAsync(me, "decision.verified", "decision", d.Id, "success",
            $"KPI {d.KpiCode}: {d.FactAtCreation} → {now}, итог: {d.Status}");

        return Ok(new
        {
            decisionId = id,
            outcome = d.Status,
            before = d.FactAtCreation,
            after = now,
            note = improved
                ? $"KPI улучшился ({d.FactAtCreation} → {now} {def.Unit}) — решение сработало"
                : $"KPI не улучшился ({d.FactAtCreation} → {now} {def.Unit}) — решение не сработало",
        });
    }

    [HttpGet("{id}/history")]
    public async Task<IActionResult> History(string id) =>
        Ok(await db.AuditLog.AsNoTracking()
            .Where(a => a.ObjectType == "decision" && a.ObjectId == id)
            .OrderBy(a => a.AtUtc)
            .ToListAsync());
}
