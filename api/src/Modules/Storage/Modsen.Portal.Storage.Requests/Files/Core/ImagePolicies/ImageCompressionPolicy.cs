using System.Net.Mime;
using Modsen.Portal.Core;

namespace Modsen.Portal.Storage.Requests.Files.Core.ImagePolicies;

/// <summary>
/// Provides rules for image compression and resized image naming.
/// </summary>
internal static class ImageCompressionPolicy
{
    /// <summary>
    /// Determines whether the file should be compressed based on its purpose
    /// and content type.
    /// </summary>
    public static bool ShouldCompress(ApplicationFilePurpose purpose, string contentType)
    {
        return GetTargetSizes(purpose).Count > 0 &&
            IsResizableImage(contentType);
    }

    /// <summary>
    /// Returns target resize dimensions for a file purpose.
    /// Original file is uploaded separately and is not included here.
    /// </summary>
    public static IReadOnlyCollection<ApplicationFileSize> GetTargetSizes(ApplicationFilePurpose purpose)
    {
        return purpose switch
        {
            ApplicationFilePurpose.UserAvatar => ApplicationImageSizes.UserAvatarSizes,
            ApplicationFilePurpose.Merch => ApplicationImageSizes.MerchSizes,
            _ => []
        };
    }

    /// <summary>
    /// Checks whether the file content type supports image resizing.
    /// </summary>
    public static bool IsResizableImage(string contentType)
    {
        return SupportedImageContentTypes.Contains(contentType);
    }

    /// <summary>
    /// Determines whether compression should be applied
    /// based on requested file size and content type.
    /// </summary>
    public static bool RequiresCompression(
        ApplicationFileSize size,
        string contentType)
    {
        return size != ApplicationFileSize.Original &&
            contentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Returns maximum image dimension for the given file size.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown when called for <see cref="ApplicationFileSize.Original"/>.
    /// </exception>
    public static int GetMaxSizeForCompression(ApplicationFileSize size)
    {
        return size switch
        {
            ApplicationFileSize.Medium => ApplicationImageSizes.Medium,
            ApplicationFileSize.Small => ApplicationImageSizes.Small,
            ApplicationFileSize.SmallSquare => ApplicationImageSizes.SmallSquare,

            ApplicationFileSize.Original =>
                throw new InvalidOperationException(
                    "Original file size does not require compression."),

            _ => throw new ArgumentOutOfRangeException(nameof(size), size, "Unsupported file size")
        };
    }

    /// <summary>
    /// Builds file path for a resized image using size-based suffix.
    /// </summary>
    public static string BuildResizedPath(string originalPath, ApplicationFileSize maxSize)
    {
        var ext = Path.GetExtension(originalPath);
        var baseName = Path.GetFileNameWithoutExtension(originalPath);

        return maxSize switch
        {
            ApplicationFileSize.Medium =>
                $"{baseName}_{ApplicationImageSizes.Medium}{ext}",

            ApplicationFileSize.Small =>
                $"{baseName}_{ApplicationImageSizes.Small}{ext}",

            ApplicationFileSize.SmallSquare =>
                $"{baseName}_{ApplicationImageSizes.SmallSquare}_square{ext}",

            _ => originalPath
        };
    }

    private static readonly HashSet<string> SupportedImageContentTypes =
        new(StringComparer.OrdinalIgnoreCase) { MediaTypeNames.Image.Jpeg, MediaTypeNames.Image.Png };
}
