using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Merch;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.UseCases.ValidationRules;
using Modsen.Portal.Merch.DataAccess.Npgsql;
using Modsen.Portal.Merch.Requests.Core.Dto;
using Modsen.Portal.Merch.Requests.Transactions.Core.Composers;

namespace Modsen.Portal.Merch.Requests.Transactions.Requests;

internal sealed class AddItemTransactionRequest : ICommandHandler<AddItemTransactionRequest.Command, int>
{
    private readonly IReceiptTransactionsComposer _receiptTransactionsComposer;
    private readonly IWriteOffTransactionsComposer _writeOffTransactionsComposer;
    private readonly ITransferTransactionsComposer _transferTransactionsComposer;
    private readonly IMerchDbContext _merchDbContext;

    public AddItemTransactionRequest(
        IReceiptTransactionsComposer receiptTransactionsComposer,
        IWriteOffTransactionsComposer writeOffTransactionsComposer,
        ITransferTransactionsComposer transferTransactionsComposer,
        IMerchDbContext merchDbContext)
    {
        _receiptTransactionsComposer = receiptTransactionsComposer;
        _writeOffTransactionsComposer = writeOffTransactionsComposer;
        _transferTransactionsComposer = transferTransactionsComposer;
        _merchDbContext = merchDbContext;
    }

    public async Task<int> Handle(Command request, CancellationToken cancellationToken)
    {
        await CheckEntitiesExistAsync(request, cancellationToken);

        if (request.SizeCounts.All(x => x.Count == 0))
        {
            throw new InvalidDataAppException("You cannot add transaction with 0 merch items");
        }

        var result = 0;

        if (request.TransactionType == MerchTransactionType.Receipt)
        {
            var transactions = _receiptTransactionsComposer.Compose(new ReceiptTransactionsComposerRequest
            {
                ItemId = request.ItemId,
                SizeCounts = request.SizeCounts,
                WarehouseId = request.WarehouseId,
                Notes = request.Notes
            }).ToArray();

            _merchDbContext.ItemTransactions.AddRange(transactions);
            result = transactions.Length;
        }
        else
        {
            var transactionsStream = request.TransactionType switch
            {
                MerchTransactionType.WriteOff => _writeOffTransactionsComposer.Compose(new WriteOffTransactionsComposerRequest
                {
                    ItemId = request.ItemId,
                    SizeCounts = request.SizeCounts,
                    WarehouseId = request.WarehouseId,
                    Notes = request.Notes,
                    UserRecipientId = request.UserRecipientId!.Value
                }, cancellationToken),
                MerchTransactionType.Transfer => _transferTransactionsComposer.Compose(new TransferTransactionsComposerRequest
                {
                    ItemId = request.ItemId,
                    SizeCounts = request.SizeCounts,
                    WarehouseId = request.WarehouseId,
                    Notes = request.Notes,
                    RecipientWarehouseId = request.WarehouseRecipientId!.Value,
                }, cancellationToken),
                _ => throw new InvalidDataAppException("Merch transaction type is not exist"),
            };

            await foreach (var transaction in transactionsStream.WithCancellation(cancellationToken))
            {
                _merchDbContext.ItemTransactions.Add(transaction);
                result++;
            }
        }

        await _merchDbContext.SaveChangesAsync(cancellationToken);

        return result;
    }

    private async Task CheckEntitiesExistAsync(Command request, CancellationToken cancellationToken)
    {
        var itemInfo = await _merchDbContext.Items
            .Where(item => item.Id == request.ItemId)
            .Include(item => item.ItemSizeMinAmounts)
            .FirstOrDefaultAsync(cancellationToken);

        if (itemInfo is null)
        {
            throw new EntityNotFoundException("Item is not exist");
        }

        var isItemContainAllSizes = request.SizeCounts
            .All(sizeCount => itemInfo.ItemSizeMinAmounts
                .Any(sizeMinAmount => sizeMinAmount.SizeId == sizeCount.SizeId && sizeMinAmount.WarehouseId == request.WarehouseId));

        if (!isItemContainAllSizes)
        {
            throw new EntityNotFoundException("Item is not contain size");
        }

        var warehouse = await _merchDbContext.Warehouses
            .FirstOrDefaultAsync(warehouse => warehouse.Id == request.WarehouseId, cancellationToken);

        if (warehouse is null)
        {
            throw new EntityNotFoundException("Warehouse is not exist");
        }

        if (request.UserRecipientId is not null)
        {
            var userRecipient = await _merchDbContext.MerchUsers
                .FirstOrDefaultAsync(user => user.UserId == request.UserRecipientId, cancellationToken);

            if (userRecipient is null)
            {
                throw new EntityNotFoundException("User recipient is not exist");
            }
        }

        if (request.WarehouseRecipientId is not null)
        {
            var warehouseRecipient = await _merchDbContext.Warehouses
                .FirstOrDefaultAsync(x => x.Id == request.WarehouseRecipientId && request.WarehouseRecipientId != null, cancellationToken);

            if (warehouseRecipient is null)
            {
                throw new EntityNotFoundException("Warehouse recipient is not exist");
            }
        }

        if (itemInfo.ArchivedAtUtc is not null)
        {
            throw new InvalidDataAppException("You cannot add transaction with archived merch item");
        }
    }

    internal sealed record Command : ICommand<int>, ITransactionalRequest
    {
        public MerchTransactionType TransactionType { get; set; }
        public int ItemId { get; set; }
        public IReadOnlyCollection<SizeCountDto> SizeCounts { get; set; } = [];
        public int WarehouseId { get; set; }
        public int? UserRecipientId { get; set; }
        public int? WarehouseRecipientId { get; set; }
        public string? Notes { get; set; }
    }

    internal sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(command => command.TransactionType)
                .IsInEnum();

            RuleFor(command => command.ItemId)
                .NotNull();

            RuleFor(command => command.SizeCounts)
                .NotEmpty();

            RuleFor(command => command.WarehouseId)
                .NotNull();

            RuleFor(command => command.Notes)
                    .LongTextType1(false);

            When(command => command.TransactionType == MerchTransactionType.Receipt, () =>
            {
                RuleFor(command => command.UserRecipientId)
                    .Null();

                RuleFor(command => command.WarehouseRecipientId)
                    .Null();
            });

            When(command => command.TransactionType == MerchTransactionType.WriteOff, () =>
            {
                RuleFor(command => command.UserRecipientId)
                    .NotNull();

                RuleFor(command => command.WarehouseRecipientId)
                    .Null();

                RuleFor(command => command.SizeCounts)
                    .Must(sizeCounts => sizeCounts.All(sc => sc.Count >= 0))
                    .WithMessage("For WriteOff transaction count cannot be negative.");
            });

            When(command => command.TransactionType == MerchTransactionType.Transfer, () =>
            {
                RuleFor(command => command.UserRecipientId)
                    .Null();

                RuleFor(command => command.WarehouseRecipientId)
                    .NotNull();
            });
        }
    }
}
