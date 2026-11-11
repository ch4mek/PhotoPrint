namespace Modsen.Portal.TeamBoard.Requests.Payload.Core.CoreDto;

internal class UserProjectDetailsCoreDto
{
    public int ProjectId { get; set; }
    public decimal Fte { get; set; }
    public DateOnly UserOnProjectFrom { get; set; }
    public DateOnly? UserOnProjectTo { get; set; }
    public bool IsTimeOnProjectBillable { get; set; }
}
