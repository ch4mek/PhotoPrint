using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Merch.DataAccess.Npgsql;

namespace Modsen.Portal.Merch.Requests.Kits.Core.BusinessRules;

internal interface IKitNameUniqueBusinessRule
{
    Task<bool> IsCorrespondsToBusiness(string name, int? id, CancellationToken cancellationToken);
}

internal sealed class KitNameUniqueBusinessRule : IKitNameUniqueBusinessRule
{
    private readonly IMerchDbContext _dbContext;

    public KitNameUniqueBusinessRule(IMerchDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> IsCorrespondsToBusiness(string name, int? id, CancellationToken cancellationToken)
    {
        return !await _dbContext.Kits.AnyAsync(kit => kit.Name == name && kit.Id != id, cancellationToken);
    }
}
