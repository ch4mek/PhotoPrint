using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.Reviews.Conversations.Core.Loaders;

namespace Modsen.Portal.TeamBoard.Requests.Reviews.Conversations.Core.BusinessRules;

internal interface IIsUserHavingConversationBusinessRule : IBusinessRuleWithFailReason<IsUserHavingConversationModel>;

internal record IsUserHavingConversationModel(int UserId, int ConversationId);

internal class IsUserHavingConversationBusinessRule : IIsUserHavingConversationBusinessRule
{
    private readonly IConversationLoader _conversationLoader;

    public IsUserHavingConversationBusinessRule(IConversationLoader getConversationByIdDbQuery)
    {
        _conversationLoader = getConversationByIdDbQuery;
    }

    public async Task<BusinessRuleResult> IsCorrespondsToBusiness(IsUserHavingConversationModel? model,
        CancellationToken cancellationToken)
    {
        var conversation = await _conversationLoader.LoadAsync(model!.ConversationId, cancellationToken);

        if (conversation == null)
        {
            throw new EntityNotFoundException($"There is no conversation with Id {model.ConversationId}");
        }

        return conversation.UserId == model.UserId
            ? BusinessRuleResult.Success()
            : BusinessRuleResult.Fail($"User does not have conversation with Id {model.ConversationId}");
    }
}
