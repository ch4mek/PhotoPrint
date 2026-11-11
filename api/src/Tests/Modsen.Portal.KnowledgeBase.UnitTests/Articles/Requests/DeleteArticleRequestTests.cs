using MockQueryable.Moq;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.KnowledgeBase.DataAccess.Npgsql;
using Modsen.Portal.KnowledgeBase.DataAccess.Npgsql.Entities;
using Modsen.Portal.KnowledgeBase.Requests.Articles.Requests;
using Moq;

namespace Modsen.Portal.KnowledgeBase.UnitTests.Articles.Requests;

public class DeleteArticleRequestTests
{
    private readonly Mock<IKnowledgeBaseDbContext> _dbContext = new();
    private readonly DeleteArticleRequest _handler;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public DeleteArticleRequestTests()
    {
        _handler = new DeleteArticleRequest(_dbContext.Object);
    }

    [Fact]
    public async Task HandleWhenArticleExistsShouldDeleteSuccessfully()
    {
        // Arrange
        var articleId = 1;
        var article = CreateArticleEntity(articleId);
        MockDbContext([article], article.ArticleTags, article.ArticleReferences);

        var command = new DeleteArticleRequest.Command(articleId);

        // Act
        var result = await _handler.Handle(command, _cancellationToken);

        // Assert
        result.Should().Be(articleId);
        _dbContext.Verify(x => x.ArticleTags.RemoveRange(article.ArticleTags), Times.Once);
        _dbContext.Verify(x => x.ArticleReferences.RemoveRange(article.ArticleReferences), Times.Once);
        _dbContext.Verify(x => x.Articles.Remove(article), Times.Once);
        _dbContext.Verify(x => x.SaveChangesAsync(_cancellationToken), Times.Once);
    }

    [Fact]
    public async Task HandleWhenArticleDoesNotExistShouldThrowEntityNotFoundException()
    {
        // Arrange
        MockDbContext([], [], []);
        var command = new DeleteArticleRequest.Command(123);

        // Act & Assert
        await Assert
            .ThrowsAsync<EntityNotFoundException>(async () => await _handler.Handle(command, _cancellationToken));
    }

    private ArticleEntity CreateArticleEntity(int id)
    {
        var article = new ArticleEntity
        {
            Id = id,
            Name = "Test Article",
            GroupId = 1,
            Text = "Some text",
            Keywords = "tag",
            ArticleTags = [new ArticleTagEntity() { Id = 1, ArticleId = id }],
            ArticleReferences =
                [new ArticleReferenceEntity() { Id = 1, ArticleId = id, ExternalUrl = "https://test.com" }]
        };

        return article;
    }

    private void MockDbContext(
        IEnumerable<ArticleEntity> articles,
        IEnumerable<ArticleTagEntity> tags,
        IEnumerable<ArticleReferenceEntity> references)
    {
        _dbContext.Setup(x => x.Articles).Returns(articles.ToArray().BuildMockDbSet().Object);
        _dbContext.Setup(x => x.ArticleTags).Returns(tags.ToArray().BuildMockDbSet().Object);
        _dbContext.Setup(x => x.ArticleReferences).Returns(references.ToArray().BuildMockDbSet().Object);
    }
}
