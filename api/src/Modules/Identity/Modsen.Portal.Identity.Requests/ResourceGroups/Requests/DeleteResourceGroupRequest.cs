using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.Requests.ResourceGroups.Core.BusinessRules;

namespace Modsen.Portal.Identity.Requests.ResourceGroups.Requests;

internal sealed class DeleteResourceGroupRequest : ICommandHandler<DeleteResourceGroupRequest.Command, bool>
{
    private readonly IIdentityDbContext _identityDbContext;
    private readonly ICanUserUpdateGroupBusinessRule _canDeleteGroupBusinessRule;

    public DeleteResourceGroupRequest(
        IIdentityDbContext identityDbContext,
        ICanUserUpdateGroupBusinessRule canDeleteGroupBusinessRule)
    {
        _identityDbContext = identityDbContext;
        _canDeleteGroupBusinessRule = canDeleteGroupBusinessRule;
    }

    public async Task<bool> Handle(Command request, CancellationToken cancellationToken)
    {
        var group = await _identityDbContext.ResourceGroups
            .FirstOrDefaultAsync(group => group.Id == request.Id, cancellationToken);

        if (group is null)
        {
            throw new EntityNotFoundException(PortalMessages.GroupNotFound);
        }

        var isUserCanDeleteGroup = await _canDeleteGroupBusinessRule.IsCorrespondsToBusiness(
            new CanUserUpdateGroupBusinessRule.Request(group.DepartmentId),
            cancellationToken);

        if (!isUserCanDeleteGroup.IsSuccess)
        {
            throw new ActionNotAllowedException(isUserCanDeleteGroup.Reason!);
        }

        var isGroupHaveUsers = await _identityDbContext.Users.AnyAsync(
            u => u.IsActive && u.ResourceGroupId == request.Id,
            cancellationToken);

        if (isGroupHaveUsers)
        {
            throw new ActionNotAllowedException(PortalMessages.GroupCannotBeDeleted);
        }

        _identityDbContext.ResourceGroups.Remove(group);

        await _identityDbContext.SaveChanges(cancellationToken);

        return true;
    }

    internal sealed record Command(int Id) : ICommand<bool>, ITransactionalRequest;
}
