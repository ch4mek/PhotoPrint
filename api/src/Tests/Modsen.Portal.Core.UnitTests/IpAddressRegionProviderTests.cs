using Modsen.Portal.Core.Authentication.TokenContext;

namespace Modsen.Portal.Core.UnitTests;

public class IpAddressRegionProviderTests
{
    private readonly IIpAddressRegionProvider _provider = new IpAddressRegionProvider();

    [Theory]
    [InlineData("127.0.0.1", null, false)]
    [InlineData("127.0.0.1", new string[0], false)]
    [InlineData("127.0.0.1", new[] { "127.0.0.2"}, false)]
    [InlineData("127.0.0.1", new[] { "127.0.0.2", "127.0.0.3" }, false)]
    [InlineData("127.0.0.1", new[] { "127.0.0.1", "127.0.0.3" }, true)]
    [InlineData("127.0.0.1", new[] { "127.0.0.1" }, true)]
    public void IsIssuedForCases(string ipAddress, string[]? candidates, bool expected)
    {
        // Arrange-Act
        var actual = _provider.IsIssuedFor(ipAddress, candidates);

        // Assert
        actual.Should().Be(expected);
    }

    [Theory]
    [InlineData("127.0.0.1", new [] { "127.0.0.1" })]
    [InlineData("TEST", new [] { "TEST" })]
    [InlineData("217.21.54.219", new [] { "217.21.54.219", "178.124.193.89" })]
    [InlineData("178.124.193.89", new [] { "217.21.54.219", "178.124.193.89" })]
    [InlineData("91.149.184.73", new [] { "91.149.184.73", "178.124.220.229" })]
    [InlineData("178.124.220.229", new [] { "91.149.184.73", "178.124.220.229" })]
    public void GetAddressesCases(string ipAddress, string[]? addresses)
    {
        // Arrange-Act
        var actual = _provider.GetAddresses(ipAddress);

        // Assert
        actual.Should().BeEquivalentTo(addresses);
    }
}
