using MockQueryable.Moq;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.KnowledgeBase.DataAccess.Npgsql;
using Modsen.Portal.KnowledgeBase.DataAccess.Npgsql.Entities;
using Modsen.Portal.KnowledgeBase.Requests.ArticleGroups.Core;
using Modsen.Portal.KnowledgeBase.Requests.ArticleGroups.Requests;
using Moq;

namespace Modsen.Portal.KnowledgeBase.UnitTests.Groups;

public class UpdateGroupNameRequestTests
{
    private readonly UpdateGroupNameRequest _updateGroupNameRequest;
    private readonly Mock<IKnowledgeBaseDbContext> _knowledgeBaseDbContextMock = new();
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContext = new();
    private readonly Mock<IIsGroupWithSameNameExistBusinessRule> _isGroupWithSameNameExistBusinessRuleMock = new();

    private readonly List<GroupEntity> _groupEntities =
    [
        new() { Id = 1, Name = "132" }
    ];

    public UpdateGroupNameRequestTests()
    {
        _knowledgeBaseDbContextMock
            .Setup(mock => mock.Groups)
            .Returns(_groupEntities.BuildMockDbSet().Object);

        _updateGroupNameRequest = new UpdateGroupNameRequest(
            _knowledgeBaseDbContextMock.Object,
            _authenticatedUserContext.Object,
            _isGroupWithSameNameExistBusinessRuleMock.Object);
    }

    [Fact]
    public async Task HandleWhenGroupExistShouldReturnNewName()
    {
        //Assert
        var groupId = 1;
        var newName = "newName";

        _authenticatedUserContext
            .Setup(x => x.HasClaim(It.IsAny<string>()))
            .Returns(true);

        _isGroupWithSameNameExistBusinessRuleMock
            .Setup(mock => mock.IsCorrespondToRuleAsync(
                It.IsAny<int?>(), It.IsAny<string>(), _cancellationToken, null))
            .ReturnsAsync((false, null));

        var command = new UpdateGroupNameRequest.Command(groupId, newName);

        // Arrange
        var result = await _updateGroupNameRequest.Handle(command, _cancellationToken);

        // Act
        result.Should().Be(newName);
    }

    [Fact]
    public async Task HandleWhenGroupIsNotExistShouldThrowException()
    {
        //Assert
        var groupId = 2;
        var newName = "newName";

        var command = new UpdateGroupNameRequest.Command(groupId, newName);

        // Arrange
        var result = async () => await _updateGroupNameRequest.Handle(command, _cancellationToken);

        // Act
        await result.Should()
            .ThrowAsync<EntityNotFoundException>()
            .WithMessage("Group is not exist");
    }
}
