using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Communication;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.UseCases.ValidationRules;
using Modsen.Portal.TimeTrack.Contract.Events;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TimeTrack.UseCases.BusinessRules;
using Modsen.Portal.TimeTrack.UseCases.Projects.Requests.RequestDtos;

namespace Modsen.Portal.TimeTrack.UseCases.Projects.Requests;

internal sealed class CreateProjectRequest : ICommandHandler<CreateProjectRequest.Command, ProjectDto>{
    private readonly IHexColorGenerator _hexColorGenerator;
    private readonly ICanUserBeProjectManagerBusinessRule _canUserBeProjectManagerBusinessRule;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly ICommunicationBus _communicationBus;
    private readonly ITimeTrackDbContext _timeTrackDbContext;

    public CreateProjectRequest(
        IHexColorGenerator hexColorGenerator,
        ICanUserBeProjectManagerBusinessRule canUserBeProjectManagerBusinessRule,
        IAuthenticatedUserContext authenticatedUserContext,
        ICommunicationBus communicationBus,
        ITimeTrackDbContext timeTrackDbContext)
    {
        _hexColorGenerator = hexColorGenerator;
        _canUserBeProjectManagerBusinessRule = canUserBeProjectManagerBusinessRule;
        _authenticatedUserContext = authenticatedUserContext;
        _communicationBus = communicationBus;
        _timeTrackDbContext = timeTrackDbContext;
    }

    public async Task<ProjectDto> Handle(
        Command request,
        CancellationToken cancellationToken)
    {
        await ValidateRequest(request, cancellationToken);

        if (request.Color is null)
        {
            request = request with { Color = _hexColorGenerator.GenerateColor() };
        }

        var currentUserId = _authenticatedUserContext.GetRequiredUserId();

        var entity = new Project()
        {
            Name = request.Name,
            Color = request.Color,
            IsPublic = request.IsPublic,
            IsBillable = request.IsBillable,
            IsActive = true,
            ClientId = request.ClientId,
            ManagerUserId = request.ManagerId,
            CreatedById = currentUserId,
            Architecture = request.Architecture,
            Industry = request.Industry,
            CloudServices = request.CloudServices,
            TechStack = request.TechStack
        };

        _timeTrackDbContext.Projects.Add(entity);

        await _timeTrackDbContext.SaveChanges(cancellationToken);

        var client = await _timeTrackDbContext.Clients
            .Where(client => client.Id == entity.ClientId)
            .FirstOrDefaultAsync(cancellationToken);

        var notification = new ProjectCreatedEvent
        {
            EventId = Guid.CreateVersion7().ToString(),
            ClientId = client!.Id,
            ProjectId = entity.Id,
            ProjectName = entity.Name,
            ClientName = client.Name,
            CreatedById = _authenticatedUserContext.UserId,
            IsActive = entity.IsActive,
            IsPublic = entity.IsPublic,
            Color = entity.Color,
        };

        await _communicationBus.PublishAsync(notification, cancellationToken);

        return ProjectDto.Of(entity);
    }

    private async Task ValidateRequest(Command request, CancellationToken cancellationToken)
    {
        if (request is { IsPublic: true, IsBillable: true })
        {
            throw new InvalidDataAppException(PortalMessages.ProjectCantBePublicAndBillable);
        }

        var isProjectWithSameNameExist = await _timeTrackDbContext.Projects
            .AnyAsync(project => project.Name.ToLower() == request.Name.ToLower(), cancellationToken);

        if (isProjectWithSameNameExist)
        {
            throw new EntityConflictException(PortalMessages.ProjectWithTheSameNameExists);
        }

        if (_authenticatedUserContext.GetRoles().IsHeadOfDepartment() && request.ManagerId.HasValue)
        {
            throw new ActionNotAllowedException(PortalMessages.HeadOfDepartmentCannotAssignProjectManager);
        }

        if (request.ManagerId is not null)
        {
            var canUserBeManager = await _canUserBeProjectManagerBusinessRule.IsCorrespondsToBusiness(request.ManagerId.Value, cancellationToken);

            if (!canUserBeManager)
            {
                throw new ActionNotAllowedException(PortalMessages.UserCantBeManager);
            }
        }
    }

    internal sealed record Command(string Name, string? Color, int? ManagerId, int ClientId, bool IsPublic,
        bool IsBillable, string? TechStack, string? Industry, string? CloudServices, string? Architecture)
        : ICommand<ProjectDto>, ITransactionalRequest, ITimeTrackRequest;

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
