using System.Net.Mime;
using Microsoft.IO;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Services;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;

namespace Modsen.Portal.CloudStorage.AwsS3;

internal sealed class ImageSharpScaler : IImageScaler
{
    private static readonly RecyclableMemoryStreamManager Streams = new();

    public IReadOnlyList<Stream> ScaleWithoutUpscale(
        Stream inputStream,
        string contentType,
        IDictionary<ApplicationFileSize, int> maxSizes)
    {
        if (maxSizes.Count == 0)
        {
            return [];
        }

        Image image;
        try
        {
            image = Image.Load(inputStream);
        }
        catch (UnknownImageFormatException)
        {
            return CopyOriginalForEachSize(inputStream, maxSizes.Count);
        }

        var results = new List<Stream>(maxSizes.Count);

        try
        {
            using (image)
            {
                var encoder = ResolveEncoder(image.Metadata.DecodedImageFormat, contentType);

                foreach (var (fileSize, maxSize) in maxSizes)
                {
                    using var resizedImage = image.Clone(x => { });

                    if (fileSize == ApplicationFileSize.SmallSquare)
                    {
                        CropToSquare(resizedImage);
                    }

                    var ratio = Math.Min(1.0, Math.Min(
                        (double)maxSize / resizedImage.Width,
                        (double)maxSize / resizedImage.Height));

                    if (ratio < 1.0)
                    {
                        resizedImage.Mutate(ctx => ctx.Resize(
                            Math.Max(1, (int)(resizedImage.Width * ratio)),
                            Math.Max(1, (int)(resizedImage.Height * ratio))));
                    }

                    var output = Streams.GetStream();

                    resizedImage.Save(output, encoder);

                    output.Position = 0;
                    results.Add(output);
                }
            }

            return results;
        }
        catch
        {
            foreach (var s in results)
            {
                s?.Dispose();
            }

            throw;
        }
    }

    private static List<Stream> CopyOriginalForEachSize(Stream input, int count)
    {
        if (input.CanSeek)
        {
            input.Position = 0;
        }

        using var buffer = Streams.GetStream();
        input.CopyTo(buffer);
        var bytes = buffer.GetBuffer();
        var length = (int)buffer.Length;

        var copies = new List<Stream>(count);
        for (var i = 0; i < count; i++)
        {
            var s = Streams.GetStream();
            s.Write(bytes, 0, length);
            s.Position = 0;
            copies.Add(s);
        }

        return copies;
    }

    private static void CropToSquare(Image image)
    {
        var size = Math.Min(image.Width, image.Height);

        var x = (image.Width - size) / 2;
        var y = (image.Height - size) / 2;

        image.Mutate(ctx => ctx.Crop(new Rectangle(x, y, size, size)));
    }

    private static IImageEncoder ResolveEncoder(IImageFormat? sourceFormat, string contentType)
    {
        return sourceFormat switch
        {
            JpegFormat => new JpegEncoder { Quality = 85 },
            PngFormat => new PngEncoder(),
            _ => contentType switch
            {
                MediaTypeNames.Image.Jpeg => new JpegEncoder { Quality = 85 },
                MediaTypeNames.Image.Png => new PngEncoder(),
                _ => new JpegEncoder { Quality = 85 }
            }
        };
    }
}
