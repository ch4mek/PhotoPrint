using Modsen.Portal.Core.Utils.Extensions;

namespace Modsen.Portal.Core.UnitTests;

public class StringExtensionsUnitTests
{
    [Theory]
    [InlineData(null, "")]
    [InlineData("", "")]
    [InlineData("f", "F")]
    [InlineData("F", "F")]
    [InlineData("fa", "Fa")]
    [InlineData("FA", "FA")]
    public void WithFirstCharToUpperShouldReturnCorrectValue(string? input, string expected)
    {
        // act
        var actual = input.WithFirstCharToUpper();

        // assert
        actual.Should().Be(expected);
    }
}
