using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.Migrator.MigrationTasks;

internal sealed class PublishExistingUsersToTeamBoardMigrationTask : IMigrationTask
{
    private readonly TeamBoardDbContext _teamBoardDbContext;
    private readonly IdentityModuleDbContext _identityModuleDbContext;

    public PublishExistingUsersToTeamBoardMigrationTask(
        TeamBoardDbContext teamBoardDbContext,
        IdentityModuleDbContext identityModuleDbContext)
    {
        _teamBoardDbContext = teamBoardDbContext;
        _identityModuleDbContext = identityModuleDbContext;
    }

    public string TaskName => "PublishExistingUsersToTeamBoard";

    public void Execute()
    {
        var page = 0;
        bool hasAnyUsers;

        var teamBoardUserIds = new List<int>();

        do
        {
            var users = _identityModuleDbContext.Users
                .AsNoTracking()
                .Include(x => x.UserRoles)
                .ThenInclude(x => x.Role)
                .OrderBy(x => x.Id)
                .Skip(page * MigratorTaskConstants.BatchSize)
                .Take(MigratorTaskConstants.BatchSize)
                .ToList();

            var teamBoardUsers = new List<TeamBoardUser>();

            page++;

            foreach (var user in users)
            {
                var teamBoardUser = _teamBoardDbContext.TeamBoardUsers.FirstOrDefault(x => x.UserId == user.Id);
                if (teamBoardUser == null)
                {
                    teamBoardUser = new TeamBoardUser { UserId = user.Id,
                        FirstName = "test",
                        LastName = "test"
                    };

                    CopyFields(teamBoardUser, user);
                    _teamBoardDbContext.Add(teamBoardUser);
                }
                else
                {
                    CopyFields(teamBoardUser, user);
                }

                teamBoardUsers.Add(teamBoardUser);
                teamBoardUserIds.Add(teamBoardUser.UserId);
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

        _teamBoardDbContext.TeamBoardUsers
            .Where(teamBoardUser => !teamBoardUserIds.Contains(teamBoardUser.UserId) && teamBoardUser.Email != "ModsenCompany@modsen-software.com")
            .ExecuteUpdate(setPropertyCalls => setPropertyCalls.SetProperty(
                teamBoardUser => teamBoardUser.IsActive, false));
        _teamBoardDbContext.SaveChanges();
    }

    private static void CopyFields(TeamBoardUser teamBoardUser, User user)
    {
        var email = user.Email;

        if (user.UserRoles.Any(y => y.Role!.Name == PortalRoleNames.Intern) && teamBoardUser.Email is null)
        {
            teamBoardUser.PersonalEmail = email;

            email = null;
        }

        teamBoardUser.FirstName = user.FirstName;
        teamBoardUser.LastName = user.LastName;
        teamBoardUser.FirstNameRu = user.FirstNameRu;
        teamBoardUser.LastNameRu = user.LastNameRu;
        teamBoardUser.PatronymicRu = user.PatronymicRu;
        teamBoardUser.Email = email;
        teamBoardUser.DepartmentId = user.DepartmentId;
        teamBoardUser.IsActive = user.IsActive;
        teamBoardUser.Position = user.UserRoles.Select(x => x.Role!.Name).FirstOrDefault();
        teamBoardUser.PositionOrderWeight = user.UserRoles.Select(x => x.Role!.OrderWeight).FirstOrDefault();
        teamBoardUser.AvatarColor = user.AvatarColor;
        teamBoardUser.AvatarFileId = user.AvatarFileId;
        teamBoardUser.Location = (user.Location ?? LocationType.Minsk).ToString();
        teamBoardUser.ResourceGroupId = user.ResourceGroupId;
    }
}
