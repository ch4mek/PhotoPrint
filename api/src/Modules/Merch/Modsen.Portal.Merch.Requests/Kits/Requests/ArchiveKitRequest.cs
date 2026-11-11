using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Merch;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Merch.DataAccess.Npgsql;
using Modsen.Portal.Merch.Requests.Kits.Requests.RequestDto;

namespace Modsen.Portal.Merch.Requests.Kits.Requests;

internal sealed class ArchiveKitRequest : ICommandHandler<ArchiveKitRequest.Command, KitStatusDto>
{
    private readonly IAuthenticatedUserContext _userContext;
    private readonly IMerchDbContext _merchDbContext;
    private readonly IClockService _clockService;

    public ArchiveKitRequest(
        IAuthenticatedUserContext userContext,
        IMerchDbContext merchDbContext,
        IClockService clockService)
    {
        _userContext = userContext;
        _merchDbContext = merchDbContext;
        _clockService = clockService;
    }

    public async Task<KitStatusDto> Handle(Command request, CancellationToken cancellationToken)
    {
        var kit = await _merchDbContext.Kits.FirstOrDefaultAsync(k => k.Id == request.KitId, cancellationToken)
            ?? throw new InvalidDataAppException(PortalMessages.MerchKitNotFound);

        if (kit.Status == MerchItemStatus.Archived)
        {
            return KitStatusDto.Of(kit);
        }

        kit.Status = MerchItemStatus.Archived;
        kit.ArchivedAtUtc = _clockService.UtcNow;
        kit.ArchivedById = _userContext.UserId;
        kit.UpdatedAtUtc = _clockService.UtcNow;
        kit.UpdatedById = _userContext.UserId;

        await _merchDbContext.SaveChangesAsync(cancellationToken);

        return KitStatusDto.Of(kit);
    }

    internal sealed record Command(int KitId) : ICommand<KitStatusDto>, ITransactionalRequest;
}
