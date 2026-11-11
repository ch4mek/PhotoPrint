using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Merch.Requests.Items.Core.DbQueries;
using Modsen.Portal.Merch.Requests.Items.Requests.RequestDto;

namespace Modsen.Portal.Merch.Requests.Items.Requests;

internal sealed class GetItemsLookupRequest : IQueryHandler<GetItemsLookupRequest.Query, List<ItemLookupDto>>
{
    private readonly IGetAvailableItemsDbQuery _getAvailableItemsDbQuery;

    public GetItemsLookupRequest(IGetAvailableItemsDbQuery getAvailableItemsDbQuery)
    {
        _getAvailableItemsDbQuery = getAvailableItemsDbQuery;
    }

    public async Task<List<ItemLookupDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        var availableItems = await _getAvailableItemsDbQuery.ExecuteAsync(cancellationToken);

        return availableItems
            .OrderBy(item => item.Id)
            .Select(item => new ItemLookupDto(item.Id, item.Name, item.IsClothing))
            .ToList();
    }

    internal sealed record Query : IQuery<List<ItemLookupDto>>;

}
