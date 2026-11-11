using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.Cv.DataAccess.Npgsql;
using Modsen.Portal.Cv.Requests.CvUsers.Core.CoreDto;

namespace Modsen.Portal.Cv.Requests.CvUsers.Requests;

internal sealed class GetCvUsersLookupRequest
    : IQueryHandler<GetCvUsersLookupRequest.Query, List<CvUserLookupDto>>
{
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly ICvDbContext _cvDbContext;

    public GetCvUsersLookupRequest(
        IAuthenticatedUserContext authenticatedUserContext,
        ICvDbContext cvDbContext)
    {
        _authenticatedUserContext = authenticatedUserContext;
        _cvDbContext = cvDbContext;
    }

    public async Task<List<CvUserLookupDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        var cvUsers = await GetUsersAsync(cancellationToken);

        return cvUsers;
    }

    private async Task<List<CvUserLookupDto>> GetUsersAsync(CancellationToken cancellationToken)
    {
        var query = _cvDbContext.CvUsers
            .OrderBy(user => user.UserId)
            .Where(user => user.IsActive);

        if (_authenticatedUserContext.GetRoles().IsHeadOfDepartment())
        {
            query = query.Where(user => user.DepartmentId == _authenticatedUserContext.DepartmentId);
        }

        if (_authenticatedUserContext.GetRoles().IsGroupManager())
        {
            query = query.Where(user => user.ResourceGroupId == _authenticatedUserContext.ResourceGroupId);
        }

        if (_authenticatedUserContext.GetRoles().IsEmployee() ||
            _authenticatedUserContext.GetRoles().IsSystemAdministrator())
        {
            query = query.Where(user => user.UserId == _authenticatedUserContext.UserId);
        }

        return await query
            .Select(user => new CvUserLookupDto
            {
                Id = user.UserId,
                FullName = user.LastName + " " + user.FirstName,
                FullNameRu = user.LastNameRu == null && user.FirstNameRu == null ?
                    null :
                    ToFullName.Reverse(user.FirstNameRu, user.LastNameRu).Trim(),
            })
            .ToListAsync(cancellationToken);
    }

    internal sealed record Query : IQuery<List<CvUserLookupDto>>;
}
