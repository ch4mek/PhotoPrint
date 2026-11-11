using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Communication;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Identity.Contract.Events;
using Modsen.Portal.Identity.DataAccess.Npgsql;

namespace Modsen.Portal.Identity.Requests.ResourceGroups.Requests
{
    internal sealed class SyncResourceGroupsRequest : ICommandHandler<SyncResourceGroupsRequest.Command, int>
    {
        private readonly IIdentityDbContext _identityDbContext;
        private readonly ICommunicationBus _communicationBus;

        public SyncResourceGroupsRequest(IIdentityDbContext identityDbContext,
            ICommunicationBus communicationBus)
        {
            _identityDbContext = identityDbContext;
            _communicationBus = communicationBus;
        }

        public async Task<int> Handle(Command request, CancellationToken cancellationToken)
        {
            var events = await _identityDbContext.ResourceGroups
                .AsNoTracking()
                .Select(x => new ResourceGroupSyncEvent
                {
                    EventId = Guid.CreateVersion7().ToString(),
                    ResourceGroupId = x.Id,
                    ResourceGroupName = x.Name,
                })
                .ToArrayAsync(CancellationToken.None);

            await _communicationBus.PublishAsync(events, CancellationToken.None);

            return events.Length;
        }

        internal sealed record Command : ICommand<int>, ITransactionalRequest;
    }
}
