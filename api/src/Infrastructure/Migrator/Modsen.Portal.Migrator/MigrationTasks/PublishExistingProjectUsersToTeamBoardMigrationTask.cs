using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Database;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;

namespace Modsen.Portal.Migrator.MigrationTasks;

internal sealed class PublishExistingProjectUsersToTeamBoardMigrationTask : IMigrationTask
{
    private const decimal s_defaultFte = 1.0000m;
    private const string s_defaultSeniorityLevelId = "Trainee";

    private readonly TeamBoardDbContext _teamBoardDbContext;

    public PublishExistingProjectUsersToTeamBoardMigrationTask(
        TeamBoardDbContext teamBoardDbContext)
    {
        _teamBoardDbContext = teamBoardDbContext;
    }

    public string TaskName => "PublishExistingProjectUsersToTeamBoard";

    public void Execute()
    {
        _teamBoardDbContext.Database.ExecuteSql($"""
                       INSERT INTO `teamboard_ProjectUsers` (`Id`, `UserId`, `ProjectId`, `UserOnProjectFrom`, `UserOnProjectTo`,
                                                             `UpdatedById`, `UpdatedAtUtc`, `IsTimeOnProjectBillable`, `Fte`,
                                                             `UserOnProjectSeniorityLevelId`)
                           SELECT `Id`, `UserId`, `ProjectId`, `UserOnProjectFrom`, `UserOnProjectTo`,
                                  `UpdatedById`, `UpdatedAtUtc`, `IsTimeOnProjectBillable`, {s_defaultFte} as `Fte`,
                                   {s_defaultSeniorityLevelId} as `UserOnProjectSeniorityLevelId`
                           FROM `timetrack_ProjectUsers` pu_from
                           WHERE pu_from.`Id` NOT IN (SELECT pu_to.`Id` FROM `teamboard_ProjectUsers` pu_to)
                       """);
    }
}
