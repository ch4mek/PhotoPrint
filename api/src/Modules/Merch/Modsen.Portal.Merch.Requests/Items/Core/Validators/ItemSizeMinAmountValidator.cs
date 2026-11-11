using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Merch.Requests.Items.Core.BusinessRules;
using Modsen.Portal.Merch.Requests.Items.Core.CoreDto;

namespace Modsen.Portal.Merch.Requests.Items.Core.Validators;

internal interface IItemSizeMinAmountValidator
{
    Task ValidateAsync(ICollection<SizeMinAmountCoreDto> minAmounts, bool isClothing,
        CancellationToken cancellationToken);
}

internal sealed class ItemSizeMinAmountValidator : IItemSizeMinAmountValidator
{
    private readonly IAreAllStoragesListedBusinessRule _areAllStoragesListedBusinessRule;
    private readonly IAreAllSizesListedBusinessRule _areAllSizesListedBusinessRule;

    public ItemSizeMinAmountValidator(IAreAllStoragesListedBusinessRule areAllStoragesListedBusinessRule,
        IAreAllSizesListedBusinessRule areAllSizesListedBusinessRule)
    {
        _areAllStoragesListedBusinessRule = areAllStoragesListedBusinessRule;
        _areAllSizesListedBusinessRule = areAllSizesListedBusinessRule;
    }

    public async Task ValidateAsync(ICollection<SizeMinAmountCoreDto> minAmounts, bool isClothing,
        CancellationToken cancellationToken)
    {
        var areAllStoragesListed = await _areAllStoragesListedBusinessRule.IsCorrespondsToBusiness(minAmounts,
            cancellationToken);

        if (!areAllStoragesListed)
        {
            throw new EntityNotFoundException(PortalMessages.NotAllStoragesAreListed);
        }

        var areAllSizesListed = await _areAllSizesListedBusinessRule.IsCorrespondsToBusiness(minAmounts,
            isClothing,
            cancellationToken);

        if (!areAllSizesListed)
        {
            throw new EntityNotFoundException(PortalMessages.NotAllSizesAreListed);
        }
    }
}
