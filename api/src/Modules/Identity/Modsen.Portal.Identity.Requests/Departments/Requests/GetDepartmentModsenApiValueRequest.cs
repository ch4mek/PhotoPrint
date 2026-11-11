using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Identity.DataAccess.Npgsql.Cache;

namespace Modsen.Portal.Identity.Requests.Departments.Requests;

internal sealed class GetDepartmentModsenApiValueRequest
    : IQueryHandler<GetDepartmentModsenApiValueRequest.Query, string?>
{
    private readonly IDepartmentsCache _departmentsCache;

    public GetDepartmentModsenApiValueRequest(IDepartmentsCache departmentsCache)
    {
        _departmentsCache = departmentsCache;
    }

    public async Task<string?> Handle(Query request, CancellationToken cancellationToken)
    {
        var value = await _departmentsCache.GetDepartmentById(request.DepartmentId, cancellationToken);
        return value?.ModsenApiValue;
    }

    internal sealed record Query(string DepartmentId) : IQuery<string?>, IIdentityRequest;
}
