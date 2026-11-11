using MockQueryable.Moq;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Storage.DataAccess.Npgsql;
using Modsen.Portal.Storage.DataAccess.Npgsql.Entities;
using Modsen.Portal.Storage.Requests.Files.Requests;
using Moq;

namespace Modsen.Portal.Storage.UnitTests.Files.Requests;

public class DeleteFileRequestTests
{
    private readonly Mock<ICloudStorageManager> _cloudStorageManagerMock = new();
    private readonly Mock<IStorageDbContext> _dbContextMock = new();
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly DeleteFileRequest _handler;

    public DeleteFileRequestTests()
    {
        _handler = new DeleteFileRequest(_cloudStorageManagerMock.Object, _dbContextMock.Object);
    }

    [Fact]
    public async Task HandleWhenFileDoesNotExistShouldThrowNotFoundException()
    {
        // Arrange
        var file = CreateEntity("1", FileSource.User);
        SetupFiles([file]);

        // Act
        var act = () => _handler.Handle(new DeleteFileRequest.Command("2", FileSource.User),
            _cancellationToken);

        // Assert
        await act.Should().ThrowAsync<EntityNotFoundException>();
    }

    [Fact]
    public async Task HandleWhenFileExistsButHasNotAllowedFileSourceShouldThrowActionNotAllowedException()
    {
        // Arrange
        var file = CreateEntity("1", FileSource.User);
        SetupFiles([file]);

        // Act
        var act = () => _handler.Handle(new DeleteFileRequest.Command("1", FileSource.Recruitment),
            _cancellationToken);

        // Assert
        await act.Should().ThrowAsync<ActionNotAllowedException>();
    }

    [Fact]
    public async Task HandleWhenFileExistsAndHasAllowedFileSourceShouldNotBeErrorThrown()
    {
        // Arrange
        var file = CreateEntity("1", FileSource.Recruitment);
        SetupFiles([file]);

        // Act
        var act = () => _handler.Handle(new DeleteFileRequest.Command("1", FileSource.Recruitment),
            _cancellationToken);

        // Assert
        await act.Should().NotThrowAsync();
    }

    private void SetupFiles(List<ApplicationFileEntity> files)
    {
        _dbContextMock.Setup(x => x.ApplicationFiles)
            .Returns(files.BuildMockDbSet().Object);
    }

    private ApplicationFileEntity CreateEntity(string fileId, FileSource fileSource)
    {
        return new ApplicationFileEntity
        {
            FileId = fileId,
            BucketName = "bucket",
            VirtualDirectory = "dir",
            FilePath = "avatar.png",
            OriginalFileName = "avatar.png",
            ContentType = "image/png",
            UploadedByUserId = 1,
            AvailableTill = new(2026, 04, 23, 0, 0, 0, DateTimeKind.Utc),
            FileSource = fileSource
        };
    }
}
