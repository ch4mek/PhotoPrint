using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Merch.DataAccess.Npgsql;
using Modsen.Portal.Merch.Requests.Items.Core.CoreDto;

namespace Modsen.Portal.Merch.Requests.Items.Core.BusinessRules;

internal interface IAreAllStoragesListedBusinessRule
{
    Task<bool> IsCorrespondsToBusiness(ICollection<SizeMinAmountCoreDto> amounts, CancellationToken cancellationToken);
}

internal sealed class AreAllStoragesListedBusinessRule : IAreAllStoragesListedBusinessRule
{
    private readonly IMerchDbContext _dbContext;

    public AreAllStoragesListedBusinessRule(IMerchDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> IsCorrespondsToBusiness(ICollection<SizeMinAmountCoreDto> amounts,
        CancellationToken cancellationToken)
    {
        var storagesIds = amounts.Select(amount => amount.StorageId).Distinct().ToArray();
        var existingStoragesIds = await _dbContext.Warehouses.Select(warehouse => warehouse.Id)
            .ToListAsync(cancellationToken);

        var areAllStoragesListed = storagesIds.Length == existingStoragesIds.Count &&
            storagesIds.All(id => existingStoragesIds.Contains(id));

        return areAllStoragesListed;
    }
}
