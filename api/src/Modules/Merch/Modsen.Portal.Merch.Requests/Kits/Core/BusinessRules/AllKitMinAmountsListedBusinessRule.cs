using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Merch.DataAccess.Npgsql;
using Modsen.Portal.Merch.Requests.Kits.Requests.RequestDto;

namespace Modsen.Portal.Merch.Requests.Kits.Core.BusinessRules;

internal interface IAllKitMinAmountsListedBusinessRule
{
    Task<bool> IsCorrespondsToBusiness(IReadOnlyCollection<KitMinAmountDto> minAmounts, CancellationToken cancellationToken);
}

internal sealed class AllKitMinAmountsListedBusinessRule : IAllKitMinAmountsListedBusinessRule
{
    private readonly IMerchDbContext _dbContext;

    public AllKitMinAmountsListedBusinessRule(IMerchDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> IsCorrespondsToBusiness(IReadOnlyCollection<KitMinAmountDto> minAmounts,
        CancellationToken cancellationToken)
    {
        var storagesIds = await _dbContext.Warehouses
            .Select(warehouse => warehouse.Id).ToListAsync(cancellationToken);

        var areAllStoragesListed = minAmounts.Select(amount => amount.StorageId)
            .All(storageId => storagesIds.Contains(storageId)) && storagesIds.Count == minAmounts.Count;

        if (!areAllStoragesListed)
        {
            return false;
        }

        return true;
    }
}
