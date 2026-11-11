using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Merch.DataAccess.Npgsql;
using Modsen.Portal.Merch.DataAccess.Npgsql.Entities;

namespace Modsen.Portal.Migrator.MigrationTasks;

internal sealed class PublishExistingUsersToMerchMigrationTask : IMigrationTask
{
    private readonly MerchDbContext _merchDbContext;
    private readonly IdentityModuleDbContext _identityModuleDbContext;

    public PublishExistingUsersToMerchMigrationTask(
        MerchDbContext merchDbContext,
        IdentityModuleDbContext identityModuleDbContext)
    {
        _merchDbContext = merchDbContext;
        _identityModuleDbContext = identityModuleDbContext;
    }

    public string TaskName => "PublishExistingUsersToMerch";

    public void Execute()
    {
        var page = 0;
        var size = MigratorTaskConstants.BatchSize;
        bool hasAnyUsers;

        var merchUserIds = new List<int>();

        do
        {
            var users = _identityModuleDbContext.Users
                .AsNoTracking()
                .Where(x => !x.UserRoles.Any(y => y.Role!.Name == PortalRoleNames.Intern))
                .OrderBy(x => x.Id)
                .Skip(page * size)
                .Take(size)
                .ToList();

            var merchUsers = new List<MerchUserEntity>();

            page++;

            foreach (var user in users)
            {
                var merchUser = _merchDbContext.MerchUsers.FirstOrDefault(x => x.UserId == user.Id);
                if (merchUser == null)
                {
                    merchUser = new MerchUserEntity { UserId = user.Id,
                        Email = "test",
                        LastName = "test",
                        FirstName = "test"
                    };

                    CopyFields(merchUser, user);
                    _merchDbContext.Add(merchUser);
                }
                else
                {
                    CopyFields(merchUser, user);
                }

                merchUsers.Add(merchUser);
                merchUserIds.Add(merchUser.UserId);
            }

            _merchDbContext.SaveChanges();

            foreach (var merchUser in merchUsers)
            {
                _merchDbContext.MerchUsers.Entry(merchUser).State = EntityState.Detached;
            }

            hasAnyUsers = _identityModuleDbContext.Users
                .Where(x => !x.UserRoles.Any(y => y.Role!.Name == PortalRoleNames.Intern))
                .OrderBy(x => x.Id)
                .Skip(page * size)
                .Any();
        } while (hasAnyUsers);

        _merchDbContext.MerchUsers
            .Where(merchUser => !merchUserIds.Contains(merchUser.UserId) && merchUser.Email != "ModsenCompany@modsen-software.com")
            .ExecuteDelete();

        _merchDbContext.SaveChanges();
    }

    private static void CopyFields(MerchUserEntity merchUser, User user)
    {
        merchUser.Email = user.Email!;
        merchUser.LastName = user.LastName;
        merchUser.FirstName = user.FirstName;
        merchUser.DepartmentId = user.DepartmentId;
    }
}
