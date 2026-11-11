using MockQueryable.Moq;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Storage.DataAccess.Npgsql;
using Modsen.Portal.Storage.DataAccess.Npgsql.Entities;
using Modsen.Portal.Storage.Requests.Files.Core.CoreDto;
using Modsen.Portal.Storage.Requests.Files.Core.Loaders;
using Modsen.Portal.Storage.Requests.Files.Requests;
using Moq;

namespace Modsen.Portal.Storage.UnitTests.Files.Requests;

public class GetFileRequestTests
{
    private readonly Mock<ICloudStorageManager> _cloudStorageManagerMock = new();
    private readonly Mock<IStorageDbContext> _dbContextMock = new();
    private readonly Mock<IClockService> _clockServiceMock = new();
    private readonly Mock<IFileStreamLoader> _fileStreamLoaderMock = new();

    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly DateTime _utcNow = new(2026, 04, 23, 0, 0, 0, DateTimeKind.Utc);

    private readonly GetFileRequest _handler;

    public GetFileRequestTests()
    {
        _clockServiceMock.Setup(x => x.UtcNow).Returns(_utcNow);

        _handler = new GetFileRequest(
            _cloudStorageManagerMock.Object,
            _dbContextMock.Object,
            _clockServiceMock.Object,
            _fileStreamLoaderMock.Object);
    }

    [Fact]
    public async Task Handle_WhenResizedExists_ReturnsResizedAndNoFallback()
    {
        // Arrange
        var file = CreateEntity("f1", "avatar.png", "image/png");
        SetupFiles([file]);

        var expectedStream = new MemoryStream([1, 2, 3]);

        _cloudStorageManagerMock
            .Setup(x => x.FileExistsByMetadataAsync(file.VirtualDirectory, It.IsAny<string>(), _cancellationToken))
            .ReturnsAsync(true);

        _fileStreamLoaderMock
            .Setup(x => x.LoadAsync(It.Is<FileCoreDto>(f => f.FilePath.Contains($"_{ApplicationImageSizes.Small}")),
                _cancellationToken))
            .ReturnsAsync(expectedStream);

        // Act
        var result = await _handler.Handle(new GetFileRequest.Query(file.FileId, ApplicationFileSize.Small),
            _cancellationToken);

        // Assert
        result.ContentStream.Should().BeSameAs(expectedStream);
        result.IsFallbackToOriginal.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_WhenResizedDoesNotExist_ReturnsOriginalFallback()
    {
        // Arrange
        var file = CreateEntity("f2", "avatar.png", "image/png");
        SetupFiles([file]);

        var expectedOriginalStream = new MemoryStream([5, 6, 7]);

        _cloudStorageManagerMock
            .Setup(x => x.FileExistsByMetadataAsync(file.VirtualDirectory, It.IsAny<string>(), _cancellationToken))
            .ReturnsAsync(false);

        _fileStreamLoaderMock
            .Setup(x => x.LoadAsync(It.Is<FileCoreDto>(f => f.FilePath == file.FilePath), _cancellationToken))
            .ReturnsAsync(expectedOriginalStream);

        // Act
        var result = await _handler.Handle(new GetFileRequest.Query(file.FileId, ApplicationFileSize.Small),
            _cancellationToken);

        // Assert
        result.ContentStream.Should().BeSameAs(expectedOriginalStream);
        result.IsFallbackToOriginal.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WhenSizeIsOriginal_ReturnsOriginalWithoutFallback()
    {
        // Arrange
        var file = CreateEntity("f3", "avatar.png", "image/png");
        SetupFiles([file]);

        var expectedOriginalStream = new MemoryStream("\t\t\t"u8.ToArray());

        _fileStreamLoaderMock
            .Setup(x => x.LoadAsync(It.Is<FileCoreDto>(f => f.FilePath == file.FilePath), _cancellationToken))
            .ReturnsAsync(expectedOriginalStream);

        // Act
        var result = await _handler.Handle(new GetFileRequest.Query(file.FileId), _cancellationToken);

        // Assert
        result.ContentStream.Should().BeSameAs(expectedOriginalStream);
        result.IsFallbackToOriginal.Should().BeNull();
    }

    private void SetupFiles(List<ApplicationFileEntity> files)
    {
        _dbContextMock.Setup(x => x.ApplicationFiles)
            .Returns(files.BuildMockDbSet().Object);
    }

    private ApplicationFileEntity CreateEntity(string fileId, string filePath, string contentType)
    {
        return new ApplicationFileEntity
        {
            FileId = fileId,
            BucketName = "bucket",
            VirtualDirectory = "dir",
            FilePath = filePath,
            OriginalFileName = filePath,
            ContentType = contentType,
            UploadedByUserId = 1,
            AvailableTill = _utcNow.AddDays(1)
        };
    }
}
