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
using Modsen.Portal.TeamBoard.Requests.Reviews.DevelopmentPlans.Requests.RequestDto;

namespace Modsen.Portal.TeamBoard.Requests.Reviews.DevelopmentPlans.Requests;

internal sealed class CreateDevelopmentPlanListRequest : ICommandHandler<CreateDevelopmentPlanListRequest.Command, List<CreateDevelopmentPlanDto>>
{
    private readonly ITeamBoardDbContext _teamBoardDbContext;
    private readonly ICanTeamBoardUserBeRetrievedBusinessRule _canTeamBoardUserBeRetrievedBusinessRule;
    public CreateDevelopmentPlanListRequest(
        ITeamBoardDbContext teamBoardDbContext,
        ICanTeamBoardUserBeRetrievedBusinessRule canTeamBoardUserBeRetrievedBusinessRule)
    {
        _teamBoardDbContext = teamBoardDbContext;
        _canTeamBoardUserBeRetrievedBusinessRule = canTeamBoardUserBeRetrievedBusinessRule;
    }

    public async Task<List<CreateDevelopmentPlanDto>> Handle(Command request, CancellationToken cancellationToken)
    {
        await ValidateAsync(request, cancellationToken);

        var developmentPlans = await CreateDevelopmentPlanListAsync(request, cancellationToken);

        var createdDevPlanList = developmentPlans.ConvertAll(x => new CreateDevelopmentPlanDto
        (
            Id: x.Id,
            StartDate: x.StartDate,
            DeadlineDate: x.DeadlineDate,
            EndDate: x.EndDate,
            Summary: x.Summary,
            Notes: x.Notes,
            UserId: x.UserId,
            ConversationId: x.ConversationId
        ));

        return createdDevPlanList;

        
    }

    private async Task ValidateAsync(Command request, CancellationToken cancellationToken)
    {
        var userIds = request.Input.Select(x => x.UserId).ToHashSet();

        var hasInterns = await _teamBoardDbContext.TeamBoardUsers
            .Where(x => userIds.Contains(x.UserId))
            .AnyAsync(x => x.Position == PortalRoleNames.Intern, cancellationToken);

        if (hasInterns)
        {
            throw new ActionNotAllowedException(PortalMessages.NotAllowedOperationWithIntern);
        }

        foreach (var devPlan in request.Input)
        {
            if (!await _canTeamBoardUserBeRetrievedBusinessRule.IsCorrespondsToBusiness(devPlan.UserId, AccessOptions.Manager, cancellationToken))
            {
                throw new ActionNotAllowedException();
            }
        }
    }

    private async Task<List<DataAccess.Npgsql.DatabaseEntities.DevelopmentPlan>> CreateDevelopmentPlanListAsync(Command request, CancellationToken cancellationToken)
    {
        var developmentPlans = request.Input.ConvertAll(x => new DataAccess.Npgsql.DatabaseEntities.DevelopmentPlan
        {
            StartDate = x.StartDate,
            DeadlineDate = x.DeadlineDate,
            EndDate = x.EndDate,
            Summary = x.Summary,
            Notes = x.Notes,
            UserId = x.UserId,
            Source = request.Source ?? DevelopmentPlanSource.Unknown,
            ConversationId = x.ConversationId
        });

        await _teamBoardDbContext.DevelopmentPlans.AddRangeAsync(developmentPlans, cancellationToken);
        await _teamBoardDbContext.SaveChangesAsync(cancellationToken);
        return developmentPlans;
    }

    internal sealed record Command(List<CreateDevelopmentPlanDto> Input, DevelopmentPlanSource? Source) : ICommand<List<CreateDevelopmentPlanDto>>, ITransactionalRequest;

    internal sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleForEach(command => command.Input).SetValidator(new DevelopmentPlansDtoValidator());
        }

        private sealed class DevelopmentPlansDtoValidator : AbstractValidator<CreateDevelopmentPlanDto>
        {
            public DevelopmentPlansDtoValidator()
            {
                RuleFor(x => x.StartDate).NotNull();
                When(x => x.DeadlineDate is not null, () => RuleFor(x => x.DeadlineDate)
                    .GreaterThan(x => x.StartDate));
                When(x => x.EndDate is not null, () => RuleFor(x => x.EndDate)
                    .GreaterThan(x => x.StartDate)
                    .WithMessage("The date range is invalid"));
                RuleFor(x => x.Summary).NotNull().NotEmpty()
                    .LongTextType()
                    .SpacesOrPunctuationSymbolsRule();
                RuleFor(x => x.Notes)
                    .BeEmptyNullOrBeLongTextType()
                    .SpacesOrPunctuationSymbolsRule();
            }
        }
    }
}
