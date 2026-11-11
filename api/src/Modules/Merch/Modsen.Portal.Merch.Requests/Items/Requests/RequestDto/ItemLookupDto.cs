namespace Modsen.Portal.Merch.Requests.Items.Requests.RequestDto;

internal sealed record ItemLookupDto(
    int Id,
    string Name,
    bool IsClothing);
