using System.Text.Json;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Communication;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Notification;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Notification.Contract.Events;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.Requests.Contracts.Requests.RequestDtos;
using Modsen.Portal.TeamBoard.Requests.TeamBoardPortalSettings;

namespace Modsen.Portal.TeamBoard.Requests.Contracts.Requests;

internal sealed class ContractExpirationNoticeRequest : ICommandHandler<ContractExpirationNoticeRequest.Command, Unit>
{
    private readonly ITeamBoardDbContext _teamBoardDbContext;
    private readonly ICommunicationBus _communicationBus;
    private readonly IClockService _clockService;
    private readonly ITeamBoardPortalSettingsAccessor _teamBoardPortalSettingsAccessor;

    public ContractExpirationNoticeRequest(
        ITeamBoardDbContext teamBoardDbContext,
        ICommunicationBus communicationBus,
        IClockService clockService,
        ITeamBoardPortalSettingsAccessor teamBoardPortalSettingsAccessor)
    {
        _teamBoardDbContext = teamBoardDbContext;
        _communicationBus = communicationBus;
        _clockService = clockService;
        _teamBoardPortalSettingsAccessor = teamBoardPortalSettingsAccessor;
    }

    public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
    {
        var settings = await _teamBoardPortalSettingsAccessor.GetSettings(cancellationToken);
        var today = _clockService.TodayUtc();
        var contractExpirationFirstReminderInDays = today.AddDays(settings.ContractExpirationFirstReminderInDays - 1);
        var contractExpirationSecondReminderInDays = today.AddDays(settings.ContractExpirationSecondReminderInDays - 1);

        var contracts = await _teamBoardDbContext.UserContracts
            .Include(x => x.User)
            .Where(x => x.User != null && x.User.IsActive)
            .Where(x => !x.FireDate.HasValue)
            .Where(x => x.IsActive &&
                x.User!.Position != PortalRoleNames.Ceo &&
                x.User.Position != PortalRoleNames.Cto &&
                (x.ContractTo == contractExpirationFirstReminderInDays ||
                    x.ContractTo == contractExpirationSecondReminderInDays))
            .Select(x => new {
                x.UserId,
                x.ContractTo,
                x.User!.ResourceGroupId,
                x.User.DepartmentId,
                x.User!.Position,
                x.User.FirstName,
                x.User.LastName
            })
            .ToListAsync(cancellationToken);

        foreach (var contract in contracts)
        {
            var managerId = await GetManagerId(
                contract.Position,
                contract.ResourceGroupId,
                contract.DepartmentId,
                cancellationToken);

            if (managerId is null || contract.ContractTo is null)
            {
                continue;
            }

            var daysLeft = contract.ContractTo.Value.DayNumber - today.DayNumber + 1;

            await SendContractExpiringNotification(contract.UserId, contract.FirstName, contract.LastName,
                daysLeft, managerId.Value, cancellationToken);
        }

        return Unit.Value;
    }

    public async Task<int?> GetManagerId(string? position, int? resourceGroupId, string? departmentId,
        CancellationToken cancellationToken)
    {
        switch (position)
        {
            case PortalRoleNames.Employee:
            case PortalRoleNames.SystemAdministrator:
                {
                    if (resourceGroupId != null)
                    {
                        var groupManager = await _teamBoardDbContext.TeamBoardUsers
                            .Where(u =>
                                u.IsActive &&
                                u.Position == PortalRoleNames.GroupManager &&
                                u.ResourceGroupId == resourceGroupId.Value)
                            .Select(x => new { x.UserId })
                            .FirstOrDefaultAsync(cancellationToken);

                        if (groupManager is not null)
                        {
                            return groupManager.UserId;
                        }
                    }

                    var headOfDepartmentId = await GetHeadOfDepartmentByDepartmentId(departmentId!, cancellationToken);

                    if (headOfDepartmentId is not null)
                    {
                        return headOfDepartmentId;
                    }

                    return await GetDepartmentManagerId(departmentId!, cancellationToken);
                }
            case PortalRoleNames.GroupManager:
                {
                    var headOfDepartmentId = await GetHeadOfDepartmentByDepartmentId(departmentId!, cancellationToken);

                    if (headOfDepartmentId is not null)
                    {
                        return headOfDepartmentId;
                    }

                    return await GetDepartmentManagerId(departmentId!, cancellationToken);
                }
            case PortalRoleNames.HeadOfDepartment:
                {
                    return await GetDepartmentManagerId(departmentId!, cancellationToken);
                }
            default:
                {
                    return null;
                }
        }
    }

    private async Task<int?> GetDepartmentManagerId(string departmentId, CancellationToken cancellationToken)
    {
        var department = await _teamBoardDbContext.TeamboardDepartments
            .FirstOrDefaultAsync(x => x.DepartmentId == departmentId, cancellationToken);

        if (department is null)
        {
            return null;
        }

        var managerIds = await GetUserIdsByRoleName(department.DepartmentReportingToRoleName, cancellationToken);

        return managerIds.FirstOrDefault();
    }

    private async Task<int?> GetHeadOfDepartmentByDepartmentId(string departmentId, CancellationToken cancellationToken)
    {
        var headOfDepartment = await _teamBoardDbContext.TeamBoardUsers
            .Where(x =>
                x.IsActive &&
                x.Position == PortalRoleNames.HeadOfDepartment &&
                x.DepartmentId == departmentId)
            .Select(x => new { x.UserId })
            .FirstOrDefaultAsync(cancellationToken);

        return headOfDepartment?.UserId;
    }

    private Task<List<int>> GetUserIdsByRoleName(string? roleName, CancellationToken cancellationToken)
    {
        return _teamBoardDbContext.TeamBoardUsers
            .Where(x => x.IsActive && x.Position == roleName)
            .Select(x => x.UserId)
            .ToListAsync(cancellationToken);
    }

    private async Task SendContractExpiringNotification(int userId, string firstName, string lastName, int daysLeft, int managerId, CancellationToken cancellationToken)
    {
        var contractExpirationOutput = new ContractExpirationOutputDto
        {
            UserId = userId,
            UserFirstName = firstName,
            UserLastName = lastName,
            DaysLeft = daysLeft,
        };

        var notification = new NotificationsCreatedEvent
        {
            EventId = Guid.CreateVersion7().ToString(),
            UserIds = new List<int> { managerId },
            Arguments = JsonSerializer.Serialize(contractExpirationOutput),
            NotificationChannel = NotificationChannels.SignalR,
            NotificationType = NotificationTypes.EmployeeContractExpiring
        };

        await _communicationBus.PublishAsync(notification, cancellationToken);
    }


    internal sealed record Command : ICommand<Unit>, ITransactionalRequest;
}

