using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Equipment.DataAccess.Npgsql;
using Modsen.Portal.Equipment.Requests.Users.Requests.RequestDtos;

namespace Modsen.Portal.Equipment.Requests.Users.Requests;

internal sealed class GetUserLookupRequest : IQueryHandler<GetUserLookupRequest.Query, List<UserLookupDto>>
{
    private readonly IEquipmentDbContext _equipmentDbContext;

    public GetUserLookupRequest(IEquipmentDbContext equipmentDbContext)
    {
        _equipmentDbContext = equipmentDbContext;
    }

    public async Task<List<UserLookupDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        var query = _equipmentDbContext.EquipmentUsers.AsQueryable();

        if (request.Roles.Count > 0)
        {
            query = query.Where(x => request.Roles.Contains(x.Position ?? string.Empty));
        }

        return await query
            .Where(x => x.IsActive)
            .OrderBy(x => x.UserId)   
            .Select(x => new UserLookupDto
            {
                Id = x.UserId,
                FirstName = x.FirstName,
                LastName = x.LastName,
                DepartmentId = x.DepartmentId,
                ResourceGroupId = x.ResourceGroupId,
                PositionDescription = x.Position
            })
            .ToListAsync(cancellationToken);
    }

    internal sealed record Query(List<string> Roles)
        : IQuery<List<UserLookupDto>>, IEquipmentRequest;
}
