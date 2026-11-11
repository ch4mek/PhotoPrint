using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.UseCases.ValidationRules;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.Requests.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.Reviews.DevelopmentPlans.Core;
using Modsen.Portal.TeamBoard.Requests.Reviews.DevelopmentPlans.Requests.RequestDto;

namespace Modsen.Portal.TeamBoard.Requests.Reviews.DevelopmentPlans.Requests;

internal sealed class UpdateDevelopmentPlanRequest : ICommandHandler<UpdateDevelopmentPlanRequest.Command, DevelopmentPlanDto>{
    private readonly IDevelopmentPlanLoader _developmentPlanLoader;
    private readonly ICanTeamBoardUserBeRetrievedBusinessRule _canTeamBoardUserBeRetrievedBusinessRule;
    private readonly ITeamBoardDbContext _teamBoardDbContext;
    public UpdateDevelopmentPlanRequest(IDevelopmentPlanLoader getDevelopmentPlanByIdDbQuery,
        ICanTeamBoardUserBeRetrievedBusinessRule canTeamBoardUserBeRetrievedBusinessRule,
        ITeamBoardDbContext teamBoardDbContext)
    {
        _developmentPlanLoader = getDevelopmentPlanByIdDbQuery;
        _canTeamBoardUserBeRetrievedBusinessRule = canTeamBoardUserBeRetrievedBusinessRule;
        _teamBoardDbContext = teamBoardDbContext;
    }

    public async Task<DevelopmentPlanDto> Handle(Command request, CancellationToken cancellationToken)
    {
        var developmentPlan = await _developmentPlanLoader.LoadAsync(request.Id, cancellationToken);

        if (developmentPlan == null)
        {
            throw new EntityNotFoundException($"There is no development plan with Id {request.Id}");
        }

        if (developmentPlan.UserPosition.IsIntern())
        {
            throw new ActionNotAllowedException(PortalMessages.NotAllowedOperationWithIntern);
        }

        var canBeRetrieved = await _canTeamBoardUserBeRetrievedBusinessRule
            .IsCorrespondsToBusiness(developmentPlan.UserId, Portal.Core.AccessOptions.Manager, cancellationToken);

        if (!canBeRetrieved)
        {
            throw new ActionNotAllowedException();
        }

        if (request.DeadlineDate != null && request.DeadlineDate <= developmentPlan.StartDate)
        {
            throw new InvalidDataAppException("The date range is invalid");
        }

        var updatedDevelopmentPlan = await UpdateDevelopmentPlanAsync(request, cancellationToken);

        return new DevelopmentPlanDto(updatedDevelopmentPlan.Id, updatedDevelopmentPlan.StartDate,
            updatedDevelopmentPlan.DeadlineDate, updatedDevelopmentPlan.EndDate, updatedDevelopmentPlan.Summary,
            updatedDevelopmentPlan.Notes, updatedDevelopmentPlan.UserId);
    }

    private async Task<DataAccess.Npgsql.DatabaseEntities.DevelopmentPlan> UpdateDevelopmentPlanAsync(Command request, CancellationToken cancellationToken)
    {
        var updatedDevelopmentPlan = await _teamBoardDbContext.DevelopmentPlans.FirstAsync(x => x.Id == request.Id, cancellationToken);

        updatedDevelopmentPlan.DeadlineDate = request.DeadlineDate;
        updatedDevelopmentPlan.Summary = request.Summary;
        updatedDevelopmentPlan.Notes = request.Notes;

        await _teamBoardDbContext.SaveChangesAsync(cancellationToken);
        return updatedDevelopmentPlan;
    }

    internal sealed record Command(int Id, DateOnly? DeadlineDate, string Summary, string? Notes)
        : ICommand<DevelopmentPlanDto>, ITransactionalRequest;

    internal sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
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
