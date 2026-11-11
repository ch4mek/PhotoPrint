using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.TeamBoard;
using Modsen.Portal.Core.UseCases.ValidationRules;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.Interviews.Core;
using Modsen.Portal.TeamBoard.Requests.Interviews.Core.Validators;
using Modsen.Portal.TeamBoard.Requests.Interviews.Requests.RequestDto;

namespace Modsen.Portal.TeamBoard.Requests.Interviews.Requests;

internal sealed class CreateInterviewRequest : ICommandHandler<CreateInterviewRequest.Command, UserInterviewDto>
{
    private readonly ITeamBoardDbContext _teamBoardDbContext;
    private readonly IInterviewLoader _interviewLoader;
    private readonly IInterviewUsersValidator _interviewUserValidator;
    private readonly IInterviewSaleManagerValidator _interviewSaleManagerValidator;

    public CreateInterviewRequest(
        ITeamBoardDbContext teamBoardDbContext,
        IInterviewLoader interviewLoader,
        IInterviewUsersValidator interviewUserValidator,
        IInterviewSaleManagerValidator interviewSaleManagerValidator)
    {
        _teamBoardDbContext = teamBoardDbContext;
        _interviewLoader = interviewLoader;
        _interviewUserValidator = interviewUserValidator;
        _interviewSaleManagerValidator = interviewSaleManagerValidator;
    }

    public async Task<UserInterviewDto> Handle(Command request, CancellationToken cancellationToken)
    {
        var user = await _teamBoardDbContext.TeamBoardUsers
            .FirstOrDefaultAsync(x => x.UserId == request.UserId, cancellationToken);

        if (user != null && user.Position.IsIntern())
        {
            throw new ActionNotAllowedException(PortalMessages.NotAllowedOperationWithIntern);
        }

        var saleManager = await _teamBoardDbContext.TeamBoardUsers
            .FirstOrDefaultAsync(x => x.UserId == request.SaleManagerId, cancellationToken);

        await _interviewUserValidator.Validate(new InterviewUsersValidator.Request(user), cancellationToken);
        await _interviewSaleManagerValidator.Validate(new InterviewSaleManagerValidator.Request(request.SaleManagerId, saleManager), cancellationToken, true);

        var newInterview = new UserInterview
        {
            UserId = request.UserId,
            UserInterviewFromUtc = request.InterviewTimeFromUtc,
            UserInterviewDate = request.InterviewDate,
            Specialization = request.Specialization,
            UserInterviewToUtc = null,
            ClientName = request.Client,
            SalesResponsibleUserId = request.SaleManagerId,
            Status = request.Status,
            InterviewType = request.Type,
            JiraTask = request.JiraTask,
            ReasonForRefusal = request.ReasonForRefusal,
            Notes = request.Notes,
            GroupId = Guid.CreateVersion7(),
            IsActive = true,
        };

        await _teamBoardDbContext.UserInterviews.AddAsync(newInterview, cancellationToken);
        await _teamBoardDbContext.SaveChangesAsync(cancellationToken);

        return (await _interviewLoader.Load(newInterview.Id, cancellationToken))!;
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
        string? Notes) : ICommand<UserInterviewDto>, ITransactionalRequest;

    internal sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.UserId)
                .NotNull().NotEmpty();

            RuleFor(x => x.Specialization)
                .NotNull().NotEmpty()
                .ShortTextType()
                .SpacesOrPunctuationSymbolsRule();

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
        }
    }
}
