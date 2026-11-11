namespace Modsen.Portal.Merch.Requests.Items.Requests.RequestDto;

internal sealed class AvailableFiltersDto
{
    public required IReadOnlyCollection<MerchItemLookupDto> Items { get; set; } = [];
    public required IReadOnlyCollection<StorageLookupDto> Storages { get; set; } = [];
}
