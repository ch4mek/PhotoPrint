using Modsen.Portal.Merch.DataAccess.Npgsql;
using Modsen.Portal.Merch.DataAccess.Npgsql.Entities;

namespace Modsen.Portal.Merch.Requests.Items.Core.DbQueries;

internal interface IGetMerchItemByIdDbQuery
{
    ValueTask<ItemEntity?> ExecuteAsync(int id, CancellationToken cancellationToken);
}

internal sealed class GetMerchItemByIdDbQuery : IGetMerchItemByIdDbQuery
{
    private readonly IMerchDbContext _dbContext;

    public GetMerchItemByIdDbQuery(IMerchDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public ValueTask<ItemEntity?> ExecuteAsync(int id, CancellationToken cancellationToken)
    {
        return _dbContext.Items.FindAsync([id], cancellationToken);
    }
}
