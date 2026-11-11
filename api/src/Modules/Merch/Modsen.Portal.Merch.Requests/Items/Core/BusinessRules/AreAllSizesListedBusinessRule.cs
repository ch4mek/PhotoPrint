using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Merch.DataAccess.Npgsql;
using Modsen.Portal.Merch.Requests.Items.Core.CoreDto;

namespace Modsen.Portal.Merch.Requests.Items.Core.BusinessRules;

internal interface IAreAllSizesListedBusinessRule
{
    Task<bool> IsCorrespondsToBusiness(ICollection<SizeMinAmountCoreDto> amounts, bool isClothing,
        CancellationToken cancellationToken);
}

internal sealed class AreAllSizesListedBusinessRule : IAreAllSizesListedBusinessRule
{
    private readonly IMerchDbContext _dbContext;

    public AreAllSizesListedBusinessRule(IMerchDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> IsCorrespondsToBusiness(ICollection<SizeMinAmountCoreDto> amounts,
        bool isClothing,
        CancellationToken cancellationToken)
    {
        var sizesIds = amounts
            .GroupBy(amount => amount.StorageId)
            .Select(grouping => grouping.Select(amount =>amount.SizeId).ToList())
            .ToList();

        var query = _dbContext
            .MerchSizes
            .AsQueryable();

        query = isClothing
            ? query.Where(size => size.OrderWeight != 0)
            : query.Where(size => size.OrderWeight == 0);


        var existingSizesIds = await query.Select(size => size.Id).ToListAsync(cancellationToken);

        foreach (var sizeIds in sizesIds)
        {
            var areAllSizesWithinWarehouseListed = sizeIds.Count == existingSizesIds.Count &&
                sizeIds.All(id => existingSizesIds.Contains(id));

            if (!areAllSizesWithinWarehouseListed)
            {
                return false;
            }
        }

        return true;
    }
}
