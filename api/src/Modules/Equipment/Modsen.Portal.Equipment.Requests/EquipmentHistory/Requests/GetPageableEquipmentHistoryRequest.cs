using FluentValidation;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.OData;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.UseCases.ValidationRules;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.Equipment.DataAccess.Npgsql;
using Modsen.Portal.Equipment.DataAccess.Npgsql.OData;

namespace Modsen.Portal.Equipment.Requests.EquipmentHistory.Requests;

internal sealed class GetPageableEquipmentHistoryRequest
    : IQueryHandler<GetPageableEquipmentHistoryRequest.Query, PageableListOfItems<OdpEquipmentHolder>>
{
    private readonly IEquipmentDbContext _equipmentDbContext;

    public GetPageableEquipmentHistoryRequest(IEquipmentDbContext equipmentDbContext)
    {
        _equipmentDbContext = equipmentDbContext;
    }

    public async Task<PageableListOfItems<OdpEquipmentHolder>> Handle(Query request, CancellationToken cancellationToken)
    {
        var queryOptions = request.QueryOptions;

        var query = _equipmentDbContext.EquipmentAssigns
            .AsNoTracking()
            .Include(equipmentAssign => equipmentAssign.User)
            .Include(equipmentAssign => equipmentAssign.Approver)
            .Where(equipmentAssign => equipmentAssign.EquipmentId == request.EquipmentId)
            .AsSplitQuery()
            .Select(OdpEquipmentHolder.Map);

        var total = await queryOptions.RebuildWithoutPagination()
            .ApplyTo(query)
            .OfType<OdpEquipmentHolder>()
            .CountAsync(cancellationToken);

        if (total == 0)
        {
            return EmptyPageableListOfItems<OdpEquipmentHolder>.Value;
        }

        var result = await queryOptions
            .ApplyTo(query)
            .OfType<OdpEquipmentHolder>()
            .ToListAsync(cancellationToken);

        return result.ToPageable(queryOptions, total);
    }

    internal sealed record Query(int EquipmentId, ODataQueryOptions<OdpEquipmentHolder> QueryOptions)
        : IQuery<PageableListOfItems<OdpEquipmentHolder>>;

    internal sealed class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.QueryOptions).SetValidator(new ODataQueryOptionsValidator());
            RuleFor(x => x.QueryOptions.RawValues).DivisiblePaginationRule();
        }
    }
}
