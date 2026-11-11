using System.Globalization;
using Kontur.Api.Infrastructure;
using Kontur.Core.Csv;
using Kontur.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace Kontur.Api.Data;

/// <summary>Загрузка демо-пользователей и синтетического датасета (dataset/seed, воспроизводимость ТЗ §9.5).</summary>
public static class Seeder
{
    private static DateTime Utc(string s) => DateTimeOffset.Parse(s, CultureInfo.InvariantCulture).UtcDateTime;

    public static async Task SeedAsync(KonturDbContext db, IConfiguration cfg, ILogger log)
    {
        await SeedUsersAsync(db);

        if (await db.Orders.AnyAsync())
        {
            log.LogInformation("Датасет уже загружен, пропускаем");
            return;
        }

        var path = cfg["Dataset:Path"] ?? "../../../dataset/seed";
        var full = ResolveDatasetPath(path);
        log.LogInformation("Загрузка датасета из {Path}", full);

        foreach (var r in Csv.Read(Path.Combine(full, "departments.csv")))
            db.Departments.Add(new Department { Id = r["id"], Name = r["name"] });

        foreach (var r in Csv.Read(Path.Combine(full, "products.csv")))
            db.Products.Add(new Product
            {
                Id = r["id"], Name = r["name"],
                PriceByn = decimal.Parse(r["price_byn"], CultureInfo.InvariantCulture),
                ProductionDays = int.Parse(r["production_days"], CultureInfo.InvariantCulture),
            });

        foreach (var r in Csv.Read(Path.Combine(full, "customers.csv")))
            db.Customers.Add(new Customer
            {
                Id = r["id"], Name = r["name"], Segment = r["segment"], Channel = r["channel"],
                FirstOrderAtUtc = r["first_order_at"] == "" ? null : Utc(r["first_order_at"]),
            });
        await db.SaveChangesAsync();

        foreach (var r in Csv.Read(Path.Combine(full, "orders.csv")))
            db.Orders.Add(new OrderRow
            {
                Id = r["id"], CustomerId = r["customer_id"], ProductId = r["product_id"],
                Quantity = int.Parse(r["quantity"], CultureInfo.InvariantCulture),
                AmountByn = decimal.Parse(r["amount_byn"], CultureInfo.InvariantCulture),
                Channel = r["channel"], Status = r["status"], StationId = r["station_id"],
                CreatedAtUtc = Utc(r["created_at"]), DueAtUtc = Utc(r["due_at"]),
                DeliveredOnTime = r["delivered_on_time"] switch { "true" => true, "false" => false, _ => null },
            });
        await db.SaveChangesAsync();

        var orderDates = await db.Orders.ToDictionaryAsync(o => o.Id, o => o.CreatedAtUtc);
        foreach (var r in Csv.Read(Path.Combine(full, "order_events.csv")))
            db.OrderEvents.Add(new OrderEventRow
            {
                OrderId = r["order_id"], Status = r["status"],
                StationId = r["station_id"] == "" ? null : r["station_id"],
                AtUtc = Utc(r["at"]), Actor = r["actor"],
            });
        foreach (var r in Csv.Read(Path.Combine(full, "defects.csv")))
            db.Defects.Add(new DefectRow
            {
                OrderId = r["order_id"], StationId = r["station_id"], Shift = r["shift"],
                Cause = r["cause"], DetectedBy = r["detected_by"],
                CostByn = decimal.Parse(r["cost_byn"], CultureInfo.InvariantCulture),
                OccurredAtUtc = orderDates.GetValueOrDefault(r["order_id"], Utc(r["shift"].Split('/')[0])),
            });
        await db.SaveChangesAsync();

        foreach (var r in Csv.Read(Path.Combine(full, "interactions.csv")))
            db.Interactions.Add(new InteractionRow
            {
                CustomerId = r["customer_id"], Channel = r["channel"], Topic = r["topic"],
                OrderId = r["order_id"] == "" ? null : r["order_id"],
                CreatedAtUtc = Utc(r["created_at"]),
                FirstReplyMin = int.Parse(r["first_reply_min"], CultureInfo.InvariantCulture),
                SlaMin = int.Parse(r["sla_min"], CultureInfo.InvariantCulture),
                InSla = r["in_sla"] == "true",
            });
        await db.SaveChangesAsync();

        log.LogInformation("Датасет загружен: {Orders} заказов, {Events} событий, {Defects} дефектов, {Tickets} обращений",
            await db.Orders.CountAsync(), await db.OrderEvents.CountAsync(),
            await db.Defects.CountAsync(), await db.Interactions.CountAsync());
    }

    /// <summary>Ищем dataset/seed: от BaseDirectory вверх по дереву (bin/Debug/net9.0 → корень репо).</summary>
    private static string ResolveDatasetPath(string configured)
    {
        if (Path.IsPathRooted(configured) && Directory.Exists(configured)) return configured;
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        for (var i = 0; i < 8 && dir is not null; i++, dir = dir.Parent)
        {
            var candidate = Path.Combine(dir.FullName, "dataset", "seed");
            if (Directory.Exists(candidate)) return candidate;
        }
        throw new InvalidOperationException($"Не найдена папка dataset/seed (конфиг: {configured})");
    }

    private static async Task SeedUsersAsync(KonturDbContext db)
    {
        var users = new (string Login, string FullName, string Role, string Scope)[]
        {
            ("director", "Ирина Шевчик (директор)", "director", "*"),
            ("prod-head", "Александр Ковалёв (рук. производства)", "department_head", "dep-prod"),
            ("analyst", "Ольга Кузнецова (аналитик)", "analyst", "*"),
            ("hr", "Марина Морозова (HR)", "hr", "dep-hr"),
            ("marketing", "Павел Бельский (маркетолог)", "marketing", "dep-mkt"),
            ("support", "Светлана Дубовик (поддержка)", "support_operator", "dep-sup"),
            ("moderator", "Николай Романов (модератор)", "moderator", "dep-prod"),
            ("prod-worker", "Иван Гончар (оператор производства)", "production_worker", "dep-prod"),
            ("employee", "Татьяна Павлюкова (сотрудник)", "employee", "dep-prod"),
            ("client", "Дмитрий Левчук (клиент picPac)", "client", "*"),
        };
        foreach (var (login, fullName, role, scope) in users)
            if (!await db.Users.AnyAsync(u => u.Login == login))
                db.Users.Add(new AppUser
                {
                    Id = $"user-{login}", Login = login, FullName = fullName,
                    Role = role, DepartmentScope = scope, PasswordHash = Auth.Hash(login, "demo1234"),
                });
        await db.SaveChangesAsync();
    }
}
