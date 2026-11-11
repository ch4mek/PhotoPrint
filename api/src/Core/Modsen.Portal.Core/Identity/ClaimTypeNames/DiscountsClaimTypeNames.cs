namespace Modsen.Portal.Core.Identity;

public static class DiscountsClaimTypeNames
{
    public const string GroupId = "DISСOUNTS";

    public static readonly ClaimType ViewDiscounts = new("ViewDiscounts", 100);
    public static readonly ClaimType ManageDiscountCategory = new("ManageDiscountCategory", 200);
    public static readonly ClaimType ManageDiscountCard = new("ManageDiscountCard", 300);
    public static readonly ClaimType ManageDiscountOrder = new("ManageDiscountOrder", 400);
    public static readonly ClaimType ManageDiscountsExport = new("ManageDiscountsExport", 500);
}
