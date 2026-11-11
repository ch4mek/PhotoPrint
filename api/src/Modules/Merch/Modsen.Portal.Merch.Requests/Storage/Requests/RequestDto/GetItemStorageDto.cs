namespace Modsen.Portal.Merch.Requests.Storage.Requests.RequestDto;

internal class GetItemStorageDto
{
    public required int Top { get; set; }
    public required int Skip { get; set; }

    public StorageQueryOptionsDto? QueryOptions { get; set; }
}
