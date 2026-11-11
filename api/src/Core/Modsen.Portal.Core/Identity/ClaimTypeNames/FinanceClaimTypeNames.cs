namespace Modsen.Portal.Core.Identity;

public static class FinanceClaimTypeNames
{
    public const string GroupId = "P&L";

    public static readonly ClaimType ViewRates = new("ViewRates", 100);
    public static readonly ClaimType ViewAllRates = new("ViewAllRates", 200);
    public static readonly ClaimType ManageRates = new ("ManageRates", 300);
    public static readonly ClaimType ManageAllRates = new("ManageAllRates", 400);
    public static readonly ClaimType ViewManagementTab = new("ViewManagementTab", 500);
    public static readonly ClaimType ManageExpensesTab = new("ManageExpensesTab", 600);
}
