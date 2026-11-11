namespace Modsen.Portal.Core.Identity;

public static class MerchClaimTypeNames
{
    public const string GroupId = "MERCH";

    public static readonly ClaimType ViewMerch = new("ViewMerch", 100);
    public static readonly ClaimType ManageMerchTransactions = new("ManageMerchTransactions", 200);
    public static readonly ClaimType ManageItemsAndKits = new("ManageItemsAndKits", 300);
}
