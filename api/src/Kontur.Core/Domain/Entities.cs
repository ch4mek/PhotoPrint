namespace Kontur.Core.Domain;

/// <summary>Пользователь платформы (10 демо-ролей, пароли только тестовые).</summary>
public class AppUser
{
    public string Id { get; set; } = "";
    public string Login { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public string FullName { get; set; } = "";
    public string Role { get; set; } = "";
    /// <summary>"*" — все подразделения, иначе id одного подразделения (data-scope, УП-05).</summary>
    public string DepartmentScope { get; set; } = "*";
}

public class Department
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
}

public class Customer
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string Segment { get; set; } = "";
    public string Channel { get; set; } = "";
    public DateTime? FirstOrderAtUtc { get; set; }
}

public class Product
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public decimal PriceByn { get; set; }
    public int ProductionDays { get; set; }
}

public class OrderRow
{
    public string Id { get; set; } = "";
    public string CustomerId { get; set; } = "";
    public string ProductId { get; set; } = "";
    public int Quantity { get; set; }
    public decimal AmountByn { get; set; }
    public string Channel { get; set; } = "";
    public string Status { get; set; } = "";
    public string StationId { get; set; } = "";
    public DateTime CreatedAtUtc { get; set; }
    public DateTime DueAtUtc { get; set; }
    public bool? DeliveredOnTime { get; set; }
}

public class OrderEventRow
{
    public long Id { get; set; }
    public string OrderId { get; set; } = "";
    public string Status { get; set; } = "";
    public string? StationId { get; set; }
    public DateTime AtUtc { get; set; }
    public string Actor { get; set; } = "system";
}

public class DefectRow
{
    public long Id { get; set; }
    public string OrderId { get; set; } = "";
    public string StationId { get; set; } = "";
    public string Shift { get; set; } = "";
    public string Cause { get; set; } = "";
    public string DetectedBy { get; set; } = "";
    public decimal CostByn { get; set; }
    /// <summary>Дата дефекта (в демо = дата создания заказа).</summary>
    public DateTime OccurredAtUtc { get; set; }
}

public class InteractionRow
{
    public long Id { get; set; }
    public string CustomerId { get; set; } = "";
    public string Channel { get; set; } = "";
    public string Topic { get; set; } = "";
    public string? OrderId { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public int FirstReplyMin { get; set; }
    public int SlaMin { get; set; }
    public bool InSla { get; set; }
}

/// <summary>Карточка решения — сердце петли «сигнал → действие → проверка».</summary>
public class Decision
{
    public string Id { get; set; } = "";
    public string Title { get; set; } = "";
    public string Problem { get; set; } = "";
    public string Impact { get; set; } = "";
    public string Cause { get; set; } = "";
    public string ActionPlan { get; set; } = "";
    public string OwnerId { get; set; } = "";
    public string OwnerName { get; set; } = "";
    public DateTime Deadline { get; set; }
    public int Priority { get; set; } = 3;
    /// <summary>new → in_progress → done → verified | failed.</summary>
    public string Status { get; set; } = "new";
    public string KpiCode { get; set; } = "";
    public string RefEntity { get; set; } = "";
    /// <summary>Значение KPI на момент создания (для повторной проверки, слайд 5 ТЗ).</summary>
    public double? FactAtCreation { get; set; }
    public double? FactAtVerify { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public string CreatedBy { get; set; } = "";
}

/// <summary>Единый журнал: кто, когда, что, объект, результат (ОТ-04).</summary>
public class AuditEntry
{
    public long Id { get; set; }
    public DateTime AtUtc { get; set; }
    public string ActorId { get; set; } = "";
    public string ActorLogin { get; set; } = "";
    public string ActorType { get; set; } = "user"; // user | assistant | system
    public string Action { get; set; } = "";
    public string ObjectType { get; set; } = "";
    public string ObjectId { get; set; } = "";
    public string Result { get; set; } = ""; // success | denied | error
    public string Detail { get; set; } = "";
}
