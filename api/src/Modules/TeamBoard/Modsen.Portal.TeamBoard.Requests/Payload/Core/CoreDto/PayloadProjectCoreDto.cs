namespace Modsen.Portal.TeamBoard.Requests.Payload.Core.CoreDto;

public class PayloadProjectCoreDto
{
    public int ProjectId { get; set; }
    public int ClientId { get; set; }
    public required string Name { get; set; }
}
