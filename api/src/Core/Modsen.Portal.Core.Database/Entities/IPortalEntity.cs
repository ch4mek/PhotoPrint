namespace Modsen.Portal.Core.Database.Entities;

public interface IPortalEntity
{
    public int? CreatedById { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}
