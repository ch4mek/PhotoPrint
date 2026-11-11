using System.Net.Http.Json;
using System.Text.Json;
using Modsen.Portal.Jira.Contract;
using Modsen.Portal.Jira.Contract.Exceptions;

namespace Modsen.Portal.Jira;

internal static class JiraPermissionResponseReader
{
    public static async Task<bool> HasBrowseProjectsPermissionAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        try
        {
            var permissionResponse = await response.Content.ReadFromJsonAsync<JiraPermissionResponse>(cancellationToken);

            return permissionResponse?.Permissions is not null &&
                permissionResponse.Permissions.TryGetValue(JiraConstants.BrowseProjectsPermission, out var browseProjectsPermission) &&
                browseProjectsPermission.HavePermission == true;
        }
        catch (JsonException)
        {
            throw new JiraCredentialsCheckFailedException();
        }
    }
}
