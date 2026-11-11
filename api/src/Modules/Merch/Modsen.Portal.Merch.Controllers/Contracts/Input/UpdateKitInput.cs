using Modsen.Portal.Merch.Requests.Kits.Requests.RequestDto;

namespace Modsen.Portal.Merch.Controllers.Contracts.Input;

internal sealed record UpdateKitInput(
    string Name,
    string? PhotoFileId,
    string? Description,
    IReadOnlyCollection<AddKitCompoundDto> Compounds,
    IReadOnlyCollection<KitMinAmountDto> MinAmounts)
{
    public UpdateKitDto ToRequestDto(int id)
    {
        return new UpdateKitDto(
            Id: id,
            Name: Name,
            PhotoFileId: PhotoFileId,
            Description: Description,
            Compounds: Compounds,
            MinAmounts: MinAmounts);
    }

}
