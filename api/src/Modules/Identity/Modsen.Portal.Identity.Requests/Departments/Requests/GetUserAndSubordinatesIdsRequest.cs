using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Identity.Contract;
using Modsen.Portal.Identity.DataAccess.Npgsql;

namespace Modsen.Portal.Identity.Requests.Departments.Requests;

internal sealed class GetUserAndSubordinatesIdsRequest : IQueryHandler<GetUserAndSubordinatesIdsRequest.Query, List<int>>
{
    private readonly IIdentityDbContext _identityDbContext;
    private readonly IIdentityContract _identityContract;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;

    public GetUserAndSubordinatesIdsRequest(IAuthenticatedUserContext authenticatedUserContext, IIdentityContract identityContract,
        IIdentityDbContext identityDbContext)
    {
        _authenticatedUserContext = authenticatedUserContext;
        _identityContract = identityContract;
        _identityDbContext = identityDbContext;
    }

    public async Task<List<int>> Handle(Query request, CancellationToken cancellationToken)
    {
        var initiatorId = _authenticatedUserContext.GetRequiredUserId();
        var userIds = new List<int>();

        if (_authenticatedUserContext.GetRoles().IsHeadOfDepartment())
        {
            var subordinatesIds = await _identityContract.UsersWorkInDepartment(
                year: request.Year,
                month: request.Month,
                departmentIds: [_authenticatedUserContext.DepartmentId!],
                cancellationToken: cancellationToken);
            userIds.AddRange(subordinatesIds);
        }
        else if (_authenticatedUserContext.GetRoles().IsGroupManager())
        {
            var groupMembers = await _identityDbContext.Users
                .Where(x => x.ResourceGroupId == _authenticatedUserContext.ResourceGroupId)
                .Select(x => x.Id)
                .ToListAsync(cancellationToken);

            userIds.AddRange(groupMembers);
        }
        else
        {
            userIds.Add(initiatorId);
        }

        if (!request.IncludeSelf)
        {
            userIds.Remove(initiatorId);
        }

        return userIds;
    }

    internal sealed record Query(int Year, int Month, bool IncludeSelf = false) : IQuery<List<int>>;
}
