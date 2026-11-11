namespace Modsen.Portal.Core.Identity;

public static class HrClaimTypeNames
{
    public const string GroupId = "HR";

    public static readonly ClaimType ManageHrModule = new("ManageHRModule", 100);
}
