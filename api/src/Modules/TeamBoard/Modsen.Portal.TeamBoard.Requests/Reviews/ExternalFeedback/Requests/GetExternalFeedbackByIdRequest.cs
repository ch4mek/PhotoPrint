using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.Requests.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.Reviews.ExternalFeedback.Requests.RequestDto;

namespace Modsen.Portal.TeamBoard.Requests.Reviews.ExternalFeedback.Requests;

internal sealed class GetExternalFeedbackByIdRequest : IQueryHandler<GetExternalFeedbackByIdRequest.Query, ExternalFeedbackWithProjectAndConversationDto>
{
    private readonly ICanTeamBoardUserBeRetrievedBusinessRule _canTeamBoardUserBeRetrievedBusinessRule;
    private readonly ITeamBoardDbContext _teamBoardDbContext;
    public GetExternalFeedbackByIdRequest(
        ICanTeamBoardUserBeRetrievedBusinessRule canTeamBoardUserBeRetrievedBusinessRule,
        ITeamBoardDbContext teamBoardDbContext)
    {
        _canTeamBoardUserBeRetrievedBusinessRule = canTeamBoardUserBeRetrievedBusinessRule;
        _teamBoardDbContext = teamBoardDbContext;
    }

    public async Task<ExternalFeedbackWithProjectAndConversationDto> Handle(Query request, CancellationToken cancellationToken)
    {
        var externalFeedback = await _teamBoardDbContext.ExternalFeedbacks.AsNoTracking()
            .Include(x => x.Project)
            .Include(x => x.User)
            .Include(x => x.Conversation)
                .ThenInclude(y => y!.Supervisor)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (externalFeedback == null)
        {
            throw new EntityNotFoundException($"There is no external feedback with Id {request.Id}");
        }

        if (externalFeedback.User != null && externalFeedback.User.Position.IsIntern())
        {
            throw new ActionNotAllowedException(PortalMessages.NotAllowedOperationWithIntern);
        }

        var canBeRetrieved = await _canTeamBoardUserBeRetrievedBusinessRule.IsCorrespondsToBusiness(
            externalFeedback.UserId, AccessOptions.Manager, cancellationToken);

        if (!canBeRetrieved)
        {
            throw new ActionNotAllowedException();
        }

        var conversationSupervisorFullName = externalFeedback.Conversation?.Supervisor?.FirstName != null
            ? $"{externalFeedback.Conversation?.Supervisor?.FirstName} {externalFeedback.Conversation?.Supervisor?.LastName}"
            : null;

        return new ExternalFeedbackWithProjectAndConversationDto(externalFeedback.Id,
            externalFeedback.ExternalFeedbackDate, externalFeedback.ExternalFeedbackType, externalFeedback.Feedback,
            externalFeedback.ActionsTodo, externalFeedback.Reviewer, externalFeedback.ConversationId,
            conversationSupervisorFullName, externalFeedback.Conversation?.Date, externalFeedback.ProjectId,
            externalFeedback.Project!.ProjectName, externalFeedback.UserId);
    }

    internal sealed record Query(int Id) : IQuery<ExternalFeedbackWithProjectAndConversationDto>;
}
