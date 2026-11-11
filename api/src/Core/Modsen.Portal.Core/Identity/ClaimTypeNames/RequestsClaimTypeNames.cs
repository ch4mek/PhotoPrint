namespace Modsen.Portal.Core.Identity;

public static class RequestsClaimTypeNames
{
    public const string GroupId = "REQUESTS";

    public static readonly ClaimType ManageAllRequests = new("ManageAllRequests", 100);
    public static readonly ClaimType ManageRequests = new("ManageRequests", 200);
    public static readonly ClaimType ViewAllRequests = new("ViewAllRequests", 300);
}
