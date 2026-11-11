namespace Modsen.Portal.Core;

/// <summary>
/// Defines commonly used image dimensions for resized images.
/// </summary>
public static class ApplicationImageSizes
{
    /// <summary>
    /// Medium image dimension (in pixels).
    /// </summary>
    public const int Medium = 300;

    /// <summary>
    /// Small image dimension (in pixels).
    /// </summary>
    public const int Small = 150;

    /// <summary>
    /// Small squared image dimension (in pixels).
    /// </summary>
    public const int SmallSquare = 150;

    /// <summary>
    /// Image sizes used for user avatar generation.
    /// </summary>
    public static IReadOnlyList<ApplicationFileSize> UserAvatarSizes =>
        [ApplicationFileSize.Medium, ApplicationFileSize.Small, ApplicationFileSize.SmallSquare];

    /// <summary>
    /// Image sizes used for merch generation.
    /// </summary>
    public static IReadOnlyList<ApplicationFileSize> MerchSizes =>
        [ApplicationFileSize.Small];

    /// <summary>
    /// All possible resized image dimensions that may exist in storage.
    /// Used by infrastructure components (e.g. delete).
    /// </summary>
    public static IReadOnlyList<ApplicationFileSize> AllImageSizes { get; } = UserAvatarSizes
        .Concat(MerchSizes)
        .Distinct()
        .OrderBy(x => x)
        .ToArray();
}
