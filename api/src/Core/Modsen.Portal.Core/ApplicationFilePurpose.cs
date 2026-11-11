namespace Modsen.Portal.Core;

/// <summary>
/// Business purpose of the uploaded file.
/// </summary>
public enum ApplicationFilePurpose
{
    /// <summary>General file without special processing.</summary>
    General = 0,

    /// <summary>User avatar image.</summary>
    UserAvatar = 1,

    /// <summary>Merch image.</summary>
    Merch = 2
}
