namespace Modsen.Portal.Core.Identity;

public static class StaffingClaimTypeNames
{
    public const string GroupId = "STAFFING";

    public static readonly ClaimType ManageAllInterviews = new("ManageAllInterviews", 100);
    public static readonly ClaimType ManageInterviews = new("ManageInterviews", 200);
    public static readonly ClaimType ViewProjectExits = new("ViewProjectExits", 300);
    public static readonly ClaimType ManageAllProjectExits = new("ManageAllProjectExits", 400);
}
