using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Database;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;

namespace Modsen.Portal.Migrator.MigrationTasks;

internal sealed class PublishExistingClientsToTeamBoardMigrationTask : IMigrationTask
{
    private readonly TimeTrackDbContext _timeTrackDbContext;
    private readonly TeamBoardDbContext _teamBoardDbContext;

    public PublishExistingClientsToTeamBoardMigrationTask(
        TimeTrackDbContext timeTrackDbContext,
        TeamBoardDbContext teamBoardDbContext)
    {
        _timeTrackDbContext = timeTrackDbContext;
        _teamBoardDbContext = teamBoardDbContext;
    }

    public string TaskName => "PublishExistingClientsToTeamBoard";

    public void Execute()
    {
        var page = 0;
        var size = MigratorTaskConstants.BatchSize;
        bool hasAnyClient;

        do
        {
            var timeTrackClients = _timeTrackDbContext.Clients
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .Skip(page * size)
                .Take(size)
                .ToList();

            var clients = new List<TeamBoardClient>();

            page++;

            foreach (var timeTrackClient in timeTrackClients)
            {
                var client = _teamBoardDbContext.TeamBoardClients.FirstOrDefault(x => x.ClientId == timeTrackClient.Id);
                if (client == null)
                {
                    client = new TeamBoardClient
                    {
                        ClientId = timeTrackClient.Id,
                        Name = timeTrackClient.Name,
                        IsActive = timeTrackClient.IsActive
                    };

                    _teamBoardDbContext.Add(client);
                }
                else
                {
                    client.Name = timeTrackClient.Name;
                    client.IsActive = timeTrackClient.IsActive;
                }

                clients.Add(client);
            }

            _teamBoardDbContext.SaveChanges();

            foreach (var client in clients)
            {
                _teamBoardDbContext.TeamBoardClients.Entry(client).State = EntityState.Detached;
            }

            hasAnyClient = _timeTrackDbContext.Clients
                .OrderBy(x => x.Id)
                .Skip(page * size)
                .Any();

        } while (hasAnyClient);
    }
}

