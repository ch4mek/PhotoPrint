using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Communication;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TeamBoard.Contract;
using Modsen.Portal.TimeTrack.Contract.Events;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TimeTrack.UseCases.BusinessRules;
using Modsen.Portal.TimeTrack.UseCases.Projects.Core.BusinessRules;
using Modsen.Portal.TimeTrack.UseCases.Projects.Requests.RequestDtos;

namespace Modsen.Portal.TimeTrack.UseCases.Projects.Requests;

internal sealed class UpdateUserProjectAssignmentRequest
    : ICommandHandler<UpdateUserProjectAssignmentRequest.Command, UserProjectAssignmentDto>
{
    private readonly ITimeTrackDbContext _timeTrackDbContext;
    private readonly ICanTimeTrackUserBeRetrievedBusinessRule _canTimeTrackUserBeRetrievedBusinessRule;
    private readonly ICanUserProjectBeCreatedBusinessRule _canUserProjectBeCreatedBusinessRule;
    private readonly ICanUserProjectBeDecreasedBusinessRule _canUserProjectBeDecreasedBusinessRule;
    private readonly ICommunicationBus _communicationBus;
    private readonly ICanUserProjectAssignmentBeUpdatedBusinessRule _canUserProjectAssignmentBeUpdatedBusinessRule;
    private readonly ITeamBoardContract _teamBoardContract;

    public UpdateUserProjectAssignmentRequest(
        ITimeTrackDbContext timeTrackDbContext,
        ICanTimeTrackUserBeRetrievedBusinessRule canTimeTrackUserBeRetrievedBusinessRule,
        ICanUserProjectBeCreatedBusinessRule canUserProjectBeCreatedBusinessRule,
        ICanUserProjectBeDecreasedBusinessRule canUserProjectBeDecreasedBusinessRule,
        ICommunicationBus communicationBus,
        ICanUserProjectAssignmentBeUpdatedBusinessRule canUserProjectAssignmentBeUpdatedBusinessRule,
        ITeamBoardContract teamBoardContract)
    {
        _timeTrackDbContext = timeTrackDbContext;
        _canTimeTrackUserBeRetrievedBusinessRule = canTimeTrackUserBeRetrievedBusinessRule;
        _canUserProjectBeCreatedBusinessRule = canUserProjectBeCreatedBusinessRule;
        _canUserProjectBeDecreasedBusinessRule = canUserProjectBeDecreasedBusinessRule;
        _communicationBus = communicationBus;
        _canUserProjectAssignmentBeUpdatedBusinessRule = canUserProjectAssignmentBeUpdatedBusinessRule;
        _teamBoardContract = teamBoardContract;
    }

    public async Task<UserProjectAssignmentDto> Handle(Command request, CancellationToken cancellationToken)
    {
        var projectUser = await _timeTrackDbContext.ProjectUsers
            .Include(projectUser => projectUser.User)
            .Include(projectUser => projectUser.Project)
                .ThenInclude(project => project!.Client)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (projectUser is null)
        {
            throw new EntityNotFoundException($"There is no assignment with Id {request.Id}");
        }

        await ValidateRequestAsync(request, projectUser, cancellationToken);

        var isUserOrProjectUpdated = projectUser.UserId != request.UserId || projectUser.ProjectId != request.ProjectId;

        projectUser.UserId = request.UserId;
        projectUser.ProjectId = request.ProjectId;
        projectUser.IsTimeOnProjectBillable = request.IsTimeOnProjectBillable;
        projectUser.Fte = request.Fte;
        projectUser.UserOnProjectFrom = request.UserOnProjectFrom;
        projectUser.UserOnProjectTo = request.UserOnProjectTo;

        await _timeTrackDbContext.SaveChanges(cancellationToken);

        await PublishNotificationsAsync(request, projectUser.User!.Position, cancellationToken);

        var date = DateOnly.FromDateTime(DateTime.UtcNow);
        var isActive = projectUser.UserOnProjectTo == null
                || (projectUser.UserOnProjectFrom <= date && date <= projectUser.UserOnProjectTo)
                || date <= projectUser.UserOnProjectFrom;

        if (isUserOrProjectUpdated)
        {
            projectUser = await _timeTrackDbContext.ProjectUsers
               .Include(x => x.User)
               .Include(x => x.Project)
                   .ThenInclude(project => project!.Client)
               .FirstAsync(x => x.Id == request.Id, cancellationToken);
        }

        return new UserProjectAssignmentDto
        {
            Id = projectUser.Id,
            ProjectId = projectUser.ProjectId,
            UserId = projectUser.UserId,
            IsTimeOnProjectBillable = projectUser.IsTimeOnProjectBillable,
            UserOnProjectFrom = projectUser.UserOnProjectFrom,
            UserOnProjectTo = projectUser.UserOnProjectTo,
            ClientName = projectUser.Project!.Client!.Name,
            ProjectName = projectUser.Project.Name,
            UserFullName = projectUser.User!.LastName + " " + projectUser.User.FirstName,
            IsActive = isActive,
        };
    }

    private async Task ValidateRequestAsync(
        Command request,
        ProjectUser projectUser,
        CancellationToken cancellationToken)
    {
        await ValidateEntitiesExistAsync(request, cancellationToken);
        await ValidateUpdatePermissionsAsync(request, projectUser, cancellationToken);
        await ValidateDateRangeChangesAsync(request, projectUser, cancellationToken);
        await ValidateUserProjectChangeAsync(request, projectUser, cancellationToken);
    }

    private async Task ValidateEntitiesExistAsync(Command request, CancellationToken cancellationToken)
    {
        var isProjectExist = await _timeTrackDbContext.Projects
            .AnyAsync(project => project.Id == request.ProjectId, cancellationToken);

        if (!isProjectExist)
        {
            throw new EntityNotFoundException($"There is no project with Id {request.ProjectId}");
        }

        var isTimeTrackUserExisting = await _timeTrackDbContext.TimeTrackUsers
            .AnyAsync(user => user.UserId == request.UserId, cancellationToken);

        if (!isTimeTrackUserExisting)
        {
            throw new EntityNotFoundException($"There is no user with Id {request.UserId}");
        }
    }

    private async Task ValidateUpdatePermissionsAsync(
        Command request,
        ProjectUser projectUser,
        CancellationToken cancellationToken)
    {
        if (projectUser.User!.Position.IsIntern() && request.IsTimeOnProjectBillable)
        {
            throw new ActionNotAllowedException("Intern cannot be assigned to the project with billable hours");
        }

        var seniorityLevel = await _teamBoardContract.GetSeniorityLevelByUserProjectId(
            projectUser.Id, cancellationToken);

        var canBeUpdated = await _canUserProjectAssignmentBeUpdatedBusinessRule.IsCorrespondsToBusiness(
            new CanUserProjectAssignmentBeUpdatedModel(
                new AssignLessFieldsModels(projectUser.IsTimeOnProjectBillable, projectUser.Fte, seniorityLevel.Id),
                new AssignLessFieldsModels(request.IsTimeOnProjectBillable, request.Fte, request.UserOnProjectSeniorityLevelId),
                projectUser.UserId,
                projectUser.ProjectId,
                projectUser.UserOnProjectFrom,
                projectUser.UserOnProjectTo),
            cancellationToken);
        if (!canBeUpdated.IsSuccess)
        {
            throw new ActionNotAllowedException(canBeUpdated.Reason!);
        }

        var canBeRetrieved = await _canTimeTrackUserBeRetrievedBusinessRule.IsCorrespondsToBusiness(
            new CanTimeTrackUserBeRetrievedBusinessRuleModel(projectUser.UserId), cancellationToken);
        if (!canBeRetrieved.IsSuccess)
        {
            throw new ActionNotAllowedException(canBeRetrieved.Reason!);
        }
    }

    private async Task ValidateDateRangeChangesAsync(
        Command request,
        ProjectUser projectUser,
        CancellationToken cancellationToken)
    {
        if (IsDateRangeExpanded(request, projectUser))
        {
            await ValidateDateExpansionAsync(request, projectUser, cancellationToken);
        }

        if (IsDateRangeContracted(request, projectUser))
        {
            await ValidateDateContractionAsync(request, projectUser, cancellationToken);
        }
    }

    private async Task ValidateDateExpansionAsync(
        Command request,
        ProjectUser projectUser,
        CancellationToken cancellationToken)
    {
        var result = await _canUserProjectBeCreatedBusinessRule.IsCorrespondsToBusiness(
            new CanUserProjectBeCreatedModel(projectUser.Id, projectUser.UserId, projectUser.ProjectId,
                request.UserOnProjectFrom, request.UserOnProjectTo), cancellationToken);
        if (!result.IsSuccess)
        {
            throw new InvalidDataAppException(result.Reason!);
        }
    }

    private async Task ValidateDateContractionAsync(
        Command request,
        ProjectUser projectUser,
        CancellationToken cancellationToken)
    {
        var result = await _canUserProjectBeDecreasedBusinessRule.IsCorrespondsToBusiness(
            new CanUserProjectBeDecreasedModel(projectUser.ProjectId, projectUser.UserId, projectUser.UserOnProjectFrom,
                projectUser.UserOnProjectTo, request.UserOnProjectFrom, request.UserOnProjectTo), cancellationToken);
        if (!result.IsSuccess)
        {
            throw new InvalidDataAppException(result.Reason!);
        }
    }

    private static bool IsDateRangeExpanded(Command request, ProjectUser projectUser)
    {
        return request.UserOnProjectFrom < projectUser.UserOnProjectFrom ||
            request.UserOnProjectTo > projectUser.UserOnProjectTo ||
            request.UserOnProjectTo == null && projectUser.UserOnProjectTo != null;
    }

    private static bool IsDateRangeContracted(Command request, ProjectUser projectUser)
    {
        return request.UserOnProjectFrom > projectUser.UserOnProjectFrom ||
            request.UserOnProjectTo < projectUser.UserOnProjectTo ||
            request.UserOnProjectTo != null && projectUser.UserOnProjectTo == null;
    }

    private async Task ValidateUserProjectChangeAsync(
        Command request,
        ProjectUser projectUser,
        CancellationToken cancellationToken)
    {
        var isUserOrProjectChanged = request.UserId != projectUser.UserId || request.ProjectId != projectUser.ProjectId;

        if (!isUserOrProjectChanged)
        {
            return;
        }

        var assignments = await _timeTrackDbContext.ProjectUsers
            .Where(x => x.ProjectId == request.ProjectId && x.UserId == request.UserId)
            .Select(x => new { x.Id, x.UserOnProjectFrom, x.UserOnProjectTo })
            .ToListAsync(cancellationToken);

        if (assignments.Count > 0)
        {
            var canBeCreated = await _canUserProjectBeCreatedBusinessRule.IsCorrespondsToBusiness(
                new CanUserProjectBeCreatedModel(Id: null, request.UserId, request.ProjectId, request.UserOnProjectFrom,
                    request.UserOnProjectTo), cancellationToken);

            if (!canBeCreated.IsSuccess)
            {
                throw new InvalidDataAppException(canBeCreated.Reason!);
            }
        }

        var timesheetFilled = await _timeTrackDbContext.Timesheets
            .AnyAsync(x => x.ProjectId == request.ProjectId && x.UserId == request.UserId, cancellationToken);

        if (timesheetFilled)
        {
            throw new InvalidDataAppException(PortalMessages.TimesheetIsFilledForPeriod);
        }
    }


    private Task PublishNotificationsAsync(Command request, string? userPosition, CancellationToken cancellationToken)
    {
        var notification = new ProjectUserUpdatedEvent
        {
            EventId = Guid.CreateVersion7().ToString(),
            ProjectUserId = request.Id,
            UserId = request.UserId,
            UserPosition = userPosition,
            ProjectId = request.ProjectId,
            Fte = request.Fte,
            IsTimeOnProjectBillable = request.IsTimeOnProjectBillable,
            UserOnProjectSeniorityLevelId = request.UserOnProjectSeniorityLevelId,
            UserOnProjectFrom = request.UserOnProjectFrom,
            UserOnProjectTo = request.UserOnProjectTo
        };

        return _communicationBus.PublishAsync(notification, cancellationToken);
    }

    internal sealed record Command(int Id,
        int UserId,
        int ProjectId,
        bool IsTimeOnProjectBillable,
        decimal Fte,
        string UserOnProjectSeniorityLevelId,
        DateOnly UserOnProjectFrom,
        DateOnly? UserOnProjectTo)
     : ICommand<UserProjectAssignmentDto>, ITransactionalRequest;

    internal sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotNull().NotEmpty();

            RuleFor(x => x.UserId).NotNull().NotEmpty();

            RuleFor(x => x.ProjectId).NotNull().NotEmpty();

            RuleFor(x => x.UserOnProjectSeniorityLevelId).NotNull().NotEmpty();

            RuleFor(x => x.Fte).NotNull().InclusiveBetween(0m, 1m);

            RuleFor(x => x.IsTimeOnProjectBillable).NotNull();

            RuleFor(x => x.UserOnProjectFrom).NotNull();

        }
    }
}
