using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Cv;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.Cv.DataAccess.Npgsql;
using Modsen.Portal.Cv.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Cv.Requests.Cvs.Core.CoreDto;

namespace Modsen.Portal.Cv.Requests.Cvs.Requests;

internal sealed class ChangeCvStatusRequest : ICommandHandler<ChangeCvStatusRequest.Command, CvCoreDto>
{
    private readonly ICvDbContext _cvDbContext;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;

    public ChangeCvStatusRequest(
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

        if (!IsCurrentUserCanChangeStatus(cv))
        {
            throw new ActionNotAllowedException();
        }

        if (cv.Status == CvStatus.Sent)
        {
            throw new InvalidDataAppException("CV has already been sent");
        }

        cv.Status = request.Status;
        await SetClient(cv, request.ClientId, request.ClientName, cancellationToken);
        await SetProject(cv, request.ProjectId, request.ProjectName, cancellationToken);

        await _cvDbContext.SaveChangesAsync(cancellationToken);

        return CvCoreDto.Of(cv);
    }

    private bool IsCurrentUserCanChangeStatus(CvEntity cv)
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

    private async Task SetClient(CvEntity cv, string? clientId, string? clientName, CancellationToken token)
    {
        if (clientId != null)
        {
            var client = await _cvDbContext.CvClients
                    .FirstOrDefaultAsync(x => x.ClientId == clientId, token)
                ?? throw new EntityNotFoundException(PortalMessages.ClientNotFound);

            cv.ClientId = client.ClientId;
        }
        else if (clientName != null)
        {
            var client = await _cvDbContext.CvClients
                .FirstOrDefaultAsync(x => x.Name == clientName, token);

            if (client == null)
            {
                client = new CvClientEntity
                {
                    ClientId = Guid.CreateVersion7().ToString(),
                    Name = clientName,
                    IsActive = true
                };
                await _cvDbContext.CvClients.AddAsync(client, token);
            }

            cv.ClientId = client.ClientId;
        }

    }

    private async Task SetProject(CvEntity cv, string? projectId, string? projectName, CancellationToken token)
    {
        if (projectId != null)
        {
            var project = await _cvDbContext.CvProjects
                    .FirstOrDefaultAsync(x => x.ProjectId == projectId, token)
                ?? throw new EntityNotFoundException(PortalMessages.ProjectNotFound);

            cv.ProjectId = project.ProjectId;
        }
        else if(projectName != null)
        {
            var project = await _cvDbContext.CvProjects
                .FirstOrDefaultAsync(x => x.Name == projectName, token);

            if (project == null)
            {
                project = new CvProjectEntity
                {
                    ProjectId = Guid.CreateVersion7().ToString(),
                    Name = projectName,
                    IsActive = true
                };
                await _cvDbContext.CvProjects.AddAsync(project, token);
            }

            cv.ProjectId = project.ProjectId;
        }
        else
        {
            cv.ProjectId = null;
        }
    }

    internal sealed record Command(
        int Id,
        string? ClientId,
        string? ProjectId,
        string? ClientName,
        string? ProjectName,
        CvStatus Status) :
        ICommand<CvCoreDto>, ITransactionalRequest;

    internal sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            When(x => x.ClientId.IsNullOrEmpty(), () =>
                RuleFor(x => x.ClientName)
                    .NotNull()
                    .NotEmpty());
        }
    }
}
