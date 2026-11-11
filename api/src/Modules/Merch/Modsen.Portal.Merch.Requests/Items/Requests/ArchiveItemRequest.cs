using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Merch;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Merch.DataAccess.Npgsql;
using Modsen.Portal.Merch.Requests.Items.Core.DbQueries;
using Modsen.Portal.Merch.Requests.Items.Requests.RequestDto;

namespace Modsen.Portal.Merch.Requests.Items.Requests;

internal sealed class ArchiveItemRequest : ICommandHandler<ArchiveItemRequest.Command, ItemStatusDto>
{
    private readonly IGetMerchItemByIdDbQuery _getItemDbQuery;
    private readonly IAuthenticatedUserContext _userContext;
    private readonly IMerchDbContext _merchDbContext;

    public ArchiveItemRequest(IGetMerchItemByIdDbQuery getItemDbQuery,
        IAuthenticatedUserContext userContext,
        IMerchDbContext merchDbContext)
    {
        _getItemDbQuery = getItemDbQuery;
        _userContext = userContext;
        _merchDbContext = merchDbContext;
    }

    public async Task<ItemStatusDto> Handle(Command request, CancellationToken cancellationToken)
    {
        var item = await _getItemDbQuery.ExecuteAsync(request.ItemId, cancellationToken)
            ?? throw new InvalidDataAppException(PortalMessages.MerchItemNotFound);

        if (item.Status == MerchItemStatus.Archived)
        {
            return ItemStatusDto.Of(item);
        }

        var totalInStock = await _merchDbContext.ItemTransactions
            .Where(transaction => transaction.ItemId == item.Id)
            .SumAsync(transaction => transaction.Count, cancellationToken);

        if (totalInStock != 0)
        {
            throw new InvalidDataAppException("Items with existing stock cannot be archived. " +
                "To archive this item write-off it from Storage tab");
        }

        var kitsIds = await _merchDbContext.KitItems
            .Where(kitItem => kitItem.ItemId == item.Id && kitItem.Kit!.Status == MerchItemStatus.Available)
            .Select(kitItem => kitItem.KitId)
            .ToListAsync(cancellationToken);

        if (kitsIds.Count > 0)
        {
            var kitsToArchive = await _merchDbContext.Kits
                .Where(kit => kitsIds.Contains(kit.Id))
                .ToListAsync(cancellationToken);

            foreach (var kit in kitsToArchive)
            {
                kit.Status = MerchItemStatus.Archived;

                kit.ArchivedById = _userContext.UserId;
                kit.ArchivedAtUtc = DateTime.UtcNow;

                kit.UpdatedById = _userContext.UserId;
                kit.UpdatedAtUtc = DateTime.UtcNow;
            }
        }

        item.Status = MerchItemStatus.Archived;
        item.ArchivedAtUtc = DateTime.UtcNow;
        item.ArchivedById = _userContext.UserId;

        item.UpdatedAtUtc = DateTime.UtcNow;
        item.UpdatedById = _userContext.UserId;

        await _merchDbContext.SaveChangesAsync(cancellationToken);

        return ItemStatusDto.Of(item);
    }

    internal sealed record Command(int ItemId) : ICommand<ItemStatusDto>, ITransactionalRequest;
}
