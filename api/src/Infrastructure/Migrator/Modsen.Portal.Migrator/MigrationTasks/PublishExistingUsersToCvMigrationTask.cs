using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Cv.DataAccess.Npgsql;
using Modsen.Portal.Cv.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.Migrator.MigrationTasks;

internal sealed class PublishExistingUsersToCvMigrationTask : IMigrationTask
{
    private readonly IdentityModuleDbContext _identityModuleDbContext;
    private readonly CvDbContext _cvDbContext;

    public PublishExistingUsersToCvMigrationTask(
        IdentityModuleDbContext identityModuleDbContext,
        CvDbContext cvDbContext)
    {
        _identityModuleDbContext = identityModuleDbContext;
        _cvDbContext = cvDbContext;
    }

    public string TaskName => "PublishExistingUsersToCv";

    public void Execute()
    {
        var page = 0;
        bool hasAnyUsers;

        var cvUsersIds = new List<int>();

        do
        {
            var users = _identityModuleDbContext.Users
                .Include(x => x.UserRoles)
                .ThenInclude(x => x.Role)
                .AsNoTracking()
                .Where(x => !x.UserRoles.Any(y => y.Role!.Name == PortalRoleNames.Intern))
                .OrderBy(x => x.Id)
                .Skip(page * MigratorTaskConstants.BatchSize)
                .Take(MigratorTaskConstants.BatchSize)
                .ToList();

            var cvUsers = new List<CvUserEntity>();

            page++;

            foreach (var user in users)
            {
                var cvUser = _cvDbContext.CvUsers.FirstOrDefault(x => x.UserId == user.Id);
                if (cvUser == null)
                {
                    cvUser = new CvUserEntity
                    {
                        UserId = user.Id,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email!
                    };

                    CopyFields(cvUser, user);
                    _cvDbContext.Add(cvUser);
                }
                else
                {
                    CopyFields(cvUser, user);
                }

                cvUsers.Add(cvUser);
                cvUsersIds.Add(cvUser.UserId);
            }

            _cvDbContext.SaveChanges();

            foreach (var cvUser in cvUsers)
            {
                _cvDbContext.CvUsers.Entry(cvUser).State = EntityState.Detached;
            }

            hasAnyUsers = _identityModuleDbContext.Users
                .Where(x => !x.UserRoles.Any(y => y.Role!.Name == PortalRoleNames.Intern))
                .OrderBy(x => x.Id)
                .Skip(page * MigratorTaskConstants.BatchSize)
                .Any();
        } while (hasAnyUsers);

        _cvDbContext.CvUsers
            .Where(cvUser =>
                !cvUsersIds.Contains(cvUser.UserId) &&
                cvUser.Email != "ModsenCompany@modsen-software.com")
            .ExecuteUpdate(setPropertyCalls => setPropertyCalls
                .SetProperty(cvUser => cvUser.IsActive, false));

        _cvDbContext.SaveChanges();
    }

    private static void CopyFields(CvUserEntity cvUser, User user)
    {
        cvUser.Email = user.Email!;
        cvUser.FirstName = user.FirstName;
        cvUser.LastName = user.LastName;
        cvUser.FirstNameRu = user.FirstNameRu;
        cvUser.LastNameRu = user.LastNameRu;
        cvUser.DepartmentId = user.DepartmentId;
        cvUser.IsActive = user.IsActive;
        cvUser.ResourceGroupId = user.ResourceGroupId;
        cvUser.Position = user.UserRoles.Select(x => x.Role?.Name).FirstOrDefault();
        cvUser.Location = user.Location.ToString();
    }
}
