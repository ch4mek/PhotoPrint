namespace Modsen.Portal.TeamBoard.Requests.Payload.Core.CoreDto;

public class RegisteredLocationHistoryCoreDto
{
    public int RegisteredLocationId { get; set; }
    public DateOnly DateFrom { get; set; }
    public DateOnly? DateTo { get; set; }
}
