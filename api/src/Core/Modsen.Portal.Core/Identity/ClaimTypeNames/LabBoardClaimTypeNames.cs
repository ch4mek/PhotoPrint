namespace Modsen.Portal.Core.Identity;

public static class LabBoardClaimTypeNames
{
    public const string GroupId = "LABBOARD";

    public static readonly ClaimType ManageInterns = new("ManageInterns", 100);
    public static readonly ClaimType ManageAllInterns = new("ManageAllInterns", 200);
}
