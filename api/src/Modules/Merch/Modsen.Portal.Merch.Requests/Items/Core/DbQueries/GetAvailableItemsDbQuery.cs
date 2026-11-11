using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Merch;
using Modsen.Portal.Merch.DataAccess.Npgsql;
using Modsen.Portal.Merch.DataAccess.Npgsql.Entities;

namespace Modsen.Portal.Merch.Requests.Items.Core.DbQueries;

internal interface IGetAvailableItemsDbQuery
{
    Task<List<ItemEntity>> ExecuteAsync(CancellationToken cancellationToken);
}

internal sealed class GetAvailableItemsDbQuery : IGetAvailableItemsDbQuery
{
    private readonly IMerchDbContext _dbContext;

    public GetAvailableItemsDbQuery(IMerchDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<List<ItemEntity>> ExecuteAsync(CancellationToken cancellationToken)
    {
        return _dbContext.Items
            .Where(item => item.Status != MerchItemStatus.Archived)
            .ToListAsync(cancellationToken);
    }
}
