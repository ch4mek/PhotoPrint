using Microsoft.Net.Http.Headers;
using Modsen.Portal.Core.Utils.Extensions;

namespace Modsen.Portal.Storage.Controllers.Helpers;

public static class MultipartRequestHelper
{
    public static string GetBoundary(MediaTypeHeaderValue? contentType)
    {
        ArgumentNullException.ThrowIfNull(contentType);

        var boundary = HeaderUtilities.RemoveQuotes(contentType.Boundary).Value;
        if (boundary.IsNullOrWhiteSpace())
        {
            throw new InvalidDataException("Missing content-type boundary.");
        }
        return boundary;
    }

    public static bool IsMultipartContentType(string? contentType)
    {
        return !contentType.IsNullOrEmpty() &&
            contentType.Contains("multipart/", StringComparison.OrdinalIgnoreCase);
    }
}
