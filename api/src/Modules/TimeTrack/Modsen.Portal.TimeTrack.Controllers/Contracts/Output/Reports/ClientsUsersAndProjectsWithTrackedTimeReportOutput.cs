namespace Modsen.Portal.TimeTrack.Controllers.Contracts.Output.Reports;

internal class ClientsUsersAndProjectsWithTrackedTimeReportOutput
{
    public IReadOnlyCollection<ClientWithTrackedTimeReportOutput> PerClients { get; set; } = [];
    public IReadOnlyCollection<UserWithTrackedTimeReportOutput> PerUsers { get; set; } = [];
    public IReadOnlyCollection<ProjectWithTrackedTimeReportOutput> PerProjects { get; set; } = [];
}
