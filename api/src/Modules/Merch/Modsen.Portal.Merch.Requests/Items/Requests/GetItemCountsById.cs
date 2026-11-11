using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Merch.Requests.Core.Dto;
using Modsen.Portal.Merch.Requests.Core.Loaders;

namespace Modsen.Portal.Merch.Requests.Items.Requests;

internal sealed class GetItemCountsById : IQueryHandler<GetItemCountsById.Query, IReadOnlyCollection<SizeCountDto>>
{
    private readonly IItemsCountLoader _itemsCountLoader;
    public GetItemCountsById(IItemsCountLoader itemsCountLoader)
    {
        _itemsCountLoader = itemsCountLoader;
    }

    public async Task<IReadOnlyCollection<SizeCountDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        var reuslt = await _itemsCountLoader.LoadAsync(request.Id, request.WarehouseId, cancellationToken);
        return reuslt.SizeCounts;
    }

    internal sealed record Query(int Id, int WarehouseId) : IQuery<IReadOnlyCollection<SizeCountDto>>;
}
