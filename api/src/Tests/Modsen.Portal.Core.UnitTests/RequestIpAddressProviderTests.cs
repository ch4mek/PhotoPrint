using System.Net;
using Modsen.Portal.Core.Authentication.TokenContext;
using Modsen.Portal.Core.Context;
using Moq;

namespace Modsen.Portal.Core.UnitTests;

public class RequestIpAddressProviderTests
{
    private readonly IRequestIpAddressProvider _provider;
    private readonly Mock<IRequestExecutionContext> _executionContext = new();
    private readonly Mock<IHostEnvironmentContext> _environmentContext = new();
    
    public RequestIpAddressProviderTests()
    {
        _provider = new RequestIpAddressProvider(_executionContext.Object, _environmentContext.Object);
    }

    [Fact]
    public void GetAddressShouldReturnRealIpHeaderValueIfHeaderProvided()
    {
        // Arrange
        _executionContext.Setup(x => x.GetRealIpHeader())
            .Returns("REAL_IP_HEADER_VALUE");

        // Act
        var actual = _provider.GetAddress();

        // Assert
        actual.Should().Be("REAL_IP_HEADER_VALUE");
    }

    [Fact]
    public void GetAddressShouldReturnForwardedForHeaderValueIfHeaderProvidedForProductionEnv()
    {
        // Arrange
        _executionContext.Setup(x => x.GetRealIpHeader())
            .Returns(default(string?));

        _executionContext.Setup(x => x.GetForwardedForHeader())
            .Returns("FORWARDED_FOR_HEADER_VALUE");

        _environmentContext.Setup(x => x.IsProductionEnvironment())
            .Returns(true);

        // Act
        var actual = _provider.GetAddress();

        // Assert
        actual.Should().Be("FORWARDED_FOR_HEADER_VALUE");
    }

    [Fact]
    public void GetAddressShouldReturnForwardedForHeaderValueIfHeaderProvidedForStagingEnv()
    {
        // Arrange
        _executionContext.Setup(x => x.GetRealIpHeader())
            .Returns(default(string?));

        _executionContext.Setup(x => x.GetForwardedForHeader())
            .Returns("FORWARDED_FOR_HEADER_VALUE");

        _environmentContext.Setup(x => x.IsProductionEnvironment())
            .Returns(false);

        _environmentContext.Setup(x => x.IsStagingEnvironment())
            .Returns(true);

        // Act
        var actual = _provider.GetAddress();

        // Assert
        actual.Should().Be("FORWARDED_FOR_HEADER_VALUE");
    }

    [Fact]
    public void GetAddressShouldReturnRemoteIpIfHeadersAreNotProvidedForStagingEnv()
    {
        // Arrange
        _executionContext.Setup(x => x.GetRealIpHeader())
            .Returns(default(string?));

        _executionContext.Setup(x => x.GetForwardedForHeader())
            .Returns(default(string?));

        _executionContext.Setup(x => x.RemoteIpAddress())
            .Returns(IPAddress.Parse("127.127.127.127"));

        _environmentContext.Setup(x => x.IsProductionEnvironment())
            .Returns(false);

        _environmentContext.Setup(x => x.IsStagingEnvironment())
            .Returns(true);

        // Act
        var actual = _provider.GetAddress();

        // Assert
        actual.Should().Be("127.127.127.127");
    }

    [Fact]
    public void GetAddressShouldReturnRemoteIpIfHeadersAreNotProvidedForProductionEnv()
    {
        // Arrange
        _executionContext.Setup(x => x.GetRealIpHeader())
            .Returns(default(string?));

        _executionContext.Setup(x => x.GetForwardedForHeader())
            .Returns(default(string?));

        _executionContext.Setup(x => x.RemoteIpAddress())
            .Returns(IPAddress.Parse("127.127.127.127"));

        _environmentContext.Setup(x => x.IsProductionEnvironment())
            .Returns(true);

        _environmentContext.Setup(x => x.IsStagingEnvironment())
            .Returns(false);

        // Act
        var actual = _provider.GetAddress();

        // Assert
        actual.Should().Be("127.127.127.127");
    }


    [Fact]
    public void GetAddressShouldReturnLocalhostIfHeadersAndRemoteIpAreNotProvided()
    {
        // Arrange
        _executionContext.Setup(x => x.GetRealIpHeader())
            .Returns(default(string?));

        _executionContext.Setup(x => x.GetForwardedForHeader())
            .Returns(default(string?));

        _executionContext.Setup(x => x.RemoteIpAddress())
            .Returns(default(IPAddress));

        _environmentContext.Setup(x => x.IsProductionEnvironment())
            .Returns(true);

        // Act
        var actual = _provider.GetAddress();

        // Assert
        actual.Should().Be("::");
    }
}
