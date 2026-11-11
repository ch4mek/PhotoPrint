using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Merch.DataAccess.Npgsql.Entities;
[assembly: InternalsVisibleTo("Modsen.Portal.Merch.Requests")]
namespace Modsen.Portal.Merch.DataAccess.Npgsql;

internal interface IMerchDbContext
{
    public DbSet<MerchUserEntity> MerchUsers { get; }

    public DbSet<ItemEntity> Items { get; set; }

    public DbSet<ItemSizeMinAmountEntity> ItemSizeMinAmounts { get; set; }

    public DbSet<ItemTransactionEntity> ItemTransactions { get; set; }

    public DbSet<ItemTransactionGroupEntity> ItemTransactionGroups { get; set; }

    public DbSet<KitEntity> Kits { get; set; }

    public DbSet<KitMinAmountEntity> KitMinAmounts { get; set; }

    public DbSet<KitItemEntity> KitItems { get; set; }

    public DbSet<MerchSizeEntity> MerchSizes { get; set; }

    public DbSet<WarehouseEntity> Warehouses{ get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
