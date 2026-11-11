using MockQueryable.Moq;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services;
using Modsen.Portal.KnowledgeBase.DataAccess.Npgsql;
using Modsen.Portal.KnowledgeBase.DataAccess.Npgsql.Entities;
using Modsen.Portal.KnowledgeBase.Requests.Articles.Core;
using Modsen.Portal.KnowledgeBase.Requests.Articles.Requests;
using Modsen.Portal.KnowledgeBase.Requests.Articles.Requests.RequestDto;
using Modsen.Portal.KnowledgeBase.Requests.Core;
using Moq;

namespace Modsen.Portal.KnowledgeBase.UnitTests.Articles.Requests;

public class UpdateArticleRequestTests
{
    private readonly Mock<IKnowledgeBaseDbContext> _dbContext = new();
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContext = new();
    private readonly Mock<IClockService> _clockService = new();
    private readonly Mock<IArticleLoader> _articleLoader = new();
    private readonly Mock<IUrlsFromTextExtractor> _urlsFromTextExtractor = new();
    private readonly Mock<IIsArticleWithSameNameExistBusinessRule> _isArticleWithSameNameExistBusinessRule = new();

    private readonly UpdateArticleRequest _handler;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public UpdateArticleRequestTests()
    {
        _handler = new UpdateArticleRequest(
            _dbContext.Object,
            _authenticatedUserContext.Object,
            _clockService.Object,
            _articleLoader.Object,
            _urlsFromTextExtractor.Object,
            _isArticleWithSameNameExistBusinessRule.Object
        );

        _authenticatedUserContext.Setup(x => x.UserId).Returns(1);
        _clockService.Setup(x => x.UtcNow).Returns(DateTime.UtcNow);
    }

    [Fact]
    public async Task HandleWhenArticleExistsAndValidDataShouldBeSuccess()
    {
        // Arrange
        var entity = CreateArticleEntity(1, 1, "Old", []);
        MockKnowledgeBaseDbContext([entity], entity.ArticleTags, entity.ArticleReferences, [], [
            new GroupEntity { Id = 1, Name = "name" }
        ]);

        _isArticleWithSameNameExistBusinessRule
            .Setup(x => x.IsCorrespondToRuleAsync(1, "New", _cancellationToken, 1))
            .ReturnsAsync(false);

        _urlsFromTextExtractor.Setup(x => x.ExtractUrls("Text")).Returns(["http://test"]);

        _articleLoader
            .Setup(x => x.LoadAsync(1, _cancellationToken))
            .ReturnsAsync(new ArticleDto { Id = 1, Name = "New", Text = "text" });

        var command = new UpdateArticleRequest.Command(1, 1, "New", null, "Text", "Keywords", true, false, []);

        // Act
        var result = await _handler.Handle(command, _cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.Name.Should().Be("New");
    }

    [Fact]
    public async Task HandleWhenArticleNotFoundShouldBeFailed()
    {
        // Arrange
        MockKnowledgeBaseDbContext([], [], [], [], []);

        var command = new UpdateArticleRequest.Command(1, 1, "New", null, "Text", "Keywords", true, false, []);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(async () =>
            await _handler.Handle(command, _cancellationToken));
    }

    [Fact]
    public async Task HandleWhenArticleWithSameNameExistsShouldBeFailed()
    {
        // Arrange
        var entity = CreateArticleEntity(1, 1, "Old", []);
        MockKnowledgeBaseDbContext([entity], entity.ArticleTags, entity.ArticleReferences, [], [
            new GroupEntity { Id = 1, Name = "name" }
        ]);

        _isArticleWithSameNameExistBusinessRule
            .Setup(x => x.IsCorrespondToRuleAsync(1, "Old", _cancellationToken, 1))
            .ReturnsAsync(true);

        var command = new UpdateArticleRequest.Command(1, 1, "Old", null, "Text", "Keywords", true, false, []);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidDataAppException>(async () =>
            await _handler.Handle(command, _cancellationToken));
    }

    [Fact]
    public async Task HandleWhenGroupDoesNotExistShouldBeFailed()
    {
        // Arrange
        var entity = CreateArticleEntity(1, 2, "Article", []);
        MockKnowledgeBaseDbContext([entity], entity.ArticleTags, entity.ArticleReferences, [], []);

        _isArticleWithSameNameExistBusinessRule
            .Setup(x => x.IsCorrespondToRuleAsync(1, "Article", _cancellationToken, 1))
            .ReturnsAsync(false);

        var command = new UpdateArticleRequest.Command(1, 1, "Article", null, "Text", "Keywords", true, false, []);

        // Act & Assert
        await Assert.ThrowsAsync<EntityConflictException>(async () =>
            await _handler.Handle(command, _cancellationToken));
    }

    private void MockKnowledgeBaseDbContext(
        IEnumerable<ArticleEntity> articles,
        IEnumerable<ArticleTagEntity> articleTags,
        IEnumerable<ArticleReferenceEntity> articleReferences,
        IEnumerable<TagEntity> tags,
        IEnumerable<GroupEntity> groups
    )
    {
        _dbContext.Setup(x => x.Articles).Returns(articles.ToArray().BuildMockDbSet().Object);
        _dbContext.Setup(x => x.ArticleTags).Returns(articleTags.ToArray().BuildMockDbSet().Object);
        _dbContext.Setup(x => x.ArticleReferences).Returns(articleReferences.ToArray().BuildMockDbSet().Object);
        _dbContext.Setup(x => x.Tags).Returns(tags.ToArray().BuildMockDbSet().Object);
        _dbContext.Setup(x => x.Groups).Returns(groups.ToArray().BuildMockDbSet().Object);
    }

    private ArticleEntity CreateArticleEntity(int id, int groupId, string name, List<string> tags)
    {
        return new ArticleEntity
        {
            Id = id,
            GroupId = groupId,
            Name = name,
            Text = "",
            ArticleTags = [],
            ArticleReferences = [],
            Keywords = "keyword"
        };
    }
}
