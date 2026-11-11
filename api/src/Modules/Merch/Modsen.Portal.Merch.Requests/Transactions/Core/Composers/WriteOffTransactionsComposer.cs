using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Merch;
using Modsen.Portal.Identity.Contract;
using Modsen.Portal.Merch.DataAccess.Npgsql;
using Modsen.Portal.Merch.DataAccess.Npgsql.Entities;
using Modsen.Portal.Merch.Requests.Core.Dto;

namespace Modsen.Portal.Merch.Requests.Transactions.Core.Composers;

internal interface IWriteOffTransactionsComposer
{
    IAsyncEnumerable<ItemTransactionEntity> Compose(WriteOffTransactionsComposerRequest request, CancellationToken cancellationToken);
    IAsyncEnumerable<ItemTransactionEntity> ComposeKit(
            int kitId, int amount, int warehouseId, int recipientUserId, string? notes, IReadOnlyCollection<SizeCountDto> requestedSizeCounts,
            CancellationToken cancellationToken);
}

internal sealed class WriteOffTransactionsComposer : IWriteOffTransactionsComposer
{
    private readonly IMerchDbContext _merchDbContext;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly IIdentityContract _identityContract;
    public WriteOffTransactionsComposer(
        IMerchDbContext merchDbContext,
        IAuthenticatedUserContext authenticatedUserContext,
        IIdentityContract identityContract)
    {
        _merchDbContext = merchDbContext;
        _authenticatedUserContext = authenticatedUserContext;
        _identityContract = identityContract;
    }

    public async IAsyncEnumerable<ItemTransactionEntity> Compose(
        WriteOffTransactionsComposerRequest request,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var group = new ItemTransactionGroupEntity
        {
            Notes = request.Notes,
            CreatedById = _authenticatedUserContext.GetRequiredUserId()
        };

        await ValidateUserAbleRecieveCount(request.UserRecipientId, cancellationToken);

        await foreach (var transaction in ComposeTransactions(request.ItemId, request.WarehouseId,
            request.UserRecipientId, group, request.SizeCounts, cancellationToken))
        {
            yield return transaction;
        }
    }


    public async IAsyncEnumerable<ItemTransactionEntity> ComposeKit(
            int kitId, int amount, int warehouseId, int recipientUserId, string? notes, IReadOnlyCollection<SizeCountDto> requestedSizeCounts,
            [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var kit = await _merchDbContext.Kits
            .Include(x => x.KitItems)
            .ThenInclude(x => x.Item)
            .FirstOrDefaultAsync(x => x.Id == kitId, cancellationToken);

        await ValidateUserAbleRecieveCount(recipientUserId, cancellationToken);

        foreach (var kititem in kit?.KitItems ?? [])
        {
            var group = new ItemTransactionGroupEntity
            {
                Notes = notes,
                CreatedById = _authenticatedUserContext.GetRequiredUserId()
            };

            var sizeCounts = kititem.Item!.IsClothing
                ? requestedSizeCounts
                : [new() { Count = amount * kititem.Count, SizeId = "One" }];

            await foreach (var transaction in ComposeTransactions(kititem.ItemId,
                warehouseId, recipientUserId, group, sizeCounts, cancellationToken))
            {
                yield return transaction;
            }
        }
    }

    private async IAsyncEnumerable<ItemTransactionEntity> ComposeTransactions(
        int itemId,
        int warehouseId,
        int userRecipientId,
        ItemTransactionGroupEntity group,
        IReadOnlyCollection<SizeCountDto> sizeCounts,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        foreach (var sizeCount in sizeCounts)
        {
            var itemsCount = await AvailableItemsCount(
                warehouseId,
                itemId,
                sizeCount.SizeId,
                cancellationToken);

            if (itemsCount < sizeCount.Count)
            {
                throw new InvalidDataAppException("You cannot write off the requested amount. The amount is bigger than available in storage.");
            }

            yield return new ItemTransactionEntity()
            {
                Type = MerchTransactionType.WriteOff,
                Group = group,
                ItemId = itemId,
                SizeId = sizeCount.SizeId,
                WarehouseId = warehouseId,
                Count = -sizeCount.Count,
                SenderId = warehouseId,
                UserRecipientId = userRecipientId,
                CreatedById = _authenticatedUserContext.GetRequiredUserId(),
                CreatedAtUtc = DateTime.UtcNow
            };
        }
    }
    private Task<int> AvailableItemsCount(
        int warehouseId,
        int itemId,
        string sizeId,
        CancellationToken cancellationToken)
    {
        return _merchDbContext.ItemTransactions
            .Where(transaction => transaction.WarehouseId == warehouseId)
            .Where(transaction => transaction.ItemId == itemId)
            .Where(transaction => transaction.SizeId == sizeId)
            .SumAsync(transaction => transaction.Count, cancellationToken);
    }

    private async Task ValidateUserAbleRecieveCount(int userId, CancellationToken cancellationToken)
    {
        if (!await _identityContract.IsActive(userId, cancellationToken))
        {
            throw new InvalidDataAppException(PortalMessages.UserIsInactive);
        }
    }
}

internal sealed record WriteOffTransactionsComposerRequest
{
    public int ItemId { get; set; }
    public IReadOnlyCollection<SizeCountDto> SizeCounts { get; set; } = [];
    public int WarehouseId { get; set; }
    public string? Notes { get; set; }
    public int UserRecipientId { get; set; }
}
