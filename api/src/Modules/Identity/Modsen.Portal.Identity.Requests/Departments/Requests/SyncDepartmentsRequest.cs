using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Communication;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Identity.Contract.Events;
using Modsen.Portal.Identity.Contract.Models;
using Modsen.Portal.Identity.DataAccess.Npgsql;

namespace Modsen.Portal.Identity.Requests.Departments.Requests
{
    internal sealed class SyncDepartmentsRequest : ICommandHandler<SyncDepartmentsRequest.Command, int>
    {
        private readonly IIdentityDbContext _identityDbContext;
        private readonly ICommunicationBus _communicationBus;

        public SyncDepartmentsRequest(IIdentityDbContext identityDbContext,
            ICommunicationBus communicationBus)
        {
            _identityDbContext = identityDbContext;
            _communicationBus = communicationBus;
        }

        public async Task<int> Handle(Command request, CancellationToken cancellationToken)
        {
            var departments = await _identityDbContext.Departments
                .AsNoTracking()
                .Select(x => new DepartmentDto
                {
                    Id = x.Id,
                    ShortName = x.ShortName,
                    FullName = x.FullName,
                    DepartmentOrderWeight = x.OrderWeight,
                    DepartmentReportingToRoleName = x.DepartmentReportingToRole!.Name!,
                    DepartmentReportingToRoleId = x.DepartmentReportingToRole.Id,
                })
                .ToListAsync(CancellationToken.None);

            var events = departments
                .Select(x => new DepartmentSyncEvent
                {
                    DepartmentFullName = x.FullName,
                    DepartmentId = x.Id,
                    DepartmentShortName = x.ShortName,
                    EventId = Guid.CreateVersion7().ToString(),
                    DepartmentOrderWeight = x.DepartmentOrderWeight,
                    DepartmentReportingToRoleId = x.DepartmentReportingToRoleId,
                    DepartmentReportingToRoleName = x.DepartmentReportingToRoleName,
                })
                .ToArray();

            await _communicationBus.PublishAsync(events, CancellationToken.None);

            return events.Length;
        }

        internal sealed record Command : ICommand<int>, ITransactionalRequest;
    }
}
