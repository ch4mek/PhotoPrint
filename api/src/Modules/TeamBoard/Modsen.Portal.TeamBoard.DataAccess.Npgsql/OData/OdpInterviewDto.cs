namespace Modsen.Portal.TeamBoard.DataAccess.Npgsql.OData;

public class OdpInterviewDto
{
    public OdpInterview Main { get; set; } = null!;
    public IReadOnlyCollection<OdpInterview> Stages { get; set; } = [];
}
