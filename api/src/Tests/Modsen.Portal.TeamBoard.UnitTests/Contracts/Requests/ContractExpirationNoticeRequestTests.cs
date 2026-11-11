using MediatR;
using MockQueryable.Moq;
using Modsen.Portal.Core.Communication;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Notification;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Notification.Contract.Events;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.Contracts.Requests;
using Modsen.Portal.TeamBoard.Requests.TeamBoardPortalSettings;
using Modsen.Portal.TeamBoard.Requests.TeamBoardPortalSettings.CoreDto;
using Moq;

namespace Modsen.Portal.TeamBoard.UnitTests.Contracts.Requests;

public class ContractExpirationNoticeRequestTests
{
    private readonly Mock<ITeamBoardDbContext> _teamBoardDbContextMock = new();
    private readonly Mock<ICommunicationBus> _communicationBusMock = new();
    private readonly Mock<IClockService> _clockServiceMock = new();
    private readonly Mock<ITeamBoardPortalSettingsAccessor> _settingsAccessorMock = new();
    private readonly ContractExpirationNoticeRequest _handler;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly TeamBoardPortalSettingsDto _settings = new()
    {
        ContractExpirationFirstReminderInDays = 45,
        ContractExpirationSecondReminderInDays = 90
    };

    public ContractExpirationNoticeRequestTests()
    {
        _handler = new ContractExpirationNoticeRequest(
            _teamBoardDbContextMock.Object,
            _communicationBusMock.Object,
            _clockServiceMock.Object,
            _settingsAccessorMock.Object);

        _clockServiceMock.Setup(x => x.TodayUtc()).Returns(DateOnly.FromDateTime(DateTime.UtcNow));

        _settingsAccessorMock
            .Setup(x => x.GetSettings(_cancellationToken))
            .ReturnsAsync(_settings);
    }

    [Fact]
    public async Task HandleNoContractsFoundShouldReturnUnitWithoutNotifications()
    {
        // Arrange
        var command = new ContractExpirationNoticeRequest.Command();
        SetupUserContracts([]);

        // Act
        var result = await _handler.Handle(command, _cancellationToken);

        // Assert
        result.Should().Be(Unit.Value);
        _communicationBusMock.Verify(
            x => x.PublishAsync(It.IsAny<NotificationsCreatedEvent>(), _cancellationToken),
            Times.Never());
    }

    [Fact]
    public async Task HandleContractExpiringInFirstReminderPeriodShouldSendNotificationToGroupManager()
    {
        // Arrange
        var command = new ContractExpirationNoticeRequest.Command();
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var contractTo = today.AddDays(_settings.ContractExpirationFirstReminderInDays - 1);
        var contract = new
        {
            UserId = 1,
            ContractTo = contractTo,
            ResourceGroupId = 100,
            DepartmentId = "Js",
            Position = PortalRoleNames.Employee,
            FirstName = "John",
            LastName = "Doe",
            Email = "test",
            IsActive = true,
        };
        var contracts = new List<object> { contract };
        var groupManagerId = 2;

        SetupUserContracts(contracts);
        SetupTeamBoardUsers([(groupManagerId, PortalRoleNames.GroupManager, 100, "Js")]);
        SetupDepartments([("Js", PortalRoleNames.Cto)]);

        // Act
        var result = await _handler.Handle(command, _cancellationToken);

        // Assert
        result.Should().Be(Unit.Value);
        _communicationBusMock.Verify(
            x => x.PublishAsync(
                It.Is<NotificationsCreatedEvent>(n =>
                    n.UserIds.Contains(groupManagerId) &&
                    n.NotificationType == NotificationTypes.EmployeeContractExpiring &&
                    n.NotificationChannel == NotificationChannels.SignalR),
                _cancellationToken),
            Times.Once());
    }

    [Fact]
    public async Task HandleContractExpiringInSecondReminderPeriodForHeadOfDepartmentShouldSendNotificationToCeo()
    {
        // Arrange
        var command = new ContractExpirationNoticeRequest.Command();
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var contractTo = today.AddDays(_settings.ContractExpirationSecondReminderInDays - 1);
        var contract = new
        {
            UserId = 1,
            ContractTo = contractTo,
            ResourceGroupId = (int?)null,
            DepartmentId = "Net",
            Position = PortalRoleNames.HeadOfDepartment,
            FirstName = "Jane",
            LastName = "Smith",
            Email = "test",
            IsActive = true,
        };
        var contracts = new List<object> { contract };
        var ceoId = 3;

        SetupUserContracts(contracts);
        SetupTeamBoardUsers([(ceoId, PortalRoleNames.Ceo, null, null)]);
        SetupDepartments([("Net", PortalRoleNames.Ceo)]);

        // Act
        var result = await _handler.Handle(command, _cancellationToken);

        // Assert
        result.Should().Be(Unit.Value);
        _communicationBusMock.Verify(
            x => x.PublishAsync(
                It.Is<NotificationsCreatedEvent>(n =>
                    n.UserIds.Contains(ceoId) &&
                    n.NotificationType == NotificationTypes.EmployeeContractExpiring &&
                    n.NotificationChannel == NotificationChannels.SignalR),
                _cancellationToken),
            Times.Once());
    }

    [Fact]
    public async Task HandleContractWithoutContractToShouldSkipNotification()
    {
        // Arrange
        var command = new ContractExpirationNoticeRequest.Command();
        var contract = new
        {
            UserId = 1,
            ContractTo = (DateOnly?)null,
            ResourceGroupId = 100,
            DepartmentId = "Js",
            Position = PortalRoleNames.Employee,
            FirstName = "John",
            LastName = "Doe",
            Email = "test",
            IsActive = true,
        };
        var contracts = new List<object> { contract };

        SetupUserContracts(contracts);
        SetupTeamBoardUsers([(2, PortalRoleNames.GroupManager, 100, "Js")]);
        SetupDepartments([("Js", PortalRoleNames.Cto)]);

        // Act
        var result = await _handler.Handle(command, _cancellationToken);

        // Assert
        result.Should().Be(Unit.Value);
        _communicationBusMock.Verify(
            x => x.PublishAsync(It.IsAny<NotificationsCreatedEvent>(), _cancellationToken),
            Times.Never());
    }

    [Fact]
    public async Task HandleSystemAdministratorContractShouldSendNotificationToHeadOfDepartment()
    {
        // Arrange
        var command = new ContractExpirationNoticeRequest.Command();
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var contractTo = today.AddDays(_settings.ContractExpirationFirstReminderInDays - 1);
        var contract = new
        {
            UserId = 1,
            ContractTo = contractTo,
            ResourceGroupId = (int?)null,
            DepartmentId = "Js",
            Position = PortalRoleNames.SystemAdministrator,
            FirstName = "Alice",
            LastName = "Brown",
            Email = "test",
            IsActive = true,
        };
        var contracts = new List<object> { contract };
        var headOfDepartmentId = 4;

        SetupUserContracts(contracts);
        SetupTeamBoardUsers([(headOfDepartmentId, PortalRoleNames.HeadOfDepartment, null, "Js")]);
        SetupDepartments([("Js", PortalRoleNames.Cto)]);

        // Act
        var result = await _handler.Handle(command, _cancellationToken);

        // Assert
        result.Should().Be(Unit.Value);
        _communicationBusMock.Verify(
            x => x.PublishAsync(
                It.Is<NotificationsCreatedEvent>(n =>
                    n.UserIds.Contains(headOfDepartmentId) &&
                    n.NotificationType == NotificationTypes.EmployeeContractExpiring &&
                    n.NotificationChannel == NotificationChannels.SignalR),
                _cancellationToken),
            Times.Once());
    }

    [Fact]
    public async Task GetManagerIdEmployeeWithNoGroupManagerShouldReturnHeadOfDepartment()
    {
        // Arrange
        var position = PortalRoleNames.Employee;
        var resourceGroupId = 100;
        var departmentId = "Js";
        var headOfDepartmentId = 4;

        SetupTeamBoardUsers([(headOfDepartmentId, PortalRoleNames.HeadOfDepartment, null, departmentId)]);
        SetupDepartments([(departmentId, PortalRoleNames.Cto)]);

        // Act
        var managerId = await _handler.GetManagerId(position, resourceGroupId, departmentId, _cancellationToken);

        // Assert
        managerId.Should().Be(headOfDepartmentId);
    }

    [Fact]
    public async Task GetManagerIdHeadOfDepartmentInSalesShouldReturnCeo()
    {
        // Arrange
        var position = PortalRoleNames.HeadOfDepartment;
        var departmentId = "Sales";
        var ceoId = 3;

        SetupTeamBoardUsers([(ceoId, PortalRoleNames.Ceo, null, null)]);
        SetupDepartments([(departmentId, PortalRoleNames.Ceo)]);

        // Act
        var managerId = await _handler.GetManagerId(position, null, departmentId, _cancellationToken);

        // Assert
        managerId.Should().Be(ceoId);
    }

    [Fact]
    public async Task GetManagerIdGroupManagerWithNoHeadOfDepartmentShouldReturnCto()
    {
        // Arrange
        var position = PortalRoleNames.GroupManager;
        var departmentId = "Js";
        var ctoId = 5;

        SetupTeamBoardUsers([(ctoId, PortalRoleNames.Cto, null, null)]);
        SetupDepartments([(departmentId, PortalRoleNames.Cto)]);

        // Act
        var managerId = await _handler.GetManagerId(position, null, departmentId, _cancellationToken);

        // Assert
        managerId.Should().Be(ctoId);
    }

    private void SetupUserContracts(List<object> contracts)
    {
        var userContracts = contracts.Select(c => new UserContract
        {
            UserId = (int)c.GetType().GetProperty("UserId")!.GetValue(c)!,
            ContractTo = (DateOnly?)c.GetType().GetProperty("ContractTo")!.GetValue(c),
            IsActive = true,
            User = new TeamBoardUser
            {
                UserId = (int)c.GetType().GetProperty("UserId")!.GetValue(c)!,
                ResourceGroupId = (int?)c.GetType().GetProperty("ResourceGroupId")!.GetValue(c),
                DepartmentId = (string?)c.GetType().GetProperty("DepartmentId")!.GetValue(c),
                Position = (string)c.GetType().GetProperty("Position")!.GetValue(c)!,
                FirstName = (string)c.GetType().GetProperty("FirstName")!.GetValue(c)!,
                LastName = (string)c.GetType().GetProperty("LastName")!.GetValue(c)!,
                Email = (string)c.GetType().GetProperty("Email")!.GetValue(c)!,
                IsActive = (bool)c.GetType().GetProperty("IsActive")!.GetValue(c)!,
            }
        }).ToList();

        var dbSetMock = userContracts.BuildMockDbSet();
        _teamBoardDbContextMock
            .Setup(x => x.UserContracts)
            .Returns(dbSetMock.Object);
    }

    private void SetupTeamBoardUsers(List<(int UserId, string Position, int? ResourceGroupId, string? DepartmentId)> users)
    {
        var teamBoardUsers = users.Select(u => new TeamBoardUser
        {
            UserId = u.UserId,
            Position = u.Position,
            ResourceGroupId = u.ResourceGroupId,
            DepartmentId = u.DepartmentId,
            IsActive = true,
            FirstName = "test",
            LastName = "test",
            Email = "test"
        }).ToList();

        var dbSetMock = teamBoardUsers.BuildMockDbSet();
        _teamBoardDbContextMock
            .Setup(x => x.TeamBoardUsers)
            .Returns(dbSetMock.Object);
    }

    private void SetupDepartments(List<(string DepartmentId, string DepartmentReportingToRoleName)> departments)
    {
        var dbDepartments = departments.Select(d => new TeamboardDepartment
        {
            DepartmentId = d.DepartmentId,
            FullName = d.DepartmentId,
            ShortName = d.DepartmentId,
            DepartmentReportingToRoleName = d.DepartmentReportingToRoleName,
        }).ToList();

        var dbSetMock = dbDepartments.BuildMockDbSet();
        _teamBoardDbContextMock
            .Setup(x => x.TeamboardDepartments)
            .Returns(dbSetMock.Object);
    }
}
