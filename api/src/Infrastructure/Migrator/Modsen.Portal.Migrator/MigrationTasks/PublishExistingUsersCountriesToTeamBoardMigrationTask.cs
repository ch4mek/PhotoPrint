using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.Migrator.MigrationTasks;

internal sealed class PublishExistingUsersCountriesToTeamBoardMigrationTask : IMigrationTask
{
    private readonly TeamBoardDbContext _teamBoardDbContext;
    private readonly IdentityModuleDbContext _identityModuleDbContext;

    public PublishExistingUsersCountriesToTeamBoardMigrationTask(
        TeamBoardDbContext teamBoardDbContext,
        IdentityModuleDbContext identityModuleDbContext)
    {
        _teamBoardDbContext = teamBoardDbContext;
        _identityModuleDbContext = identityModuleDbContext;
    }

    public string TaskName => "PublishExistingUsersCountriesToTeamBoard";

    public void Execute()
    {
        var page = 0;
        bool hasAnyUsers;

        do
        {
            var users = _identityModuleDbContext.Users
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .Skip(page * MigratorTaskConstants.BatchSize)
                .Take(MigratorTaskConstants.BatchSize)
                .ToList();

            var teamBoardUsers = new List<TeamBoardUser>();

            page++;

            foreach (var user in users)
            {
                var teamBoardUser = _teamBoardDbContext.TeamBoardUsers.FirstOrDefault(x => x.UserId == user.Id);
                if (teamBoardUser != null)
                {
                    CopyCountry(teamBoardUser, user);

                    teamBoardUsers.Add(teamBoardUser);
                }
            }

            _teamBoardDbContext.SaveChanges();

            foreach (var teamBoardUser in teamBoardUsers)
            {
                _teamBoardDbContext.TeamBoardUsers.Entry(teamBoardUser).State = EntityState.Detached;
            }

            hasAnyUsers = _identityModuleDbContext.Users
                .OrderBy(x => x.Id)
                .Skip(page * MigratorTaskConstants.BatchSize)
                .Any();
        } while (hasAnyUsers);
    }

    private static void CopyCountry(TeamBoardUser teamBoardUser, User user)
    {
        teamBoardUser.CountryId = user.CountryId;
    }
}
