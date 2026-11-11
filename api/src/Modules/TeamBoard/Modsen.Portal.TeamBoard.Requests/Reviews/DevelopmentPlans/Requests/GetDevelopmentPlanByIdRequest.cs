using FluentValidation;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TeamBoard.Requests.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.Reviews.DevelopmentPlans.Core;
using Modsen.Portal.TeamBoard.Requests.Reviews.DevelopmentPlans.Requests.RequestDto;

namespace Modsen.Portal.TeamBoard.Requests.Reviews.DevelopmentPlans.Requests;

internal sealed class GetDevelopmentPlanByIdRequest : IQueryHandler<GetDevelopmentPlanByIdRequest.Query, DevelopmentPlanDto>
{
    private readonly IDevelopmentPlanLoader _developmentPlanLoader;
    private readonly ICanTeamBoardUserBeRetrievedBusinessRule _canTeamBoardUserBeRetrievedBusinessRule;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;

    public GetDevelopmentPlanByIdRequest(IDevelopmentPlanLoader getDevelopmentPlanByIdDbQuery,
        ICanTeamBoardUserBeRetrievedBusinessRule canTeamBoardUserBeRetrievedBusinessRule,
        IAuthenticatedUserContext authenticatedUserContext)
    {
        _developmentPlanLoader = getDevelopmentPlanByIdDbQuery;
        _canTeamBoardUserBeRetrievedBusinessRule = canTeamBoardUserBeRetrievedBusinessRule;
        _authenticatedUserContext = authenticatedUserContext;
    }

    public async Task<DevelopmentPlanDto> Handle(Query request, CancellationToken cancellationToken)
    {
        var developmentPlan = await _developmentPlanLoader.LoadAsync(request.Id, cancellationToken);

        if (developmentPlan == null)
        {
            throw new EntityNotFoundException($"There is no development plan with {request.Id}");
        }

        if (developmentPlan.UserPosition.IsIntern())
        {
            throw new ActionNotAllowedException(PortalMessages.NotAllowedOperationWithIntern);
        }

        var canBeRetrieved = await _canTeamBoardUserBeRetrievedBusinessRule.IsCorrespondsToBusiness(developmentPlan.UserId, cancellationToken);

        if (!canBeRetrieved)
        {
            throw new ActionNotAllowedException();
        }

        var isSelfPlan = _authenticatedUserContext.GetRequiredUserId() == developmentPlan.UserId;

        return new DevelopmentPlanDto(
            developmentPlan.Id,
            developmentPlan.StartDate,
            developmentPlan.DeadlineDate,
            developmentPlan.EndDate,
            developmentPlan.Summary,
            isSelfPlan ? null : developmentPlan.Notes,
            developmentPlan.UserId);
    }

    internal sealed record Query(int Id) : IQuery<DevelopmentPlanDto>;

    internal sealed class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotNull().NotEmpty();
        }
    }
}
