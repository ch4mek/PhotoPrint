using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.KnowledgeBase.Requests.Articles.Core;
using Modsen.Portal.KnowledgeBase.Requests.Articles.Requests;
using Modsen.Portal.KnowledgeBase.Requests.Articles.Requests.RequestDto;
using Moq;

namespace Modsen.Portal.KnowledgeBase.UnitTests.Articles.Requests;

public class GetArticleByIdRequestTests
{
    private readonly Mock<IArticleLoader> _articleLoader = new();

    private readonly GetArticleByIdRequest _handler;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public GetArticleByIdRequestTests()
    {
        _handler = new GetArticleByIdRequest(_articleLoader.Object);
    }

    [Fact]
    public async Task HandleWithExistingIdShouldBeSuccess()
    {
        // Arrange
        var articleId = 1;
        var query = new GetArticleByIdRequest.Query(articleId);
        _articleLoader.Setup(x => x.LoadAsync(articleId, _cancellationToken))
            .ReturnsAsync(new ArticleDto { Id = articleId, Name = null, Text = null });

        // Act
        var result = await _handler.Handle(query, _cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(articleId);
    }

    [Fact]
    public async Task HandleWithNonExistingIdShouldBeFailed()
    {
        // Arrange
        const int articleId = 1;
        var query = new GetArticleByIdRequest.Query(articleId);
        _articleLoader.Setup(x => x.LoadAsync(It.IsAny<int>(), _cancellationToken))
            .ThrowsAsync(new EntityNotFoundException());

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(async () => await _handler.Handle(query, _cancellationToken));
    }
}
