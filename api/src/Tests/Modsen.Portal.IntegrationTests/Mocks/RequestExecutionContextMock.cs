using System.Net;
using Modsen.Portal.Core.Context;

namespace Modsen.Portal.IntegrationTests.Mocks;

internal class RequestExecutionContextMock : IRequestExecutionContext
{
    private string _ipAddress = "127.0.0.1";

    public IPAddress? RemoteIpAddress()
    {
        return IPAddress.Parse(_ipAddress);
    }

    public string? GetRealIpHeader()
    {
        return _ipAddress;
    }

    public string? GetForwardedForHeader()
    {
        return _ipAddress;
    }

    public void ChangeMockIpAddress(string ipAddress)
    {
        _ipAddress = ipAddress;
    }

    public Task<AuthenticationResult> AuthenticateAsync(string scheme)
    {
        return Task.FromResult(new AuthenticationResult
        {
            Succeeded = true,
            Email = "test@modsen-software.com",
            GivenName = "Test",
            Surname = "Test",
            ReturnUrl = "Return"
        });
    }
}
