using Modsen.Portal.Core.Services;
using Modsen.Portal.Storage.DataAccess.Npgsql.Entities;
using Modsen.Portal.Storage.Requests.Files.Core.CoreDto;

namespace Modsen.Portal.Storage.Requests.Files.Core.Loaders;

internal interface IFileStreamLoader
{
    Task<Stream> LoadAsync(ApplicationFileEntity applicationFile, CancellationToken cancellationToken);
    Task<Stream> LoadAsync(FileCoreDto file, CancellationToken cancellationToken);
}

internal class FileStreamLoader : IFileStreamLoader
{
    private readonly ICloudStorageManager _cloudStorageManager;
    private readonly IHttpClientFactory _httpClientFactory;

    public FileStreamLoader(ICloudStorageManager cloudStorageManager, IHttpClientFactory httpClientFactory)
    {
        _cloudStorageManager = cloudStorageManager;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<Stream> LoadAsync(ApplicationFileEntity applicationFile, CancellationToken cancellationToken)
    {
        var url = await _cloudStorageManager.GenerateUrl(applicationFile.VirtualDirectory, applicationFile.FilePath,
           cancellationToken);

        using var httpClient = _httpClientFactory.CreateClient();
        return await httpClient.GetStreamAsync(new Uri(url), cancellationToken);
    }

    public async Task<Stream> LoadAsync(FileCoreDto file, CancellationToken cancellationToken)
    {
        var url = await _cloudStorageManager.GenerateUrl(file.VirtualDirectory, file.FilePath,
           cancellationToken);

        using var httpClient = _httpClientFactory.CreateClient();
        return await httpClient.GetStreamAsync(new Uri(url), cancellationToken);
    }
}
