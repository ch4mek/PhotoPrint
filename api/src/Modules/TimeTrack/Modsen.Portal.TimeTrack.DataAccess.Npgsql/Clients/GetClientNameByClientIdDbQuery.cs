using Microsoft.EntityFrameworkCore;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Clients;

namespace Modsen.Portal.TimeTrack.DataAccess.Npgsql.Clients;

internal class GetClientNameByClientIdDbQuery : IGetClientNameByClientIdDbQuery
{
    private readonly ITimeTrackDbContext _dbContext;

    public GetClientNameByClientIdDbQuery(ITimeTrackDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<string?> GetClientNameByClientIdAsync(
        int clientId,
        CancellationToken cancellationToken)
    {
        return await _dbContext.Clients
                .Where(client => client.Id == clientId)
                .Select(client => client.Name)
                .FirstOrDefaultAsync(cancellationToken);
    }
}
