using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.TeamBoard;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.Reviews.PerformanceReviews.Requests.RequestDto;

namespace Modsen.Portal.TeamBoard.Requests.Reviews.PerformanceReviews.Requests;

internal sealed class CreatePerformanceReviewRequest : ICommandHandler<CreatePerformanceReviewRequest.Command, PerformanceReviewDto>
{
    private readonly ITeamBoardDbContext _teamBoardDbContext;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly IClockService _clockService;
    private readonly ICanTeamBoardUserBeRetrievedBusinessRule _canTeamBoardUserBeRetrievedBusinessRule;
    public CreatePerformanceReviewRequest(
        ITeamBoardDbContext teamBoardDbContext,
        IAuthenticatedUserContext authenticatedUserContext,
        IClockService clockService,
        ICanTeamBoardUserBeRetrievedBusinessRule canTeamBoardUserBeRetrievedBusinessRule)
    {
        _teamBoardDbContext = teamBoardDbContext;
        _authenticatedUserContext = authenticatedUserContext;
        _clockService = clockService;
        _canTeamBoardUserBeRetrievedBusinessRule = canTeamBoardUserBeRetrievedBusinessRule;
    }

    public async Task<PerformanceReviewDto> Handle(Command request, CancellationToken cancellationToken)
    {
        var isPerformanceReviewForSameDateExisting = await _teamBoardDbContext.UserPerformanceReviews
            .AnyAsync(x => x.UserId == request.RequestDto.UserId && x.InterviewDateFromUtc == request.RequestDto.Date, cancellationToken);

        if (isPerformanceReviewForSameDateExisting)
        {
            throw new EntityConflictException("Performance review for the same date already exists");
        }

        var isIntern = await _teamBoardDbContext.TeamBoardUsers
            .AnyAsync(x => x.UserId == request.RequestDto.UserId && x.Position == PortalRoleNames.Intern, cancellationToken);

        if (isIntern)
        {
            throw new ActionNotAllowedException(PortalMessages.NotAllowedOperationWithIntern);
        }

        if (!await _canTeamBoardUserBeRetrievedBusinessRule.IsCorrespondsToBusiness(request.RequestDto.UserId, Portal.Core.AccessOptions.Manager, cancellationToken))
        {
            throw new ActionNotAllowedException();
        }

        await _teamBoardDbContext.UserPerformanceReviews.AddAsync(new UserPerformanceReview
        {
            InterviewDateFromUtc = request.RequestDto.Date,
            InterviewDateToUtc = request.RequestDto.Date.AddHours(1),
            UserId = request.RequestDto.UserId,
            Type = UserPerformanceReviewType.ScheduledPerformanceReview,
            Status = UserPerformanceReviewStatus.Finished,
            CreatedAtUtc = _clockService.UtcNow,
            CreatedById = _authenticatedUserContext.UserId,
            Agreements = null,
            EmployeeFeedback = null,
            ResponsibleUserFeedback = null,
            ConversationId = null
        }, cancellationToken);

        await _teamBoardDbContext.SaveChangesAsync(cancellationToken);

        return new PerformanceReviewDto
        {
            UserId = request.RequestDto.UserId,
            Date = request.RequestDto.Date
        };
    }

    internal sealed record Command(CreatePerformanceReviewDto RequestDto)
        : ICommand<PerformanceReviewDto>, ITransactionalRequest;

    internal sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.RequestDto).SetValidator(new CreatePerformanceReviewDto.Validator());
        }
    }

}
