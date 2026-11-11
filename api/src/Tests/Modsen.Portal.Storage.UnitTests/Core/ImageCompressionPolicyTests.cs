using Modsen.Portal.Core;
using Modsen.Portal.Storage.Requests.Files.Core.ImagePolicies;

namespace Modsen.Portal.Storage.UnitTests.Core;

public class ImageCompressionPolicyTests
{
    [Theory]
    [InlineData(ApplicationFilePurpose.UserAvatar, "image/jpeg", true)]
    [InlineData(ApplicationFilePurpose.UserAvatar, "image/png", true)]
    [InlineData(ApplicationFilePurpose.UserAvatar, "application/pdf", false)]
    [InlineData(ApplicationFilePurpose.Merch, "image/png", true)]
    [InlineData(ApplicationFilePurpose.Merch, "application/pdf", false)]
    [InlineData(ApplicationFilePurpose.General, "image/jpeg", false)]
    [InlineData(ApplicationFilePurpose.General, "image/png", false)]
    public void ShouldCompress_ShouldReturnExpectedResult(
        ApplicationFilePurpose purpose,
        string contentType,
        bool expected)
    {
        // Act
        var result = ImageCompressionPolicy.ShouldCompress(purpose, contentType);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(ApplicationFileSize.Medium, "image/jpeg", true)]
    [InlineData(ApplicationFileSize.Small, "image/png", true)]
    [InlineData(ApplicationFileSize.Medium, "application/pdf", false)]
    [InlineData(ApplicationFileSize.Original, "image/jpeg", false)]
    [InlineData(ApplicationFileSize.Original, "image/png", false)]
    public void RequiresCompression_ReturnsExpectedResult(
        ApplicationFileSize size,
        string contentType,
        bool expected)
    {
        // Act
        var result = ImageCompressionPolicy.RequiresCompression(size, contentType);

        // Assert
        result.Should().Be(expected);
    }


    [Fact]
    public void GetMaxSizeForCompression_WhenMedium_ReturnsMediumSize()
    {
        // Arrange
        const ApplicationFileSize size = ApplicationFileSize.Medium;

        // Act
        var result = ImageCompressionPolicy.GetMaxSizeForCompression(size);

        // Assert
        result.Should().Be(ApplicationImageSizes.Medium);
    }


    [Fact]
    public void GetMaxSizeForCompression_WhenSmall_ReturnsSmallSize()
    {
        // Arrange
        const ApplicationFileSize size = ApplicationFileSize.Small;

        // Act
        var result = ImageCompressionPolicy.GetMaxSizeForCompression(size);

        // Assert
        result.Should().Be(ApplicationImageSizes.Small);
    }

    [Fact]
    public void GetMaxSizeForCompression_WhenOriginal_ThrowsException()
    {
        // Arrange
        const ApplicationFileSize size = ApplicationFileSize.Original;

        // Act
        var act = () => ImageCompressionPolicy.GetMaxSizeForCompression(size);

        // Assert
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void BuildResizedPath_AppendsSizeBeforeExtension()
    {
        // Arrange
        const string originalPath = "avatar.profile.png";

        // Act
        var result = ImageCompressionPolicy.BuildResizedPath(originalPath, ApplicationFileSize.Medium);

        // Assert
        result.Should().Be("avatar.profile_300.png");
    }

    [Fact]
    public void GetTargetSizes_WhenPurposeIsMerch_ReturnsOnlySmall()
    {
        // Act
        var sizes = ImageCompressionPolicy.GetTargetSizes(ApplicationFilePurpose.Merch);

        // Assert
        sizes.Should().BeEquivalentTo([ApplicationFileSize.Small]);
    }

    [Fact]
    public void GetTargetSizes_WhenPurposeIsGeneral_ReturnsEmpty()
    {
        // Act
        var sizes = ImageCompressionPolicy.GetTargetSizes(ApplicationFilePurpose.General);

        // Assert
        sizes.Should().BeEmpty();
    }

    [Fact]
    public void AllImageSizes_ShouldContainUnionWithoutDuplicates()
    {
        // Act
        var result = ApplicationImageSizes.AllImageSizes;

        // Assert
        var expected = ApplicationImageSizes.UserAvatarSizes
            .Concat(ApplicationImageSizes.MerchSizes)
            .Distinct()
            .OrderBy(x => x)
            .ToArray();

        result.Should().Equal(expected);
    }
}
