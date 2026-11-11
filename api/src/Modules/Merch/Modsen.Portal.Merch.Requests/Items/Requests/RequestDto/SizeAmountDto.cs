namespace Modsen.Portal.Merch.Requests.Items.Requests.RequestDto;

internal sealed record SizeAmountDto(
    string SizeId,
    int StorageId,
    int Amount,
    int MinAmount){}
