using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Merch.DataAccess.Npgsql.Entities;
namespace Modsen.Portal.Merch.DataAccess.Npgsql;

internal class MerchDbContext : DbContext, IMerchDbContext
{
    public const string Schema = "merch";

    public DbSet<MerchUserEntity> MerchUsers { get; set; }

    public DbSet<ItemEntity> Items { get; set; }

    public DbSet<ItemSizeMinAmountEntity> ItemSizeMinAmounts { get; set; }

    public DbSet<ItemTransactionEntity> ItemTransactions { get; set; }

    public DbSet<ItemTransactionGroupEntity> ItemTransactionGroups { get; set; }

    public DbSet<KitEntity> Kits { get; set; }

    public DbSet<KitMinAmountEntity> KitMinAmounts { get; set; }

    public DbSet<KitItemEntity> KitItems { get; set; }

    public DbSet<MerchSizeEntity> MerchSizes { get; set; }

    public DbSet<WarehouseEntity> Warehouses { get; set; }

    public MerchDbContext(DbContextOptions<MerchDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MerchDbContext).Assembly);
    }
}
