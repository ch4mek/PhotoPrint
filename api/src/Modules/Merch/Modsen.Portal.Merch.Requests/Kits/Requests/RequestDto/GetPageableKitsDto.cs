namespace Modsen.Portal.Merch.Requests.Kits.Requests.RequestDto;

internal class GetPageableKitsDto
{
    public required int Top { get; set; }
    public required int Skip { get; set; }

    public required KitsQueryOptionsDto QueryOptions { get; set; }
}
