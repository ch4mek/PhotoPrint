using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Merch.Contract;
using Modsen.Portal.Merch.DataAccess.Npgsql;

namespace Modsen.Portal.Merch.Requests.Items.Requests;

internal sealed class GetMerchSizesRequest : IQueryHandler<GetMerchSizesRequest.Query, IReadOnlyCollection<MerchSizeDto>>
{
    private readonly IMerchDbContext _merchDbContext;

    public GetMerchSizesRequest(IMerchDbContext merchDbContext)
    {
        _merchDbContext = merchDbContext;
    }

    public async Task<IReadOnlyCollection<MerchSizeDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        return await _merchDbContext.MerchSizes
            .OrderBy(size => size.OrderWeight)
            .Select(size => new MerchSizeDto { Id = size.Id, Name = size.Name })
            .ToArrayAsync(cancellationToken);
    }

    internal sealed record Query : IQuery<IReadOnlyCollection<MerchSizeDto>>;
}
