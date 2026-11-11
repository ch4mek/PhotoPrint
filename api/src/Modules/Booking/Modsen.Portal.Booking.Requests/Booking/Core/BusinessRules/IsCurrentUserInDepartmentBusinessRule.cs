using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;

namespace Modsen.Portal.Booking.Requests.Booking.Core.BusinessRules;

internal interface IIsCurrentUserInDepartmentBusinessRule
{
    bool IsCorrespondsToBusiness(string departmentId);
}

internal class IsCurrentUserInDepartmentBusinessRule : IIsCurrentUserInDepartmentBusinessRule
{
    private readonly IAuthenticatedUserContext _authenticatedUserContext;

    public IsCurrentUserInDepartmentBusinessRule(IAuthenticatedUserContext authenticatedUserContext)
    {
        _authenticatedUserContext = authenticatedUserContext;
    }

    public bool IsCorrespondsToBusiness(string departmentId)
    {
        return !_authenticatedUserContext.GetRoles().IsHeadOfDepartment() || _authenticatedUserContext.DepartmentId == departmentId;
    }
}
