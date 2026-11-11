
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.Requests.IdentityPortalSettings;

namespace Modsen.Portal.Identity.Requests.Messages.Core.DbQueries
{
    internal interface IGetUserThreadIdDbQuery
    {
        Task<string?> ExecuteAsync(string? referenceId, MessageSource source, CancellationToken cancellationToken);
    }
    internal class GetUserThreadIdDbQuery : IGetUserThreadIdDbQuery
    {
        private readonly IIdentityPortalSettingsAccessor _identityPortalSettingsAccessor;
        private readonly IIdentityDbContext _identityDbContext;
        private readonly IClockService _clockService;

        public GetUserThreadIdDbQuery(
            IIdentityDbContext identityDbContext,
            IClockService clockService,
            IIdentityPortalSettingsAccessor identityPortalSettingsAccessor)
        {
            _identityDbContext = identityDbContext;
            _clockService = clockService;
            _identityPortalSettingsAccessor = identityPortalSettingsAccessor;
        }

        public async Task<string?> ExecuteAsync(string? referenceId, MessageSource source, CancellationToken cancellationToken)
        {
            var settings = await _identityPortalSettingsAccessor.GetSettings(cancellationToken);
            var absoluteDate = _clockService.UtcNow.AddMinutes(-settings.DigitalHrContextChangeTimeInMinutes);

            var threadId = await _identityDbContext.CustomDetails
                .Where(x => x.ReferenceType == "UserEmail")
                .Where(x => x.ReferenceId == referenceId)
                .Where(x => x.CreatedAtUtc > absoluteDate)
                .Where(x => x.MessageSource == source)
                .OrderByDescending(x => x.CreatedAtUtc)
                .Select(x => x.Value)
                .FirstOrDefaultAsync(cancellationToken);

            return threadId;
        }
    }
}
