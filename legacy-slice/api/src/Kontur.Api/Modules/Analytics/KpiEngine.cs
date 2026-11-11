using Kontur.Api.Data;
using Kontur.Core.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Kontur.Api.Modules.Analytics;

/// <summary>Паспорт KPI: формула, источник, периодичность, владелец, пороги (УП-02 «стеклянная метрика»).</summary>
public record KpiDef(
    string Code, string Title, string Unit, string Formula, string Source, string Refresh, string Owner,
    double? Yellow, double? Red, bool HigherIsBetter);

public record KpiStatus(string Code, string Title, string Unit, double Fact, double? Plan, double? Forecast,
    string Status, KpiDef Definition);

public record BreakdownItem(string Key, double Value, double? Rate, string Status, string? DrillTo);

/// <summary>KPI-движок «Пульса». Единый словарь метрик — один источник формул для UI и ассистента.</summary>
public class KpiEngine(KonturDbContext db)
{
    public static readonly Dictionary<string, KpiDef> Defs = new()
    {
        ["revenue"] = new("revenue", "Оборот", "BYN", "Σ заказов со статусом completed", "ERP/заказы", "1 ч", "Финансы", null, null, true),
        ["avg_check"] = new("avg_check", "Средний чек", "BYN", "оборот / кол-во заказов", "ERP/заказы", "1 ч", "Продажи", null, null, true),
        ["orders_count"] = new("orders_count", "Заказы", "шт", "кол-во заказов за период", "ERP/заказы", "1 ч", "Продажи", null, null, true),
        ["defect_rate"] = new("defect_rate", "Уровень брака", "%", "заказы с дефектом / все заказы × 100", "Production/QC", "15 мин", "Производство", 2, 4, false),
        ["sla_percent"] = new("sla_percent", "SLA поддержки", "%", "обращения в SLA / все обращения × 100", "Support", "15 мин", "Поддержка", 95, 85, true),
        ["on_time_percent"] = new("on_time_percent", "Заказы в срок", "%", "доставлено в срок / доставлено × 100", "ERP/статусы", "1 ч", "Логистика", 93, 85, true),
        ["new_customers"] = new("new_customers", "Новые клиенты", "чел", "первый заказ внутри периода", "CRM", "1 ч", "Маркетинг", null, null, true),
        ["active_customers"] = new("active_customers", "Активные клиенты", "чел", "делали заказ в периоде", "CRM + ERP", "1 ч", "Маркетинг", null, null, true),
    };

    public static string StatusOf(KpiDef def, double fact)
    {
        if (def.Yellow is null || def.Red is null) return "green";
        return def.HigherIsBetter
            ? fact >= def.Yellow ? "green" : fact >= def.Red ? "yellow" : "red"
            : fact < def.Yellow ? "green" : fact < def.Red ? "yellow" : "red";
    }

    private static string WeekKey(DateTime d)
    {
        var monday = d.Date.AddDays(-(((int)d.DayOfWeek + 6) % 7));
        return monday.ToString("yyyy-MM-dd");
    }

    public async Task<double> ComputeAsync(string code, DateTime from, DateTime to)
    {
        if (!Defs.ContainsKey(code)) throw new ApiException(404, $"Неизвестная метрика: {code}");
        var orders = await db.Orders.Where(o => o.CreatedAtUtc >= from && o.CreatedAtUtc < to).ToListAsync();
        return code switch
        {
            "revenue" => (double)orders.Where(o => o.Status == "completed").Sum(o => o.AmountByn),
            "avg_check" => orders.Count != 0 ? (double)orders.Where(o => o.Status == "completed").Sum(o => o.AmountByn) / orders.Count : 0,
            "orders_count" => orders.Count,
            "defect_rate" => await DefectRateAsync(orders),
            "sla_percent" => await db.Interactions.Where(i => i.CreatedAtUtc >= from && i.CreatedAtUtc < to).ToListAsync() is { Count: > 0 } inter
                ? 100.0 * inter.Count(i => i.InSla) / inter.Count
                : 0,
            "on_time_percent" => orders.Where(o => o.Status == "completed").ToList() is { Count: > 0 } done
                ? 100.0 * done.Count(o => o.DeliveredOnTime == true) / done.Count
                : 0,
            "new_customers" => await db.Customers.CountAsync(c => c.FirstOrderAtUtc >= from && c.FirstOrderAtUtc < to),
            "active_customers" => orders.Select(o => o.CustomerId).Distinct().Count(),
            _ => throw new ApiException(404, $"Неизвестная метрика: {code}"),
        };
    }

    private async Task<double> DefectRateAsync(List<Core.Domain.OrderRow> orders)
    {
        if (orders.Count == 0) return 0;
        var ids = orders.Select(o => o.Id).ToHashSet();
        var defected = (await db.Defects.Where(d => ids.Contains(d.OrderId)).Select(d => d.OrderId).Distinct().ToListAsync()).Count;
        return 100.0 * defected / orders.Count;
    }

    /// <summary>Полный Пульс: все метрики периода + красные алерты.</summary>
    public async Task<List<KpiStatus>> PulseAsync(DateTime from, DateTime to)
    {
        var result = new List<KpiStatus>();
        foreach (var def in Defs.Values)
        {
            var fact = await ComputeAsync(def.Code, from, to);
            result.Add(new KpiStatus(def.Code, def.Title, def.Unit, Math.Round(fact, 2), null, null, StatusOf(def, fact), def));
        }
        return result;
    }

    /// <summary>Drill-down: от метрики к причине (станок, тема, неделя, канал, продукт).</summary>
    public async Task<List<BreakdownItem>> BreakdownAsync(string code, string by, DateTime from, DateTime to)
    {
        if (!Defs.ContainsKey(code)) throw new ApiException(404, $"Неизвестная метрика: {code}");
        var orders = await db.Orders.Where(o => o.CreatedAtUtc >= from && o.CreatedAtUtc < to).ToListAsync();
        var orderIds = orders.Select(o => o.Id).ToHashSet();

        var items = (code, by) switch
        {
            ("defect_rate", "station") => StationBreakdown(orders, orderIds),
            ("defect_rate", "cause") => await CauseBreakdownAsync(orderIds),
            ("sla_percent", "topic") => await TopicBreakdownAsync(from, to),
            ("sla_percent", "week") => await WeekSlaBreakdownAsync(from, to),
            ("revenue", "week") => orders.Where(o => o.Status == "completed")
                .GroupBy(o => WeekKey(o.CreatedAtUtc))
                .Select(g => ToItem(g.Key, (double)Math.Round(g.Sum(o => o.AmountByn), 2), null, "green", $"period={g.Key}")),
            ("revenue", "channel") => orders.Where(o => o.Status == "completed")
                .GroupBy(o => o.Channel)
                .Select(g => ToItem(g.Key, (double)Math.Round(g.Sum(o => o.AmountByn), 2), null, "green", $"channel={g.Key}")),
            ("revenue", "product") => orders.Where(o => o.Status == "completed")
                .GroupBy(o => o.ProductId)
                .Select(g => ToItem(g.Key, (double)Math.Round(g.Sum(o => o.AmountByn), 2), null, "green", $"product={g.Key}")),
            ("on_time_percent", "week") => orders.Where(o => o.Status == "completed")
                .GroupBy(o => WeekKey(o.CreatedAtUtc))
                .Select(g => ToItem(g.Key, g.Count() != 0 ? 100.0 * g.Count(o => o.DeliveredOnTime == true) / g.Count() : 0, null,
                    StatusOf(Defs["on_time_percent"], g.Count() != 0 ? 100.0 * g.Count(o => o.DeliveredOnTime == true) / g.Count() : 100), $"period={g.Key}")),
            _ => throw new ApiException(400, $"Разрез «{by}» для метрики «{code}» не поддерживается"),
        };
        return items.OrderByDescending(i => i.Value).ToList();
    }

    private List<BreakdownItem> StationBreakdown(List<Core.Domain.OrderRow> orders, HashSet<string> orderIds)
    {
        var defects = db.Defects.Where(d => orderIds.Contains(d.OrderId)).ToList();
        return defects
            .GroupBy(d => d.StationId)
            .Select(g =>
            {
                var stationOrders = orders.Count(o => o.StationId == g.Key);
                var rate = stationOrders != 0 ? 100.0 * g.Count() / stationOrders : 0;
                return ToItem(g.Key, g.Count(), Math.Round(rate, 1), StatusOf(Defs["defect_rate"], rate), $"equipment/{g.Key}");
            })
            .ToList();
    }

    private async Task<List<BreakdownItem>> CauseBreakdownAsync(HashSet<string> orderIds) =>
        (await db.Defects.Where(d => orderIds.Contains(d.OrderId)).ToListAsync())
            .GroupBy(d => d.Cause)
            .Select(g => ToItem(g.Key, g.Count(), null, "green", $"cause={g.Key}"))
            .ToList();

    private async Task<List<BreakdownItem>> TopicBreakdownAsync(DateTime from, DateTime to) =>
        (await db.Interactions.Where(i => i.CreatedAtUtc >= from && i.CreatedAtUtc < to).ToListAsync())
            .GroupBy(i => i.Topic)
            .Select(g =>
            {
                var sla = 100.0 * g.Count(i => i.InSla) / g.Count();
                return ToItem(g.Key, Math.Round(sla, 1), g.Count(), StatusOf(Defs["sla_percent"], sla), $"topic={g.Key}");
            })
            .ToList();

    private async Task<List<BreakdownItem>> WeekSlaBreakdownAsync(DateTime from, DateTime to) =>
        (await db.Interactions.Where(i => i.CreatedAtUtc >= from && i.CreatedAtUtc < to).ToListAsync())
            .GroupBy(i => WeekKey(i.CreatedAtUtc))
            .Select(g =>
            {
                var sla = 100.0 * g.Count(i => i.InSla) / g.Count();
                return ToItem(g.Key, Math.Round(sla, 1), g.Count(), StatusOf(Defs["sla_percent"], sla), $"period={g.Key}");
            })
            .ToList();

    private static BreakdownItem ToItem(string key, double value, double? rate, string status, string drill) =>
        new(key, Math.Round(value, 2), rate, status, drill);
}
