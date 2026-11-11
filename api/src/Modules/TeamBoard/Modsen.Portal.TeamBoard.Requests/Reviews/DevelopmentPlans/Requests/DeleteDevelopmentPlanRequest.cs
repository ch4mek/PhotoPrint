using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Communication;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Identity.Contract;
using Modsen.Portal.Notification.Contract.Events;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.Requests.Core.BusinessRules;

namespace Modsen.Portal.TeamBoard.Requests.Reviews.DevelopmentPlans.Requests;

internal sealed class DeleteDevelopmentPlanRequest : ICommandHandler<DeleteDevelopmentPlanRequest.Command, int>
{
    private readonly ITeamBoardDbContext _teamBoardDbContext;
    private readonly ICanTeamBoardUserBeRetrievedBusinessRule _canTeamBoardUserBeRetrievedBusinessRule;
    private readonly ICommunicationBus _communicationBus;
    private readonly IIdentityContract _identityContract;
    public DeleteDevelopmentPlanRequest(
        ITeamBoardDbContext teamBoardDbContext,
        ICanTeamBoardUserBeRetrievedBusinessRule canTeamBoardUserBeRetrievedBusinessRule,
        ICommunicationBus communicationBus,
        IIdentityContract identityContract)
    {
        _teamBoardDbContext = teamBoardDbContext;
        _canTeamBoardUserBeRetrievedBusinessRule = canTeamBoardUserBeRetrievedBusinessRule;
        _communicationBus = communicationBus;
        _identityContract = identityContract;
    }

    public async Task<int> Handle(Command request, CancellationToken cancellationToken)
    {
        var developmentPlanEntity = await _teamBoardDbContext.DevelopmentPlans
            .Include(x => x.User)
            .FirstOrDefaultAsync(developmentPlan => developmentPlan.Id == request.Id, cancellationToken);

        if (developmentPlanEntity is null)
        {
            throw new EntityNotFoundException("Development plan is not exist");
        }

        if (developmentPlanEntity.User != null && developmentPlanEntity.User.Position.IsIntern())
        {
            throw new ActionNotAllowedException(PortalMessages.NotAllowedOperationWithIntern);
        }

        var isCanBeDeleted = await _canTeamBoardUserBeRetrievedBusinessRule
            .IsCorrespondsToBusiness(developmentPlanEntity.UserId, Portal.Core.AccessOptions.Manager, cancellationToken);

        if (!isCanBeDeleted)
        {
            throw new ActionNotAllowedException();
        }

        _teamBoardDbContext.DevelopmentPlans.Remove(developmentPlanEntity);

        await _teamBoardDbContext.SaveChangesAsync(cancellationToken);

        var approversIds = await _identityContract.GetHigherManagerIds(developmentPlanEntity.UserId, cancellationToken);
        var userIds = new List<int> { developmentPlanEntity.UserId };
        userIds.AddRange(approversIds);

        var notification = new DevelopmentPlanDeletedEvent
        {
            EventId = Guid.CreateVersion7().ToString(),
            DevelopmentPlanId = request.Id,
            UserIds = userIds
        };

        await _communicationBus.PublishAsync(notification, cancellationToken);

        return request.Id;
    }


    internal sealed record Command(int Id) : ICommand<int>, ITransactionalRequest;
}
