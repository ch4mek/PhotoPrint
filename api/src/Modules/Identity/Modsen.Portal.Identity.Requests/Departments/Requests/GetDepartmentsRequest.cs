using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Identity.Requests.Departments.Core;
using Modsen.Portal.Identity.Requests.Departments.Core.CoreDto;

namespace Modsen.Portal.Identity.Requests.Departments.Requests;

internal sealed class GetDepartmentsRequest : IQueryHandler<GetDepartmentsRequest.Query, List<DepartmentCoreDto>>
{
    private readonly IDepartmentLoader _departmentLoader;

    public GetDepartmentsRequest(IDepartmentLoader departmentLoader)
    {
        _departmentLoader = departmentLoader;
    }

    public async Task<List<DepartmentCoreDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        return await _departmentLoader.LoadDepartments(cancellationToken);
    }

    internal sealed record Query : IQuery<List<DepartmentCoreDto>>, IIdentityRequest;
}
