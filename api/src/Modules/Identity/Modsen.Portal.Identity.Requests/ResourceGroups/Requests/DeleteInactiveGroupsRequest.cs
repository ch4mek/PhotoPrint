using MediatR;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Identity.DataAccess.Npgsql;

namespace Modsen.Portal.Identity.Requests.ResourceGroups.Requests;

internal sealed class DeleteInactiveGroupsRequest : ICommandHandler<DeleteInactiveGroupsRequest.Command, Unit>
{
    private readonly IIdentityDbContext _identityDbContext;

    public DeleteInactiveGroupsRequest(IIdentityDbContext identityDbContext)
    {
        _identityDbContext = identityDbContext;
    }

    public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
    {
        await _identityDbContext.ResourceGroups
            .AsQueryable()
            .Where(group => !group.IsActive && !_identityDbContext.Users.Any(user => user.ResourceGroupId == group.Id))
            .ExecuteDeleteAsync(cancellationToken);

        return Unit.Value;
    }

    internal sealed record Command : ICommand<Unit>, ITransactionalRequest;
}
