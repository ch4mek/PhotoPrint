namespace Modsen.Portal.Core.Services;

public interface IImageScaler
{
    IReadOnlyList<Stream> ScaleWithoutUpscale(
        Stream inputStream,
        string contentType,
        IDictionary<ApplicationFileSize, int> maxSizes);
}
