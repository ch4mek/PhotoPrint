namespace Modsen.Portal.TeamBoard.Requests.Payload.KpiColors.Requests.RequestDto;

public class KpiColorDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int RightIntervalBorder { get; set; }
}
