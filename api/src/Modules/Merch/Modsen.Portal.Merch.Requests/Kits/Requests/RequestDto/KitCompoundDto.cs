namespace Modsen.Portal.Merch.Requests.Kits.Requests.RequestDto;

internal sealed record KitCompoundDto(
    int ItemId,
    bool IsClothing,
    string Name,
    int AmountInKit);
