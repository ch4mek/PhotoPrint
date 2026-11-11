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

internal sealed class ActivateKitRequest : ICommandHandler<ActivateKitRequest.Command, KitStatusDto>
{
    private readonly IAuthenticatedUserContext _userContext;
    private readonly IMerchDbContext _merchDbContext;
    private readonly IClockService _clockService;

    public ActivateKitRequest(
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

        if (kit.Status == MerchItemStatus.Available)
        {
            return KitStatusDto.Of(kit);
        }

        var archivedKitItemNames = await _merchDbContext.KitItems
            .Where(kitItem => kitItem.KitId == kit.Id && kitItem.Item!.Status == MerchItemStatus.Archived)
            .Select(kitItem => kitItem.Item!.Name)
            .ToListAsync(cancellationToken);

        if (archivedKitItemNames.Count != 0)
        {
            throw new InvalidDataAppException("The kit contains the following archived items: "
                 + string.Join(", ", archivedKitItemNames));
        }

        kit.Status = MerchItemStatus.Available;
        kit.ArchivedAtUtc = null;
        kit.ArchivedById = null;
        kit.UpdatedAtUtc = _clockService.UtcNow;
        kit.UpdatedById = _userContext.UserId;

        await _merchDbContext.SaveChangesAsync(cancellationToken);

        return KitStatusDto.Of(kit);
    }

    internal sealed record Command(int KitId) : ICommand<KitStatusDto>, ITransactionalRequest;
}
