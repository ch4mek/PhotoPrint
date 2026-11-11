using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Communication;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Identity.Contract.Events;
using Modsen.Portal.Identity.DataAccess.Npgsql;

namespace Modsen.Portal.Identity.Requests.Users.Requests
{
    internal sealed class SyncUsersRequest : ICommandHandler<SyncUsersRequest.Command, int>
    {
        private readonly IIdentityDbContext _identityDbContext;
        private readonly ICommunicationBus _communicationBus;

        public SyncUsersRequest(
            IIdentityDbContext identityDbContext,
            ICommunicationBus communicationBus)
        {
            _identityDbContext = identityDbContext;
            _communicationBus = communicationBus;
        }

        public async Task<int> Handle(Command request, CancellationToken cancellationToken)
        {
            var events = await _identityDbContext.Users
            .AsNoTracking()
            .Where(x => x.UserRoles.Any())
            .Select(x => new UserSyncEvent
            {
                Email = x.Email!,
                UserId = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
                FirstNameRu = x.FirstNameRu,
                LastNameRu = x.LastNameRu,
                PatronymicRu = x.PatronymicRu,
                DepartmentId = x.DepartmentId,
                ResourceGroupId = x.ResourceGroupId,
                CountryId = x.CountryId,
                Position = x.UserRoles.Select(f => f.Role!.Name).FirstOrDefault(),
                AvatarFileId = x.AvatarFileId,
                Color = x.Color,
                Location = x.Location,
                PositionOrderWeight = x.UserRoles.First().Role!.OrderWeight,
                EventId = Guid.CreateVersion7().ToString(),
                RegisteredLocationId = x.RegisteredLocationId,
                IsActive = x.IsActive,
                HireDate = x.HireDate,
                CurrentLocation = x.CurrentLocation,
                CreatedById = x.CreatedById
            })
            .ToArrayAsync(CancellationToken.None);

            await _communicationBus.PublishAsync(events, CancellationToken.None);

            return events.Length;
        }

        internal sealed record Command : ICommand<int>, ITransactionalRequest;
    }
}
