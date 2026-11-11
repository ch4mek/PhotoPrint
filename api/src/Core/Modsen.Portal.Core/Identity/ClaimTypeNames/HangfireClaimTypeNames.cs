namespace Modsen.Portal.Core.Identity;

public static class HangfireClaimTypeNames
{
    public const string GroupId = "HANGFIRE";

    public static readonly ClaimType HangfireDashboardAccess = new("HangfireDashboardAccess", 100);
}
