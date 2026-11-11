using MockQueryable.Moq;
using Modsen.Portal.KnowledgeBase.DataAccess.Npgsql;
using Modsen.Portal.KnowledgeBase.DataAccess.Npgsql.Entities;
using Modsen.Portal.KnowledgeBase.Requests.Tags.Requests;
using Moq;

namespace Modsen.Portal.KnowledgeBase.UnitTests.Tags;

public class UpdateTagRequestTests
{
    private readonly Mock<IKnowledgeBaseDbContext> _dbContextMock = new();

    private readonly UpdateTagRequest _handler;

    public UpdateTagRequestTests()
    {
        _handler = new UpdateTagRequest(_dbContextMock.Object);
    }

    [Fact]
    public async Task HandleWhenRequestShouldDeleteTag()
    {
        // Arrange
        var tagSet = new List<TagEntity> { new() { Id = 1, Name = "Test" } };
        _dbContextMock.Setup(db => db.Tags)
            .Returns(tagSet.BuildMockDbSet().Object);
        _dbContextMock.Setup(db => db.Tags.FindAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .Returns((object[] tag) => new ValueTask<TagEntity>(tagSet.Find(it => it.Id == (int)tag[0])));

        // Act
        var tag = await _handler.Handle(new UpdateTagRequest.Command(1, "tseT"), It.IsAny<CancellationToken>());

        // Assert
        tag.Id.Should().Be(1);
        tag.Name.Should().Be("tseT");

        _dbContextMock.Verify(db => db.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
