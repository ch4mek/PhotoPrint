using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Finance.DataAccess.Npgsql;
using Modsen.Portal.Finance.DataAccess.Npgsql.Entities;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.Migrator.MigrationTasks;

internal class PublishExistingUsersToFinanceMigrationTask : IMigrationTask
{
    private readonly IdentityModuleDbContext _identityModuleDbContext;
    private readonly FinanceDbContext _financeDbContext;

    public PublishExistingUsersToFinanceMigrationTask(
        IdentityModuleDbContext identityModuleDbContext,
        FinanceDbContext financeDbContext)
    {
        _identityModuleDbContext = identityModuleDbContext;
        _financeDbContext = financeDbContext;
    }

    public string TaskName => "PublishExistingUsersToFinance";

    public void Execute()
    {
        var page = 0;
        bool hasAnyUsers;

        var financeUserIds = new List<int>();

        do
        {
            var users = _identityModuleDbContext.Users
                .AsNoTracking()
                .Include(x => x.UserRoles)
                .ThenInclude(x => x.Role)
                .Where(x => !x.UserRoles.Any(y => y.Role!.Name == PortalRoleNames.Intern))
                .OrderBy(x => x.Id)
                .Skip(page * MigratorTaskConstants.BatchSize)
                .Take(MigratorTaskConstants.BatchSize)
                .ToList();

            var financeUsers = new List<FinanceUserEntity>();

            page++;

            foreach (var user in users)
            {
                var financeUser = _financeDbContext.FinanceUsers.FirstOrDefault(x => x.Id == user.Id);
                if (financeUser == null)
                {
                    financeUser = new FinanceUserEntity
                    {
                        Id = user.Id,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email!
                    };

                    CopyFields(financeUser, user);
                    _financeDbContext.Add(financeUser);
                }
                else
                {
                    CopyFields(financeUser, user);
                }

                financeUsers.Add(financeUser);
                financeUserIds.Add(financeUser.Id);
            }

            _financeDbContext.SaveChanges();

            foreach (var financeUser in financeUsers)
            {
                _financeDbContext.FinanceUsers.Entry(financeUser).State = EntityState.Detached;
            }

            hasAnyUsers = _identityModuleDbContext.Users
                .Where(x => !x.UserRoles.Any(y => y.Role!.Name == PortalRoleNames.Intern))
                .OrderBy(x => x.Id)
                .Skip(page * MigratorTaskConstants.BatchSize)
                .Any();
        } while (hasAnyUsers);

        _financeDbContext.FinanceUsers
            .Where(financeUser => !financeUserIds.Contains(financeUser.Id) && financeUser.Email != "ModsenCompany@modsen-software.com")
            .ExecuteUpdate(setPropertyCalls => setPropertyCalls.SetProperty(
                teamBoardUser => teamBoardUser.IsActive, false));

        _financeDbContext.SaveChanges();
    }

    private static void CopyFields(FinanceUserEntity financeUser, User user)
    {
        financeUser.FirstName = user.FirstName;
        financeUser.LastName = user.LastName;
        financeUser.FirstNameRu = user.FirstNameRu;
        financeUser.LastNameRu = user.LastNameRu;
        financeUser.PatronymicRu = user.PatronymicRu;
        financeUser.Email = user.Email!;
        financeUser.DepartmentId = user.DepartmentId;
        financeUser.IsActive = user.IsActive;
        financeUser.ResourceGroupId = user.ResourceGroupId;
    }
}
