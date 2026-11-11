using MockQueryable.Moq;
using Modsen.Portal.KnowledgeBase.DataAccess.Npgsql;
using Modsen.Portal.KnowledgeBase.DataAccess.Npgsql.Entities;
using Modsen.Portal.KnowledgeBase.Requests.Tags.Requests;
using Moq;

namespace Modsen.Portal.KnowledgeBase.UnitTests.Tags;

public class CreateTagRequestTests
{
    private readonly Mock<IKnowledgeBaseDbContext> _dbContextMock = new();

    private readonly CreateTagRequest _handler;

    public CreateTagRequestTests()
    {
        _handler = new CreateTagRequest(_dbContextMock.Object);
    }

    [Fact]
    public async Task HandleWhenRequestShouldCreateTag()
    {
        // Arrange
        _dbContextMock.Setup(db => db.Tags)
            .Returns(Array.Empty<TagEntity>().BuildMockDbSet().Object);

        // Act
        var tag = await _handler.Handle(new CreateTagRequest.Command("Test"), It.IsAny<CancellationToken>());

        // Assert
        tag.Name.Should().Be("Test");

        _dbContextMock.Verify(db => db.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _dbContextMock.Verify(db => db.Tags.Add(It.IsAny<TagEntity>()), Times.Once);
    }
}
