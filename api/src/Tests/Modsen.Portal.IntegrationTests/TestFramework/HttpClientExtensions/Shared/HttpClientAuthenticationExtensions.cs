using System.Net.Http.Headers;
using System.Web;
using Modsen.Portal.Core.Identity;

namespace Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;

internal static class HttpClientAuthenticationExtensions
{
    public static async Task AuthAs(this HttpClient client, string email)
    {
        var response = await client.GetAsync($"/api/account/tests/externalLoginCallback?email={HttpUtility.UrlEncode(email)}");
        if (!response.IsSuccessStatusCode)
        {
            var stringResponse = await response.Content.ReadAsStringAsync();
            throw new ApplicationException(stringResponse);
        }

        var token = await response.Content.ReadAsStringAsync();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    public static Task AuthAsRole(this HttpClient client, string role)
    {
        return role switch
        {
            PortalRoleNames.SystemAdministrator => client.AuthAsAdmin(),
            PortalRoleNames.Ceo => client.AuthAsCeo(),
            PortalRoleNames.Cto => client.AuthAsCto(),
            PortalRoleNames.HeadOfDepartment => client.AuthAsHeadOfDepartment(),
            PortalRoleNames.Employee => client.AuthAsEmployee(),
            PortalRoleNames.GroupManager => client.AuthAsGroupManager(),
            _ => throw new ArgumentOutOfRangeException(nameof(role), role, null)
        };
    }

    public static void Logout(this HttpClient client)
    {
        client.DefaultRequestHeaders.Authorization = null;
    }

    public static Task AuthAsCeo(this HttpClient client)
    {
        return client.AuthAs(EmailConstants.CeoEmail);
    }

    public static Task AuthAsCto(this HttpClient client)
    {
        return client.AuthAs(EmailConstants.CtoEmail);
    }

    public static Task AuthAsHeadOfDepartment(this HttpClient client)
    {
        return client.AuthAs(EmailConstants.HeadEmail);
    }

    public static Task AuthAsGroupManager(this HttpClient client)
    {
        return client.AuthAs(EmailConstants.GroupManagerEmail);
    }

    public static Task AuthAsAdmin(this HttpClient client)
    {
        return client.AuthAs(EmailConstants.AdminEmail);
    }

    public static Task AuthAsEmployee(this HttpClient client)
    {
        return client.AuthAs(EmailConstants.EmployeeEmail);
    }

    public static Task AuthAsSysAdminHead(this HttpClient client)
    {
        return client.AuthAs(EmailConstants.SysAdminHead);
    }
}
