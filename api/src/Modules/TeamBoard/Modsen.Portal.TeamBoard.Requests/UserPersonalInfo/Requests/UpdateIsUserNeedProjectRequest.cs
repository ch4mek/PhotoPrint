using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.TeamBoard.Requests.UserPersonalInfo.Requests;

internal sealed class UpdateIsUserNeedProjectRequest : ICommandHandler<UpdateIsUserNeedProjectRequest.Command, bool>
{
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly ITeamBoardDbContext _teamBoardDbContext;

    public UpdateIsUserNeedProjectRequest(ITeamBoardDbContext teamBoardDbContext,
        IAuthenticatedUserContext authenticatedUserContext)
    {
        _authenticatedUserContext = authenticatedUserContext;
        _teamBoardDbContext = teamBoardDbContext;
    }

    public async Task<bool> Handle(Command request, CancellationToken cancellationToken)
    {
        var user = await _teamBoardDbContext.TeamBoardUsers
            .Where(user => user.UserId == request.UserId && user.IsActive)
            .Select(user => new
            {
                user.ResourceGroupId,
                user.DepartmentId
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (user is null)
        {
            throw new EntityNotFoundException(PortalMessages.UserNotFound);
        }

        if (_authenticatedUserContext.GetRoles().IsHeadOfDepartment() &&
            user.DepartmentId != _authenticatedUserContext.DepartmentId)
        {
            throw new ActionNotAllowedException(PortalMessages.YouCannotChangeUserWithDifferentDepartment);
        }

        if (_authenticatedUserContext.GetRoles().IsGroupManager() &&
            user.ResourceGroupId != _authenticatedUserContext.ResourceGroupId)
        {
            throw new ActionNotAllowedException(PortalMessages.YouCannotChangeUserWithDifferentGroup);
        }

        if (_authenticatedUserContext.GetRequiredUserId() == request.UserId)
        {
            throw new ActionNotAllowedException();
        }

        var userPayloadDetails = await _teamBoardDbContext.UserPayloadDetails
            .Where(payloadDetails => payloadDetails.UserId == request.UserId)
            .FirstOrDefaultAsync(cancellationToken);

        if (userPayloadDetails is not null)
        {
            userPayloadDetails.NeedProject = request.IsNeedProject;
            userPayloadDetails.UpdatedById = _authenticatedUserContext.UserId;
        }
        else
        {
            userPayloadDetails = new UserPayloadDetails
            {
                UserId = request.UserId,
                NeedProject = request.IsNeedProject,
                CreatedById = _authenticatedUserContext.UserId
            };

            _teamBoardDbContext.UserPayloadDetails.Add(userPayloadDetails);
        }

        await _teamBoardDbContext.SaveChangesAsync(cancellationToken);

        return request.IsNeedProject;
    }

    internal sealed record Command(int UserId, bool IsNeedProject) : ICommand<bool>, ITransactionalRequest;
}
