namespace Modsen.Portal.Core.Utils;

public static class ToFullName
{
    public static string Forward(string? firstName, string? lastName)
    {
        return string.Join(" ", firstName, lastName);
    }

    public static string Reverse(string? firstName, string? lastName)
    {
        return string.Join(" ", lastName, firstName);
    }
}
