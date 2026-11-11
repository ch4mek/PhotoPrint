using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Utils.Extensions;

namespace Modsen.Portal.Core.Authentication.TokenContext;

public interface IRequestIpAddressProvider
{
    string GetAddress();
}

internal sealed class RequestIpAddressProvider : IRequestIpAddressProvider
{
    private readonly IRequestExecutionContext _requestContext;
    private readonly IHostEnvironmentContext _hostEnvironmentContext;

    public RequestIpAddressProvider(
        IRequestExecutionContext requestContext,
        IHostEnvironmentContext hostEnvironmentContext)
    {
        _requestContext = requestContext;
        _hostEnvironmentContext = hostEnvironmentContext;
    }

    public string GetAddress()
    {
        var address = _requestContext.GetRealIpHeader();
        if (!address.IsNullOrEmpty())
        {
            return address;
        }

        if (_hostEnvironmentContext.IsProductionEnvironment() ||
            _hostEnvironmentContext.IsStagingEnvironment())
        {
            address = _requestContext.GetForwardedForHeader();
        }

        if (address.IsNullOrEmpty())
        {
            address = _requestContext.RemoteIpAddress()?.ToString();
        }

        return address ?? "::";
    }
}
