namespace Modsen.Portal.Core.Identity;

public static class MicrofrontendsClaimTypeNames
{
    public const string GroupId = "MICROFRONTEND";

    public static readonly ClaimType ManageMfe = new("ManageMfe", 100);
}
