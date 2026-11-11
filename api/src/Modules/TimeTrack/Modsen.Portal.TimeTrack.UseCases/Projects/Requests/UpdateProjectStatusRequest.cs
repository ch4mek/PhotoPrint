using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Communication;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TimeTrack.Contract.Events;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;

namespace Modsen.Portal.TimeTrack.UseCases.Projects.Requests;

internal sealed class UpdateProjectStatusRequest : ICommandHandler<UpdateProjectStatusRequest.Command, bool>
{
    private readonly ITimeTrackDbContext _timeTrackDbContext;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly ICommunicationBus _communicationBus;

    public UpdateProjectStatusRequest(
        ITimeTrackDbContext timeTrackDbContext,
        IAuthenticatedUserContext authenticatedUserContext,
        ICommunicationBus communicationBus)
    {
        _timeTrackDbContext = timeTrackDbContext;
        _authenticatedUserContext = authenticatedUserContext;
        _communicationBus = communicationBus;
    }

    public async Task<bool> Handle(Command request, CancellationToken cancellationToken)
    {
        var project = await _timeTrackDbContext.Projects
            .FirstOrDefaultAsync(project => project.Id == request.Id, cancellationToken);

        if (project is null)
        {
            throw new EntityNotFoundException(PortalMessages.ProjectNotFound);
        }

        if (project.IsActive == request.IsActive)
        {
            throw new InvalidDataAppException(PortalMessages.ProjectHasSameStatus);
        }

        if (request.IsActive &&
            !_authenticatedUserContext.GetRoles().IsCeo() &&
            !_authenticatedUserContext.GetRoles().IsCto())
        {
            throw new ActionNotAllowedException(PortalMessages.YouCantActiveProject);
        }

        var isProjectHasActiveAssignments = await IsProjectHasActiveAssignments(request.Id, cancellationToken);

        if (!request.IsActive && (project.ManagerUserId != null || isProjectHasActiveAssignments))
        {
            throw new InvalidDataAppException(PortalMessages.CantDeactivateProjectWithManagerAndUsers);
        }

        if (!request.IsActive && _authenticatedUserContext.GetRoles().IsHeadOfDepartment() &&
            (project.CreatedById == null || project.CreatedById != _authenticatedUserContext.GetRequiredUserId()))
        {
            throw new ActionNotAllowedException(PortalMessages.YouCantDeactivateProject);
        }

        project.IsActive = request.IsActive;

        await _timeTrackDbContext.SaveChanges(cancellationToken);

        var notification = new ProjectDeactivatedEvent
        {
            EventId = Guid.CreateVersion7().ToString(),
            ProjectId = project.Id.ToString(CultureInfo.InvariantCulture)
        };

        await _communicationBus.PublishAsync(notification, cancellationToken);

        return request.IsActive;
    }

    private Task<bool> IsProjectHasActiveAssignments(int projectId, CancellationToken cancellationToken)
    {
        var dateNow = DateOnly.FromDateTime(DateTime.UtcNow);

        return _timeTrackDbContext.Projects
            .Where(project => project.ProjectUsers.Any(projectsUser => projectsUser.UserOnProjectTo == null ||
                dateNow <= projectsUser.UserOnProjectFrom ||
                (projectsUser.UserOnProjectTo >= dateNow && projectsUser.UserOnProjectFrom <= dateNow)))
            .AnyAsync(project => project.Id == projectId, cancellationToken);
    }

    internal sealed record Command(int Id, bool IsActive)
        : ICommand<bool>, ITransactionalRequest, ITimeTrackRequest;
}
