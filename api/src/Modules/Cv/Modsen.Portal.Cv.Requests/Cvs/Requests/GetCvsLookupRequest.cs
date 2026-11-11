using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Cv.DataAccess.Npgsql;
using Modsen.Portal.Cv.Requests.Cvs.Core.CoreDto;

namespace Modsen.Portal.Cv.Requests.Cvs.Requests;

internal sealed class GetCvsLookupRequest
    : IQueryHandler<GetCvsLookupRequest.Query, List<CvLookupDto>>
{
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly ICvDbContext _cvDbContext;

    public GetCvsLookupRequest(
        IAuthenticatedUserContext authenticatedUserContext,
        ICvDbContext cvDbContext)
    {
        _authenticatedUserContext = authenticatedUserContext;
        _cvDbContext = cvDbContext;
    }

    public async Task<List<CvLookupDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        var cvs = await GetCvsAsync(cancellationToken);

        return cvs;
    }

    private async Task<List<CvLookupDto>> GetCvsAsync(CancellationToken cancellationToken)
    {
        var query = _cvDbContext.Cvs.AsNoTracking();

        if (_authenticatedUserContext.GetRoles().IsHeadOfDepartment())
        {
            query = query.Where(x => x.CvUser!.DepartmentId == _authenticatedUserContext.DepartmentId);
        }

        if (_authenticatedUserContext.GetRoles().IsGroupManager())
        {
            query = query.Where(x => x.CvUser!.ResourceGroupId == _authenticatedUserContext.ResourceGroupId);
        }

        if (_authenticatedUserContext.GetRoles().IsEmployee() ||
            _authenticatedUserContext.GetRoles().IsSystemAdministrator())
        {
            query = query.Where(x => x.UserId == _authenticatedUserContext.UserId);
        }

        return await query
            .Where(x => !x.IsDeleted)
            .OrderBy(x => x.Id)
            .Select(x => new CvLookupDto
            {
                Id = x.Id,
                CvName = x.CvName
            })
            .ToListAsync(cancellationToken);
    }

    internal sealed record Query : IQuery<List<CvLookupDto>>;
}
