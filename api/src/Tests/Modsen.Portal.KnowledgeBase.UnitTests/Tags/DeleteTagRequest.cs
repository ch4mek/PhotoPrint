using MockQueryable.Moq;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.KnowledgeBase.DataAccess.Npgsql;
using Modsen.Portal.KnowledgeBase.DataAccess.Npgsql.Entities;
using Modsen.Portal.KnowledgeBase.Requests.Tags.Requests;
using Moq;

namespace Modsen.Portal.KnowledgeBase.UnitTests.Tags;

public class DeleteTagRequestTests
{
    private readonly Mock<IKnowledgeBaseDbContext> _dbContextMock = new();

    private readonly DeleteTagRequest _handler;

    public DeleteTagRequestTests()
    {
        _handler = new DeleteTagRequest(_dbContextMock.Object);
    }

    [Fact]
    public async Task HandleWhenRequestShouldDeleteTag()
    {
        // Arrange
        var articleSet = new List<ArticleTagEntity> { new() { Id = 1, TagId = 1, ArticleId = 1 } };
        _dbContextMock.Setup(db => db.ArticleTags)
            .Returns(articleSet.BuildMockDbSet().Object);

        _dbContextMock.Setup(db => db.DiscountTags)
            .Returns(Array.Empty<DiscountTagEntity>().BuildMockDbSet().Object);

        var tagSet = new List<TagEntity> { new() { Id = 1, Name = "Test", ArticleTags = articleSet } };
        _dbContextMock.Setup(db => db.Tags)
            .Returns(tagSet.BuildMockDbSet().Object);


        // Act
        var id = await _handler.Handle(new DeleteTagRequest.Command(1), It.IsAny<CancellationToken>());

        // Assert
        id.Should().Be(1);

        _dbContextMock.Verify(db => db.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _dbContextMock.Verify(db => db.Tags.Remove(It.IsAny<TagEntity>()), Times.Once);
    }

    [Fact]
    public async Task HandleWhenRequest404ShouldThrowNotFound()
    {
        // Arrange
        _dbContextMock.Setup(db => db.ArticleTags).Returns(Array.Empty<ArticleTagEntity>().BuildMockDbSet().Object);
        _dbContextMock.Setup(db => db.DiscountTags).Returns(Array.Empty<DiscountTagEntity>().BuildMockDbSet().Object);

        var tagSet = new List<TagEntity> { new() { Id = 1, Name = "Test" } };
        _dbContextMock.Setup(db => db.Tags)
            .Returns(tagSet.BuildMockDbSet().Object);
        _dbContextMock.Setup(db => db.Tags.Remove(It.IsAny<TagEntity>()))
            .Callback((TagEntity tag) => tagSet.Remove(tag));

        // Act
        _ = await _handler.Handle(new DeleteTagRequest.Command(1), It.IsAny<CancellationToken>());
        var test = () => _handler.Handle(new DeleteTagRequest.Command(1), It.IsAny<CancellationToken>());

        // Assert
        await test.Should().ThrowAsync<EntityNotFoundException>();
    }
}
