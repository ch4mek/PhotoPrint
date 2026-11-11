using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Finance.DataAccess.Npgsql;
using Modsen.Portal.Finance.DataAccess.Npgsql.Entities;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;

namespace Modsen.Portal.Migrator.MigrationTasks;

internal class PublishExistingClientsToFinanceMigrationTask : IMigrationTask
{
    private readonly TimeTrackDbContext _timeTrackDbContext;
    private readonly FinanceDbContext _financeDbContext;

    public PublishExistingClientsToFinanceMigrationTask(
        TimeTrackDbContext timeTrackDbContext,
        FinanceDbContext financeDbContext)
    {
        _timeTrackDbContext = timeTrackDbContext;
        _financeDbContext = financeDbContext;
    }

    public string TaskName => "PublishExistingClientsToFinance";

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

            var clients = new List<FinanceClientEntity>();

            page++;

            foreach (var timeTrackClient in timeTrackClients)
            {
                var clientId = timeTrackClient.Id;
                var client = _financeDbContext.FinanceClients.FirstOrDefault(x => x.Id == clientId);
                if (client == null)
                {
                    client = new FinanceClientEntity
                    {
                        Id = clientId,
                        Name = timeTrackClient.Name,
                        LegalEntity = timeTrackClient.LegalEntity,
                        IsActive = timeTrackClient.IsActive,
                        SalesManagerId = timeTrackClient.SalesManagerId,
                    };

                    _financeDbContext.Add(client);
                }
                else
                {
                    client.Name = timeTrackClient.Name;
                    client.IsActive = timeTrackClient.IsActive;
                    client.LegalEntity = timeTrackClient.LegalEntity;
                    client.SalesManagerId = timeTrackClient.SalesManagerId;
                }

                clients.Add(client);
            }

            _financeDbContext.SaveChanges();

            foreach (var client in clients)
            {
                _financeDbContext.FinanceClients.Entry(client).State = EntityState.Detached;
            }

            hasAnyClient = _timeTrackDbContext.Clients
                .OrderBy(x => x.Id)
                .Skip(page * size)
                .Any();

        } while (hasAnyClient);
    }
}

