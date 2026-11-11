namespace Modsen.Portal.Core.Identity;

public static class WikiClaimTypeNames
{
    public const string GroupId = "WIKI";

    public static readonly ClaimType ViewWiki = new("ViewWiki", 100);
    public static readonly ClaimType ManageArticle = new("ManageArticle", 200);
    public static readonly ClaimType ManageGroup = new("ManageGroup", 300);
    public static readonly ClaimType ManageSubgroup = new("ManageSubgroup", 400);
    public static readonly ClaimType ManageOrder = new("ManageOrder", 500);
    public static readonly ClaimType ManageWikiExport = new("ManageWikiExport", 600);
}
