using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Cv.DataAccess.Npgsql;
using Modsen.Portal.Cv.Requests.CvClients.Core.CoreDto;

namespace Modsen.Portal.Cv.Requests.CvClients.Requests;

internal sealed class GetCvClientsLookupRequest
    : IQueryHandler<GetCvClientsLookupRequest.Query, List<CvClientLookupDto>>
{
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly ICvDbContext _cvDbContext;

    public GetCvClientsLookupRequest(
        IAuthenticatedUserContext authenticatedUserContext,
        ICvDbContext cvDbContext)
    {
        _authenticatedUserContext = authenticatedUserContext;
        _cvDbContext = cvDbContext;
    }

    public async Task<List<CvClientLookupDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        var cvIds = await GetCvsIdsAsync(cancellationToken);

        var cvClients = await GetClientsAsync(cvIds, cancellationToken);

        return cvClients;
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

    private async Task<List<CvClientLookupDto>> GetClientsAsync(int[] cvIds, CancellationToken cancellationToken)
    {
        var cvClients = await _cvDbContext.CvClients
            .Where(x =>
                x.Cvs.Any(cv => cvIds.Contains(cv.Id) && !cv.IsDeleted) &&
                x.IsActive)
            .OrderBy(x => x.CreatedAtUtc)
            .Select(x => new CvClientLookupDto
            {
                Id = x.ClientId,
                Name = x.Name,
            })
            .ToListAsync(cancellationToken);

        return cvClients;
    }

    internal sealed record Query : IQuery<List<CvClientLookupDto>>;
}
