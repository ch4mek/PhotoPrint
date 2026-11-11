using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Finance.DataAccess.Npgsql;

namespace Modsen.Portal.Migrator.MigrationTasks;

internal class PublishExistingProjectUsersToFinanceMigrationTask : IMigrationTask
{
    private readonly FinanceDbContext _financeDbContext;

    public PublishExistingProjectUsersToFinanceMigrationTask(
        FinanceDbContext financeDbContext)
    {
        _financeDbContext = financeDbContext;
    }

    public string TaskName => "PublishExistingProjectUsersToFinance";

    public void Execute()
    {
        _financeDbContext.Database.ExecuteSql($"""
                       INSERT INTO `finance_FinanceProjectUsers` (`Id`, `UserId`, `ProjectId`, `UserOnProjectFrom`, `UserOnProjectTo`)
                           SELECT `Id`, `UserId`, `ProjectId`, `UserOnProjectFrom`, `UserOnProjectTo`
                           FROM `timetrack_ProjectUsers` pu_from
                           WHERE pu_from.`Id` NOT IN (SELECT pu_to.`Id` FROM `finance_FinanceProjectUsers` pu_to)
                               AND NOT EXISTS (
                                   SELECT 1
                                   FROM `identity_AspNetUserRoles` ur
                                   JOIN `identity_AspNetRoles` r ON ur.`RoleId` = r.`Id`
                                   WHERE ur.`UserId` = pu_from.`UserId`
                                     AND r.`Name` = {PortalRoleNames.Intern}
                               )
                       """);
    }
}
