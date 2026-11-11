namespace Modsen.Portal.Core;

public static class RevokeReasons
{
    public const string RegisteredNewToken = "registered new token for user";
    public const string Logout = "logout";
    public const string InfoUpdated = "user info has been updated";
    public const string ClaimsChanged = "claims permission has been changed";
    public const string EmergencyShutdown = "emergency shutdown";
    public const string ProjectPermissionsChanged = "project permission assignment has been changed";
    public const string UserDeactivated = "user has been deactivated";
    public const string DepartmentChanged = "department has been changed";
    public const string PermissionAssignmentChanged = "permission assignment has been changed";
}
