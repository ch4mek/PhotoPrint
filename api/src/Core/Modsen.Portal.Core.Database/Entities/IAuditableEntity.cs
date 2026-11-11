namespace Modsen.Portal.Core.Database.Entities;

public interface IAuditableEntity : IPortalEntity
{
    public int? UpdatedById { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
}
