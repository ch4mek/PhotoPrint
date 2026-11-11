using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Identity.Requests.Departments.Core;
using Modsen.Portal.Identity.Requests.Departments.Core.CoreDto;

namespace Modsen.Portal.Identity.Requests.Departments.Requests;

internal sealed class GetDepartmentsClassificationRequest :
    IQueryHandler<GetDepartmentsClassificationRequest.Query, List<DepartmentClassificationCoreDto>>
{
    private readonly IDepartmentLoader _departmentLoader;

    public GetDepartmentsClassificationRequest(IDepartmentLoader departmentLoader)
    {
        _departmentLoader = departmentLoader;
    }

    public async Task<List<DepartmentClassificationCoreDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        return await _departmentLoader.LoadDepartmentsClassifications(cancellationToken);
    }

    internal sealed record Query : IQuery<List<DepartmentClassificationCoreDto>>, IIdentityRequest;
}
