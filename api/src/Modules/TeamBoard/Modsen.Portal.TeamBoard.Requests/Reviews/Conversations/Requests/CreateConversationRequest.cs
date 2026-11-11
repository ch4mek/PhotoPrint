using FluentValidation;
using Modsen.Portal.Core.Context;
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
internal sealed class CreateConversationRequest : ICommandHandler<CreateConversationRequest.Command, ConversationDto>
{
    private readonly ITeamBoardDbContext _teamBoardDbContext;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly ICreatePerformanceReviewComposer _createPerformanceReviewDbQuery;
    private readonly IIsPerformanceReviewForTodayExistingBusinessRule _isPerformanceReviewForTodayExistingBusinessRule;
    private readonly IUpdatePerformanceReviewComposer _updatePerformanceReviewDbQuery;
    private readonly IIsTheSamePerformanceReviewDbQuery _isTheSamePerformanceReviewDbQuery;
    private readonly IDevelopmentPlansApi _developmentPlansApi;
    private readonly IUserSalaryApi _userSalaryApi;
    private readonly IIsUserHasAccessToMutateConversation _isUserHasAccessToConversation;

    public CreateConversationRequest(
        IAuthenticatedUserContext authenticatedUserContext,
        ICreatePerformanceReviewComposer createPerformanceReviewDbQuery,
        IIsPerformanceReviewForTodayExistingBusinessRule isPerformanceReviewForTodayExistingBusinessRule,
        IUpdatePerformanceReviewComposer updatePerformanceReviewDbQuery,
        IIsTheSamePerformanceReviewDbQuery isTheSamePerformanceReviewDbQuery,
        IDevelopmentPlansApi developmentPlansApi,
        IUserSalaryApi userSalaryApi,
        ITeamBoardDbContext teamBoardDbContext,
        IIsUserHasAccessToMutateConversation isUserHasAccessToConversation)
    {
        _authenticatedUserContext = authenticatedUserContext;
        _createPerformanceReviewDbQuery = createPerformanceReviewDbQuery;
        _isPerformanceReviewForTodayExistingBusinessRule = isPerformanceReviewForTodayExistingBusinessRule;
        _updatePerformanceReviewDbQuery = updatePerformanceReviewDbQuery;
        _isTheSamePerformanceReviewDbQuery = isTheSamePerformanceReviewDbQuery;
        _developmentPlansApi = developmentPlansApi;
        _userSalaryApi = userSalaryApi;
        _teamBoardDbContext = teamBoardDbContext;
        _isUserHasAccessToConversation = isUserHasAccessToConversation;
    }

    public async Task<ConversationDto> Handle(Command request, CancellationToken cancellationToken)
    {
        await ValidateUserAccessAsync(request.RequestDto, cancellationToken);

        var supervisorId = _authenticatedUserContext.GetRequiredUserId();
        var conversation = await CreateConversationAsync(request.RequestDto, supervisorId, cancellationToken);

        AssignConversationIdToRelatedEntities(conversation.Id, request.RequestDto);
        await UpdateRelatedEntitiesAsync(request.RequestDto, cancellationToken);

        if (request.RequestDto.ConversationType == ConversationType.PerformanceReview)
        {
            return await HandlePerformanceReviewConversationAsync(conversation, request.RequestDto, cancellationToken);
        }

        return BuildConversationDto(conversation, request.RequestDto, request.RequestDto.Passed.ToString());
    }

    private async Task ValidateUserAccessAsync(CreateConversationDto requestDto, CancellationToken cancellationToken)
    {
        var userHasAccess = await _isUserHasAccessToConversation.IsCorrespondsToBusiness(
            new IsUserHasAccessToMutateConversation.Request(
                requestDto.UserId,
                requestDto.ConversationType,
                requestDto.PerformanceReviewDate,
                requestDto.UserPerformanceReviewId,
                requestDto.Passed,
                requestDto.Salaries,
                requestDto.DevelopmentPlans),
            cancellationToken);

        if (!userHasAccess.IsSuccess)
        {
            throw new ActionNotAllowedException(userHasAccess.Reason!);
        }
    }

    private static void AssignConversationIdToRelatedEntities(int conversationId, CreateConversationDto requestDto)
    {
        foreach (var developmentPlan in requestDto.DevelopmentPlans)
        {
            developmentPlan.ConversationId = conversationId;
        }

        foreach (var salary in requestDto.Salaries)
        {
            salary.ConversationId = conversationId;
        }
    }

    private async Task UpdateRelatedEntitiesAsync(CreateConversationDto requestDto, CancellationToken cancellationToken)
    {
        await _developmentPlansApi.UpdateDevelopmentPlanListWithCreating(requestDto.UserId, requestDto.DevelopmentPlans, cancellationToken);
        await _userSalaryApi.UpdateUserSalaryListWithCreating(requestDto.UserId, requestDto.Salaries, cancellationToken);
    }

    private async Task<ConversationDto> HandlePerformanceReviewConversationAsync(ConversationCoreDto conversation, CreateConversationDto requestDto, CancellationToken cancellationToken)
    {
        await ValidatePerformanceReviewAsync(requestDto, cancellationToken);

        if (requestDto.UserPerformanceReviewId == null)
        {
            return await CreatePerformanceReviewAndBuildDtoAsync(conversation, requestDto, cancellationToken);
        }

        return await UpdatePerformanceReviewAndBuildDtoAsync(conversation, requestDto, cancellationToken);
    }

    private async Task ValidatePerformanceReviewAsync(CreateConversationDto requestDto, CancellationToken cancellationToken)
    {
        if (requestDto.UserPerformanceReviewId == null)
        {
            await EnsureNoPerformanceReviewForTodayAsync(requestDto.UserId, requestDto.PerformanceReviewDate!.Value, cancellationToken);
            return;
        }

        var isTheSamePerformanceReview = await _isTheSamePerformanceReviewDbQuery.ExecuteAsync(
            requestDto.UserPerformanceReviewId.Value,
            requestDto.UserId,
            requestDto.PerformanceReviewDate!.Value,
            cancellationToken);

        if (!isTheSamePerformanceReview)
        {
            await EnsureNoPerformanceReviewForTodayAsync(requestDto.UserId, requestDto.PerformanceReviewDate!.Value, cancellationToken);
        }
    }

    private async Task EnsureNoPerformanceReviewForTodayAsync(int userId, DateTime performanceReviewDate, CancellationToken cancellationToken)
    {
        var result = await _isPerformanceReviewForTodayExistingBusinessRule.IsCorrespondsToBusiness(
            new IsPerformanceReviewForTodayExistingBusinessRuleRequestModel(userId, performanceReviewDate), cancellationToken);

        if (!result.IsSuccess)
        {
            throw new EntityConflictException(result.Reason!);
        }
    }

    private async Task<ConversationDto> CreatePerformanceReviewAndBuildDtoAsync(ConversationCoreDto conversation, CreateConversationDto requestDto, CancellationToken cancellationToken)
    {
        var createRequest = new CreatePerformanceReviewCoreDto(
            requestDto.UserId,
            requestDto.PerformanceReviewDate!,
            requestDto.Passed,
            requestDto.Theses,
            requestDto.EmployeeTheses,
            requestDto.Agreements,
            conversation.Id);

        var response = await _createPerformanceReviewDbQuery.Compose(createRequest, cancellationToken);
        return BuildConversationDto(conversation, requestDto, response.Status);
    }

    private async Task<ConversationDto> UpdatePerformanceReviewAndBuildDtoAsync(ConversationCoreDto conversation, CreateConversationDto requestDto, CancellationToken cancellationToken)
    {
        var updateRequest = new UpdatePerformanceReviewCoreDto(
            requestDto.UserPerformanceReviewId!.Value,
            conversation.Id,
            requestDto.PerformanceReviewDate!,
            requestDto.Theses,
            requestDto.EmployeeTheses,
            requestDto.Agreements,
            requestDto.Passed,
            conversation.UserId);

        var result = await _updatePerformanceReviewDbQuery.Compose(updateRequest, cancellationToken);

        if (result == null)
        {
            throw new EntityNotFoundException($"There is no performance review with Id {requestDto.UserPerformanceReviewId}");
        }

        return BuildConversationDto(conversation, requestDto, result.Status);
    }

    private static ConversationDto BuildConversationDto(ConversationCoreDto conversation, CreateConversationDto requestDto, string? performanceReviewStatus)
    {
        return new ConversationDto(
            conversation.Id,
            conversation.Date,
            conversation.ConversationType,
            conversation.ConversationResult,
            conversation.Theses,
            conversation.EmployeeTheses,
            conversation.Agreements,
            requestDto.PerformanceReviewDate,
            performanceReviewStatus,
            conversation.SupervisorId,
            conversation.UserId,
            requestDto.Passed,
            requestDto.UserPerformanceReviewId,
            conversation.Files.Select(x => new ConversationFileDto(x.Id, x.FileId, x.FileName)).ToList(),
            conversation.DocumentLinks.Select(x => new ConversationDocumentLinkDto(x.Id, x.Link)).ToList(),
            requestDto.Salaries,
            requestDto.DevelopmentPlans);
    }

    public async Task<ConversationCoreDto> CreateConversationAsync(CreateConversationDto request,
        int supervisorId,
        CancellationToken cancellationToken)
    {
        var conversation = new Conversation
        {
            Date = request.Date,
            ConversationType = request.ConversationType,
            ConversationResult = request.ConversationResult,
            Theses = request.Theses,
            EmployeeTheses = request.EmployeeTheses,
            Agreements = request.Agreements,
            SupervisorId = supervisorId,
            UserId = request.UserId,
        };

        var entry = await _teamBoardDbContext.Conversations.AddAsync(conversation, cancellationToken);
        await _teamBoardDbContext.SaveChangesAsync(cancellationToken);

        var conversationFiles = request.Files.Select(x => new ConversationFile()
        {
            ConversationId = entry.Entity.Id,
            FileId = x.FileId,
            FileName = x.FileName,
            FileSize = x.FileSize,
        }).ToList();

        var links = request.DocumentLinks.Select(x => new ConversationDocumentLink()
        {
            ConversationId = entry.Entity.Id,
            Link = x
        }).ToList();

        _teamBoardDbContext.ConversationDocumentLinks.AddRange(links);
        _teamBoardDbContext.ConversationFiles.AddRange(conversationFiles);
        await _teamBoardDbContext.SaveChangesAsync(cancellationToken);

        if (request.UserPerformanceReviewId != null)
        {
            var userPerformanceReview = _teamBoardDbContext.UserPerformanceReviews.FirstOrDefault(x => x.Id == request.UserPerformanceReviewId);
            userPerformanceReview!.ConversationId = entry.Entity.Id;

            await _teamBoardDbContext.SaveChangesAsync(cancellationToken);

            return new ConversationCoreDto(
                entry.Entity.Id,
                entry.Entity.Date,
                entry.Entity.ConversationType.GetDescription(),
                entry.Entity.ConversationResult.ToString(),
                entry.Entity.Theses,
                entry.Entity.EmployeeTheses,
                entry.Entity.Agreements,
                null,
                null,
                entry.Entity.SupervisorId,
                entry.Entity.UserId,
                userPerformanceReview.Id,
                conversationFiles.Select(x => new ConversationFileCoreDto(x.Id, x.FileId, x.FileName)).ToList(),
                links.Select(x => new ConversationDocumentLinkCoreDto(x.Id, x.Link)).ToList()
                );
        }

        return new ConversationCoreDto(
            entry.Entity.Id,
            entry.Entity.Date,
            entry.Entity.ConversationType.GetDescription(),
            entry.Entity.ConversationResult.ToString(),
            entry.Entity.Theses,
            entry.Entity.EmployeeTheses,
            entry.Entity.Agreements,
            null,
            null,
            entry.Entity.SupervisorId,
            entry.Entity.UserId,
            null,
            conversationFiles.Select(x => new ConversationFileCoreDto(x.Id, x.FileId, x.FileName)).ToList(),
            links.Select(x => new ConversationDocumentLinkCoreDto(x.Id, x.Link)).ToList());
    }

    internal sealed record Command(
        CreateConversationDto RequestDto) : ICommand<ConversationDto>, ITransactionalRequest;

    internal sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.RequestDto).SetValidator(new CreateConversationDto.CreateConversationCommandValidator());
        }
    }
}
