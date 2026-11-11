using FluentValidation;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using Microsoft.OData.UriParser;
using Modsen.Portal.Core.Merch;
using Modsen.Portal.Core.OData;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.UseCases.ValidationRules;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.Merch.DataAccess.Npgsql;
using Modsen.Portal.Merch.Requests.Transactions.Requests.RequestDto;

namespace Modsen.Portal.Merch.Requests.Transactions.Requests
{
    internal sealed class GetTransactionsRquest : IQueryHandler<GetTransactionsRquest.Query, PageableListOfItems<TransationOdpDto>>
    {
        private readonly IMerchDbContext _merchDbContext;

        public GetTransactionsRquest(IMerchDbContext merchDbContext)
        {
            _merchDbContext = merchDbContext;
        }

        public async Task<PageableListOfItems<TransationOdpDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var queryOptions = request.QueryOptions;
            var isOrderByIdAsc = GetIdOrdering(queryOptions);
            var projectedQuery = GetQuery(request, isOrderByIdAsc);

            var appliedQuery = (IQueryable<TransationOdpDto>)queryOptions.ApplyTo(projectedQuery);
            var paginated = await appliedQuery.ToListAsync(cancellationToken);

            var totalCount = await queryOptions
                .RebuildWithoutPagination()
                .ApplyTo(projectedQuery)
                .Cast<TransationOdpDto>()
                .CountAsync(cancellationToken);

            if (totalCount == 0)
            {
                return EmptyPageableListOfItems<TransationOdpDto>.Value;
            }

            var groupCodes = paginated.Select(g => g.GroupCode).ToList();

            var transactions = await _merchDbContext.ItemTransactions
                .AsNoTracking()
                .Where(t => groupCodes.Contains(t.GroupCode))
                .Where(t => !request.IdFrom.HasValue || !request.IdTo.HasValue ||
                            (t.Id >= request.IdFrom.Value && t.Id <= request.IdTo.Value))
                .Select(x => new TransactionDto
                {
                    Id = x.Id,
                    Amount = x.Count,
                    GroupCode = x.GroupCode,
                    WearhouseName = x.Warehouse!.Location,
                    Size = x.SizeId
                })
                .ToListAsync(cancellationToken);

            foreach (var group in paginated)
            {
                var groupTransactions = transactions
                    .Where(t => t.GroupCode == group.GroupCode);

                group.Transactions = isOrderByIdAsc switch
                {
                    null or true => groupTransactions.OrderBy(t => t.Id),
                    false => groupTransactions.OrderByDescending(t => t.Id),
                };
            }

            return paginated.ToPageable(queryOptions, totalCount);
        }

        private IQueryable<TransationOdpDto> GetQuery(Query request, bool? isOrderByIdAsc)
        {
            var baseQuery = _merchDbContext.ItemTransactionGroups
                            .AsNoTracking()
                            .AsSplitQuery()
                            .Where(t => !request.IdFrom.HasValue || !request.IdTo.HasValue ||
                                        t.ItemTransactions.Any(x => x.Id >= request.IdFrom.Value && x.Id <= request.IdTo.Value));

            var groupedQuery = baseQuery
                .Select(g => new
                {
                    Group = g,
                    g.GroupCode,
                    FirstTransaction = g.ItemTransactions
                        .OrderBy(t => t.Id)
                        .Select(t => new
                        {
                            t.Item!.IsClothing,
                            t.Item.PhotoFileId,
                            t.Item.Name,
                            t.Item.Description,
                            t.Type,
                            t.OrderWeight,
                            SenderLocation = t.Sender!.Location,
                            RecipientUser = t.UserRecipient,
                            RecipientWarehouseLocation = t.WarehouseRecipient!.Location,
                        })
                        .FirstOrDefault(),

                    SumAmount = g.ItemTransactions.Any(t => t.Type == MerchTransactionType.Transfer && t.Count > 0)
                        ? g.ItemTransactions.Where(t => t.Count > 0).Sum(t => t.Count)
                        : g.ItemTransactions.Sum(t => t.Count),

                    OrderTransactionId = isOrderByIdAsc == true
                        ? g.ItemTransactions.Min(t => t.Id)
                        : g.ItemTransactions.Max(t => t.Id),

                    IdFrom = g.ItemTransactions.Min(t => t.Id),
                    IdTo = g.ItemTransactions.Max(t => t.Id),
                    WarehouseNames = g.ItemTransactions.Select(t => t.Warehouse!.Location).Distinct(),
                });

            var projectedQuery = groupedQuery.Select(g => new TransationOdpDto
            {
                Date = g.Group.CreatedAtUtc,
                GroupCode = g.Group.GroupCode,
                Notes = g.Group.Notes,
                Creator = g.Group.CreatedByUser!.LastName + " " + g.Group.CreatedByUser.FirstName,
                SumAmount = g.SumAmount,
                IsClothing = g.FirstTransaction!.IsClothing,
                ElementFileId = g.FirstTransaction.PhotoFileId,
                ElementName = g.FirstTransaction.Name,
                ElementDescription = g.FirstTransaction.Description,
                Type = g.FirstTransaction.Type,
                OrderWeight = g.FirstTransaction.OrderWeight,
                SenderName = g.FirstTransaction.SenderLocation,
                Recipient = g.FirstTransaction.RecipientUser != null
                    ? g.FirstTransaction.RecipientUser.LastName + " " + g.FirstTransaction.RecipientUser.FirstName
                    : g.FirstTransaction.RecipientWarehouseLocation,
                WarehouseNames = g.WarehouseNames,
                OrderTransactionId = g.OrderTransactionId,
                TransactionIdFrom = g.IdFrom,
                TransactionIdTo = g.IdTo,
            });

            return projectedQuery;
        }

        private static bool? GetIdOrdering(ODataQueryOptions<TransationOdpDto> queryOptions)
        {
            var orderByNode = queryOptions.OrderBy?.OrderByNodes
                .OfType<OrderByPropertyNode>()
                .FirstOrDefault(n =>
                    n.Property.Name.Equals(nameof(TransationOdpDto.OrderTransactionId), StringComparison.OrdinalIgnoreCase));
            if (orderByNode is null)
            {
                return null;
            }

            return orderByNode.Direction == OrderByDirection.Ascending;
        }

        internal sealed record Query(ODataQueryOptions<TransationOdpDto> QueryOptions,
            int? IdFrom, int? IdTo)
            : IQuery<PageableListOfItems<TransationOdpDto>>;

        internal sealed class Validator : AbstractValidator<Query>
        {
            public Validator()
            {
                RuleFor(x => x.QueryOptions).SetValidator(new ODataQueryOptionsValidator());
            }
        }
    }
}
