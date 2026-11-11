using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Merch;
using Modsen.Portal.Merch.DataAccess.Npgsql;
using Modsen.Portal.Merch.DataAccess.Npgsql.Entities;
using Modsen.Portal.Merch.Requests.Core.Dto;

namespace Modsen.Portal.Merch.Requests.Transactions.Core.Composers;

internal interface IReceiptTransactionsComposer
{
    IEnumerable<ItemTransactionEntity> Compose(ReceiptTransactionsComposerRequest request);
    IAsyncEnumerable<ItemTransactionEntity> ComposeKit(
            int kitId,
            int amount,
            int warehouseId,
            string? notes,
            IReadOnlyCollection<SizeCountDto> requestedSizeCounts,
            CancellationToken cancellationToken);
}

internal sealed class ReceiptTransactionsComposer : IReceiptTransactionsComposer
{
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly IMerchDbContext _merchDbContext;
    public ReceiptTransactionsComposer(
        IAuthenticatedUserContext authenticatedUserContext,
        IMerchDbContext merchDbContext)
    {
        _authenticatedUserContext = authenticatedUserContext;
        _merchDbContext = merchDbContext;
    }

    public IEnumerable<ItemTransactionEntity> Compose(ReceiptTransactionsComposerRequest request)
    {
        var group = new ItemTransactionGroupEntity
        {
            Notes = request.Notes,
            CreatedById = _authenticatedUserContext.GetRequiredUserId()
        };

        foreach (var transaction in ComposeTransactions(request.WarehouseId, group, request.ItemId, request.SizeCounts))
        {
            yield return transaction;
        }
    }

    public async IAsyncEnumerable<ItemTransactionEntity> ComposeKit(
            int kitId, int amount, int warehouseId, string? notes, IReadOnlyCollection<SizeCountDto> requestedSizeCounts,
            [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var kit = await _merchDbContext.Kits
            .Include(x => x.KitItems)
            .ThenInclude(x => x.Item)
            .FirstOrDefaultAsync(x => x.Id == kitId, cancellationToken);

        foreach (var kitItem in kit?.KitItems ?? [])
        {
            var group = new ItemTransactionGroupEntity
            {
                Notes = notes,
                CreatedById = _authenticatedUserContext.GetRequiredUserId()
            };

            var sizeCounts = kitItem.Item!.IsClothing
                ? requestedSizeCounts
                : [new() { Count = amount * kitItem.Count, SizeId = "One" }];

            foreach (var transaction in ComposeTransactions(warehouseId, group, kitItem.ItemId, sizeCounts))
            {
                yield return transaction;
            }
        }
    }

    private IEnumerable<ItemTransactionEntity> ComposeTransactions(int warehouseId,
        ItemTransactionGroupEntity group,
        int itemId,
        IReadOnlyCollection<SizeCountDto> sizeCounts)
    {
        foreach (var sizeCount in sizeCounts)
        {
            yield return new ItemTransactionEntity()
            {
                Type = MerchTransactionType.Receipt,
                Group = group,
                ItemId = itemId,
                SizeId = sizeCount.SizeId,
                WarehouseId = warehouseId,
                Count = sizeCount.Count,
                WarehouseRecipientId = warehouseId,
                CreatedById = _authenticatedUserContext.GetRequiredUserId(),
                CreatedAtUtc = DateTime.UtcNow
            };

        }
    }
}

internal sealed record ReceiptTransactionsComposerRequest
{
    public int ItemId { get; set; }
    public IReadOnlyCollection<SizeCountDto> SizeCounts { get; set; } = [];
    public int WarehouseId { get; set; }
    public string? Notes { get; set; }
}
