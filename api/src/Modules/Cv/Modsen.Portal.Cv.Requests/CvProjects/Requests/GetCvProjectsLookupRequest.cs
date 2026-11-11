using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Cv.DataAccess.Npgsql;
using Modsen.Portal.Cv.Requests.CvProjects.Core.CoreDto;

namespace Modsen.Portal.Cv.Requests.CvProjects.Requests;

internal sealed class GetCvProjectsLookupRequest
    : IQueryHandler<GetCvProjectsLookupRequest.Query, List<CvProjectLookupDto>>
{
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly ICvDbContext _cvDbContext;

    public GetCvProjectsLookupRequest(
        IAuthenticatedUserContext authenticatedUserContext,
        ICvDbContext cvDbContext)
    {
        _authenticatedUserContext = authenticatedUserContext;
        _cvDbContext = cvDbContext;
    }

    public async Task<List<CvProjectLookupDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        var cvIds = await GetCvsIdsAsync(cancellationToken);

        var cvProjects = await GetProjectsAsync(cvIds, cancellationToken);

        return cvProjects;
    }

    private async Task<int[]> GetCvsIdsAsync(CancellationToken cancellationToken)
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
            .Select(x => x.Id)
            .ToArrayAsync(cancellationToken);
    }

    private async Task<List<CvProjectLookupDto>> GetProjectsAsync(int[] cvIds, CancellationToken cancellationToken)
    {
        var cvProjects = await _cvDbContext.CvProjects
            .Where(x =>
                x.Cvs.Any(cv => cvIds.Contains(cv.Id) && !cv.IsDeleted) &&
                x.IsActive)
            .OrderBy(x => x.CreatedAtUtc)
            .Select(x => new CvProjectLookupDto
            {
                Id = x.ProjectId,
                Name = x.Name,
            })
            .ToListAsync(cancellationToken);

        return cvProjects;
    }

    internal sealed record Query : IQuery<List<CvProjectLookupDto>>;
}
