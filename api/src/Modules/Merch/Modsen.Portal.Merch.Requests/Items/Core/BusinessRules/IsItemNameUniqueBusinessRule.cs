using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Merch.DataAccess.Npgsql;

namespace Modsen.Portal.Merch.Requests.Items.Core.BusinessRules;

internal interface IIsItemNameUniqueBusinessRule
{
    Task<bool> IsCorrespondsToBusiness(int? id, string name, CancellationToken cancellationToken);
}

internal sealed class IsItemNameUniqueBusinessRule : IIsItemNameUniqueBusinessRule
{
    private readonly IMerchDbContext _merchDbContext;

    public IsItemNameUniqueBusinessRule(IMerchDbContext merchDbContext)
    {
        _merchDbContext = merchDbContext;
    }

    public async Task<bool> IsCorrespondsToBusiness(int? id, string name, CancellationToken cancellationToken)
    {
        return !await _merchDbContext.Items.AnyAsync(item =>
            item.Id != id &&
            item.Name == name,
            cancellationToken);
    }
}
