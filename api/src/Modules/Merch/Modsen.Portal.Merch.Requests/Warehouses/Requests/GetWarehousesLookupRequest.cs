using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Merch.DataAccess.Npgsql;
using Modsen.Portal.Merch.Requests.Warehouses.Requests.RequestDtos;

namespace Modsen.Portal.Merch.Requests.Warehouses.Requests;

internal sealed class GetWarehousesLookupRequest : IQueryHandler<GetWarehousesLookupRequest.Query, List<WarehouseDto>>
{
    private readonly IMerchDbContext _merchDbContext;

    public GetWarehousesLookupRequest(IMerchDbContext merchDbContext)
    {
        _merchDbContext = merchDbContext;
    }

    public Task<List<WarehouseDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        return _merchDbContext.Warehouses
            .Select(warehouse => new WarehouseDto
            {
                Id = warehouse.Id,
                Location = warehouse.Location
            })
            .ToListAsync(cancellationToken);
    }

    internal sealed record Query : IQuery<List<WarehouseDto>>;
}
