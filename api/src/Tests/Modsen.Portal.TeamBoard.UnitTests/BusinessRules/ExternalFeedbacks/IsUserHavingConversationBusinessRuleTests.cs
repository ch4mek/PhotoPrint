using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.TeamBoard;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.TeamBoard.Requests.Reviews.Conversations.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.Reviews.Conversations.Core.CoreDto;
using Modsen.Portal.TeamBoard.Requests.Reviews.Conversations.Core.Loaders;
using Moq;

namespace Modsen.Portal.TeamBoard.UnitTests.BusinessRules.ExternalFeedbacks;

public class IsUserHavingConversationBusinessRuleTests
{
    private readonly Mock<IConversationLoader> _getConversationByIdDbQueryMock = new();
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    private readonly IsUserHavingConversationBusinessRule _businessRule;

    public IsUserHavingConversationBusinessRuleTests()
    {
        _businessRule = new IsUserHavingConversationBusinessRule(_getConversationByIdDbQueryMock.Object);
    }

    [Fact]
    public async Task IsCorrespondsToBussinesWhenConversationBelongsToUserReturnSuccess()
    {
        // Arrange
        var model = new IsUserHavingConversationModel(UserId: 5, ConversationId: 1);
        var conversation = new ConversationCoreDto(
            Id: 1,
            DateOnly.FromDateTime(DateTime.UtcNow),
            ConversationType.Conversation.GetDescription(),
            ConversationResult.Great.ToString(),
            Theses: Guid.CreateVersion7().ToAlphabetic(),
            EmployeeTheses: Guid.CreateVersion7().ToAlphabetic(),
            Agreements: Guid.CreateVersion7().ToAlphabetic(),
            null,
            null,
            SupervisorId: 3,
            model.UserId,
            null,
            new List<ConversationFileCoreDto>(),
            new List<ConversationDocumentLinkCoreDto>());

        _getConversationByIdDbQueryMock.Setup(x => x.LoadAsync(model.ConversationId, _cancellationToken))
            .ReturnsAsync(conversation);

        // Act
        var result = await _businessRule.IsCorrespondsToBusiness(model, _cancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task IsCorrespondsToBussinesWhenConversationDoesNotBelongToUserReturnFail()
    {
        // Arrange
        var model = new IsUserHavingConversationModel(UserId: 5, ConversationId: 1);

        var conversation = new ConversationCoreDto(
            Id: 1,
            DateOnly.FromDateTime(DateTime.UtcNow),
            ConversationType.Conversation.GetDescription(),
            ConversationResult.Bad.ToString(),
            Theses: Guid.CreateVersion7().ToAlphabetic(),
            EmployeeTheses: Guid.CreateVersion7().ToAlphabetic(),
            Agreements: Guid.CreateVersion7().ToAlphabetic(),
            null,
            null,
            SupervisorId: 3,
            UserId: 6,
            null,
            new List<ConversationFileCoreDto>(),
            new List<ConversationDocumentLinkCoreDto>());

        _getConversationByIdDbQueryMock.Setup(x => x.LoadAsync(model.ConversationId, _cancellationToken))
            .ReturnsAsync(conversation);

        // Act
        var result = await _businessRule.IsCorrespondsToBusiness(model, _cancellationToken);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Reason.Should().BeEquivalentTo($"User does not have conversation with Id {model.ConversationId}");
    }

    [Fact]
    public async Task IsCorrespondsToBussinesWhenConversationDoesNotExistThrowEntityNotFoundException()
    {
        // Arrange
        var model = new IsUserHavingConversationModel(UserId: 5, ConversationId: 1);

        _getConversationByIdDbQueryMock.Setup(x => x.LoadAsync(model.ConversationId, _cancellationToken))
            .ReturnsAsync((ConversationCoreDto)null);

        // Act
        var result = async () => await _businessRule.IsCorrespondsToBusiness(model, _cancellationToken);

        // Assert
        await result.Should().ThrowAsync<EntityNotFoundException>();
    }
}
