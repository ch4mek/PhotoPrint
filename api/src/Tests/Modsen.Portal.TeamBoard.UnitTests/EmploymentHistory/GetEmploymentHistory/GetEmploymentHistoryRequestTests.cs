using MockQueryable.Moq;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.EmploymentHistory.Requests;
using Modsen.Portal.TimeTrack.Contract;
using Moq;

namespace Modsen.Portal.TeamBoard.UnitTests.EmploymentHistory.GetEmploymentHistory;

public class GetEmploymentHistoryRequestTests
{
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock = new();
    private readonly Mock<ITeamBoardDbContext> _teamBoardDbContextMock = new();
    private readonly Mock<ICanTeamBoardUserBeRetrievedBusinessRule> _canTeamBoardUserBeRetrievedBusinessRuleMock = new();
    private readonly CancellationToken _token = CancellationToken.None;

    private readonly GetEmploymentHistoryRequest _handler;
    private readonly Mock<ITimeTrackContract> _timeTrackContractMock = new();

    public GetEmploymentHistoryRequestTests()
    {        
        _authenticatedUserContextMock
            .Setup(x => x.UserId)
            .Returns(1);

        _timeTrackContractMock
            .Setup(x => x.GetUserIdsByProjectManagerAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        _handler = new GetEmploymentHistoryRequest(
            _authenticatedUserContextMock.Object,
            _teamBoardDbContextMock.Object,
            _timeTrackContractMock.Object,
            _canTeamBoardUserBeRetrievedBusinessRuleMock.Object);
    }

    [Fact]
    public async Task HandleWhenTeamBoardUserDoesNotExistThrowEntityNotFoundException()
    {
        // Arrange
        var userId = 123456;

        var query = new GetEmploymentHistoryRequest.Query(userId);

        _authenticatedUserContextMock
            .Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.Ceo]);

        _authenticatedUserContextMock
            .Setup(x => x.UserId)
            .Returns(1);

        _teamBoardDbContextMock.SetupGet(x => x.TeamBoardUsers)
            .Returns(Array.Empty<TeamBoardUser>().BuildMockDbSet().Object); 

        // Act
        var action = () => _handler.Handle(query, _token);

        // Assert
        await action.Should().ThrowAsync<EntityNotFoundException>(PortalMessages.UserNotFound);
    }

    [Fact]
    public async Task HandleWhenUserDoesNotHaveClaimsAndNotManagerThrowEntityNotFoundException()
    {
        // Arrange
        var userId = 123456;
        var user = new TeamBoardUser
        {
            Email = "Email",
            FirstName = "FirstName",
            LastName = "LastName",
            UserId = userId,
            IsActive = true,
        };

        var query = new GetEmploymentHistoryRequest.Query(userId);

        _authenticatedUserContextMock.Setup(x => x.UserId).Returns(1);
        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.Employee]);
        _authenticatedUserContextMock.Setup(x => x.HasClaim(AccountantClaimTypeNames.ManageAccountant.Name)).Returns(false);
        _authenticatedUserContextMock.Setup(x => x.HasClaim(CoordinatorClaimTypeNames.ManageProjectCoordinator.Name)).Returns(false);
        _authenticatedUserContextMock.Setup(x => x.HasClaim(HrClaimTypeNames.ManageHrModule.Name)).Returns(false);

        _teamBoardDbContextMock.SetupGet(x => x.TeamBoardUsers)
            .Returns(new[]{ user }.BuildMockDbSet().Object);

        // Act
        var action = () => _handler.Handle(query, _token);

        // Assert
        await action.Should().ThrowAsync<EntityNotFoundException>(PortalMessages.UserNotFound);
    }

    [Fact]
    public async Task HandleWhenUserDoesNotHaveClaimsAnGmAnotherGroupThrowEntityNotFoundException()
    {
        // Arrange
        var userId = 123456;
        var user = new TeamBoardUser
        {
            Email = "Email",
            FirstName = "FirstName",
            LastName = "LastName",
            UserId = userId,
            IsActive = true,
        };

        var query = new GetEmploymentHistoryRequest.Query(userId);

        _authenticatedUserContextMock.Setup(x => x.UserId).Returns(1);
        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.GroupManager]);
        _authenticatedUserContextMock.Setup(x => x.ResourceGroupId).Returns(22);
        _authenticatedUserContextMock.Setup(x => x.HasClaim(AccountantClaimTypeNames.ManageAccountant.Name)).Returns(false);
        _authenticatedUserContextMock.Setup(x => x.HasClaim(CoordinatorClaimTypeNames.ManageProjectCoordinator.Name)).Returns(false);
        _authenticatedUserContextMock.Setup(x => x.HasClaim(HrClaimTypeNames.ManageHrModule.Name)).Returns(false);

        _teamBoardDbContextMock.SetupGet(x => x.TeamBoardUsers)
            .Returns(new[] { user }.BuildMockDbSet().Object);

        // Act
        var action = () => _handler.Handle(query, _token);

        // Assert
        await action.Should().ThrowAsync<EntityNotFoundException>(PortalMessages.UserNotFound);
    }

    [Fact]
    public async Task HandleWhenUserDoesNotHaveClaimsAndHeadOfAnotherDepartmentThrowEntityNotFoundException()
    {
        // Arrange
        var userId = 123456;
        var user = new TeamBoardUser
        {
            Email = "Email",
            FirstName = "FirstName",
            LastName = "LastName",
            UserId = userId,
            IsActive = true,
            DepartmentId = "Js"
        };

        var query = new GetEmploymentHistoryRequest.Query(userId);

        _authenticatedUserContextMock.Setup(x => x.UserId).Returns(1);
        _authenticatedUserContextMock.Setup(x => x.DepartmentId).Returns("Net");
        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.HeadOfDepartment]);
        _authenticatedUserContextMock.Setup(x => x.HasClaim(AccountantClaimTypeNames.ManageAccountant.Name)).Returns(false);
        _authenticatedUserContextMock.Setup(x => x.HasClaim(CoordinatorClaimTypeNames.ManageProjectCoordinator.Name)).Returns(false);
        _authenticatedUserContextMock.Setup(x => x.HasClaim(HrClaimTypeNames.ManageHrModule.Name)).Returns(false);

        _teamBoardDbContextMock.SetupGet(x => x.TeamBoardUsers)
            .Returns(new[] { user }.BuildMockDbSet().Object);

        // Act
        var action = () => _handler.Handle(query, _token);

        // Assert
        await action.Should().ThrowAsync<EntityNotFoundException>(PortalMessages.UserNotFound);
    }

    [Fact]
    public async Task HandleWhenTeamBoardUserExistsWhenAuthUserHasClaimShouldReturnTeamBoardUser()
    {
        // Arrange
        var userId = 5;

        var query = new GetEmploymentHistoryRequest.Query(userId);

        var contractEndDate = new DateOnly(2023, 2, 25);
        var professionalActivityStartDate = new DateOnly(2020, 6, 14);
        var firstProjectStartDate = new DateOnly(2022, 1, 5);
        var hireDate = new DateOnly(2022, 1, 1);

        _authenticatedUserContextMock.Setup(x => x.UserId).Returns(1);
        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.Employee]);
        _authenticatedUserContextMock.Setup(x => x.HasClaim(AccountantClaimTypeNames.ManageAccountant.Name)).Returns(true);
        _authenticatedUserContextMock.Setup(x => x.HasClaim(CoordinatorClaimTypeNames.ManageProjectCoordinator.Name)).Returns(false);
        _authenticatedUserContextMock.Setup(x => x.HasClaim(HrClaimTypeNames.ManageHrModule.Name)).Returns(false);

        _canTeamBoardUserBeRetrievedBusinessRuleMock
            .Setup(x => x.IsCorrespondsToBusiness(It.IsAny<int>(), It.IsAny<AccessOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var user = new TeamBoardUser
        {
            IsActive = true,
            UserId = userId,
            FirstName = "test",
            LastName = "test",
            Email = "test",
            HireDate = hireDate,
            CareerStartedAt = professionalActivityStartDate,
            Contracts =
            [
                new()
                {
                    ContractFrom = hireDate, ContractTo = contractEndDate, FireDate = hireDate.AddMonths(1)
                }
            ],
            UserProjects =
            [
                new() { UserId = userId, UserOnProjectFrom = firstProjectStartDate }
            ]
        };

        _teamBoardDbContextMock.Setup(x => x.TeamBoardUsers)
            .Returns(new List<TeamBoardUser>{ user }.BuildMockDbSet().Object);


        // Act
        var result = await _handler.Handle(query, _token);

        // Assert
        result.Should().NotBeNull();
        result.ContractEndDate.Should().Be(contractEndDate);
        result.ProfessionalActivityStartDate.Should().Be(professionalActivityStartDate);
        result.FirstProjectStartDate.Should().Be(firstProjectStartDate);
        result.HireDate.Should().Be(hireDate);
    }

    [Fact]
    public async Task HandleWhenTeamBoardUserExistsWhenAuthUserIsHeadShouldReturnTeamBoardUser()
    {
        // Arrange
        var userId = 5;

        var query = new GetEmploymentHistoryRequest.Query(userId);

        var contractEndDate = new DateOnly(2023, 2, 25);
        var professionalActivityStartDate = new DateOnly(2020, 6, 14);
        var firstProjectStartDate = new DateOnly(2022, 1, 5);
        var hireDate = new DateOnly(2022, 1, 1);

        _authenticatedUserContextMock.Setup(x => x.UserId).Returns(1);
        _authenticatedUserContextMock.Setup(x => x.DepartmentId).Returns("Net");
        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.HeadOfDepartment]);
        _authenticatedUserContextMock.Setup(x => x.HasClaim(AccountantClaimTypeNames.ManageAccountant.Name)).Returns(false);
        _authenticatedUserContextMock.Setup(x => x.HasClaim(CoordinatorClaimTypeNames.ManageProjectCoordinator.Name)).Returns(false);
        _authenticatedUserContextMock.Setup(x => x.HasClaim(HrClaimTypeNames.ManageHrModule.Name)).Returns(false);

        _canTeamBoardUserBeRetrievedBusinessRuleMock
            .Setup(x => x.IsCorrespondsToBusiness(It.IsAny<int>(), It.IsAny<AccessOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var user = new TeamBoardUser
        {
            IsActive = true,
            UserId = userId,
            DepartmentId = "Net",
            Position = "Employee",
            FirstName = "test",
            LastName = "test",
            Email = "test",
            HireDate = hireDate,
            CareerStartedAt = professionalActivityStartDate,
            Contracts =
            [
                new()
                {
                    ContractFrom = hireDate, ContractTo = contractEndDate, FireDate = hireDate.AddMonths(1)
                }
            ],
            UserProjects =
            [
                new() { UserId = userId, UserOnProjectFrom = firstProjectStartDate }
            ]
        };

        _teamBoardDbContextMock.Setup(x => x.TeamBoardUsers)
            .Returns(new List<TeamBoardUser> { user }.BuildMockDbSet().Object);


        // Act
        var result = await _handler.Handle(query, _token);

        // Assert
        result.Should().NotBeNull();
        result.ContractEndDate.Should().Be(contractEndDate);
        result.ProfessionalActivityStartDate.Should().Be(professionalActivityStartDate);
        result.FirstProjectStartDate.Should().Be(firstProjectStartDate);
        result.HireDate.Should().Be(hireDate);
    }

    [Fact]
    public async Task HandleWhenTeamBoardUserExistsWhenAuthUserIsGmShouldReturnTeamBoardUser()
    {
        // Arrange
        var userId = 5;

        var query = new GetEmploymentHistoryRequest.Query(userId);

        var contractEndDate = new DateOnly(2023, 2, 25);
        var professionalActivityStartDate = new DateOnly(2020, 6, 14);
        var firstProjectStartDate = new DateOnly(2022, 1, 5);
        var hireDate = new DateOnly(2022, 1, 1);

        _authenticatedUserContextMock.Setup(x => x.UserId).Returns(1);
        _authenticatedUserContextMock.Setup(x => x.ResourceGroupId).Returns(622);
        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.GroupManager]);
        _authenticatedUserContextMock.Setup(x => x.HasClaim(AccountantClaimTypeNames.ManageAccountant.Name)).Returns(false);
        _authenticatedUserContextMock.Setup(x => x.HasClaim(CoordinatorClaimTypeNames.ManageProjectCoordinator.Name)).Returns(false);
        _authenticatedUserContextMock.Setup(x => x.HasClaim(HrClaimTypeNames.ManageHrModule.Name)).Returns(false);

        _canTeamBoardUserBeRetrievedBusinessRuleMock
            .Setup(x => x.IsCorrespondsToBusiness(It.IsAny<int>(), It.IsAny<AccessOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var user = new TeamBoardUser
        {
            IsActive = true,
            UserId = userId,
            DepartmentId = "Net",
            ResourceGroupId = 622,
            FirstName = "test",
            LastName = "test",
            Email = "test",
            HireDate = hireDate,
            CareerStartedAt = professionalActivityStartDate,
            Contracts =
            [
                new()
                {
                    ContractFrom = hireDate, ContractTo = contractEndDate, FireDate = hireDate.AddMonths(1)
                }
            ],
            UserProjects =
            [
                new() { UserId = userId, UserOnProjectFrom = firstProjectStartDate }
            ]
        };

        _teamBoardDbContextMock.Setup(x => x.TeamBoardUsers)
            .Returns(new List<TeamBoardUser> { user }.BuildMockDbSet().Object);


        // Act
        var result = await _handler.Handle(query, _token);

        // Assert
        result.Should().NotBeNull();
        result.ContractEndDate.Should().Be(contractEndDate);
        result.ProfessionalActivityStartDate.Should().Be(professionalActivityStartDate);
        result.FirstProjectStartDate.Should().Be(firstProjectStartDate);
        result.HireDate.Should().Be(hireDate);
    }
}
