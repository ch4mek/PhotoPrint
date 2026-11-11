using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Cv.DataAccess.Npgsql;
using Modsen.Portal.Cv.Requests.Cvs.Core.CoreDto;

namespace Modsen.Portal.Cv.Requests.Cvs.Requests;

internal sealed class GetCvByIdRequest
    : IQueryHandler<GetCvByIdRequest.Query, CvCoreDto>
{
    private readonly ICvDbContext _cvDbContext;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;

    public GetCvByIdRequest(
        ICvDbContext cvDbContext,
        IAuthenticatedUserContext authenticatedUserContext)
    {
        _cvDbContext = cvDbContext;
        _authenticatedUserContext = authenticatedUserContext;
    }

    public async Task<CvCoreDto> Handle(Query request, CancellationToken cancellationToken)
    {
        var query = _cvDbContext.Cvs
            .Include(x => x.CvUser)
            .Include(x => x.WorkExperiences)
            .Include(x => x.Languages)
            .Include(x => x.ProjectExperiences)
            .Include(x => x.SkillGroups)
            .ThenInclude(x => x.Skills)
            .AsSplitQuery()
            .AsNoTracking();

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

        var cv = await query
            .Where(x => x.Id == request.Id)
            .Where(x => !x.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);

        if (cv is null)
        {
            throw new EntityNotFoundException(PortalMessages.CvNotFound);
        }

        return CvCoreDto.Of(cv);
    }

    internal sealed record Query(int Id) : IQuery<CvCoreDto>;
}
