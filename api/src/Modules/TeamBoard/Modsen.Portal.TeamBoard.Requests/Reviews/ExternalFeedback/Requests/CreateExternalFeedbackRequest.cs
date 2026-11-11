using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.TeamBoard;
using Modsen.Portal.Core.UseCases.ValidationRules;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.Requests.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.Core.DbQueries;
using Modsen.Portal.TeamBoard.Requests.Reviews.Conversations.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.Reviews.Conversations.Core.DbQueries;
using Modsen.Portal.TeamBoard.Requests.Reviews.ExternalFeedback.Requests.RequestDto;

namespace Modsen.Portal.TeamBoard.Requests.Reviews.ExternalFeedback.Requests;

internal sealed class CreateExternalFeedbackRequest
    : ICommandHandler<CreateExternalFeedbackRequest.Command, ExternalFeedbackDto>
{
    private readonly IIsTeamBoardUserExistWithAccessControlDbQuery _isTeamBoardUserExistingWithAccessControlDbQuery;
    private readonly IIsConversationExistingDbQuery _isConversationExistingDbQuery;
    private readonly ICanTeamBoardUserBeRetrievedBusinessRule _canTeamBoardUserBeRetrievedBusinessRule;
    private readonly IIsUserHavingConversationBusinessRule _isUserHavingConversationBusinessRule;
    private readonly ITeamBoardDbContext _teamboardDbContext;
    public CreateExternalFeedbackRequest(
        IIsTeamBoardUserExistWithAccessControlDbQuery isTeamBoardUserExistingWithAccessControlDbQuery,
        IIsConversationExistingDbQuery isConversationExistingDbQuery,
        ICanTeamBoardUserBeRetrievedBusinessRule canTeamBoardUserBeRetrievedBusinessRule,
        IIsUserHavingConversationBusinessRule isUserHavingConversationBusinessRule,
        ITeamBoardDbContext teamboardDbContext)
    {
        _isTeamBoardUserExistingWithAccessControlDbQuery = isTeamBoardUserExistingWithAccessControlDbQuery;
        _isConversationExistingDbQuery = isConversationExistingDbQuery;
        _canTeamBoardUserBeRetrievedBusinessRule = canTeamBoardUserBeRetrievedBusinessRule;
        _isUserHavingConversationBusinessRule = isUserHavingConversationBusinessRule;
        _teamboardDbContext = teamboardDbContext;
    }

    public async Task<ExternalFeedbackDto> Handle(Command request, CancellationToken cancellationToken)
    {
        await ValidateRequestAsync(request, cancellationToken);

        var externalFeedback = new DataAccess.Npgsql.DatabaseEntities.ExternalFeedback
        {
            ExternalFeedbackDate = request.ExternalFeedbackDate,
            ExternalFeedbackType = request.ExternalFeedbackType,
            Feedback = request.Feedback,
            ActionsTodo = request.ActionsTodo,
            Reviewer = request.Reviewer,
            ProjectId = request.ProjectId,
            UserId = request.UserId,
            ConversationId = request.ConversationId
        };

        await _teamboardDbContext.ExternalFeedbacks.AddAsync(externalFeedback, cancellationToken);
        await _teamboardDbContext.SaveChangesAsync(cancellationToken);

        return new ExternalFeedbackDto(externalFeedback.Id, externalFeedback.ExternalFeedbackDate,
            externalFeedback.ExternalFeedbackType, externalFeedback.Feedback, externalFeedback.ActionsTodo,
            externalFeedback.Reviewer, externalFeedback.ConversationId, externalFeedback.ProjectId,
            externalFeedback.UserId);
    }

    private async Task ValidateRequestAsync(Command request, CancellationToken cancellationToken)
    {
        var isActiveProjectExist = await _teamboardDbContext.TeamBoardProjects
            .AnyAsync(project => project.Id == request.ProjectId && project.IsActive, cancellationToken);
        if (!isActiveProjectExist)
        {
            throw new EntityNotFoundException($"There is no active project with Id {request.ProjectId}");
        }

        if (!await _isTeamBoardUserExistingWithAccessControlDbQuery.ExecuteAsync(request.UserId, cancellationToken: cancellationToken))
        {
            throw new EntityNotFoundException($"There is no user with Id {request.UserId}");
        }

        if (!await IsUserAssignedToProjectAsync(request.UserId, request.ProjectId, cancellationToken))
        {
            throw new EntityNotFoundException($"User with Id {request.UserId} not assigned to project with Id {request.ProjectId}");
        }

        if (request.ConversationId != null)
        {
            if (!await _isConversationExistingDbQuery.ExecuteAsync(request.ConversationId.Value, cancellationToken))
            {
                throw new EntityNotFoundException($"There is no conversation with Id {request.ConversationId.Value}");
            }

            var isUserHavingConversation = await _isUserHavingConversationBusinessRule.IsCorrespondsToBusiness(
                new IsUserHavingConversationModel(request.UserId, request.ConversationId.Value), cancellationToken);

            if (!isUserHavingConversation.IsSuccess)
            {
                throw new InvalidDataAppException(isUserHavingConversation.Reason!);
            }
        }

        var canBeRetrieved = await _canTeamBoardUserBeRetrievedBusinessRule.IsCorrespondsToBusiness(request.UserId,
            Portal.Core.AccessOptions.Manager,
            cancellationToken);

        if (!canBeRetrieved)
        {
            throw new ActionNotAllowedException();
        }
    }

    private Task<bool> IsUserAssignedToProjectAsync(int userId, int projectId, CancellationToken cancellationToken)
    {
        return _teamboardDbContext.ProjectUsers.AnyAsync(user =>
                user.UserId == userId &&
                user.ProjectId == projectId,
            cancellationToken);
    }

    internal sealed record Command(DateOnly ExternalFeedbackDate, ExternalFeedbackType ExternalFeedbackType, string Feedback,
        string? ActionsTodo, string? Reviewer, int? ConversationId, int ProjectId, int UserId)
        : ICommand<ExternalFeedbackDto>, ITransactionalRequest;

    internal sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.ExternalFeedbackDate).NotNull();
            RuleFor(x => x.ProjectId).NotNull().NotEmpty();
            RuleFor(x => x.UserId).NotNull().NotEmpty();
            RuleFor(x => x.Feedback).NotNull().NotEmpty();
            RuleFor(x => x.Reviewer)
                .HyphensSpacesRule()
                .LongAlphabeticWithHyphensSpaces();
            RuleFor(x => x.Feedback).LongTextType();
            RuleFor(x => x.ActionsTodo).BeNullOrBeLongTextType();
        }
    }
}
