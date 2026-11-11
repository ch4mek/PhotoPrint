using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Merch;
using Modsen.Portal.Core.Services.BusinessRules;
using Modsen.Portal.Merch.DataAccess.Npgsql;
using Modsen.Portal.Merch.Requests.Kits.Requests.RequestDto;

namespace Modsen.Portal.Merch.Requests.Kits.Core.BusinessRules;

internal interface IAreCompoundsValidBusinessRule
{
    Task<BusinessRuleResult> IsCorrespondsToBusiness(IReadOnlyCollection<AddKitCompoundDto> compounds, CancellationToken cancellationToken);
}

internal sealed class AreCompoundsValidBusinessRule : IAreCompoundsValidBusinessRule
{
    private readonly IMerchDbContext _dbContext;

    public AreCompoundsValidBusinessRule(IMerchDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<BusinessRuleResult> IsCorrespondsToBusiness(IReadOnlyCollection<AddKitCompoundDto> compounds,
        CancellationToken cancellationToken)
    {
        var availableCompounds = await _dbContext.Items
            .Where(item =>
                item.Status != MerchItemStatus.Archived &&
                compounds.Select(compound => compound.ItemId).Contains(item.Id))
            .ToListAsync(cancellationToken);

        if (availableCompounds.Count != compounds.Count)
        {
            return BusinessRuleResult.Fail(PortalMessages.NotAllCompoundsExistOrAvailable);
        }

        var clothingCount = availableCompounds.Count(item => item.IsClothing);
        if (clothingCount > 1)
        {
            return BusinessRuleResult.Fail(PortalMessages.OnlyOneClothingInKit);
        }

        return BusinessRuleResult.Success();
    }
}
