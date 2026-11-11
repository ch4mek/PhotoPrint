using System.Net.Mime;
using MassTransit;
using MockQueryable.Moq;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Storage.Contract.Events;
using Modsen.Portal.Storage.DataAccess.Npgsql;
using Modsen.Portal.Storage.DataAccess.Npgsql.Entities;
using Modsen.Portal.Storage.Requests.Consumers;
using Modsen.Portal.Storage.Requests.Jobs;
using Moq;

namespace Modsen.Portal.Storage.UnitTests.Consumers;

public class ApplicationFileUploadedEventConsumerTests
{
    private readonly Mock<IStorageDbContext> _dbContextMock = new();
    private readonly Mock<IApplicationFileVariantsResizeJobDispatcher> _resizeJobSchedulerMock = new();

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    private readonly ApplicationFileUploadedNotificationEventConsumer _consumer;

    public ApplicationFileUploadedEventConsumerTests()
    {
        _consumer = new ApplicationFileUploadedNotificationEventConsumer(
            _dbContextMock.Object, _resizeJobSchedulerMock.Object);
    }

    [Fact]
    public async Task Consume_WhenFileNotFound_ShouldNotScheduleJob()
    {
        // Arrange
        SetupFiles([]);
        var message = CreateMessage("missing", ApplicationFilePurpose.Merch, MediaTypeNames.Image.Jpeg);
        var context = CreateConsumeContext(message);

        // Act
        var act = () => _consumer.Consume(context.Object);

        // Assert
        await act.Should().ThrowAsync<EntityNotFoundException>();
        _resizeJobSchedulerMock.Verify(
            x => x.Dispatch(It.IsAny<ApplicationFileUploadedEvent>()),
            Times.Never);
    }

    [Fact]
    public async Task Consume_WhenFileIsNotResizable_ShouldNotScheduleJob()
    {
        // Arrange
        var file = CreateEntity("f1", "f1.pdf", MediaTypeNames.Application.Pdf);
        SetupFiles([file]);

        var message = CreateMessage(file.FileId, ApplicationFilePurpose.Merch, file.ContentType);
        var context = CreateConsumeContext(message);

        // Act
        await _consumer.Consume(context.Object);

        // Assert
        _resizeJobSchedulerMock.Verify(
            x => x.Dispatch(It.IsAny<ApplicationFileUploadedEvent>()),
            Times.Never);
    }

    [Fact]
    public async Task Consume_WhenPurposeHasNoTargetSizes_ShouldNotScheduleJob()
    {
        // Arrange
        var file = CreateEntity("f2", "f2.jpg", MediaTypeNames.Image.Jpeg);
        SetupFiles([file]);

        var message = CreateMessage(file.FileId, ApplicationFilePurpose.General, file.ContentType);
        var context = CreateConsumeContext(message);

        // Act
        await _consumer.Consume(context.Object);

        // Assert
        _resizeJobSchedulerMock.Verify(
            x => x.Dispatch(It.IsAny<ApplicationFileUploadedEvent>()),
            Times.Never);
    }

    [Fact]
    public async Task Consume_WhenImageAndPurposeSupported_ShouldScheduleJob()
    {
        // Arrange
        var file = CreateEntity("f3", "f3.jpg", MediaTypeNames.Image.Jpeg);
        SetupFiles([file]);

        var message = CreateMessage(file.FileId, ApplicationFilePurpose.Merch, file.ContentType);
        var context = CreateConsumeContext(message);

        // Act
        await _consumer.Consume(context.Object);

        // Assert
        _resizeJobSchedulerMock.Verify(
            x => x.Dispatch(
                It.Is<ApplicationFileUploadedEvent>(e =>
                    e.FileId == message.FileId &&
                    e.FilePurpose == message.FilePurpose &&
                    e.ContentType == message.ContentType)),
            Times.Once);
    }

    private void SetupFiles(List<ApplicationFileEntity> files)
    {
        _dbContextMock.Setup(x => x.ApplicationFiles)
            .Returns(files.BuildMockDbSet().Object);
    }

    private static ApplicationFileEntity CreateEntity(string fileId, string filePath, string contentType)
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
            AvailableTill = DateTime.UtcNow.AddDays(1)
        };
    }

    private static ApplicationFileUploadedEvent CreateMessage(
        string fileId,
        ApplicationFilePurpose purpose,
        string contentType)
    {
        return new ApplicationFileUploadedEvent
        {
            EventId = Guid.CreateVersion7().ToString(), FileId = fileId, FilePurpose = purpose, ContentType = contentType
        };
    }

    private Mock<ConsumeContext<ApplicationFileUploadedEvent>> CreateConsumeContext(
        ApplicationFileUploadedEvent message)
    {
        var consumeContext = new Mock<ConsumeContext<ApplicationFileUploadedEvent>>();

        consumeContext.SetupGet(x => x.Message).Returns(message);
        consumeContext.SetupGet(x => x.CancellationToken).Returns(_cancellationToken);

        return consumeContext;
    }
}
