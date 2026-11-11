using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Cv.DataAccess.Npgsql;
using Modsen.Portal.Cv.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Cv.Requests.Cvs.Core.CoreDto;

namespace Modsen.Portal.Cv.Requests.Cvs.Requests;

internal sealed class DeleteCvRequest
    : ICommandHandler<DeleteCvRequest.Command, CvCoreDto>
{
    private readonly ICvDbContext _cvDbContext;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;

    public DeleteCvRequest(
        ICvDbContext cvDbContext,
        IAuthenticatedUserContext authenticatedUserContext)
    {
        _cvDbContext = cvDbContext;
        _authenticatedUserContext = authenticatedUserContext;
    }

    public async Task<CvCoreDto> Handle(Command request, CancellationToken cancellationToken)
    {
        var cv = await _cvDbContext.Cvs
            .Include(x => x.CvUser)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new EntityNotFoundException(PortalMessages.CvNotFound);

        if (!IsCurrentUserCanDeleteCv(cv))
        {
            throw new ActionNotAllowedException();
        }

        cv.IsDeleted = true;
        await _cvDbContext.SaveChangesAsync(cancellationToken);

        return CvCoreDto.Of(cv);
    }

    private bool IsCurrentUserCanDeleteCv(CvEntity cv)
    {
        if (_authenticatedUserContext.GetRoles().IsCeo() ||
            _authenticatedUserContext.GetRoles().IsCto())
        {
            return true;
        }

        if (_authenticatedUserContext.GetRoles().IsHeadOfDepartment())
        {
            return cv.CvUser?.DepartmentId == _authenticatedUserContext.DepartmentId;
        }

        if (_authenticatedUserContext.GetRoles().IsGroupManager())
        {
            return cv.CvUser?.ResourceGroupId == _authenticatedUserContext.ResourceGroupId;
        }

        return false;
    }

    internal sealed record Command(int Id) : ICommand<CvCoreDto>, ITransactionalRequest;
}
