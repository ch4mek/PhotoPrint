using Modsen.Portal.Core;

namespace Modsen.Portal.TimeTrack.Domain;

internal class Project
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public string? Color { get; set; }
    public bool IsPublic { get; set; }
    public bool IsBillable { get; set; }
    public bool IsActive { get; set; }
    public int ClientId { get; set; }
    public int? ManagerId { get; set; }
    public int? CreatedById { get; set; }
    public string? TechStack { get; set; }
    public string? Industry { get; set; }
    public string? Architecture { get; set; }
    public string? CloudServices { get; set; }
    public int? FactTrackedTimeInMinutes { get; set; }
    public int? BillableTrackedTimeInMinutes { get; set; }
    public List<int> ProjectUsers { get; set; } = [];

    public string GetProjectType()
    {
        return IsPublic
            ? PortalProjectTypeNames.Public
            : PortalProjectTypeNames.Private;
    }
}
