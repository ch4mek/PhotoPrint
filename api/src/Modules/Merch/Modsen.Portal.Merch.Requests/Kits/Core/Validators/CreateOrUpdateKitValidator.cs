using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Merch.Requests.Kits.Core.BusinessRules;
using Modsen.Portal.Merch.Requests.Kits.Core.CoreDto;

namespace Modsen.Portal.Merch.Requests.Kits.Core.Validators;

internal interface ICreateOrUpdateKitValidator
{
    Task ValidateAsync(CreateOrUpdateKitValidatorCoreDto coreDto, CancellationToken cancellationToken);
}

internal sealed class CreateOrUpdateKitValidator : ICreateOrUpdateKitValidator
{
    private readonly IAllKitMinAmountsListedBusinessRule _allKitMinAmountsListedBusinessRule;
    private readonly IAreCompoundsValidBusinessRule _areCompoundsValidBusinessRule;
    private readonly IKitNameUniqueBusinessRule _kitNameUniqueBusinessRule;

    public CreateOrUpdateKitValidator(IAllKitMinAmountsListedBusinessRule allKitMinAmountsListedBusinessRule,
        IAreCompoundsValidBusinessRule areCompoundsValidBusinessRule,
        IKitNameUniqueBusinessRule kitNameUniqueBusinessRule)
    {
        _allKitMinAmountsListedBusinessRule = allKitMinAmountsListedBusinessRule;
        _areCompoundsValidBusinessRule = areCompoundsValidBusinessRule;
        _kitNameUniqueBusinessRule = kitNameUniqueBusinessRule;
    }

    public async Task ValidateAsync(CreateOrUpdateKitValidatorCoreDto coreDto, CancellationToken cancellationToken)
    {
        var isNameUnique = await _kitNameUniqueBusinessRule.IsCorrespondsToBusiness(coreDto.Name, coreDto.Id,
            cancellationToken);

        if (!isNameUnique)
        {
            throw new InvalidDataAppException(PortalMessages.MerchKitNameExists);
        }

        var areCompoundsValidResult = await _areCompoundsValidBusinessRule.IsCorrespondsToBusiness(coreDto.Compounds,
            cancellationToken);

        if (!areCompoundsValidResult.IsSuccess)
        {
            throw new InvalidDataAppException(areCompoundsValidResult.Reason!);
        }

        var areAllStoragesListed = await _allKitMinAmountsListedBusinessRule.IsCorrespondsToBusiness(
            coreDto.MinAmounts,
            cancellationToken);

        if (!areAllStoragesListed)
        {
            throw new InvalidDataAppException(PortalMessages.NotAllStoragesAreListed);
        }
    }
}
