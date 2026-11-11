using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Merch;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.UseCases.ValidationRules;
using Modsen.Portal.Merch.DataAccess.Npgsql;
using Modsen.Portal.Merch.Requests.Core.Dto;
using Modsen.Portal.Merch.Requests.Transactions.Core.Composers;

namespace Modsen.Portal.Merch.Requests.Transactions.Requests
{
    internal sealed class AddKitTransactionRequest : ICommandHandler<AddKitTransactionRequest.Command, int>
    {
        private readonly IReceiptTransactionsComposer _receiptTransactionsComposer;
        private readonly IWriteOffTransactionsComposer _writeOffTransactionsComposer;
        private readonly ITransferTransactionsComposer _transferTransactionsComposer;
        private readonly IMerchDbContext _merchDbContext;
        public AddKitTransactionRequest(
            IMerchDbContext merchDbContext,
            IReceiptTransactionsComposer receiptTransactionsComposer,
            IWriteOffTransactionsComposer writeOffTransactionsComposer,
            ITransferTransactionsComposer transferTransactionsComposer)
        {
            _merchDbContext = merchDbContext;
            _receiptTransactionsComposer = receiptTransactionsComposer;
            _writeOffTransactionsComposer = writeOffTransactionsComposer;
            _transferTransactionsComposer = transferTransactionsComposer;
        }

        public async Task<int> Handle(Command request, CancellationToken cancellationToken)
        {
            await ValidateAsync(request, cancellationToken);

            var transactions = request.TransactionType switch
            {
                MerchTransactionType.Receipt =>
                    _receiptTransactionsComposer.ComposeKit(request.KitId,
                        request.Amount,
                        request.WarehouseId,
                        request.Notes,
                        request.SizeCounts,
                        cancellationToken),
                MerchTransactionType.Transfer =>
                    _transferTransactionsComposer.ComposeKit(
                        request.KitId,
                        request.Amount,
                        request.WarehouseId,
                        request.WarehouseRecipientId!.Value,
                        request.Notes,
                        request.SizeCounts,
                        cancellationToken),
                MerchTransactionType.WriteOff =>
                    _writeOffTransactionsComposer.ComposeKit(
                        request.KitId,
                        request.Amount,
                        request.WarehouseId,
                        request.UserRecipientId!.Value,
                        request.Notes,
                        request.SizeCounts,
                        cancellationToken),

                _ => throw new InvalidDataAppException("Merch transaction type is not exist"),
            };

            var result = 0;
            await foreach (var transaction in transactions.WithCancellation(cancellationToken))
            {
                _merchDbContext.ItemTransactions.Add(transaction);
                result++;
            }
            await _merchDbContext.SaveChangesAsync(cancellationToken);

            return result;
        }

        private async Task ValidateAsync(Command request, CancellationToken cancellationToken)
        {
            var kit = await _merchDbContext.Kits
                .Include(x => x.KitItems)
                .ThenInclude(x => x.Item)
                .FirstOrDefaultAsync(x => x.Id == request.KitId, cancellationToken);
            if (kit is null)
            {
                throw new EntityNotFoundException("Kit not found");
            }

            var warehouse = await _merchDbContext.Warehouses.FirstOrDefaultAsync(x => x.Id == request.WarehouseId, cancellationToken);
            if (warehouse is null)
            {
                throw new EntityNotFoundException("Warehouse not found");
            }

            var distinct = request.SizeCounts
                .DistinctBy(x => x.SizeId)
                .ToArray();

            if (request.SizeCounts.Count != distinct.Length)
            {
                throw new InvalidDataAppException("Invalid sizes selected");
            }

            var sizeIds = distinct.Select(x => x.SizeId).ToList();

            var allSizesExist = await _merchDbContext.MerchSizes
                .Where(x => sizeIds.Contains(x.Id))
                .Select(x => x.Id)
                .ToListAsync(cancellationToken);

            if (allSizesExist.Count != sizeIds.Count)
            {
                throw new InvalidDataAppException("Invalid sizes selected");
            }

            if (request.Amount <= 0)
            {
                throw new InvalidDataAppException("You cannot add transaction with 0 kits");
            }

            var containClothing = kit.KitItems.Any(x => x.Item!.IsClothing);
            var requestSizeSum = request.SizeCounts.Sum(x => x.Count);
            if (requestSizeSum == 0 && containClothing)
            {
                throw new InvalidDataAppException("You cannot add transaction with 0 Sizes");
            }
            var clothingTotalCount = kit.KitItems.Where(x => x.Item!.IsClothing).Sum(x => x.Count);
            if (request.Amount * clothingTotalCount != requestSizeSum && containClothing)
            {
                throw new InvalidDataAppException("Amount of sizes is not equal to total amount of this element");
            }

            if (kit.ArchivedAtUtc is not null)
            {
                throw new InvalidDataAppException("You cannot add transaction with archived merch kit");
            }
        }

        internal sealed record Command : ICommand<int>, ITransactionalRequest
        {
            public MerchTransactionType TransactionType { get; set; }
            public int KitId { get; set; }
            public int Amount { get; set; }
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

                RuleFor(command => command.KitId)
                    .NotNull();

                RuleFor(command => command.WarehouseId)
                    .NotNull();

                RuleFor(command => command.Notes)
                    .LongTextType1(false);

                RuleFor(command => command.Amount)
                    .InclusiveBetween(0, 9999);

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
}
