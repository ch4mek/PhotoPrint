namespace Modsen.Portal.TeamBoard.Requests.Payload.Core.CoreDto;

public class PayloadClientCoreDto
{
    public int ClientId { get; set; }
    public required string Name { get; set; }
    public string? Color { get; set; }
}
