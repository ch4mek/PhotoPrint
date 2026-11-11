using Kontur.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace Kontur.Api.Data;

public class KonturDbContext(DbContextOptions<KonturDbContext> options) : DbContext(options)
{
    public DbSet<AppUser> Users => Set<AppUser>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<OrderRow> Orders => Set<OrderRow>();
    public DbSet<OrderEventRow> OrderEvents => Set<OrderEventRow>();
    public DbSet<DefectRow> Defects => Set<DefectRow>();
    public DbSet<InteractionRow> Interactions => Set<InteractionRow>();
    public DbSet<Decision> Decisions => Set<Decision>();
    public DbSet<AuditEntry> AuditLog => Set<AuditEntry>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<AppUser>(e => { e.ToTable("users"); e.HasKey(x => x.Id); });
        b.Entity<Department>(e => { e.ToTable("departments"); e.HasKey(x => x.Id); });
        b.Entity<Customer>(e => { e.ToTable("customers"); e.HasKey(x => x.Id); });
        b.Entity<Product>(e => { e.ToTable("products"); e.HasKey(x => x.Id); });
        b.Entity<OrderRow>(e =>
        {
            e.ToTable("orders");
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.CreatedAtUtc);
            e.HasIndex(x => x.Status);
            e.Property(x => x.AmountByn).HasPrecision(12, 2);
        });
        b.Entity<OrderEventRow>(e => { e.ToTable("order_events"); e.HasKey(x => x.Id); e.HasIndex(x => x.OrderId); });
        b.Entity<DefectRow>(e => { e.ToTable("defects"); e.HasKey(x => x.Id); e.HasIndex(x => x.StationId); });
        b.Entity<InteractionRow>(e => { e.ToTable("interactions"); e.HasKey(x => x.Id); e.HasIndex(x => x.Topic); });
        b.Entity<Decision>(e => { e.ToTable("decisions"); e.HasKey(x => x.Id); });
        b.Entity<AuditEntry>(e => { e.ToTable("audit_log"); e.HasKey(x => x.Id); e.HasIndex(x => x.ObjectId); });
    }
}
