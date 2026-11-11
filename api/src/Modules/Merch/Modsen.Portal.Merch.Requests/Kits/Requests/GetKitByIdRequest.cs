using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Merch.DataAccess.Npgsql;
using Modsen.Portal.Merch.Requests.Kits.Requests.RequestDto;

namespace Modsen.Portal.Merch.Requests.Kits.Requests;

internal sealed class GetKitByIdRequest : IQueryHandler<GetKitByIdRequest.Query, KitDto>
{
    private readonly IMerchDbContext _dbContext;

    public GetKitByIdRequest(IMerchDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<KitDto> Handle(Query request, CancellationToken cancellationToken)
    {
        var kit = await _dbContext.Kits
            .AsSplitQuery()
            .Where(kit => kit.Id == request.Id)
            .Select(kit => new KitDto(
                kit.Id,
                kit.Name,
                kit.Status,
                kit.PhotoFileId,
                kit.Description,
                kit.KitItems
                    .Select(kitItem => new KitCompoundDto(
                        kitItem.ItemId,
                        kitItem.Item!.IsClothing,
                        kitItem.Item.Name,
                        kitItem.Count))
                    .ToList(),
                kit.KitMinAmounts
                    .Select(minAmount => new KitMinAmountDto(minAmount.WarehouseId, minAmount.MinAmount))
                    .ToList()
                )
            )
            .FirstOrDefaultAsync(cancellationToken);

        if (kit is null)
        {
            throw new EntityNotFoundException(PortalMessages.KitNotFound);
        }

        return kit;
    }

    internal sealed record Query(int Id) : IQuery<KitDto>;
}
