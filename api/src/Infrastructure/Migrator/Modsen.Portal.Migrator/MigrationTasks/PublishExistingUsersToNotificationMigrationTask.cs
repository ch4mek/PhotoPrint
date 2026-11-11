using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Notification.DataAccess.Npgsql;
using Modsen.Portal.Notification.DataAccess.Npgsql.Entities;

namespace Modsen.Portal.Migrator.MigrationTasks;

internal sealed class PublishExistingUsersToNotificationMigrationTask : IMigrationTask
{
    private readonly IdentityModuleDbContext _identityModuleDbContext;
    private readonly NotificationDbContext _notificationDbContext;

    public PublishExistingUsersToNotificationMigrationTask(
        IdentityModuleDbContext identityModuleDbContext,
        NotificationDbContext notificationDbContext)
    {
        _identityModuleDbContext = identityModuleDbContext;
        _notificationDbContext = notificationDbContext;
    }

    public string TaskName => "PublishExistingUsersToNotification";

    public void Execute()
    {
        var page = 0;
        bool hasAnyUsers;

        var notificationUsersIds = new List<int>();

        do
        {
            var users = _identityModuleDbContext.Users
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .Skip(page * MigratorTaskConstants.BatchSize)
                .Take(MigratorTaskConstants.BatchSize)
                .ToList();

            var notificationUsers = new List<NotificationUserEntity>();

            page++;

            foreach (var user in users)
            {
                var notificationUser = _notificationDbContext.NotificationUsers
                    .FirstOrDefault(x => x.UserId == user.Id);
                if (notificationUser == null)
                {
                    notificationUser = new NotificationUserEntity
                    {
                        UserId = user.Id,
                        Email = user.Email!,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                    };

                    CopyFields(notificationUser, user);
                    _notificationDbContext.Add(notificationUser);
                }
                else
                {
                    CopyFields(notificationUser, user);
                }

                notificationUsers.Add(notificationUser);
                notificationUsersIds.Add(notificationUser.UserId);
            }

            _notificationDbContext.SaveChanges();

            foreach (var notificationUser in notificationUsers)
            {
                _notificationDbContext.NotificationUsers.Entry(notificationUser).State = EntityState.Detached;
            }

            hasAnyUsers = _identityModuleDbContext.Users
                .OrderBy(x => x.Id)
                .Skip(page * MigratorTaskConstants.BatchSize)
                .Any();
        } while (hasAnyUsers);

        _notificationDbContext.NotificationUsers
            .Where(cvUser =>
                !notificationUsersIds.Contains(cvUser.UserId) &&
                cvUser.Email != "ModsenCompany@modsen-software.com")
            .ExecuteUpdate(setPropertyCalls => setPropertyCalls
                .SetProperty(cvUser => cvUser.IsActive, false));

        _notificationDbContext.SaveChanges();
    }

    private static void CopyFields(NotificationUserEntity notificationUser, User user)
    {
        notificationUser.Email = user.Email!;
        notificationUser.FirstName = user.FirstName;
        notificationUser.LastName = user.LastName;
        notificationUser.AvatarFileId = user.AvatarFileId;
        notificationUser.IsActive = user.IsActive;
    }
}
