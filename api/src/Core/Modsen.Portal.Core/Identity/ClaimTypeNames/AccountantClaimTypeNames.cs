namespace Modsen.Portal.Core.Identity;

public static class AccountantClaimTypeNames
{
    public const string GroupId = "ACCOUNTANT";
    public static readonly ClaimType ManageAccountant = new ClaimType("ManageAccountant", 100);
}
