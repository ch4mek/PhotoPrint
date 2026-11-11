using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.Migrator.MigrationTasks;

internal sealed class PublishExistingUsersToTimeTrackMigrationTask : IMigrationTask
{
    private readonly IdentityModuleDbContext _identityModuleDbContext;
    private readonly TimeTrackDbContext _timeTrackDbContext;

    public PublishExistingUsersToTimeTrackMigrationTask(
        IdentityModuleDbContext identityModuleDbContext,
        TimeTrackDbContext timeTrackDbContext)
    {
        _identityModuleDbContext = identityModuleDbContext;
        _timeTrackDbContext = timeTrackDbContext;
    }

    public string TaskName => "PublishExistingUsersToTimeTrack";

    public void Execute()
    {
        var page = 0;
        bool hasAnyUsers;

        var timeTrackUserIds = new List<int>();

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

            var timeTrackUsers = new List<TimeTrackUser>();

            page++;

            foreach (var user in users)
            {
                var teamBoardUser = _timeTrackDbContext.TimeTrackUsers.FirstOrDefault(x => x.UserId == user.Id);
                if (teamBoardUser == null)
                {
                    teamBoardUser = new TimeTrackUser { UserId = user.Id, FirstName = "test", LastName = "test", Email = "test" };

                    CopyFields(teamBoardUser, user);
                    _timeTrackDbContext.Add(teamBoardUser);
                }
                else
                {
                    CopyFields(teamBoardUser, user);
                }

                timeTrackUsers.Add(teamBoardUser);
                timeTrackUserIds.Add(teamBoardUser.UserId);
            }

            _timeTrackDbContext.SaveChanges();

            foreach (var teamBoardUser in timeTrackUsers)
            {
                _timeTrackDbContext.TimeTrackUsers.Entry(teamBoardUser).State = EntityState.Detached;
            }

            hasAnyUsers = _identityModuleDbContext.Users
                .OrderBy(x => x.Id)
                .Skip(page * MigratorTaskConstants.BatchSize)
                .Any();
        } while (hasAnyUsers);

        _timeTrackDbContext.TimeTrackUsers
            .Where(teamBoardUser => !timeTrackUserIds.Contains(teamBoardUser.UserId) && teamBoardUser.Email != "ModsenCompany@modsen-software.com")
            .ExecuteUpdate(setPropertyCalls => setPropertyCalls.SetProperty(
                teamBoardUser => teamBoardUser.IsActive, false));

        _timeTrackDbContext.SaveChanges();
    }

    private static void CopyFields(TimeTrackUser timeTrackUser, User user)
    {
        timeTrackUser.FirstName = user.FirstName;
        timeTrackUser.LastName = user.LastName;
        timeTrackUser.Email = user.Email!;
        timeTrackUser.DepartmentId = user.DepartmentId;
        timeTrackUser.IsActive = user.IsActive;
        timeTrackUser.Color = user.Color;
        timeTrackUser.AvatarFileId = user.AvatarFileId;
        timeTrackUser.ResourceGroupId = user.ResourceGroupId;

        var role = user.UserRoles.Select(x => x.Role?.Name).FirstOrDefault();
        timeTrackUser.Position = role;
    }
}
