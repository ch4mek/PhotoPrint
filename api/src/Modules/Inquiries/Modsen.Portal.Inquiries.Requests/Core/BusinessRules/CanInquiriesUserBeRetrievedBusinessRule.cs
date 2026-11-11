using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;

namespace Modsen.Portal.Inquiries.Requests.Core.BusinessRules;

internal interface ICanInquiriesUserBeRetrievedBusinessRule
{
    Task<bool> IsCorrespondsToBusiness(int userId, CancellationToken cancellationToken);
}

internal class CanInquiriesUserBeRetrievedBusinessRule : ICanInquiriesUserBeRetrievedBusinessRule
{
    private readonly IInquiriesDbContext _inquiriesDbContext;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;

    public CanInquiriesUserBeRetrievedBusinessRule(
        IAuthenticatedUserContext authenticatedUserContext, IInquiriesDbContext inquiriesDbContext)
    {
        _authenticatedUserContext = authenticatedUserContext;
        _inquiriesDbContext = inquiriesDbContext;
    }

    public async Task<bool> IsCorrespondsToBusiness(int userId, CancellationToken cancellationToken)
    {
        if (_authenticatedUserContext.UserId == userId)
        {
            return true;
        }

        if (_authenticatedUserContext.GetRoles().IsSystemAdministrator() ||
            _authenticatedUserContext.GetRoles().IsEmployee())
        {
            return false;
        }

        var currentUserId = _authenticatedUserContext.UserId;
        var user = await _inquiriesDbContext.InquiriesUsers.FindAsync([userId], cancellationToken: cancellationToken)
            ?? throw new EntityNotFoundException(PortalMessages.UserNotFound);

        if (!user.IsActive)
        {
            throw new EntityNotFoundException(PortalMessages.UserNotFound);
        }

        if (!_authenticatedUserContext.IsManager && userId != currentUserId)
        {
            return false;
        }

        if (_authenticatedUserContext.GetRoles().IsHeadOfDepartment() && user.DepartmentId != _authenticatedUserContext.DepartmentId)
        {
            return false;
        }

        if (_authenticatedUserContext.GetRoles().IsGroupManager() && user.ResourceGroupId != _authenticatedUserContext.ResourceGroupId)
        {
            return false;
        }

        if (user.Position.IsCto() || user.Position.IsCeo())
        {
            throw new EntityNotFoundException(PortalMessages.UserNotFound);
        }

        return true;
    }
}
