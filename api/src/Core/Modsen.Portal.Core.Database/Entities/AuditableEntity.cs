namespace Modsen.Portal.Core.Database.Entities;

public class AuditableEntity : IAuditableEntity
{
    public int? CreatedById { get; set; }
    public DateTime CreatedAtUtc { get; set; }

    public int? UpdatedById { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
}
