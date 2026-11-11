namespace Modsen.Portal.TimeTrack.UseCases.Dashboard.Core.CoreDto;

public class DashboardOverviewCoreDto
{
    public int TotalBillableTime { get; set; }

    public int TotalFactTime { get; set; }

    public required string TopBillableProject { get; set; }

    public int TopBillableProjectId { get; set; }

    public required string TopFactProject { get; set; }

    public int TopFactProjectId { get; set; }

    public required string TopBillableClient { get; set; }

    public int TopBillableClientId { get; set; }

    public required string TopFactClient { get; set; }

    public int TopFactClientId { get; set; }
}
