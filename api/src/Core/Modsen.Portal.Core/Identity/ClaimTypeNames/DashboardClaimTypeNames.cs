namespace Modsen.Portal.Core.Identity;

public static class DashboardClaimTypeNames
{
    public const string GroupId = "DASHBOARD";

    public static readonly ClaimType ViewDashboard = new("ViewDashboard", 100);
}
