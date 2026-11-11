using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Communication;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.UseCases.ValidationRules;
using Modsen.Portal.TimeTrack.Contract.Events;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.UseCases.BusinessRules;
using Modsen.Portal.TimeTrack.UseCases.Projects.Core.BusinessRules;
using Modsen.Portal.TimeTrack.UseCases.Projects.Requests.RequestDtos;

namespace Modsen.Portal.TimeTrack.UseCases.Projects.Requests;

internal sealed class UpdateProjectRequest : ICommandHandler<UpdateProjectRequest.Command, UpdateProjectDto>
{
    private readonly ITimeTrackDbContext _timeTrackDbContext;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly ICanUserBeProjectManagerBusinessRule _canUserBeProjectManagerBusinessRule;
    private readonly ICanUserGetProjectBusinessRule _canUserGetProjectBusinessRule;
    private readonly ICommunicationBus _communicationBus;

    public UpdateProjectRequest(
        ITimeTrackDbContext timeTrackDbContext,
        IAuthenticatedUserContext authenticatedUserContext,
        ICanUserBeProjectManagerBusinessRule canUserBeProjectManagerBusinessRule,
        ICommunicationBus communicationBus,
        ICanUserGetProjectBusinessRule canUserGetProjectBusinessRule)
    {
        _timeTrackDbContext = timeTrackDbContext;
        _authenticatedUserContext = authenticatedUserContext;
        _canUserBeProjectManagerBusinessRule = canUserBeProjectManagerBusinessRule;
        _communicationBus = communicationBus;
        _canUserGetProjectBusinessRule = canUserGetProjectBusinessRule;
    }

    public async Task<UpdateProjectDto> Handle(Command request, CancellationToken cancellationToken)
    {
        var projectEntity = await _timeTrackDbContext.Projects
            .FirstOrDefaultAsync(project => project.Id == request.Id, cancellationToken)
            ?? throw new EntityNotFoundException(PortalMessages.ProjectNotFound);

        await ValidateProjectRequest(
            request,
            projectEntity.IsActive,
            projectEntity.Id,
            projectEntity.ManagerUserId,
            projectEntity.Name,
            cancellationToken);

        var projectClientName = await GetProjectClientName(request, cancellationToken);

        var oldManagerUserId = projectEntity.ManagerUserId;

        projectEntity.ClientId = request.ClientId;
        projectEntity.Color = request.Color;
        projectEntity.IsBillable = request.IsBillable;
        projectEntity.IsPublic = request.IsPublic;
        projectEntity.ManagerUserId = request.ManagerId;
        projectEntity.Name = request.Name;

        var notification = new ProjectUpdatedEvent
        {
            EventId = Guid.CreateVersion7().ToString(),
            ClientId = projectEntity.ClientId,
            ProjectId = projectEntity.Id,
            ProjectName = projectEntity.Name,
            ClientName = projectClientName,
            UpdatedById = _authenticatedUserContext.UserId,
            IsActive = projectEntity.IsActive,
            IsPublic = projectEntity.IsPublic,
            OldManagerUserId = oldManagerUserId,
            NewManagerUserId = projectEntity.ManagerUserId,
            Color = projectEntity.Color,
        };

        await _timeTrackDbContext.SaveChanges(cancellationToken);

        await _communicationBus.PublishAsync(notification, cancellationToken);

        return new UpdateProjectDto
        {
            Id = projectEntity.Id,
            Name = projectEntity.Name,
            Color = projectEntity.Color,
            ClientId = projectEntity.ClientId,
            ManagerId = projectEntity.ManagerUserId,
            IsPublic = projectEntity.IsPublic,
            IsBillable = projectEntity.IsBillable,
            IsActive = projectEntity.IsActive,
            ClientName = projectClientName,
            IsCreatedByCurrentUser = projectEntity.CreatedById == _authenticatedUserContext.UserId,
            TrackedTime = await GetProjectTrackedTimeAsync(projectEntity.Id, cancellationToken)
        };
    }

    private Task<int> GetProjectTrackedTimeAsync(int projectId, CancellationToken cancellationToken)
    {
        return _timeTrackDbContext.Timesheets
            .Where(timesheet => timesheet.ProjectId == projectId)
            .SumAsync(timesheet => timesheet.FactTrackedInMinutes, cancellationToken);
    }

    private async Task<string> GetProjectClientName(Command request, CancellationToken cancellationToken)
    {
        var client = await _timeTrackDbContext.Clients
            .Where(client => client.Id == request.ClientId)
            .Select(client => new
            {
                client.Name,
                client.IsActive
            })
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new EntityNotFoundException(PortalMessages.ClientNotFound);

        if (client.IsActive)
        {
            return client.Name;
        }

        throw new InvalidDataAppException(PortalMessages.InactiveClientAssign);
    }

    private async Task ValidateProjectRequest(
        Command request,
        bool isProjectActive,
        int projectId,
        int? projectManagerId,
        string projectName,
        CancellationToken cancellationToken)
    {
        if (request is { IsPublic: true, IsBillable: true })
        {
            throw new InvalidDataAppException(PortalMessages.ProjectCantBePublicAndBillable);
        }

        if (!isProjectActive && _authenticatedUserContext.GetRoles().IsHeadOfDepartment())
        {
            throw new InvalidDataAppException(PortalMessages.YouCantUpdateInactiveProject);
        }

        var businessRuleResult = await _canUserGetProjectBusinessRule.IsCorrespondsToBusiness(projectId, cancellationToken);

        if (!businessRuleResult.IsSuccess)
        {
            throw new ActionNotAllowedException(businessRuleResult.Reason!);
        }

        var isUserManagerOfProject = false;

        if (request.ManagerId.HasValue)
        {
            isUserManagerOfProject = await IsManagerOfProject(projectId, request.ManagerId.Value, cancellationToken);
        }

        if (_authenticatedUserContext.GetRoles().IsHeadOfDepartment() && request.ManagerId.HasValue && !isUserManagerOfProject)
        {
            throw new ActionNotAllowedException(PortalMessages.HeadOfDepartmentCannotAssignProjectManager);
        }

        await ValidateProjectManager(request, projectManagerId, cancellationToken);

        await ValidateProjectName(request, projectName, projectId, cancellationToken);
    }

    private Task<bool> IsManagerOfProject(int projectId, int managerId, CancellationToken cancellationToken)
    {
        return _timeTrackDbContext.Projects.AnyAsync(project =>
            project.Id == projectId &&
            project.ManagerUserId == managerId, cancellationToken);
    }

    private async Task ValidateProjectManager(Command request, int? projectManagerId, CancellationToken cancellationToken)
    {
        if (request.ManagerId is not null && request.ManagerId != projectManagerId)
        {
            var canUserBeManager = await _canUserBeProjectManagerBusinessRule.IsCorrespondsToBusiness(request.ManagerId.Value, cancellationToken);

            if (!canUserBeManager)
            {
                throw new ActionNotAllowedException(PortalMessages.UserCantBeManager);
            }
        }
    }

    private async Task ValidateProjectName(
        Command request,
        string projectName,
        int projectId,
        CancellationToken cancellationToken)
    {
        if (projectName != request.Name)
        {
            var isProjectNameExist = await _timeTrackDbContext.Projects
                .AnyAsync(project => project.Name.ToLower() == request.Name.ToLower() && project.Id != projectId, cancellationToken);

            if (isProjectNameExist)
            {
                throw new EntityConflictException(PortalMessages.ProjectWithTheSameNameExists);
            }
        }
    }

    internal sealed record Command(int Id, string Name, string? Color, int ClientId, int? ManagerId, bool IsPublic,
            bool IsBillable, string? TechStack, string? Industry, string? CloudServices, string? Architecture)
        : ICommand<UpdateProjectDto>, ITransactionalRequest, ITimeTrackRequest;

    internal sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .ShortTextType()
                .SpacesOrPunctuationSymbolsRule();

            RuleFor(project => project.Color)
                .HexColorFormatRule();
        }
    }
}
