using Modsen.Portal.Merch.Requests.Kits.Requests.RequestDto;

namespace Modsen.Portal.Merch.Requests.Kits.Core.CoreDto;

internal sealed record CreateOrUpdateKitValidatorCoreDto(
    string Name,
    int? Id,
    IReadOnlyCollection<AddKitCompoundDto> Compounds,
    IReadOnlyCollection<KitMinAmountDto> MinAmounts)
{
    public static CreateOrUpdateKitValidatorCoreDto Of(UpdateKitDto updateKitDto)
    {
        return new CreateOrUpdateKitValidatorCoreDto(
            updateKitDto.Name,
            updateKitDto.Id,
            updateKitDto.Compounds,
            updateKitDto.MinAmounts);
    }

    public static CreateOrUpdateKitValidatorCoreDto Of(CreateKitDto createKitDto)
    {
        return new CreateOrUpdateKitValidatorCoreDto(
            createKitDto.Name,
            null,
            createKitDto.Compounds,
            createKitDto.MinAmounts);
    }
}
