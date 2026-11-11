using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Notification.DataAccess.Npgsql.Entities;

namespace Modsen.Portal.Notification.DataAccess.Npgsql;

internal class NotificationDbContext : DbContext, INotificationDbContext
{
    public const string Schema = "notification";

    public DbSet<NotificationEntity> Notifications { get; set; }
    public DbSet<NotificationUserEntity> NotificationUsers { get; set; }

    public NotificationDbContext(DbContextOptions<NotificationDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(NotificationDbContext).Assembly);
    }

    public Task<int> SaveChanges(CancellationToken cancellationToken)
    {
        return SaveChangesAsync(cancellationToken);
    }
}
