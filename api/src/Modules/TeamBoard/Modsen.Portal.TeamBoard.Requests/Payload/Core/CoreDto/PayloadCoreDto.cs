namespace Modsen.Portal.TeamBoard.Requests.Payload.Core.CoreDto;

public class PayloadCoreDto
{
    public int Year { get; set; }
    public int UsersTotal { get; set; }
    public IReadOnlyCollection<PayloadUserCoreDto> Users { get; set; } = [];
    public IReadOnlyCollection<PayloadProjectCoreDto> Projects { get; set; } = [];
    public IReadOnlyCollection<PayloadClientCoreDto> Clients { get; set; } = [];
}
