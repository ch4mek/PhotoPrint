using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Identity.Contract.Models;
using Modsen.Portal.Identity.DataAccess.Npgsql.Cache;

namespace Modsen.Portal.Identity.Requests.Departments.Requests;

internal sealed class GetDepartmentsModsenApiValuesRequest
    : IQueryHandler<GetDepartmentsModsenApiValuesRequest.Query, List<DepartmentModsenApiValue>>
{
    private readonly IDepartmentsCache _departmentsCache;

    public GetDepartmentsModsenApiValuesRequest(IDepartmentsCache departmentsCache)
    {
        _departmentsCache = departmentsCache;
    }

    public async Task<List<DepartmentModsenApiValue>> Handle(Query request, CancellationToken cancellationToken)
    {
        var value = await _departmentsCache.GetAllDepartments(cancellationToken);
        return value.ConvertAll(e => new DepartmentModsenApiValue(e.Id, e.ModsenApiValue));
    }

    internal sealed record Query : IQuery<List<DepartmentModsenApiValue>>, IIdentityRequest;
}
