namespace Modsen.Portal.Core.Identity;

public static class CoordinatorClaimTypeNames
{
    public const string GroupId = "PROJECT COORDINATOR";

    public static readonly ClaimType ManageProjectCoordinator = new("ManageProjectCoordinator", 100);
}
