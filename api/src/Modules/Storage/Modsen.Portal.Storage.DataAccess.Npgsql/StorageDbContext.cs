using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Outbox.Storage;
using Modsen.Portal.Storage.DataAccess.Npgsql.Entities;

[assembly: InternalsVisibleTo("Modsen.Portal.Api")]

namespace Modsen.Portal.Storage.DataAccess.Npgsql;

internal class StorageDbContext : DbContext, IStorageDbContext
{
    public const string Schema = "storage";

    public DbSet<ApplicationFileEntity> ApplicationFiles { get; set; }

    public StorageDbContext(DbContextOptions<StorageDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(StorageDbContext).Assembly);
        modelBuilder.ApplyConfiguration(new OutboxEventConfiguration("OutboxEvents", Schema));
    }
}
