using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.TeamBoard;
using Modsen.Portal.Core.UseCases.ValidationRules;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.Requests.Interviews.Core;
using Modsen.Portal.TeamBoard.Requests.Interviews.Core.AuthenticationFilters;
using Modsen.Portal.TeamBoard.Requests.Interviews.Core.Validators;
using Modsen.Portal.TeamBoard.Requests.Interviews.Requests.RequestDto;

namespace Modsen.Portal.TeamBoard.Requests.Interviews.Requests;

internal sealed class UpdateInterviewRequest : ICommandHandler<UpdateInterviewRequest.Command, UserInterviewDto>
{
    private readonly ITeamBoardDbContext _teamBoardDbContext;
    private readonly IInterviewLoader _interviewLoader;
    private readonly IInterviewsAuthenticationFilter _interviewsAuthenticationFilter;
    private readonly IInterviewUsersValidator _interviewUsersValidator;
    private readonly IInterviewSaleManagerValidator _interviewSalesManagerValidator;

    public UpdateInterviewRequest(
        ITeamBoardDbContext teamBoardDbContext,
        IInterviewLoader interviewLoader,
        IInterviewsAuthenticationFilter interviewsAuthenticationFilter,
        IInterviewUsersValidator interviewUsersValidator,
        IInterviewSaleManagerValidator interviewSalesManagerValidator)
    {
        _teamBoardDbContext = teamBoardDbContext;
        _interviewLoader = interviewLoader;
        _interviewsAuthenticationFilter = interviewsAuthenticationFilter;
        _interviewUsersValidator = interviewUsersValidator;
        _interviewSalesManagerValidator = interviewSalesManagerValidator;
    }

    public async Task<UserInterviewDto> Handle(Command request, CancellationToken cancellationToken)
    {
        var interviewToUpdate = await _interviewsAuthenticationFilter
            .Apply(_teamBoardDbContext.UserInterviews)
            .Where(x => x.IsActive)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new EntityNotFoundException("Interview not found");

        var interviewsGroup = await _interviewsAuthenticationFilter
            .Apply(_teamBoardDbContext.UserInterviews)
            .Where(x => x.IsActive)
            .Where(x => x.GroupId == interviewToUpdate.GroupId)
            .ToListAsync(cancellationToken);

        var user = await _teamBoardDbContext.TeamBoardUsers
            .FirstOrDefaultAsync(x => x.UserId == request.UserId, cancellationToken);

        if (user != null && user.Position.IsIntern())
        {
            throw new ActionNotAllowedException(PortalMessages.NotAllowedOperationWithIntern);
        }

        var saleManager = await _teamBoardDbContext.TeamBoardUsers
            .FirstOrDefaultAsync(x => x.UserId == request.SaleManagerId, cancellationToken);

        if (interviewsGroup[0].UserId != request.UserId)
        {
            await _interviewUsersValidator.Validate(new InterviewUsersValidator.Request(user), cancellationToken);
        }

        if (interviewsGroup[0].SalesResponsibleUserId != request.SaleManagerId)
        {
            await _interviewSalesManagerValidator.Validate(new InterviewSaleManagerValidator.Request(request.SaleManagerId, saleManager), cancellationToken, true);
        }

        foreach (var interviewFromGroup in interviewsGroup)
        {
            interviewFromGroup.UserId = request.UserId;
            interviewFromGroup.Specialization = request.Specialization;
            interviewFromGroup.ClientName = request.Client;
            interviewFromGroup.SalesResponsibleUserId = request.SaleManagerId;
            interviewFromGroup.JiraTask = request.JiraTask;
        }

        interviewToUpdate.UserInterviewFromUtc = request.InterviewTimeFromUtc;
        interviewToUpdate.UserInterviewDate = request.InterviewDate;
        interviewToUpdate.Status = request.Status;
        interviewToUpdate.InterviewType = request.Type;
        interviewToUpdate.Notes = request.Notes;
        interviewToUpdate.ReasonForRefusal = request.ReasonForRefusal;

        await _teamBoardDbContext.SaveChangesAsync(cancellationToken);

        return (await _interviewLoader.Load(interviewToUpdate.Id, cancellationToken))!;
    }

    internal sealed record Command(
        int Id,
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
        string? Notes) : ICommand<UserInterviewDto>, ITransactionalRequest;

    internal sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Id)
                .NotNull().NotEmpty();

            RuleFor(x => x.UserId)
                .NotNull().NotEmpty();

            RuleFor(x => x.Specialization)
                .NotNull().NotEmpty()
                .SpacesOrPunctuationSymbolsRule()
                .ShortTextType();

            When(x => x.Client != null, () =>
                RuleFor(x => x.Client)
                    .ShortTextType()
                    .SpacesOrPunctuationSymbolsRule());

            When(x => x.JiraTask != null, () =>
                RuleFor(x => x.JiraTask)
                    .LongTextType1(required: false));

            When(x => x.ReasonForRefusal != null, () =>
                RuleFor(x => x.ReasonForRefusal)
                    .ShortTextType()
                    .SpacesOrPunctuationSymbolsRule());

            When(x => x.Notes != null, () =>
                RuleFor(x => x.Notes)
                    .LongTextType1(required: false));
        }
    }
}
