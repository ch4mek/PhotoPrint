using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.UseCases.ValidationRules;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.Reviews.DevelopmentPlans.Requests.RequestDto;

namespace Modsen.Portal.TeamBoard.Requests.Reviews.DevelopmentPlans.Requests;

internal sealed class UpdateDevelopmentPlanListRequest : ICommandHandler<UpdateDevelopmentPlanListRequest.Command, List<UpdateDevelopmentPlansDto>>
{
    private readonly ITeamBoardDbContext _teamBoardDbContext;
    private readonly ICanTeamBoardUserBeRetrievedBusinessRule _canTeamBoardUserBeRetrievedBusinessRule;
    public UpdateDevelopmentPlanListRequest(
        ITeamBoardDbContext teamBoardDbContext,
        ICanTeamBoardUserBeRetrievedBusinessRule canTeamBoardUserBeRetrievedBusinessRule)
    {
        _teamBoardDbContext = teamBoardDbContext;
        _canTeamBoardUserBeRetrievedBusinessRule = canTeamBoardUserBeRetrievedBusinessRule;
    }

    public async Task<List<UpdateDevelopmentPlansDto>> Handle(Command request, CancellationToken cancellationToken)
    {
        var selectedIds = request.Input.Select(x => x.Id).ToList();
        var developmentPlans = await _teamBoardDbContext.DevelopmentPlans
            .Where(dp => selectedIds.Contains(dp.Id))
            .Select(dp => new
            {
                dp.Id,
                dp.StartDate,
                dp.DeadlineDate,
                dp.EndDate,
            })
            .ToListAsync(cancellationToken);

        var invalidDate = request.Input
            .Where(inputPlan => inputPlan.EndDate.HasValue && inputPlan.DeadlineDate.HasValue && developmentPlans
                .Any(dbPlan => dbPlan.Id == inputPlan.Id &&
                    (inputPlan.EndDate.Value <= dbPlan.StartDate || inputPlan.DeadlineDate <= dbPlan.StartDate)))
            .ToList();

        if (invalidDate.Count != 0)
        {
            throw new InvalidDataAppException("One or more EndDates/DeadlineDates are earlier than StartDates");
        }

        var responses = await UpdateDevelopmentPlanList(request, cancellationToken);

        return responses;
    }

    private async Task<List<UpdateDevelopmentPlansDto>> UpdateDevelopmentPlanList(Command request, CancellationToken cancellationToken)
    {
        var responses = new List<UpdateDevelopmentPlansDto>();
        var ids = request.Input.Select(x => x.Id).ToList();
        var userDevPlans = await _teamBoardDbContext.DevelopmentPlans
            .Include(x => x.User)
            .Where(x => ids.Contains(x.Id))
            .ToListAsync(cancellationToken);

        await ValidateAsync(userDevPlans, cancellationToken);

        foreach (var developmentPlan in request.Input)
        {
            var devPlan = userDevPlans.FirstOrDefault(x => x.Id == developmentPlan.Id);
            if (devPlan != null)
            {
                devPlan.DeadlineDate = developmentPlan.DeadlineDate;
                devPlan.EndDate = developmentPlan.EndDate ?? devPlan.EndDate;
                devPlan.Summary = developmentPlan.Summary;
                devPlan.Notes = developmentPlan.Notes ?? devPlan.Notes;

                var response = new UpdateDevelopmentPlansDto
                (
                    devPlan.Id,
                    devPlan.DeadlineDate,
                    devPlan.EndDate,
                    devPlan.Summary,
                    devPlan.Notes
                );

                responses.Add(response);
            }
        }
        await _teamBoardDbContext.SaveChangesAsync(cancellationToken);
        return responses;
    }

    private async Task ValidateAsync(List<DevelopmentPlan> developmentPlans, CancellationToken cancellationToken)
    {
        foreach (var devPlan in developmentPlans)
        {
            if (devPlan.User != null && devPlan.User.Position.IsIntern())
            {
                throw new ActionNotAllowedException(PortalMessages.NotAllowedOperationWithIntern);
            }

            if (!await _canTeamBoardUserBeRetrievedBusinessRule.IsCorrespondsToBusiness(devPlan.UserId, AccessOptions.Manager, cancellationToken))
            {
                throw new ActionNotAllowedException();
            }
        }

    }
    internal sealed record Command(List<UpdateDevelopmentPlansDto> Input) : ICommand<List<UpdateDevelopmentPlansDto>>, ITransactionalRequest;

    internal sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleForEach(command => command.Input).SetValidator(new UpdateDevelopmentPlansDtoValidator());
        }

        private sealed class UpdateDevelopmentPlansDtoValidator : AbstractValidator<UpdateDevelopmentPlansDto>
        {
            public UpdateDevelopmentPlansDtoValidator()
            {
                RuleFor(x => x.Id).NotNull().NotEmpty();
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
