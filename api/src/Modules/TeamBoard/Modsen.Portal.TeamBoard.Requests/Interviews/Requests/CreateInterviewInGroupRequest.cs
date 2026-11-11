using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.TeamBoard;
using Modsen.Portal.Core.UseCases.ValidationRules;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.Interviews.Core;
using Modsen.Portal.TeamBoard.Requests.Interviews.Core.AuthenticationFilters;
using Modsen.Portal.TeamBoard.Requests.Interviews.Core.Validators;
using Modsen.Portal.TeamBoard.Requests.Interviews.Requests.RequestDto;

namespace Modsen.Portal.TeamBoard.Requests.Interviews.Requests;

internal sealed class CreateInterviewInGroupRequest : ICommandHandler<CreateInterviewInGroupRequest.Command, UserInterviewDto>
{
    private readonly ITeamBoardDbContext _teamBoardDbContext;
    private readonly IInterviewLoader _interviewLoader;
    private readonly IInterviewsAuthenticationFilter _interviewsAuthenticationFilter;
    private readonly IInterviewUsersValidator _interviewUsersValidator;
    private readonly IInterviewSaleManagerValidator _interviewSaleManagersValidator;

    public CreateInterviewInGroupRequest(
        ITeamBoardDbContext teamBoardDbContext,
        IInterviewLoader interviewLoader,
        IInterviewsAuthenticationFilter interviewsAuthenticationFilter,
        IInterviewUsersValidator interviewUsersValidator,
        IInterviewSaleManagerValidator interviewSaleManagersValidator)
    {
        _teamBoardDbContext = teamBoardDbContext;
        _interviewLoader = interviewLoader;
        _interviewsAuthenticationFilter = interviewsAuthenticationFilter;
        _interviewUsersValidator = interviewUsersValidator;
        _interviewSaleManagersValidator = interviewSaleManagersValidator;
    }

    public async Task<UserInterviewDto> Handle(Command request, CancellationToken cancellationToken)
    {
        var user = await _teamBoardDbContext.TeamBoardUsers
            .FirstOrDefaultAsync(x => x.UserId == request.UserId, cancellationToken);
        var saleManager = await _teamBoardDbContext.TeamBoardUsers
            .FirstOrDefaultAsync(x => x.UserId == request.SaleManagerId, cancellationToken);

        var interviewGroup = await _interviewsAuthenticationFilter.Apply(_teamBoardDbContext.UserInterviews)
            .Where(x => x.IsActive)
            .Where(x => x.GroupId == request.GroupId)
            .ToListAsync(cancellationToken);

        if (interviewGroup.Count == 0)
        {
            throw new EntityNotFoundException("Interview group not found");
        }

        if (request.UserId != interviewGroup[0].UserId)
        {
            await _interviewUsersValidator.Validate(new InterviewUsersValidator.Request(user), cancellationToken);
        }

        if (request.SaleManagerId != interviewGroup[0].SalesResponsibleUserId)
        {
            await _interviewSaleManagersValidator.Validate(new InterviewSaleManagerValidator.Request(request.SaleManagerId, saleManager), cancellationToken);
        }

        foreach (var interviewsToUpdate in interviewGroup)
        {
            interviewsToUpdate.UserId = request.UserId;
            interviewsToUpdate.Specialization = request.Specialization;
            interviewsToUpdate.ClientName = request.Client;
            interviewsToUpdate.SalesResponsibleUserId = request.SaleManagerId;
            interviewsToUpdate.JiraTask = request.JiraTask;
            interviewsToUpdate.Status = UserInterviewStatus.Success;
        }

        var newInterviewStage = new UserInterview
        {
            UserId = request.UserId,
            Specialization = request.Specialization,
            UserInterviewDate = request.InterviewDate,
            UserInterviewFromUtc = request.InterviewTimeFromUtc,
            UserInterviewToUtc = null,
            ClientName = request.Client,
            Status = request.Status,
            InterviewType = request.Type,
            SalesResponsibleUserId = request.SaleManagerId,
            JiraTask = request.JiraTask,
            Notes = request.Notes,
            ReasonForRefusal = request.ReasonForRefusal,
            GroupId = request.GroupId,
            IsActive = true,
        };

        await _teamBoardDbContext.UserInterviews.AddAsync(newInterviewStage, cancellationToken);
        await _teamBoardDbContext.SaveChangesAsync(cancellationToken);

        return (await _interviewLoader.Load(newInterviewStage.Id, cancellationToken))!;
    }

    internal sealed record Command(
        int UserId,
        string Specialization,
        string? Client,
        string? JiraTask,
        UserInterviewType? Type,
        UserInterviewStatus? Status,
        DateOnly? InterviewDate,
        TimeOnly? InterviewTimeFromUtc,
        int? SaleManagerId,
        string? ReasonForRefusal,
        string? Notes,
        Guid GroupId) : ICommand<UserInterviewDto>, ITransactionalRequest;

    internal sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.UserId)
                .NotNull().NotEmpty();

            RuleFor(x => x.Specialization)
                .NotNull().NotEmpty()
                .SpacesOrPunctuationSymbolsRule()
                .ShortTextType();

            When(x => x.Client != null, () =>
                RuleFor(x => x.Client)
                    .SpacesOrPunctuationSymbolsRule()
                    .ShortTextType());

            When(x => x.JiraTask != null, () =>
                RuleFor(x => x.JiraTask)
                    .LongTextType1(required: false));

            When(x => x.ReasonForRefusal != null, () =>
                RuleFor(x => x.ReasonForRefusal)
                    .SpacesOrPunctuationSymbolsRule()
                    .ShortTextType());

            When(x => x.Notes != null, () =>
                RuleFor(x => x.Notes)
                    .LongTextType1(required: false));

            RuleFor(x => x.GroupId)
                .NotNull().NotEmpty();
        }
    }
}
