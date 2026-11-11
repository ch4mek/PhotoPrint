using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Cv.DataAccess.Npgsql;
using Modsen.Portal.Cv.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;

namespace Modsen.Portal.Migrator.MigrationTasks;

internal sealed class PublishExistingClientsToCvMigrationTask : IMigrationTask
{
    private readonly TimeTrackDbContext _timeTrackDbContext;
    private readonly CvDbContext _cvDbContext;

    public PublishExistingClientsToCvMigrationTask(
        TimeTrackDbContext timeTrackDbContext,
        CvDbContext cvDbContext)
    {
        _timeTrackDbContext = timeTrackDbContext;
        _cvDbContext = cvDbContext;
    }

    public string TaskName => "PublishExistingClientsToCv";

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

            var clients = new List<CvClientEntity>();

            page++;

            foreach (var timeTrackClient in timeTrackClients)
            {
                var clientId = timeTrackClient.Id.ToString(CultureInfo.InvariantCulture);
                var client = _cvDbContext.CvClients.FirstOrDefault(x => x.ClientId == clientId);
                if (client == null)
                {
                    client = new CvClientEntity
                    {
                        ClientId = clientId,
                        Name = timeTrackClient.Name,
                        IsActive = timeTrackClient.IsActive
                    };

                    _cvDbContext.Add(client);
                }
                else
                {
                    client.Name = timeTrackClient.Name;
                    client.IsActive = timeTrackClient.IsActive;
                }

                clients.Add(client);
            }

            _cvDbContext.SaveChanges();

            foreach (var client in clients)
            {
                _cvDbContext.CvClients.Entry(client).State = EntityState.Detached;
            }

            hasAnyClient = _timeTrackDbContext.Clients
                .OrderBy(x => x.Id)
                .Skip(page * size)
                .Any();

        } while (hasAnyClient);
    }
}
