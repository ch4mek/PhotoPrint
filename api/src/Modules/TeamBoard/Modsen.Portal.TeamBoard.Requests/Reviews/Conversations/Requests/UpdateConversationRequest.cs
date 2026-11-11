using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.TeamBoard;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.Reviews.Conversations.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.Reviews.Conversations.Core.CoreDto;
using Modsen.Portal.TeamBoard.Requests.Reviews.Conversations.Requests.RequestDto;
using Modsen.Portal.TeamBoard.Requests.Reviews.DevelopmentPlans;
using Modsen.Portal.TeamBoard.Requests.Reviews.PerformanceReviews.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.Reviews.PerformanceReviews.Core.Composers;
using Modsen.Portal.TeamBoard.Requests.Reviews.PerformanceReviews.Core.CoreDto;
using Modsen.Portal.TeamBoard.Requests.Reviews.PerformanceReviews.Core.DbQueries;
using Modsen.Portal.TeamBoard.Requests.UserSalary;

namespace Modsen.Portal.TeamBoard.Requests.Reviews.Conversations.Requests;

internal sealed class UpdateConversationRequest : ICommandHandler<UpdateConversationRequest.Command, ConversationDto>{

    private readonly ICreatePerformanceReviewComposer _createPerformanceReviewDbQuery;
    private readonly IGetPerformanceReviewByConversationIdDbQuery _getUserPerformanceReviewByConversationIdDbQuery;
    private readonly IUpdatePerformanceReviewComposer _updatePerformanceReviewDbQuery;
    private readonly IIsPerformanceReviewForTodayExistingBusinessRule _isPerformanceReviewForTodayExistingBusinessRule;
    private readonly IIsTheSamePerformanceReviewDbQuery _isTheSamePerformanceReviewDbQuery;
    private readonly IDevelopmentPlansApi _developmentPlansApi;
    private readonly IUserSalaryApi _userSalaryApi;
    private readonly ITeamBoardDbContext _teamBoardDbContext;
    private readonly IIsUserHasAccessToMutateConversation _isUserHasAccessToConversation;

    public UpdateConversationRequest(
        ICreatePerformanceReviewComposer createPerformanceReviewDbQuery,
        IGetPerformanceReviewByConversationIdDbQuery getUserPerformanceReviewByConversationIdDbQuery,
        IUpdatePerformanceReviewComposer updatePerformanceReviewDbQuery,
        IIsPerformanceReviewForTodayExistingBusinessRule isPerformanceReviewForTodayExistingBusinessRule,
        IIsTheSamePerformanceReviewDbQuery isTheSamePerformanceReviewDbQuery,
        IDevelopmentPlansApi developmentPlansApi,
        IUserSalaryApi userSalaryApi,
        ITeamBoardDbContext teamBoardDbContext,
        IIsUserHasAccessToMutateConversation isUserHasAccessToMutateConversation)
    {
        _createPerformanceReviewDbQuery = createPerformanceReviewDbQuery;
        _getUserPerformanceReviewByConversationIdDbQuery = getUserPerformanceReviewByConversationIdDbQuery;
        _updatePerformanceReviewDbQuery = updatePerformanceReviewDbQuery;
        _isPerformanceReviewForTodayExistingBusinessRule = isPerformanceReviewForTodayExistingBusinessRule;
        _isTheSamePerformanceReviewDbQuery = isTheSamePerformanceReviewDbQuery;
        _developmentPlansApi = developmentPlansApi;
        _userSalaryApi = userSalaryApi;
        _teamBoardDbContext = teamBoardDbContext;
        _isUserHasAccessToConversation = isUserHasAccessToMutateConversation;
    }

    public async Task<ConversationDto> Handle(Command request, CancellationToken cancellationToken)
    {
        var conversation = await _teamBoardDbContext.Conversations
            .FirstOrDefaultAsync(c => c.Id == request.RequestDto.Id, cancellationToken)
            ?? throw new EntityNotFoundException($"There is no conversation with Id {request.RequestDto.Id}");

        await ValidateUserAccessAsync(conversation, request.RequestDto, cancellationToken);

        var dbResponse = await UpdateConversationAsync(conversation, request.RequestDto, cancellationToken);

        if (conversation.ConversationType == ConversationType.HRConversation)
        {
            return new ConversationDto(
                dbResponse.Id,
                dbResponse.Date,
                dbResponse.ConversationType,
                dbResponse.ConversationResult,
                dbResponse.Theses,
                dbResponse.EmployeeTheses,
                dbResponse.Agreements,
                dbResponse.PerformanceReviewDate,
                dbResponse.PerformanceReviewStatus,
                dbResponse.SupervisorId,
                dbResponse.UserId,
                null,
                null,
                dbResponse.Files.Select(x => new ConversationFileDto(x.Id, x.FileId, x.FileName)).ToList(),
                dbResponse.DocumentLinks.Select(x => new ConversationDocumentLinkDto(x.Id, x.Link)).ToList(),
                [],
                []);
        }

        await _developmentPlansApi.UpdateDevelopmentPlanListWithCreating(request.RequestDto.UserId, request.RequestDto.DevelopmentPlans, cancellationToken);
        await _userSalaryApi.UpdateUserSalaryListWithCreating(request.RequestDto.UserId, request.RequestDto.Salaries, cancellationToken);


        if (request.RequestDto.ConversationType == ConversationType.PerformanceReview)
        {
            var isPerformanceReviewForTodayExisting = await _isPerformanceReviewForTodayExistingBusinessRule
                .IsCorrespondsToBusiness(new IsPerformanceReviewForTodayExistingBusinessRuleRequestModel(
                    request.RequestDto.UserId,
                    request.RequestDto.PerformanceReviewDate!.Value),
                cancellationToken);

            var performanceReviewToUpdate = await _getUserPerformanceReviewByConversationIdDbQuery.ExecuteAsync(dbResponse.Id, cancellationToken);

            var isTheSamePerformanceReview = await _isTheSamePerformanceReviewDbQuery.ExecuteAsync(
                    performanceReviewToUpdate?.Id ?? 0,
                    request.RequestDto.UserId,
                    request.RequestDto.PerformanceReviewDate.Value,
                cancellationToken);

            if (!isPerformanceReviewForTodayExisting.IsSuccess && !isTheSamePerformanceReview)
            {
                throw new EntityConflictException(isPerformanceReviewForTodayExisting.Reason!);
            }

            if (performanceReviewToUpdate != null)
            {
                var updatePerformanceReviewDbRequest = new UpdatePerformanceReviewCoreDto(
                    performanceReviewToUpdate.Id,
                    dbResponse.Id,
                    request.RequestDto.PerformanceReviewDate,
                    request.RequestDto.Theses,
                    request.RequestDto.EmployeeTheses,
                    request.RequestDto.Agreements,
                    request.RequestDto.Passed,
                    request.RequestDto.UserId);

                var result = await _updatePerformanceReviewDbQuery.Compose(updatePerformanceReviewDbRequest, cancellationToken);

                if (result == null)
                {
                    throw new EntityNotFoundException($"There is no performance review with Id {performanceReviewToUpdate}");
                }

                return new ConversationDto(
                    dbResponse.Id,
                    dbResponse.Date,
                    dbResponse.ConversationType,
                    dbResponse.ConversationResult,
                    dbResponse.Theses,
                    dbResponse.EmployeeTheses,
                    dbResponse.Agreements,
                    request.RequestDto.PerformanceReviewDate,
                    result.Status,
                    dbResponse.SupervisorId,
                    dbResponse.UserId,
                    request.RequestDto.Passed,
                    performanceReviewToUpdate.Id,
                    dbResponse.Files.Select(x => new ConversationFileDto(x.Id, x.FileId, x.FileName)).ToList(),
                    dbResponse.DocumentLinks.Select(x => new ConversationDocumentLinkDto(x.Id, x.Link)).ToList(),
                    request.RequestDto.Salaries,
                    request.RequestDto.DevelopmentPlans);
            }
            else
            {
                var createPerformanceReviewDbRequest = new CreatePerformanceReviewCoreDto(
                    request.RequestDto.UserId,
                    request.RequestDto.PerformanceReviewDate!,
                    request.RequestDto.Passed,
                    request.RequestDto.Theses,
                    request.RequestDto.EmployeeTheses,
                    request.RequestDto.Agreements,
                    dbResponse.Id);

                var result = await _createPerformanceReviewDbQuery.Compose(createPerformanceReviewDbRequest, cancellationToken);

                return new ConversationDto(
                    dbResponse.Id,
                    dbResponse.Date,
                    dbResponse.ConversationType,
                    dbResponse.ConversationResult,
                    dbResponse.Theses,
                    dbResponse.EmployeeTheses,
                    dbResponse.Agreements,
                    request.RequestDto.PerformanceReviewDate,
                    result.Status,
                    dbResponse.SupervisorId,
                    dbResponse.UserId,
                    request.RequestDto.Passed,
                    null,
                    dbResponse.Files.Select(x => new ConversationFileDto(x.Id, x.FileId, x.FileName)).ToList(),
                    dbResponse.DocumentLinks.Select(x => new ConversationDocumentLinkDto(x.Id, x.Link)).ToList(),
                    request.RequestDto.Salaries,
                    request.RequestDto.DevelopmentPlans);
            }
        }

        return new ConversationDto(
            dbResponse.Id,
            dbResponse.Date,
            dbResponse.ConversationType,
            dbResponse.ConversationResult,
            dbResponse.Theses,
            dbResponse.EmployeeTheses,
            dbResponse.Agreements,
            request.RequestDto.PerformanceReviewDate,
            dbResponse.PerformanceReviewStatus,
            dbResponse.SupervisorId,
            dbResponse.UserId,
            request.RequestDto.Passed,
            null,
            dbResponse.Files.Select(x => new ConversationFileDto(x.Id, x.FileId, x.FileName)).ToList(),
            dbResponse.DocumentLinks.Select(x => new ConversationDocumentLinkDto(x.Id, x.Link)).ToList(),
            request.RequestDto.Salaries,
            request.RequestDto.DevelopmentPlans);
    }

    private async Task ValidateUserAccessAsync(Conversation conversation, UpdateConversationDto requestDto, CancellationToken cancellationToken)
    {
        if (conversation.ConversationType != requestDto.ConversationType)
        {
            throw new EntityConflictException("Changing the conversation type is forbidden");
        }

        var userHasAccess = await _isUserHasAccessToConversation.IsCorrespondsToBusiness(
            new IsUserHasAccessToMutateConversation.Request(
                requestDto.UserId,
                requestDto.ConversationType,
                requestDto.PerformanceReviewDate,
                null,
                requestDto.Passed,
                requestDto.Salaries,
                requestDto.DevelopmentPlans),
            cancellationToken);

        if (!userHasAccess.IsSuccess)
        {
            throw new ActionNotAllowedException(userHasAccess.Reason!);
        }
    }

    public async Task<ConversationCoreDto> UpdateConversationAsync(Conversation conversation, UpdateConversationDto request, CancellationToken cancellationToken)
    {
        var linksToDelete = await _teamBoardDbContext.ConversationDocumentLinks
            .Where(x => x.ConversationId == conversation.Id).ToListAsync(cancellationToken);

        _teamBoardDbContext.ConversationDocumentLinks.RemoveRange(linksToDelete);

        var documentFilesToDelete = await _teamBoardDbContext.ConversationFiles
            .Where(x => x.ConversationId == conversation.Id).ToListAsync(cancellationToken);

        _teamBoardDbContext.ConversationFiles.RemoveRange(documentFilesToDelete);

        conversation.ConversationResult = request.ConversationResult;
        conversation.Theses = request.Theses;
        conversation.EmployeeTheses = request.EmployeeTheses;
        conversation.Agreements = request.Agreements;

        var newLinks = request.DocumentLinks
            .Select(x => new ConversationDocumentLink { ConversationId = conversation.Id, Link = x }).ToList();

        var newFiles = request.Files.Select(
            x => new ConversationFile()
            {
                ConversationId = conversation.Id,
                FileId = x.FileId,
                FileName = x.FileName,
                FileSize = x.FileSize,
            }).ToList();

        _teamBoardDbContext.ConversationFiles.AddRange(newFiles);
        _teamBoardDbContext.ConversationDocumentLinks.AddRange(newLinks);

        var performanceReviewAssignedToConversation = await _teamBoardDbContext.UserPerformanceReviews.FirstOrDefaultAsync(x => x.ConversationId == conversation.Id, cancellationToken);

        if (request.ConversationType == ConversationType.Conversation && performanceReviewAssignedToConversation != null)
        {
            var performanceReviewToRemove = await _teamBoardDbContext.UserPerformanceReviews
                .FirstOrDefaultAsync(x => x.Id == performanceReviewAssignedToConversation.Id, cancellationToken);
            if (performanceReviewToRemove != null)
            {
                _teamBoardDbContext.UserPerformanceReviews.Remove(performanceReviewToRemove);
            }

            performanceReviewAssignedToConversation.ConversationId = null;
        }

        await _teamBoardDbContext.SaveChangesAsync(cancellationToken);

        if (performanceReviewAssignedToConversation != null &&
            request.ConversationType == ConversationType.PerformanceReview)
        {
            return new ConversationCoreDto(
                request.Id,
                conversation.Date,
                conversation.ConversationType.GetDescription(),
                conversation.ConversationResult.ToString(),
                conversation.Theses,
                conversation.EmployeeTheses,
                conversation.Agreements,
                null,
                null,
                conversation.SupervisorId,
                conversation.UserId,
                performanceReviewAssignedToConversation.Id,
                newFiles.Select(x => new ConversationFileCoreDto(x.Id, x.FileId, x.FileName)).ToList(),
                newLinks.Select(x => new ConversationDocumentLinkCoreDto(x.Id, x.Link)).ToList());
        }

        return new ConversationCoreDto(request.Id,
            conversation.Date,
            conversation.ConversationType.GetDescription(),
            conversation.ConversationResult.ToString(),
            conversation.Theses,
            conversation.EmployeeTheses,
            conversation.Agreements,
            null,
            null,
            conversation.SupervisorId,
            conversation.UserId,
            null,
            newFiles.Select(x => new ConversationFileCoreDto(x.Id, x.FileId, x.FileName)).ToList(),
            newLinks.Select(x => new ConversationDocumentLinkCoreDto(x.Id, x.Link)).ToList());
    }

    internal sealed record Command(UpdateConversationDto RequestDto)
        : ICommand<ConversationDto>, ITransactionalRequest;

    internal sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.RequestDto).SetValidator(new UpdateConversationDto.UpdateConversationCommandValidator());
        }
    }
}
