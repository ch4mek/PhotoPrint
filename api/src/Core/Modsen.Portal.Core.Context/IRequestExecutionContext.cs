using System.Net;

namespace Modsen.Portal.Core.Context;

public interface IRequestExecutionContext
{
    IPAddress? RemoteIpAddress();

    string? GetRealIpHeader();

    string? GetForwardedForHeader();

    Task<AuthenticationResult> AuthenticateAsync(string scheme);
}

public class AuthenticationResult
{
    public string? Email { get; set; }
    public bool Succeeded { get; set; }
    public string? ReturnUrl { get; set; }
    public string? GivenName { get; set; }
    public string? Surname { get; set; }
}
