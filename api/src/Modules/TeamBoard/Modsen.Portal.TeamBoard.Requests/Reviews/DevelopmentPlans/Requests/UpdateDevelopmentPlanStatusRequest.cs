using System.Text.Json;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Communication;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Notification;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.TeamBoard;
using Modsen.Portal.Identity.Contract;
using Modsen.Portal.Notification.Contract.Events;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.Requests.Core.CoreDto;
using Modsen.Portal.TeamBoard.Requests.Core.DbQueries;
using Modsen.Portal.TeamBoard.Requests.Reviews.DevelopmentPlans.Core;
using Modsen.Portal.TeamBoard.Requests.Reviews.DevelopmentPlans.Core.CoreDto;
using Modsen.Portal.TeamBoard.Requests.Reviews.DevelopmentPlans.Requests.RequestDto;

namespace Modsen.Portal.TeamBoard.Requests.Reviews.DevelopmentPlans.Requests;

internal sealed class UpdateDevelopmentPlanStatusRequest : ICommandHandler<UpdateDevelopmentPlanStatusRequest.Command, DevelopmentPlanDto>
{
    private readonly IDevelopmentPlanLoader _developmentPlanLoader;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly IGetTeamBoardUsersWithNoIncludesDbQuery _getTeamBoardUsers;
    private readonly IServiceProvider _serviceProvider;
    private readonly IIdentityContract _identityContract;
    private readonly ITeamBoardDbContext _teamBoardDbContext;

    public UpdateDevelopmentPlanStatusRequest(IDevelopmentPlanLoader developmentPlanLoader,
        IAuthenticatedUserContext authenticatedUserContext,
        IServiceProvider serviceProvider,
        IIdentityContract identityContract,
        IGetTeamBoardUsersWithNoIncludesDbQuery getTeamBoardUsers,
        ITeamBoardDbContext teamBoardDbContext)
    {
        _developmentPlanLoader = developmentPlanLoader;
        _authenticatedUserContext = authenticatedUserContext;
        _serviceProvider = serviceProvider;
        _identityContract = identityContract;
        _getTeamBoardUsers = getTeamBoardUsers;
        _teamBoardDbContext = teamBoardDbContext;
    }

    public async Task<DevelopmentPlanDto> Handle(Command request, CancellationToken cancellationToken)
    {
        var developmentPlan = await _developmentPlanLoader.LoadAsync(request.DevelopmentPlanId, cancellationToken);

        if (developmentPlan == null)
        {
            throw new EntityNotFoundException();
        }

        if (developmentPlan.UserPosition.IsIntern())
        {
            throw new ActionNotAllowedException(PortalMessages.NotAllowedOperationWithIntern);
        }

        var userforChangingStatus = await _getTeamBoardUsers.ExecuteAsync(developmentPlan.UserId, cancellationToken);

        if (IsUnderReviewTransition(request, developmentPlan))
        {
            await HandleUnderReviewStatusAsync(request, developmentPlan, userforChangingStatus, cancellationToken);
        }
        else if (IsApprovedOrRejected(request))
        {
            await HandleApprovedOrRejectedAsync(request, developmentPlan, userforChangingStatus, cancellationToken);
        }

        return ToDto(developmentPlan);
    }

    private static bool IsUnderReviewTransition(Command request, DevelopmentPlanCoreDto developmentPlan)
    {
        return request.Status == DevelopmentPlanStatusType.UnderReview &&
            (developmentPlan.Status == DevelopmentPlanStatusType.None || developmentPlan.Status == DevelopmentPlanStatusType.Rejected);
    }

    private static bool IsApprovedOrRejected(Command request)
    {
        return request.Status == DevelopmentPlanStatusType.Approved || request.Status == DevelopmentPlanStatusType.Rejected;
    }

    private async Task HandleUnderReviewStatusAsync(Command request, DevelopmentPlanCoreDto developmentPlan,
        BasicTeamBoardUserCoreDto? userforChangingStatus, CancellationToken cancellationToken)
    {
        EnsurePlanOwner(developmentPlan);
        await ApplyStatusUpdateAsync(developmentPlan.Id, request.Status, request.ConfirmationDate, cancellationToken);

        if (userforChangingStatus != null)
        {
            var approversIds = await _identityContract.GetHigherManagerIds(userforChangingStatus.UserId, cancellationToken);
            await PublishNotification(request, developmentPlan.UserId,
                $"{userforChangingStatus.FullName} is expecting for review of the plan item.",
                approversIds, NotificationTypes.DevelopmentPlanSentForReview, cancellationToken);
        }
    }

    private void EnsurePlanOwner(DevelopmentPlanCoreDto developmentPlan)
    {
        if (developmentPlan.UserId != _authenticatedUserContext.UserId)
        {
            throw new EntityNotFoundException();
        }
    }

    private async Task HandleApprovedOrRejectedAsync(Command request, DevelopmentPlanCoreDto developmentPlan,
        BasicTeamBoardUserCoreDto? userforChangingStatus, CancellationToken cancellationToken)
    {
        ValidateApprovalPermissions(userforChangingStatus);
        ValidateConfirmationDate(request, developmentPlan);

        await ApplyStatusUpdateAsync(developmentPlan.Id, request.Status, request.ConfirmationDate, cancellationToken);

        if (userforChangingStatus != null)
        {
            var message = request.Status == DevelopmentPlanStatusType.Rejected
                ? "Your plan item has been rejected."
                : "Your plan item has been approved.";
            await PublishNotification(request, developmentPlan.UserId, message,
                [userforChangingStatus.UserId], NotificationTypes.DevelopmentPlanReviewed, cancellationToken);
        }
    }

    private void ValidateApprovalPermissions(BasicTeamBoardUserCoreDto? userforChangingStatus)
    {
        var roles = _authenticatedUserContext.GetRoles();

        if (_authenticatedUserContext.GetRequiredUserId() == userforChangingStatus?.UserId)
        {
            throw new EntityNotFoundException("You can't approve/rejected your Development plans");
        }

        if (roles.IsEmployee() || roles.IsSystemAdministrator() || roles.IsIntern())
        {
            throw new EntityNotFoundException("User with this role can't approve/rejected any Development plans");
        }

        if (roles.IsHeadOfDepartment() && _authenticatedUserContext.DepartmentId != userforChangingStatus?.DepartmentId)
        {
            throw new EntityNotFoundException("User with this role can't approve/rejected  User not from his department");
        }

        if (roles.IsGroupManager() && _authenticatedUserContext.ResourceGroupId != userforChangingStatus?.ResourceGroupId)
        {
            throw new EntityNotFoundException("User with this role can't approve/rejected  User not from his resource group");
        }
    }

    private static void ValidateConfirmationDate(Command request, DevelopmentPlanCoreDto developmentPlan)
    {
        if (request.Status == DevelopmentPlanStatusType.Approved && request.ConfirmationDate <= developmentPlan.StartDate)
        {
            throw new InvalidDataException("Confirmation Date can't be less or equal than Start date");
        }
    }

    private async Task ApplyStatusUpdateAsync(int developmentPlanId, DevelopmentPlanStatusType status,
        DateOnly? confirmationDate, CancellationToken cancellationToken)
    {
        var entity = await _teamBoardDbContext.DevelopmentPlans.FirstAsync(x => x.Id == developmentPlanId, cancellationToken);
        entity.Status = status;
        entity.OrderWeight = (int)status * 100;
        entity.EndDate = confirmationDate;
        await _teamBoardDbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task PublishNotification(Command command, int userId, string message, int[] messageForUserIds,
        NotificationTypes notificationType, CancellationToken cancellationToken)
    {
        var notification = GetReportGeneratingNotification(command, userId, message, messageForUserIds, notificationType);

        using (var scope = _serviceProvider.CreateScope())
        {
            var communicationBus = scope.ServiceProvider.GetRequiredService<ICommunicationBus>();
            var connectionFactory = scope.ServiceProvider.GetRequiredService<IConnectionFactory>();

            await communicationBus.PublishAsync(notification, cancellationToken);
            await connectionFactory.CommitCurrentTransaction(cancellationToken);
        }
    }

    private static NotificationsCreatedEvent GetReportGeneratingNotification(Command command,
        int userId,
        string message,
        int[] userIds,
        NotificationTypes notificationType)
    {
        var output = new DevelopmentPlanStatusChangedNotificaitonOutputDto
        {
            Status = command.Status.ToString(),
            Message = message,
            DevelopmentPlanId = command.DevelopmentPlanId,
            UserId = userId
        };

        return new NotificationsCreatedEvent
        {
            EventId = Guid.CreateVersion7().ToString(),
            UserIds = userIds.ToList(),
            Arguments = JsonSerializer.Serialize(output),
            NotificationChannel = NotificationChannels.SignalR,
            NotificationType = notificationType
        };
    }

    private static DevelopmentPlanDto ToDto(DevelopmentPlanCoreDto developmentPlan)
    {
        return new DevelopmentPlanDto(developmentPlan.Id,
            developmentPlan.StartDate,
            developmentPlan.DeadlineDate,
            developmentPlan.EndDate,
            developmentPlan.Summary,
            developmentPlan.Notes,
            developmentPlan.UserId);
    }

    internal sealed record Command(
        int DevelopmentPlanId,
        DevelopmentPlanStatusType Status,
        DateOnly? ConfirmationDate)
        : ICommand<DevelopmentPlanDto>, ITransactionalRequest;

    internal sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.DevelopmentPlanId).NotNull().NotEmpty();
            RuleFor(x => x.Status).NotEqual(DevelopmentPlanStatusType.None);
            When(x => x.Status != DevelopmentPlanStatusType.Approved, () =>
            {
                RuleFor(x => x.ConfirmationDate).Null();
            });
        }
    }
}
