namespace Modsen.Portal.Core.Identity;

public static class CvsClaimTypeNames
{
    public const string GroupId = "CVS";

    public static readonly ClaimType ManageCvs = new("ManageCvs", 100);
    public static readonly ClaimType ViewCvs = new("ViewCvs", 200);
}
