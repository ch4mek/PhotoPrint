using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Communication;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.TeamBoard;
using Modsen.Portal.TeamBoard.Contract;
using Modsen.Portal.TimeTrack.Contract.Events;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TimeTrack.UseCases.BusinessRules;
using Modsen.Portal.TimeTrack.UseCases.Projects.Core.BusinessRules;
using Modsen.Portal.TimeTrack.UseCases.Projects.Requests.RequestDtos;
using Modsen.Portal.Core.Identity;

namespace Modsen.Portal.TimeTrack.UseCases.Projects.Requests;

internal sealed class CreateUserProjectAssignmentRequest : ICommandHandler<CreateUserProjectAssignmentRequest.Command,
    UserProjectAssignmentDto>
{
    private readonly ITimeTrackDbContext _timeTrackDbContext;
    private readonly ICanTimeTrackUserBeRetrievedBusinessRule _canTimeTrackUserBeRetrievedBusinessRule;
    private readonly ICanUserProjectBeCreatedBusinessRule _canUserProjectBeCreatedBusinessRule;
    private readonly ICanUserGetProjectBusinessRule _canUserGetProjectBusinessRule;
    private readonly ICommunicationBus _communicationBus;
    private readonly ITeamBoardContract _teamBoardContract;

    public CreateUserProjectAssignmentRequest(
        ITimeTrackDbContext timeTrackDbContext,
        ICanTimeTrackUserBeRetrievedBusinessRule canTimeTrackUserBeRetrievedBusinessRule,
        ICanUserProjectBeCreatedBusinessRule canUserProjectBeCreatedBusinessRule,
        ICommunicationBus communicationBus,
        ITeamBoardContract teamBoardContract,
        ICanUserGetProjectBusinessRule canUserGetProjectBusinessRule)
    {
        _timeTrackDbContext = timeTrackDbContext;
        _canTimeTrackUserBeRetrievedBusinessRule = canTimeTrackUserBeRetrievedBusinessRule;
        _canUserProjectBeCreatedBusinessRule = canUserProjectBeCreatedBusinessRule;
        _communicationBus = communicationBus;
        _teamBoardContract = teamBoardContract;
        _canUserGetProjectBusinessRule = canUserGetProjectBusinessRule;
    }

    public async Task<UserProjectAssignmentDto> Handle(Command request, CancellationToken cancellationToken)
    {
        var user = await _timeTrackDbContext.TimeTrackUsers.FirstOrDefaultAsync(user => user.UserId == request.UserId, cancellationToken);

        await ValidateRequestAsync(request, user, cancellationToken);

        var projectUserEntity = new ProjectUser()
        {
            UserId = request.UserId,
            ProjectId = request.ProjectId,
            UserOnProjectFrom = request.UserOnProjectFrom,
            UserOnProjectTo = request.UserOnProjectTo,
            IsTimeOnProjectBillable = request.IsTimeOnProjectBillable,
            Fte = request.Fte,
        };

        _timeTrackDbContext.ProjectUsers.Add(projectUserEntity);

        await _timeTrackDbContext.SaveChanges(cancellationToken);

        var notification = new ProjectUserCreatedEvent()
        {
            EventId = Guid.CreateVersion7().ToString(),
            Id = projectUserEntity.Id,
            UserId = projectUserEntity.UserId,
            UserPosition = user!.Position,
            ProjectId = projectUserEntity.ProjectId,
            UserOnProjectFrom = projectUserEntity.UserOnProjectFrom,
            UserOnProjectTo = projectUserEntity.UserOnProjectTo,
            IsTimeOnProjectBillable = projectUserEntity.IsTimeOnProjectBillable,
            Fte = projectUserEntity.Fte,
            UserOnProjectSeniorityLevelId = request.UserOnProjectSeniorityLevelId,
        };

        await _communicationBus.PublishAsync(notification, cancellationToken);

        var currentDate = DateOnly.FromDateTime(DateTime.UtcNow);

        var assignmentInfo = await _timeTrackDbContext.ProjectUsers
            .Where(projectUser => projectUser.Id == projectUserEntity.Id)
            .Select(projectUser => new
            {
                ClientName = projectUser.Project!.Client!.Name,
                ProjectName = projectUser.Project.Name,
                UserFullName = projectUser.User!.LastName + " " + projectUser.User.FirstName,
                IsActive = currentDate <= projectUser.UserOnProjectFrom ||
                (currentDate >= projectUser.UserOnProjectFrom &&
                (projectUser.UserOnProjectTo == null || currentDate <= projectUser.UserOnProjectTo))
            })
            .FirstOrDefaultAsync(cancellationToken);

        var response = new UserProjectAssignmentDto
        {
            Id = projectUserEntity.Id,
            ProjectId = projectUserEntity.ProjectId,
            UserId = projectUserEntity.UserId,
            IsTimeOnProjectBillable = projectUserEntity.IsTimeOnProjectBillable,
            UserOnProjectFrom = projectUserEntity.UserOnProjectFrom,
            UserOnProjectTo = projectUserEntity.UserOnProjectTo,
            ClientName = assignmentInfo!.ClientName,
            ProjectName = assignmentInfo.ProjectName,
            UserFullName = assignmentInfo.UserFullName,
            IsActive = assignmentInfo.IsActive,
        };

        return response;
    }

    private async Task ValidateRequestAsync(Command request, TimeTrackUser? user, CancellationToken cancellationToken)
    {
        var isProjectExist = await _timeTrackDbContext.Projects
            .AnyAsync(project => project.Id == request.ProjectId, cancellationToken);

        if (!isProjectExist)
        {
            throw new EntityNotFoundException($"There is no project with Id {request.ProjectId}");
        }

        if (user is null)
        {
            throw new EntityNotFoundException($"There is no user with Id {request.UserId}");
        }

        if (user.Position.IsIntern() && request.IsTimeOnProjectBillable)
        {
            throw new ActionNotAllowedException("Intern cannot be assigned to the project with billable hours");
        }

        var seniorityLevels = await _teamBoardContract.GetSeniorityLevels(SeniorityLevelScopes.ProjectUser, cancellationToken);

        if (seniorityLevels.All(x => x.Id != request.UserOnProjectSeniorityLevelId))
        {
            throw new EntityNotFoundException($"There is no project seniority level with Id {request.UserOnProjectSeniorityLevelId}");
        }

        var canBeRetrieved = await _canTimeTrackUserBeRetrievedBusinessRule.IsCorrespondsToBusiness(
            new CanTimeTrackUserBeRetrievedBusinessRuleModel(request.UserId), cancellationToken);

        if (!canBeRetrieved.IsSuccess)
        {
            throw new ActionNotAllowedException(canBeRetrieved.Reason!);
        }

        var canBeCreated = await _canUserProjectBeCreatedBusinessRule.IsCorrespondsToBusiness(
            new CanUserProjectBeCreatedModel(Id: null, request.UserId, request.ProjectId, request.UserOnProjectFrom,
                request.UserOnProjectTo), cancellationToken);

        if (!canBeCreated.IsSuccess)
        {
            throw new InvalidDataAppException(canBeCreated.Reason!);
        }

        var canRetrieveProject = await _canUserGetProjectBusinessRule.IsCorrespondsToBusiness(request.ProjectId, cancellationToken);

        if (!canRetrieveProject.IsSuccess)
        {
            throw new ActionNotAllowedException(canRetrieveProject.Reason!);
        }
    }

    internal sealed record Command(int UserId, int ProjectId, DateOnly UserOnProjectFrom, DateOnly? UserOnProjectTo,
        bool IsTimeOnProjectBillable, decimal Fte, string UserOnProjectSeniorityLevelId)
        : ICommand<UserProjectAssignmentDto>, ITransactionalRequest;

    internal sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.UserId).NotNull().NotEmpty();

            RuleFor(x => x.ProjectId).NotNull().NotEmpty();

            RuleFor(x => x.Fte).NotNull().InclusiveBetween(0m, 1m);

            RuleFor(x => x.UserOnProjectFrom).NotNull();

            When(x => x.UserOnProjectTo is not null, () =>
                RuleFor(x => x.UserOnProjectTo).GreaterThanOrEqualTo(x => x.UserOnProjectFrom));

            RuleFor(x => x.IsTimeOnProjectBillable).NotNull();
        }
    }
}
