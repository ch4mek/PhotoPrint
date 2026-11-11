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
using Modsen.Portal.KnowledgeBase.Requests.Core.Calculators;
using Moq;

namespace Modsen.Portal.KnowledgeBase.UnitTests.Articles.Requests;

public class CreateArticleRequestTests
{
    private readonly Mock<IKnowledgeBaseDbContext> _dbContext = new();
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContext = new();
    private readonly Mock<IClockService> _clockService = new();
    private readonly Mock<IArticleLoader> _articleLoader = new();
    private readonly Mock<IUrlsFromTextExtractor> _urlsFromTextExtractor = new();
    private readonly Mock<IMaxOrderWeightCalculator> _maxOrderWeightCalculator = new();
    private readonly Mock<IIsArticleWithSameNameExistBusinessRule> _isArticleWithSameNameExistBusinessRule = new();

    private readonly CreateArticleRequest _handler;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public CreateArticleRequestTests()
    {
        _handler = new CreateArticleRequest(
            _dbContext.Object,
            _authenticatedUserContext.Object,
            _clockService.Object,
            _articleLoader.Object,
            _urlsFromTextExtractor.Object,
            _maxOrderWeightCalculator.Object,
            _isArticleWithSameNameExistBusinessRule.Object);

        _authenticatedUserContext.Setup(x => x.UserId).Returns(1);
    }

    private void MockKnowledgeBaseDbContext(
        IEnumerable<ArticleEntity> articles,
        IEnumerable<GroupEntity> groups,
        IEnumerable<TagEntity> tags,
        IEnumerable<ArticleTagEntity> articleTags,
        IEnumerable<ArticleReferenceEntity> articleReferences)
    {
        _dbContext.Setup(x => x.Articles).Returns(articles.ToArray().BuildMockDbSet().Object);
        _dbContext.Setup(x => x.Groups).Returns(groups.ToArray().BuildMockDbSet().Object);
        _dbContext.Setup(x => x.Tags).Returns(tags.ToArray().BuildMockDbSet().Object);
        _dbContext.Setup(x => x.ArticleTags).Returns(articleTags.ToArray().BuildMockDbSet().Object);
        _dbContext.Setup(x => x.ArticleReferences).Returns(articleReferences.ToArray().BuildMockDbSet().Object);
    }

    [Fact]
    public async Task HandleWhenDataIsCorrectShouldCreateSuccessfully()
    {
        // Arrange
        var command = new CreateArticleRequest.Command(
            GroupId: 10,
            Name: "TestArticle",
            Icon: null,
            Text: "Some text",
            Keywords: "key",
            IsActive: true,
            IsAvailableOnMainScreen: false,
            Tags: ["tag1"]
        );

        var group = new GroupEntity { Id = 10, Name = "Group" };

        MockKnowledgeBaseDbContext([], [group], [], [], []);

        _isArticleWithSameNameExistBusinessRule.Setup(x =>
                x.IsCorrespondToRuleAsync(10, "TestArticle", _cancellationToken, null))
            .ReturnsAsync(false);

        _maxOrderWeightCalculator.Setup(x => x.CalculateAsync(10, _cancellationToken))
            .ReturnsAsync(5);

        _articleLoader.Setup(x => x.LoadAsync(It.IsAny<int>(), _cancellationToken))
            .ReturnsAsync(new ArticleDto
            {
                Id = 1,
                GroupId = 10,
                Name = "TestArticle",
                Text = "Some text",
                IsActive = true,
                IsAvailableOnMainScreen = false,
                Keywords = "key",
                ArticleTags =
                [
                    new TagDto() { Name = "tag1" }
                ],
                ArticleReferences = []
            });

        // Act
        var result = await _handler.Handle(command, _cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(command.Name);
        result.GroupId.Should().Be(command.GroupId);
        result.IsActive.Should().Be(command.IsActive);
        result.IsAvailableOnMainScreen.Should().Be(command.IsAvailableOnMainScreen);
        result.ArticleTags.Select(x => x.Name).Should().Contain("tag1");
    }

    [Fact]
    public async Task HandleWhenGroupDoesNotExistShouldThrowEntityNotFoundException()
    {
        // Arrange
        var command = new CreateArticleRequest.Command(
            999, "Name", null, "Text", "Key", true, false, ["tag1"]);

        MockKnowledgeBaseDbContext([], [], [], [], []);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() =>
            _handler.Handle(command, _cancellationToken));
    }

    [Fact]
    public async Task HandleWhenArticleWithSameNameExistsShouldThrowInvalidDataAppException()
    {
        // Arrange
        var group = new GroupEntity { Id = 10, Name = "name" };
        MockKnowledgeBaseDbContext([], [group], [], [], []);

        var command = new CreateArticleRequest.Command(
            10, "SameName", null, "Text", "Key", true, false, ["tag1"]);

        _isArticleWithSameNameExistBusinessRule.Setup(x =>
                x.IsCorrespondToRuleAsync(10, "SameName", _cancellationToken, null))
            .ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidDataAppException>(() =>
            _handler.Handle(command, _cancellationToken));
    }
}
