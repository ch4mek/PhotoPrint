using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Modsen.Portal.Core.Database;

namespace Modsen.Portal.Notification.DataAccess.Npgsql;

internal class DesignTimeNotificationDbContext : DesignTimeBaseDbContext, IDesignTimeDbContextFactory<NotificationDbContext>
{
    public NotificationDbContext CreateDbContext(string[] args)
    {
        return new NotificationDbContext(new DbContextOptionsBuilder<NotificationDbContext>().UseNpgsql(GetConnectionString()).Options);
    }
}
