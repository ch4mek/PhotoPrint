using MockQueryable.Moq;
using Modsen.Portal.KnowledgeBase.DataAccess.Npgsql;
using Modsen.Portal.KnowledgeBase.DataAccess.Npgsql.Entities;
using Modsen.Portal.KnowledgeBase.Requests.Articles.Core;
using Modsen.Portal.KnowledgeBase.Requests.Articles.Requests;
using Modsen.Portal.KnowledgeBase.Requests.Articles.Requests.RequestDto;
using Moq;

namespace Modsen.Portal.KnowledgeBase.UnitTests.Articles.Requests;

public class GetArticlesBySearchParametersTests
{
    private readonly Mock<IKnowledgeBaseDbContext> _dbContext = new();
    private readonly Mock<IArticleLoader> _articleLoader = new();
    private readonly GetArticlesBySearchParameters _handler;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public GetArticlesBySearchParametersTests()
    {
        _handler = new GetArticlesBySearchParameters(_dbContext.Object, _articleLoader.Object);
    }

    [Fact]
    public async Task HandleWhenNoFiltersShouldReturnAllArticles()
    {
        // Arrange
        var articles = new List<ArticleEntity>
        {
            CreateArticleEntity(1, groupId: 10, name: "Alpha", keywords: "cloud"),
            CreateArticleEntity(2, groupId: 20, name: "Beta", keywords: "data")
        };

        MockDbContext(articles);

        _articleLoader
            .Setup(x => x.LoadAsync(It.IsAny<int[]>(), _cancellationToken))
            .ReturnsAsync([
                new ArticleDto { Name = "name", Text = "text" }, new ArticleDto { Name = "name2", Text = "text2" }
            ]);

        var query = new GetArticlesBySearchParameters.Query(null, null, null, null);

        // Act
        var result = await _handler.Handle(query, _cancellationToken);

        // Assert
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task HandleWhenFilterByGroupShouldReturnMatchedArticles()
    {
        // Arrange
        var articles = new List<ArticleEntity>
        {
            CreateArticleEntity(1, groupId: 10, name: "Alpha", keywords: "cloud"),
            CreateArticleEntity(2, groupId: 20, name: "Beta", keywords: "data")
        };

        MockDbContext(articles);

        _articleLoader
            .Setup(x => x.LoadAsync(It.Is<int[]>(ids => ids.SequenceEqual(new[] { 1 })), _cancellationToken))
            .ReturnsAsync([new ArticleDto { Id = 1, Name = "name", Text = "text" }]);

        var query = new GetArticlesBySearchParameters.Query(10, null, null, null);

        // Act
        var result = await _handler.Handle(query, _cancellationToken);

        // Assert
        result.Should().HaveCount(1);
        result.First().Id.Should().Be(1);
    }

    private ArticleEntity CreateArticleEntity(int id, int groupId, string name, string keywords)
    {
        return new ArticleEntity
        {
            Id = id,
            GroupId = groupId,
            Name = name,
            Text = "Some text",
            Keywords = keywords,
            ArticleTags = []
        };
    }

    private void MockDbContext(IEnumerable<ArticleEntity> articles)
    {
        _dbContext.Setup(x => x.Articles).Returns(articles.ToArray().BuildMockDbSet().Object);
    }
}
