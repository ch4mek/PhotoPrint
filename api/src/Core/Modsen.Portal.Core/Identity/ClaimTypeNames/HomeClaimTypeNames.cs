namespace Modsen.Portal.Core.Identity;

public static class HomeClaimTypeNames
{
    public const string GroupId = "HOME";
    public static readonly ClaimType ViewHome = new("ViewHome", 100);
}
