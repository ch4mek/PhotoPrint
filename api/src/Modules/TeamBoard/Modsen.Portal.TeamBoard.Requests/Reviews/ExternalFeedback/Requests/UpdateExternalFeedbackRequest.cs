using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.TeamBoard;
using Modsen.Portal.Core.UseCases.ValidationRules;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.Requests.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.Reviews.Conversations.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.Reviews.Conversations.Core.DbQueries;
using Modsen.Portal.TeamBoard.Requests.Reviews.ExternalFeedback.Requests.RequestDto;

namespace Modsen.Portal.TeamBoard.Requests.Reviews.ExternalFeedback.Requests;

internal sealed class UpdateExternalFeedbackRequest : ICommandHandler<UpdateExternalFeedbackRequest.Command, ExternalFeedbackDto>{

    private readonly IIsConversationExistingDbQuery _isConversationExistingDbQuery;
    private readonly ICanTeamBoardUserBeRetrievedBusinessRule _canTeamBoardUserBeRetrievedBusinessRule;
    private readonly IIsUserHavingConversationBusinessRule _isUserHavingConversationBusinessRule;
    private readonly ITeamBoardDbContext _teamBoardDbContext;
    public UpdateExternalFeedbackRequest(
        IIsConversationExistingDbQuery isConversationExistingDbQuery,
        ICanTeamBoardUserBeRetrievedBusinessRule canTeamBoardUserBeRetrievedBusinessRule,
        IIsUserHavingConversationBusinessRule isUserHavingConversationBusinessRule,
        ITeamBoardDbContext teamBoardDbContext)
    {
        _isConversationExistingDbQuery = isConversationExistingDbQuery;
        _canTeamBoardUserBeRetrievedBusinessRule = canTeamBoardUserBeRetrievedBusinessRule;
        _isUserHavingConversationBusinessRule = isUserHavingConversationBusinessRule;
        _teamBoardDbContext = teamBoardDbContext;
    }

    public async Task<ExternalFeedbackDto> Handle(Command request, CancellationToken cancellationToken)
    {
        var externalFeedback = await _teamBoardDbContext.ExternalFeedbacks
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (externalFeedback == null)
        {
            throw new EntityNotFoundException($"There is no external feedback with Id {request.Id}");
        }

        if (externalFeedback.User != null && externalFeedback.User.Position.IsIntern())
        {
            throw new ActionNotAllowedException(PortalMessages.NotAllowedOperationWithIntern);
        }

        await ValidateRequestAsync(externalFeedback, request, cancellationToken);

        externalFeedback = await UpdateExternalFeedbackAsync(externalFeedback, request, cancellationToken);

        return new ExternalFeedbackDto(externalFeedback!.Id, externalFeedback.ExternalFeedbackDate, externalFeedback.ExternalFeedbackType,
            externalFeedback.Feedback, externalFeedback.ActionsTodo, externalFeedback.Reviewer, externalFeedback.ConversationId,
            externalFeedback.ProjectId, externalFeedback.UserId);
    }

    private async Task<DataAccess.Npgsql.DatabaseEntities.ExternalFeedback> UpdateExternalFeedbackAsync(
        DataAccess.Npgsql.DatabaseEntities.ExternalFeedback externalFeedback,
        Command request,
        CancellationToken cancellationToken)
    {
        externalFeedback.ExternalFeedbackType = request.ExternalFeedbackType;
        externalFeedback.ActionsTodo = request.ActionsTodo;
        externalFeedback.Reviewer = request.Reviewer;
        externalFeedback.ConversationId = request.ConversationId;

        await _teamBoardDbContext.SaveChangesAsync(cancellationToken);

        return externalFeedback;
    }

    private async Task ValidateRequestAsync(
        DataAccess.Npgsql.DatabaseEntities.ExternalFeedback externalFeedback,
        Command request,
        CancellationToken cancellationToken)
    {
        var canBeRetrieved = await _canTeamBoardUserBeRetrievedBusinessRule.IsCorrespondsToBusiness(
            externalFeedback.UserId, AccessOptions.Manager, cancellationToken);

        if (!canBeRetrieved)
        {
            throw new ActionNotAllowedException();
        }

        if (request.ConversationId == null)
        {
            return;
        }

        if (!await _isConversationExistingDbQuery.ExecuteAsync(request.ConversationId.Value, cancellationToken))
        {
            throw new EntityNotFoundException($"There is no conversation with Id {request.ConversationId.Value}");
        }

        var isUserHavingConversation = await _isUserHavingConversationBusinessRule.IsCorrespondsToBusiness(
            new IsUserHavingConversationModel(externalFeedback.UserId, request.ConversationId.Value), cancellationToken);

        if (!isUserHavingConversation.IsSuccess)
        {
            throw new InvalidDataAppException(isUserHavingConversation.Reason!);
        }
    }

    internal sealed record Command(int Id, ExternalFeedbackType ExternalFeedbackType, string? ActionsTodo, int? ConversationId,
        string? Reviewer) : ICommand<ExternalFeedbackDto>, ITransactionalRequest;

    internal sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotNull().NotEmpty();
            RuleFor(x => x.Reviewer)
                .HyphensSpacesRule()
                .LongAlphabeticWithHyphensSpaces();
            RuleFor(x => x.ActionsTodo).BeNullOrBeLongTextType();
        }
    }
}
