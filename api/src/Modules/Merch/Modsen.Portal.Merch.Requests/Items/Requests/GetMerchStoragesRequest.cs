using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Merch.Contract;
using Modsen.Portal.Merch.DataAccess.Npgsql;

namespace Modsen.Portal.Merch.Requests.Items.Requests;

internal sealed class GetMerchStoragesRequest : IQueryHandler<GetMerchStoragesRequest.Query, IReadOnlyCollection<MerchStorageDto>>
{
    private readonly IMerchDbContext _merchDbContext;

    public GetMerchStoragesRequest(IMerchDbContext merchDbContext)
    {
        _merchDbContext = merchDbContext;
    }

    public async Task<IReadOnlyCollection<MerchStorageDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        return await _merchDbContext.Warehouses
            .Select(storage => new MerchStorageDto { Id = storage.Id, Name = storage.Location })
            .ToArrayAsync(cancellationToken);
    }

    internal sealed record Query : IQuery<IReadOnlyCollection<MerchStorageDto>>;
}
