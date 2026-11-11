using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.Migrator.MigrationTasks;

internal sealed class PublishExistingUsersRegisteredLocationsToTeamBoardMigrationTask : IMigrationTask
{
    private readonly TeamBoardDbContext _teamBoardDbContext;
    private readonly IdentityModuleDbContext _identityModuleDbContext;

    public PublishExistingUsersRegisteredLocationsToTeamBoardMigrationTask(
        TeamBoardDbContext teamBoardDbContext,
        IdentityModuleDbContext identityModuleDbContext)
    {
        _teamBoardDbContext = teamBoardDbContext;
        _identityModuleDbContext = identityModuleDbContext;
    }

    public string TaskName => "PublishExistingUsersRegisteredLocationsToTeamBoard";

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

            var registeredLocations = new List<RegisteredLocationHistory>();

            page++;

            foreach (var user in users)
            {
                if (_teamBoardDbContext.TeamBoardUsers.Any(x => x.UserId == user.Id)
                    && _teamBoardDbContext.Countries.Any(x => x.Id == user.RegisteredLocationId))
                {
                    var registeredLocation = CreateRegisteredLocation(user);

                    registeredLocations.Add(registeredLocation);
                }
            }

            _teamBoardDbContext.RegisteredLocationHistory.AddRange(registeredLocations);

            _teamBoardDbContext.SaveChanges();

            foreach (var registeredLocation in registeredLocations)
            {
                _teamBoardDbContext.RegisteredLocationHistory.Entry(registeredLocation).State = EntityState.Detached;
            }

            hasAnyUsers = _identityModuleDbContext.Users
                .OrderBy(x => x.Id)
                .Skip(page * MigratorTaskConstants.BatchSize)
                .Any();
        } while (hasAnyUsers);
    }

    private static RegisteredLocationHistory CreateRegisteredLocation(User user)
    {
        return new RegisteredLocationHistory
        {
            UserId = user.Id,
            DateFrom = DateOnly.FromDateTime(DateTime.UtcNow),
            RegisteredLocationId = user.RegisteredLocationId
        };
    }
}
