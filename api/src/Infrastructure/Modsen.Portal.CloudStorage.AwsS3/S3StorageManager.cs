using System.Net;
using System.Net.Mime;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;
using Microsoft.IO;
using Modsen.Portal.Core.Services;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;

namespace Modsen.Portal.CloudStorage.AwsS3;

internal sealed class S3StorageManager : ICloudStorageManager
{
    private readonly IAmazonS3 _s3Client;
    private readonly S3Options _options;
    private readonly TimeSpan _expirationDays = TimeSpan.FromDays(5);

    private static readonly RecyclableMemoryStreamManager _recyclableMemoryManager = new();

    public S3StorageManager(IAmazonS3 s3Client, IOptions<S3Options> options)
    {
        _s3Client = s3Client;
        _options = options.Value ?? throw new ArgumentNullException(nameof(options));
    }

    public Task<string> GenerateUrl(string virtualDirectory, string fileKey, CancellationToken cancellationToken)
    {
        return _s3Client.GetPreSignedURLAsync(new GetPreSignedUrlRequest
        {
            BucketName = _options.BucketName,
            Key = $"{virtualDirectory}/{fileKey}",
            Verb = HttpVerb.GET,
            Expires = DateTime.UtcNow.Add(_expirationDays)
        });
    }

    public async Task<string> StoreFileAsync(string virtualDirectory, string fileKey, string? contentType,
        byte[] content,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(content);

        await using (var stream = new MemoryStream(content))
        {
            var putRequest = new PutObjectRequest
            {
                BucketName = _options.BucketName,
                Key = $"{virtualDirectory}/{fileKey}",
                InputStream = stream,
                ContentType = contentType ?? MediaTypeNames.Application.Octet,
                ServerSideEncryptionMethod = ServerSideEncryptionMethod.AES256
            };

            await _s3Client.PutObjectAsync(putRequest, cancellationToken);
        }

        return await GenerateUrl(virtualDirectory, fileKey, cancellationToken);
    }

    public async Task<string> StoreFileAsync(string virtualDirectory, string fileKey, string? contentType,
        Stream streamContent,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(streamContent);

        await using var tempStream = _recyclableMemoryManager.GetStream();
        await streamContent.CopyToAsync(tempStream, cancellationToken);
        tempStream.Position = 0;
        streamContent.Position = 0;

        var putRequest = new PutObjectRequest
        {
            BucketName = _options.BucketName,
            Key = $"{virtualDirectory}/{fileKey}",
            InputStream = tempStream,
            ContentType = contentType ?? MediaTypeNames.Application.Octet,
            ServerSideEncryptionMethod = ServerSideEncryptionMethod.AES256,
        };

        await _s3Client.PutObjectAsync(putRequest, cancellationToken);

        return await GenerateUrl(virtualDirectory, fileKey, cancellationToken);
    }

    public async Task<bool> FileExistsByListingAsync(string virtualDirectory, string fileKey,
        CancellationToken cancellationToken)
    {
        var response = await _s3Client.ListObjectsAsync(
            new ListObjectsRequest
            {
                BucketName = _options.BucketName, Prefix = $"{virtualDirectory}/{fileKey}", MaxKeys = 1
            }, cancellationToken);

        return response.S3Objects.Count > 0;
    }

    public async Task<bool> FileExistsByMetadataAsync(string virtualDirectory, string fileKey,
        CancellationToken cancellationToken)
    {
        try
        {
            await _s3Client.GetObjectMetadataAsync(
                _options.BucketName,
                $"{virtualDirectory}/{fileKey}",
                cancellationToken);

            return true;
        }
        catch (AmazonS3Exception ex)
            when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return false;
        }
    }

    public async Task<Stream> GetFileContentAsync(string virtualDirectory, string fileKey,
        CancellationToken cancellationToken)
    {
        var ms = new MemoryStream();

        var objectResponse = await _s3Client.GetObjectAsync(
            bucketName: _options.BucketName,
            key: $"{virtualDirectory}/{fileKey}",
            cancellationToken);

        await objectResponse.ResponseStream.CopyToAsync(ms, cancellationToken);

        ms.Position = 0;

        return ms;
    }

    public Task RemoveFileAsync(string virtualDirectory, string fileKey, CancellationToken cancellationToken)
    {
        return _s3Client.DeleteObjectAsync(
            new DeleteObjectRequest { BucketName = _options.BucketName, Key = $"{virtualDirectory}/{fileKey}", },
            cancellationToken);
    }

    public string GetBucketName()
    {
        ArgumentException.ThrowIfNullOrEmpty(_options.BucketName);
        return _options.BucketName;
    }

    public string GetVirtualDirectory()
    {
        ArgumentException.ThrowIfNullOrEmpty(_options.DefaultVirtualDirectory);
        return _options.DefaultVirtualDirectory;
    }

    public Stream ScaleImage(Stream inputStream, int maxWidth, int maxHeight)
    {
        try
        {
            return ScaleImageInternal(
                inputStream,
                maxWidth,
                maxHeight,
                allowUpscale: true);
        }
        catch (UnknownImageFormatException)
        {
            return CopyStream(inputStream);
        }
    }

    public Stream ScaleImageWithoutUpscale(Stream inputStream, int maxWidth, int maxHeight, string? contentType = null)
    {
        try
        {
            return ScaleImageInternal(
                inputStream,
                maxWidth,
                maxHeight,
                allowUpscale: false,
                contentType: contentType);
        }
        catch (UnknownImageFormatException)
        {
            return CopyStream(inputStream);
        }
    }

    public async Task CopyFileAsync(string sourceVirtualDirectory, string sourceFileKey, string destinationVirtualDirectory,
        string destinationFileKey, CancellationToken cancellationToken)
    {
        var getResponse = await _s3Client.GetObjectAsync(
            bucketName: _options.BucketName,
            key: $"{sourceVirtualDirectory}/{sourceFileKey}",
            cancellationToken);

        await using var tempStream = _recyclableMemoryManager.GetStream();
        await getResponse.ResponseStream.CopyToAsync(tempStream, cancellationToken);
        tempStream.Position = 0;

        var putRequest = new PutObjectRequest
        {
            BucketName = _options.BucketName,
            Key = $"{destinationVirtualDirectory}/{destinationFileKey}",
            InputStream = tempStream,
            ContentType = getResponse.Headers.ContentType,
            ServerSideEncryptionMethod = ServerSideEncryptionMethod.AES256,
        };

        await _s3Client.PutObjectAsync(putRequest, cancellationToken);
    }

    public Task MarkFileAsDeprecatedAsync(string virtualDirectory, string fileKey, CancellationToken cancellationToken)
    {
        return _s3Client.PutObjectTaggingAsync(new PutObjectTaggingRequest
        {
            BucketName = _options.BucketName,
            Key = $"{virtualDirectory}/{fileKey}",
            Tagging = new Tagging
            {
                TagSet =
                [
                    new Tag { Key = "legacy", Value = "true" },
                    new Tag { Key = "status", Value = "deprecated" },
                ]
            }
        }, cancellationToken);
    }

    public async Task<IReadOnlyCollection<string>> GetAllFileNamesAsync(string virtualDirectory, CancellationToken cancellationToken)
    {
        var fileNames = new List<string>();
        string? continuationToken = null;

        do
        {
            var response = await _s3Client.ListObjectsV2Async(
                new ListObjectsV2Request
                {
                    BucketName = _options.BucketName,
                    Prefix = $"{virtualDirectory}/",
                    ContinuationToken = continuationToken,
                },
                cancellationToken);

            fileNames.AddRange(response.S3Objects.Select(x => Path.GetFileName(x.Key)));

            continuationToken = response.NextContinuationToken;

        } while (continuationToken is not null);

        return fileNames;
    }

    private static RecyclableMemoryStream ScaleImageInternal(
        Stream inputStream,
        int maxWidth,
        int maxHeight,
        bool allowUpscale,
        string? contentType = null)
    {
        using var image = Image.Load(inputStream);

        var encoder = contentType switch
        {
            MediaTypeNames.Image.Jpeg => (IImageEncoder)new JpegEncoder { Quality = 85 },
            MediaTypeNames.Image.Png  => new PngEncoder(),
            _                         => new JpegEncoder { Quality = 85 }
        };

        var ratioX = (double)maxWidth / image.Width;
        var ratioY = (double)maxHeight / image.Height;

        var ratio = Math.Min(ratioX, ratioY);

        if (!allowUpscale)
        {
            ratio = Math.Min(1.0, ratio);
        }

        var newWidth = (int)(image.Width * ratio);
        var newHeight = (int)(image.Height * ratio);

        if (allowUpscale || ratio < 1)
        {
            image.Mutate(x => x.Resize(newWidth, newHeight));
        }

        var outputStream = _recyclableMemoryManager.GetStream();
        image.Save(outputStream, encoder);
        outputStream.Position = 0;

        return outputStream;
    }


    private static RecyclableMemoryStream CopyStream(Stream inputStream)
    {
        var copyStream = _recyclableMemoryManager.GetStream();
        inputStream.CopyTo(copyStream);
        copyStream.Position = 0;
        return copyStream;
    }
}
