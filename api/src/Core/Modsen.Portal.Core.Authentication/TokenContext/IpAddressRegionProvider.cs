using Modsen.Portal.Core.Utils.Extensions;

namespace Modsen.Portal.Core.Authentication.TokenContext;

public interface IIpAddressRegionProvider
{
    IReadOnlyCollection<string> GetAddresses(string ipAddress);
    bool IsIssuedFor(string ipAddress, IReadOnlyCollection<string>? candidates);
}

internal sealed class IpAddressRegionProvider : IIpAddressRegionProvider
{
    private static readonly string[] _firstRegion = ["217.21.54.219", "178.124.193.89"];
    private static readonly string[] _secondRegion = ["91.149.184.73", "178.124.220.229"];

    public IReadOnlyCollection<string> GetAddresses(string ipAddress)
    {
        if (IsIssuedFor(ipAddress, _firstRegion))
        {
            return _firstRegion;
        }

        return IsIssuedFor(ipAddress, _secondRegion)
            ? _secondRegion
            : [ipAddress];
    }

    public bool IsIssuedFor(string ipAddress, IReadOnlyCollection<string>? candidates)
    {
        return candidates != null &&
            candidates.Any(address =>
                !address.IsNullOrEmpty() &&
                address.Contains(ipAddress, StringComparison.InvariantCultureIgnoreCase));
    }
}
