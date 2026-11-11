using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;

namespace Modsen.Portal.TimeTrack.UseCases.BusinessRules;

internal interface IGetAccessedDepartmentIdForCurrentUserBusinessRule
{
    Task<string?> Execute(string? departmentId, CancellationToken cancellationToken);
}

internal class GetAccessedDepartmentIdForCurrentUserBusinessRule : IGetAccessedDepartmentIdForCurrentUserBusinessRule
{
    private readonly IAuthenticatedUserContext _authenticatedUserContext;

    public GetAccessedDepartmentIdForCurrentUserBusinessRule(IAuthenticatedUserContext authenticatedUserContext)
    {
        _authenticatedUserContext = authenticatedUserContext;
    }

    public Task<string?> Execute(string? departmentId, CancellationToken cancellationToken)
    {
        return _authenticatedUserContext.GetRoles() switch
        {
            var roles when roles.IsCto() || roles.IsCeo() => Task.FromResult(departmentId),
            var roles when roles.IsHeadOfDepartment() || roles.IsSystemAdministrator() || roles.IsEmployee()
                 || roles.IsGroupManager() => Task.FromResult(_authenticatedUserContext.DepartmentId),
            _ => throw new AppException($"Unsupported RoleType {_authenticatedUserContext.GetRoles()}")
        };
    }
}
