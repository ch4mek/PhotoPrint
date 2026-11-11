using Modsen.Portal.Core.Authentication.TokenContext;

namespace Modsen.Portal.Core.UnitTests;

public class PublicTokenMaskProviderTests
{
    [Theory]
    [InlineData(null, "[null]")]
    [InlineData("ab", "a****")]
    [InlineData("abc", "a****")]
    [InlineData("abcdef", "abc****")]
    [InlineData("abcdefghiJklmNopQrst_xyz", "abcdefgh****_xyz")]
    public void GetMaskedTokenShouldReturnMaskedStr(string? token, string expected)
    {
        // Arrange
        var provider = new PublicTokenMaskProvider();

        // Act
        var actual = provider.GetMaskedToken(token);

        // Assert
        actual.Should().Be(expected);
    }
}
