using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Notification.DataAccess.Npgsql.Entities;

[assembly: InternalsVisibleTo("Modsen.Portal.IntegrationTests")]
[assembly: InternalsVisibleTo("Modsen.Portal.Notification.UseCases")]
namespace Modsen.Portal.Notification.DataAccess.Npgsql;

internal interface INotificationDbContext
{
    DbSet<NotificationEntity> Notifications { get; set; }
    DbSet<NotificationUserEntity> NotificationUsers { get; set; }

    Task<int> SaveChanges(CancellationToken cancellationToken);
}
