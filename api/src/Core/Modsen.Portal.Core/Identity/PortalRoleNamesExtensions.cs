namespace Modsen.Portal.Core.Identity;

public static class PortalRoleNamesExtensions
{
    public static bool IsCeo(this string? roleName)
    {
        return string.Equals(roleName, PortalRoleNames.Ceo, StringComparison.Ordinal);
    }

    public static bool IsCeo(this IEnumerable<string?>? roleNames)
    {
        return roleNames?.Any(IsCeo) is true;
    }

    public static bool IsCto(this string? roleName)
    {
        return string.Equals(roleName, PortalRoleNames.Cto, StringComparison.Ordinal);
    }

    public static bool IsCto(this IEnumerable<string?>? roleNames)
    {
        return roleNames?.Any(IsCto) is true;
    }

    public static bool IsHeadOfDepartment(this string? roleName)
    {
        return string.Equals(roleName, PortalRoleNames.HeadOfDepartment, StringComparison.Ordinal);
    }

    public static bool IsHeadOfDepartment(this IEnumerable<string?>? roleNames)
    {
        return roleNames?.Any(IsHeadOfDepartment) is true;
    }

    public static bool IsGroupManager(this string? roleName)
    {
        return string.Equals(roleName, PortalRoleNames.GroupManager, StringComparison.Ordinal);
    }

    public static bool IsGroupManager(this IEnumerable<string?>? roleNames)
    {
        return roleNames?.Any(IsGroupManager) is true;
    }

    public static bool IsEmployee(this string? roleName)
    {
        return string.Equals(roleName, PortalRoleNames.Employee, StringComparison.Ordinal);
    }

    public static bool IsEmployee(this IEnumerable<string?>? roleNames)
    {
        return roleNames?.Any(IsEmployee) is true;
    }

    public static bool IsIntern(this string? roleName)
    {
        return string.Equals(roleName, PortalRoleNames.Intern, StringComparison.Ordinal);
    }

    public static bool IsIntern(this IEnumerable<string?>? roleNames)
    {
        return roleNames?.Any(IsIntern) is true;
    }

    public static bool IsSystemAdministrator(this string? roleName)
    {
        return string.Equals(roleName, PortalRoleNames.SystemAdministrator, StringComparison.Ordinal);
    }

    public static bool IsSystemAdministrator(this IEnumerable<string?>? roleNames)
    {
        return roleNames?.Any(IsSystemAdministrator) is true;
    }

    public static bool IsInOneOfTheRoles(this IEnumerable<string?>? roleNames, params IEnumerable<string> specifiedRoles)
    {
        return roleNames != null && roleNames.Any(specifiedRoles.Contains);
    }

    public static bool IsValidPosition(this string? position)
    {
        return position.IsCeo() ||
            position.IsCto() ||
            position.IsHeadOfDepartment() ||
            position.IsGroupManager() ||
            position.IsEmployee() ||
            position.IsSystemAdministrator();
    }
}
