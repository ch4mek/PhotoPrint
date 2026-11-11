using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Merch.DataAccess.Npgsql;
using Modsen.Portal.Merch.Requests.Items.Requests.RequestDto;

namespace Modsen.Portal.Merch.Requests.Items.Requests;

internal sealed class GetItemsAvailableFiltersRequest
    : IQueryHandler<GetItemsAvailableFiltersRequest.Query, AvailableFiltersDto>
{
    private readonly IMerchDbContext _merchDbContext;

    public GetItemsAvailableFiltersRequest(IMerchDbContext merchDbContext)
    {
        _merchDbContext = merchDbContext;
    }

    public async Task<AvailableFiltersDto> Handle(Query request, CancellationToken cancellationToken)
    {
        var itemsLookup = await _merchDbContext.Items
            .Select(item => new MerchItemLookupDto(item.Id, item.Name))
            .ToArrayAsync(cancellationToken);

        var storages = await _merchDbContext.Warehouses
            .Select(storage => new StorageLookupDto(storage.Id, storage.Location))
            .ToArrayAsync(cancellationToken);

        return new AvailableFiltersDto { Items = itemsLookup, Storages = storages };
    }

    internal sealed record Query : IQuery<AvailableFiltersDto>;
}
