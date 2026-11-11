using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Merch.DataAccess.Npgsql;
using Modsen.Portal.Merch.Requests.Items.Core.CoreDto;

namespace Modsen.Portal.Merch.Requests.Items.Core.Loaders;

internal interface IItemLookupLoader
{
    Task<List<ItemLookupCoreDto>> LoadAsync(int[] itemsIds, CancellationToken cancellationToken);
}

internal sealed class ItemLookupLoader : IItemLookupLoader
{
    private readonly IMerchDbContext _dbContext;

    public ItemLookupLoader(IMerchDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<List<ItemLookupCoreDto>> LoadAsync(int[] itemsIds, CancellationToken cancellationToken)
    {
        return _dbContext.Items.Where(item =>
                itemsIds.Contains(item.Id))
            .Select(item => new ItemLookupCoreDto(item.Id, item.Name, item.IsClothing))
            .ToListAsync(cancellationToken);
    }
}
