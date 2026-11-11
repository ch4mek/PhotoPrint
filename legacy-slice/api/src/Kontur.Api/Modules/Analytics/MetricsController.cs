using Kontur.Api.Infrastructure;
using Kontur.Core.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kontur.Api.Modules.Analytics;

/// <summary>Пульс компании: KPI, паспорта метрик, drill-down (УП-02, УП-03).</summary>
[ApiController]
[Route("metrics")]
[Authorize]
public class MetricsController(KpiEngine kpi, ICurrentUser me, AuditService audit) : ControllerBase
{
    /// <summary>Все KPI периода с планом/фактом/прогнозом и цветовым статусом.</summary>
    [HttpGet("pulse")]
    public async Task<IActionResult> Pulse([FromQuery] DateTime? from, [FromQuery] DateTime? to,
        [FromQuery] string? department, [FromQuery] string? product)
    {
        await EnsureScopeAsync(department);
        var (f, t) = Norm(from, to);
        var kpis = await kpi.PulseAsync(f, t);
        var alerts = kpis
            .Where(k => k.Status == "red")
            .Select(k => new
            {
                id = $"alert-{k.Code}",
                severity = "critical",
                kpiCode = k.Code,
                message = $"{k.Title}: {k.Fact:0.##} {k.Unit} — красная зона (порог {k.Definition.Red})",
                drillTo = $"/metrics/{k.Code}/breakdown",
            })
            .ToList();
        return Ok(new { period = new { from = f, to = t }, filters = new { department, product }, kpis, criticalAlerts = alerts });
    }

    /// <summary>Стеклянная метрика: формула, источник, обновление, владелец.</summary>
    [HttpGet("{code}")]
    public IActionResult Definition(string code) =>
        KpiEngine.Defs.TryGetValue(code, out var def)
            ? Ok(def)
            : throw new ApiException(404, $"Неизвестная метрика: {code}");

    /// <summary>Drill-down метрики по измерению — путь до причины отклонения.</summary>
    [HttpGet("{code}/breakdown")]
    public async Task<IActionResult> Breakdown(string code, [FromQuery] string by = "week",
        [FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null, [FromQuery] string? department = null)
    {
        await EnsureScopeAsync(department);
        var (f, t) = Norm(from, to);
        var items = await kpi.BreakdownAsync(code, by, f, t);
        return Ok(new { code, by, period = new { from = f, to = t }, items });
    }

    /// <summary>Data-scope: руководитель подразделения не видит чужое (УП-05), попытка — в аудит.</summary>
    private async Task EnsureScopeAsync(string? department)
    {
        if (string.IsNullOrEmpty(department)) return;
        if (me.Scope != "*" && department != me.Scope)
        {
            await audit.LogAsync(me, "access.denied", "department", department, "denied",
                $"запрос показателей закрытого подразделения при scope={me.Scope}");
            throw new ApiException(403, "Нет доступа к показателям этого подразделения");
        }
    }

    private static (DateTime from, DateTime to) Norm(DateTime? from, DateTime? to)
    {
        var t = to?.Date.AddDays(1) ?? DateTime.UtcNow.Date.AddDays(1);
        var f = from?.Date ?? t.AddDays(-30);
        return (f, t);
    }
}
