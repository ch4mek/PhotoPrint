using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Users;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.BusinessRules;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.CoreDto;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.DbQueries;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.Validators;

namespace Modsen.Portal.TimeTrack.UnitTests.Timesheets.Core.Validators;

public class CreateTimesheetValidatorTests
{
    private readonly Mock<IHasUserActiveAssignmentForDateBusinessRule> _hasUserActiveAssignmentForDateBusinessRuleMock = new();
    private readonly Mock<IGetProjectBillableStatusIfExistsDbQuery> _getProjectBillableStatusIfExistsDbQueryMock = new();
    private readonly Mock<ICanModifyTimesheetForDateBusinessRule> _canModifyTimesheetForDateBusinessRuleMock = new();
    private readonly Mock<ICanTrackBillableHoursBusinessRule> _canTrackBillableHoursBusinessRuleMock = new();
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock = new();
    private readonly Mock<IGetUserDetailsDbQuery> _getUserDetailsDbQuery = new();
    private readonly Mock<IGetUserActiveStatusDbQuery> _getUserActiveStatusDbQueryMock = new();
    private readonly Mock<IGetProjectActiveStatusDbQuery> _getProjectActiveStatusDbQueryMock = new();

    private readonly CreateTimesheetValidator _validator;

    private readonly CancellationToken _token = CancellationToken.None;
    private readonly DateOnly _utcNow = DateOnly.FromDateTime(DateTime.UtcNow);

    public CreateTimesheetValidatorTests()
    {
        _validator = new CreateTimesheetValidator(
            _authenticatedUserContextMock.Object,
            _getUserDetailsDbQuery.Object,
            _canTrackBillableHoursBusinessRuleMock.Object,
            _getProjectBillableStatusIfExistsDbQueryMock.Object,
            _canModifyTimesheetForDateBusinessRuleMock.Object,
            _hasUserActiveAssignmentForDateBusinessRuleMock.Object,
            _getProjectActiveStatusDbQueryMock.Object,
            _getUserActiveStatusDbQueryMock.Object);
    }

    [Theory]
    [InlineData(PortalRoleNames.Employee)]
    [InlineData(PortalRoleNames.SystemAdministrator)]
    public async Task CreateTimesheetWhenNotManagerAndProjectNotFoundThrowsEntityNotFoundException(
        string userRole)
    {
        // Arrange
        var model = new CreateTimesheetDto
        {
            UserId = 1,
            ProjectId = 1,
            FactTrackedInMinutes = 60,
            BillableTrackedInMinutes = 60,
            ReportForDate = _utcNow,
            TaskName = "Test 1"
        };

        _getProjectBillableStatusIfExistsDbQueryMock
            .Setup(query => query.ExecuteAsync(model.ProjectId, _token))
            .ReturnsAsync((bool?)null);

        _getUserActiveStatusDbQueryMock
            .Setup(query => query.ExecuteAsync(model.UserId, _token))
            .ReturnsAsync(true);

        _authenticatedUserContextMock.Setup(context => context.GetRoles()).Returns([userRole]);
        _authenticatedUserContextMock.Setup(context => context.UserId).Returns(model.UserId);

        // Act
        var result = () => _validator.ValidateAsync(model, _token);

        //Assert
        await result.Should()
            .ThrowAsync<EntityNotFoundException>()
            .WithMessage($"Project with ID {model.ProjectId} does not exists");
    }

    [Fact]
    public async Task CreateTimesheetWhenProjectIsInactiveThrowsActionNotAllowedException()
    {
        // Arrange
        var model = new CreateTimesheetDto
        {
            UserId = 1,
            ProjectId = 1,
            FactTrackedInMinutes = 60,
            BillableTrackedInMinutes = 60,
            ReportForDate = _utcNow,
            TaskName = "Test 1"
        };

        _getProjectBillableStatusIfExistsDbQueryMock
            .Setup(query => query.ExecuteAsync(model.ProjectId, _token))
            .ReturnsAsync(false);

        _getProjectActiveStatusDbQueryMock
            .Setup(query => query.ExecuteAsync(model.ProjectId, _token))
            .ReturnsAsync(false);

        _getUserActiveStatusDbQueryMock
            .Setup(query => query.ExecuteAsync(model.ProjectId, _token))
            .ReturnsAsync(true);

        // Act
        var result = () => _validator.ValidateAsync(model, _token);

        //Assert
        await result.Should()
            .ThrowAsync<ActionNotAllowedException>()
            .WithMessage(PortalMessages.CannotManageTimesheetAtInactiveProject);
    }

    [Theory]
    [InlineData(PortalRoleNames.Employee)]
    [InlineData(PortalRoleNames.SystemAdministrator)]
    public async Task CreateTimesheetWhenNotManagerUserCannotTrackBillableTimeThrowsInvalidDataAppException(
        string userRole)
    {
        // Arrange
        var model = new CreateTimesheetDto
        {
            UserId = 1,
            ProjectId = 1,
            FactTrackedInMinutes = 60,
            BillableTrackedInMinutes = 60,
            ReportForDate = _utcNow,
            TaskName = "Test 1"
        };

        _hasUserActiveAssignmentForDateBusinessRuleMock
            .Setup(rule => rule.IsCorrespondsToBusiness(model.ProjectId, model.UserId, model.ReportForDate, _token))
            .ReturnsAsync(true);

        _getProjectBillableStatusIfExistsDbQueryMock
            .Setup(query => query.ExecuteAsync(model.ProjectId, _token))
            .ReturnsAsync(true);

        _getProjectActiveStatusDbQueryMock
            .Setup(query => query.ExecuteAsync(model.ProjectId, _token))
            .ReturnsAsync(true);

        _getUserActiveStatusDbQueryMock
            .Setup(query => query.ExecuteAsync(model.UserId, _token))
            .ReturnsAsync(true);

        _authenticatedUserContextMock.Setup(context => context.GetRoles()).Returns([userRole]);
        _authenticatedUserContextMock.Setup(context => context.UserId).Returns(model.UserId);

        _canTrackBillableHoursBusinessRuleMock
            .Setup(rule => rule.IsCorrespondsToBusiness(model.UserId, model.ProjectId, model.BillableTrackedInMinutes, _token))
            .ReturnsAsync(false);

        // Act
        var result = () => _validator.ValidateAsync(model, _token);

        //Assert
        await result.Should()
            .ThrowAsync<InvalidDataAppException>()
            .WithMessage(PortalMessages.CannotTrackBillableHoursWithoutPermission);
    }

    [Theory]
    [InlineData(PortalRoleNames.Employee)]
    [InlineData(PortalRoleNames.SystemAdministrator)]
    public async Task CreateTimesheetWhenNotManagerUserTryCreateTimesheetToAnotherUserThrowsActionNotAllowedException(
        string userRole)
    {
        // Arrange
        var anotherUserId = 2;
        var model = new CreateTimesheetDto
        {
            UserId = 1,
            ProjectId = 1,
            FactTrackedInMinutes = 60,
            BillableTrackedInMinutes = 60,
            ReportForDate = _utcNow,
            TaskName = "Test 1"
        };

        _hasUserActiveAssignmentForDateBusinessRuleMock
            .Setup(rule => rule.IsCorrespondsToBusiness(model.ProjectId, model.UserId, model.ReportForDate, _token))
            .ReturnsAsync(true);

        _getProjectBillableStatusIfExistsDbQueryMock
            .Setup(query => query.ExecuteAsync(model.ProjectId, _token))
            .ReturnsAsync(true);

        _getProjectActiveStatusDbQueryMock
            .Setup(query => query.ExecuteAsync(model.ProjectId, _token))
            .ReturnsAsync(true);

        _getUserActiveStatusDbQueryMock
            .Setup(query => query.ExecuteAsync(model.UserId, _token))
            .ReturnsAsync(true);

        _authenticatedUserContextMock.Setup(context => context.GetRoles()).Returns([userRole]);
        _authenticatedUserContextMock.Setup(context => context.UserId).Returns(anotherUserId);

        _canTrackBillableHoursBusinessRuleMock
            .Setup(rule => rule.IsCorrespondsToBusiness(model.UserId, model.ProjectId, model.BillableTrackedInMinutes, _token))
            .ReturnsAsync(false);

        // Act
        var result = () => _validator.ValidateAsync(model, _token);

        //Assert
        await result.Should()
            .ThrowAsync<ActionNotAllowedException>()
            .WithMessage(PortalMessages.UserCannotCreateTimesheetForOtherUsers);
    }

    [Theory]
    [InlineData(PortalRoleNames.Employee)]
    [InlineData(PortalRoleNames.SystemAdministrator)]
    public async Task CreateTimesheetWhenNotManagerModifiesTimesheetWithApprovedOrUnderReviewStatusThrowsActionNotAllowed(
        string userRole)
    {
        // Arrange
        var model = new CreateTimesheetDto
        {
            UserId = 1,
            ProjectId = 1,
            FactTrackedInMinutes = 60,
            BillableTrackedInMinutes = 60,
            ReportForDate = _utcNow,
            TaskName = "Test 1"
        };

        _hasUserActiveAssignmentForDateBusinessRuleMock
            .Setup(rule => rule.IsCorrespondsToBusiness(model.ProjectId, model.UserId, model.ReportForDate, _token))
            .ReturnsAsync(true);

        _getProjectBillableStatusIfExistsDbQueryMock
            .Setup(query => query.ExecuteAsync(model.ProjectId, _token))
            .ReturnsAsync(true);

        _getProjectActiveStatusDbQueryMock
            .Setup(query => query.ExecuteAsync(model.ProjectId, _token))
            .ReturnsAsync(true);

        _getUserActiveStatusDbQueryMock
            .Setup(query => query.ExecuteAsync(model.UserId, _token))
            .ReturnsAsync(true);

        _authenticatedUserContextMock.Setup(context => context.GetRoles()).Returns([userRole]);
        _authenticatedUserContextMock.Setup(context => context.UserId).Returns(model.UserId);

        _canTrackBillableHoursBusinessRuleMock
            .Setup(rule => rule.IsCorrespondsToBusiness(model.UserId, model.ProjectId, model.BillableTrackedInMinutes, _token))
            .ReturnsAsync(true);

        _canModifyTimesheetForDateBusinessRuleMock
            .Setup(rule => rule.IsCorrespondsToBusiness(model.UserId, model.ReportForDate, _token))
            .ReturnsAsync(false);

        _authenticatedUserContextMock.Setup(context => context.GetRoles()).Returns([userRole]);
        _authenticatedUserContextMock.Setup(context => context.DepartmentId).Returns("Net");
        _authenticatedUserContextMock.Setup(context => context.UserId).Returns(model.UserId);

        // Act
        var result = () => _validator.ValidateAsync(model, _token);

        //Assert
        await result.Should()
            .ThrowAsync<ActionNotAllowedException>()
            .WithMessage(PortalMessages.OnlyManagersCanModifyApprovedAndSubmittedTimesheets);
    }

    [Theory]
    [InlineData(PortalRoleNames.Ceo)]
    [InlineData(PortalRoleNames.Cto)]
    [InlineData(PortalRoleNames.HeadOfDepartment)]
    public async Task CreateTimesheetWhenManagerWhenUserDoesNotExistThrowsArgumentException(string userRole)
    {
        // Arrange
        var model = new CreateTimesheetDto
        {
            UserId = 1,
            ProjectId = 1,
            FactTrackedInMinutes = 60,
            BillableTrackedInMinutes = 60,
            ReportForDate = _utcNow,
            TaskName = "Test 1"
        };

        _hasUserActiveAssignmentForDateBusinessRuleMock
            .Setup(rule => rule.IsCorrespondsToBusiness(model.ProjectId, model.UserId, model.ReportForDate, _token))
            .ReturnsAsync(true);

        _getUserActiveStatusDbQueryMock
            .Setup(query => query.ExecuteAsync(model.UserId, _token))
            .ReturnsAsync(true);

        _getUserDetailsDbQuery
            .Setup(query => query.Execute(model.UserId, _token))
            .ReturnsAsync(default(GetUserDetailsDbQueryResponse));

        _getProjectBillableStatusIfExistsDbQueryMock
            .Setup(query => query.ExecuteAsync(model.ProjectId, _token))
            .ReturnsAsync(true);

        _getProjectActiveStatusDbQueryMock
            .Setup(query => query.ExecuteAsync(model.ProjectId, _token))
            .ReturnsAsync(true);

        _authenticatedUserContextMock.Setup(context => context.GetRoles()).Returns([userRole]);
        _authenticatedUserContextMock.Setup(context => context.UserId).Returns(1);
        _authenticatedUserContextMock.Setup(context => context.IsManager).Returns(true);

        // Act
        var result = () => _validator.ValidateAsync(model, _token);

        //Assert
        await result.Should()
            .ThrowAsync<InvalidDataAppException>()
            .WithMessage(PortalMessages.UserNotFound);
    }

    [Theory]
    [InlineData(PortalRoleNames.Ceo)]
    [InlineData(PortalRoleNames.Cto)]
    [InlineData(PortalRoleNames.HeadOfDepartment)]
    public async Task CreateTimesheetWhenUserIsInactiveThrowsActionNotAllowedException(string userRole)
    {
        // Arrange
        var model = new CreateTimesheetDto
        {
            UserId = 1,
            ProjectId = 1,
            FactTrackedInMinutes = 60,
            BillableTrackedInMinutes = 60,
            ReportForDate = _utcNow,
            TaskName = "Test 1"
        };

        _hasUserActiveAssignmentForDateBusinessRuleMock
            .Setup(rule => rule.IsCorrespondsToBusiness(model.ProjectId, model.UserId, model.ReportForDate, _token))
            .ReturnsAsync(true);

        _getProjectBillableStatusIfExistsDbQueryMock
            .Setup(query => query.ExecuteAsync(model.ProjectId, _token))
            .ReturnsAsync(true);

        _getUserActiveStatusDbQueryMock
            .Setup(query => query.ExecuteAsync(model.UserId, _token))
            .ReturnsAsync(false);

        _authenticatedUserContextMock.Setup(context => context.GetRoles()).Returns([userRole]);
        _authenticatedUserContextMock.Setup(context => context.UserId).Returns(1);
        _authenticatedUserContextMock.Setup(context => context.IsManager).Returns(true);

        _getUserDetailsDbQuery
            .Setup(query => query.Execute(model.UserId, _token))
            .ReturnsAsync(default(GetUserDetailsDbQueryResponse));

        // Act
        var result = () => _validator.ValidateAsync(model, _token);

        //Assert
        await result.Should()
            .ThrowAsync<ActionNotAllowedException>()
            .WithMessage(PortalMessages.CannotManageTimesheetOfInactiveUser);
    }

    [Theory]
    [InlineData(PortalRoleNames.Ceo)]
    [InlineData(PortalRoleNames.Cto)]
    [InlineData(PortalRoleNames.HeadOfDepartment)]
    public async Task CreateTimesheetWhenManagerTryCreateTimesheetForCEOThrowsInvalidDataAppException(string userRole)
    {
        // Arrange
        var model = new CreateTimesheetDto
        {
            UserId = 1,
            ProjectId = 1,
            FactTrackedInMinutes = 60,
            BillableTrackedInMinutes = 60,
            ReportForDate = _utcNow,
            TaskName = "Test 1"
        };

        _hasUserActiveAssignmentForDateBusinessRuleMock
            .Setup(rule => rule.IsCorrespondsToBusiness(model.ProjectId, model.UserId, model.ReportForDate, _token))
            .ReturnsAsync(true);

        _getProjectBillableStatusIfExistsDbQueryMock
            .Setup(query => query.ExecuteAsync(model.ProjectId, _token))
            .ReturnsAsync(true);

        _getProjectActiveStatusDbQueryMock
            .Setup(query => query.ExecuteAsync(model.ProjectId, _token))
            .ReturnsAsync(true);

        _getUserActiveStatusDbQueryMock
            .Setup(query => query.ExecuteAsync(model.UserId, _token))
            .ReturnsAsync(true);

        _authenticatedUserContextMock.Setup(context => context.GetRoles()).Returns([userRole]);
        _authenticatedUserContextMock.Setup(context => context.UserId).Returns(1);
        _authenticatedUserContextMock.Setup(context => context.IsManager).Returns(true);

        _getUserDetailsDbQuery
            .Setup(query => query.Execute(model.UserId, _token))
            .ReturnsAsync(new GetUserDetailsDbQueryResponse { Role = PortalRoleNames.Ceo });

        // Act
        var result = () => _validator.ValidateAsync(model, _token);

        //Assert
        await result.Should()
            .ThrowAsync<InvalidDataAppException>()
            .WithMessage($"{PortalRoleNames.Ceo} can't have timesheets");
    }

    [Theory]
    [InlineData(PortalRoleNames.Ceo)]
    [InlineData(PortalRoleNames.Cto)]
    [InlineData(PortalRoleNames.HeadOfDepartment)]
    public async Task CreateTimesheetWhenManagerTryCreateTimesheetForCTOThrowsInvalidDataAppException(string userRole)
    {
        // Arrange
        var model = new CreateTimesheetDto
        {
            UserId = 1,
            ProjectId = 1,
            FactTrackedInMinutes = 60,
            BillableTrackedInMinutes = 60,
            ReportForDate = _utcNow,
            TaskName = "Test 1"
        };

        _authenticatedUserContextMock.Setup(context => context.GetRoles()).Returns([userRole]);
        _authenticatedUserContextMock.Setup(context => context.UserId).Returns(1);
        _authenticatedUserContextMock.Setup(context => context.IsManager).Returns(true);

        _getUserDetailsDbQuery
            .Setup(query => query.Execute(model.UserId, _token))
            .ReturnsAsync(new GetUserDetailsDbQueryResponse { Role = PortalRoleNames.Cto });

        _getProjectBillableStatusIfExistsDbQueryMock
            .Setup(query => query.ExecuteAsync(model.ProjectId, _token))
            .ReturnsAsync(true);

        _getProjectActiveStatusDbQueryMock
            .Setup(query => query.ExecuteAsync(model.ProjectId, _token))
            .ReturnsAsync(true);

        _getUserActiveStatusDbQueryMock
            .Setup(query => query.ExecuteAsync(model.UserId, _token))
            .ReturnsAsync(true);

        // Act
        var result = () => _validator.ValidateAsync(model, _token);

        //Assert
        await result.Should()
            .ThrowAsync<InvalidDataAppException>()
            .WithMessage($"{PortalRoleNames.Cto} can't have timesheets");
    }

    [Fact]
    public async Task CreateTimesheetWhenHeadOfDepartmentTryCreateTimesheetForUserWithAnotherDepartmentThrowsActionNotAllowedException()
    {
        // Arrange
        var model = new CreateTimesheetDto
        {
            UserId = 1,
            ProjectId = 1,
            FactTrackedInMinutes = 60,
            BillableTrackedInMinutes = 60,
            ReportForDate = _utcNow,
            TaskName = "Test 1"
        };

        _authenticatedUserContextMock.Setup(context => context.GetRoles()).Returns([PortalRoleNames.HeadOfDepartment]);
        _authenticatedUserContextMock.Setup(context => context.DepartmentId).Returns("Net");
        _authenticatedUserContextMock.Setup(context => context.UserId).Returns(1);
        _authenticatedUserContextMock.Setup(context => context.IsManager).Returns(true);

        _getUserDetailsDbQuery
            .Setup(query => query.Execute(model.UserId, _token))
            .ReturnsAsync(new GetUserDetailsDbQueryResponse { DepartmentId = "Java", Role = PortalRoleNames.Employee });

        _getProjectBillableStatusIfExistsDbQueryMock
            .Setup(query => query.ExecuteAsync(model.ProjectId, _token))
            .ReturnsAsync(true);

        _getProjectActiveStatusDbQueryMock
            .Setup(query => query.ExecuteAsync(model.ProjectId, _token))
            .ReturnsAsync(true);

        _getUserActiveStatusDbQueryMock
            .Setup(query => query.ExecuteAsync(model.UserId, _token))
            .ReturnsAsync(true);

        // Act
        var result = () => _validator.ValidateAsync(model, _token);

        //Assert
        await result.Should()
            .ThrowAsync<ActionNotAllowedException>()
            .WithMessage(PortalMessages.HeadCannotCreateTimesheetForUserFromOtherDepartment);
    }

    [Fact]
    public async Task CreateTimesheetWhenGroupManagerTryCreateTimesheetForUserWithAnotherGroupThrowsActionNotAllowedException()
    {
        // Arrange
        var model = new CreateTimesheetDto
        {
            UserId = 1,
            ProjectId = 1,
            FactTrackedInMinutes = 60,
            BillableTrackedInMinutes = 60,
            ReportForDate = _utcNow,
            TaskName = "Test 1"
        };

        _authenticatedUserContextMock.Setup(context => context.GetRoles()).Returns([PortalRoleNames.GroupManager]);
        _authenticatedUserContextMock.Setup(context => context.DepartmentId).Returns("Net");
        _authenticatedUserContextMock.Setup(context => context.UserId).Returns(1);
        _authenticatedUserContextMock.Setup(context => context.IsManager).Returns(true);
        _authenticatedUserContextMock.Setup(context => context.ResourceGroupId).Returns(1);

        _getUserDetailsDbQuery
            .Setup(query => query.Execute(model.UserId, _token))
            .ReturnsAsync(new GetUserDetailsDbQueryResponse
            {
                ResourceGroupId = 2,
                DepartmentId = "Net",
                Role = PortalRoleNames.Employee
            });

        _getProjectBillableStatusIfExistsDbQueryMock
            .Setup(query => query.ExecuteAsync(model.ProjectId, _token))
            .ReturnsAsync(true);

        _getProjectActiveStatusDbQueryMock
            .Setup(query => query.ExecuteAsync(model.ProjectId, _token))
            .ReturnsAsync(true);

        _getUserActiveStatusDbQueryMock
            .Setup(query => query.ExecuteAsync(model.UserId, _token))
            .ReturnsAsync(true);

        // Act
        var result = () => _validator.ValidateAsync(model, _token);

        //Assert
        await result.Should()
            .ThrowAsync<ActionNotAllowedException>()
            .WithMessage(PortalMessages.GroupManagerCannotCreateTimesheetForUserFromOtherGroup);
    }

    [Theory]
    [InlineData(PortalRoleNames.Employee)]
    [InlineData(PortalRoleNames.SystemAdministrator)]
    public async Task CreateTimesheetWhenHasNoActiveAssignmentSuccessfulResult(string userRole)
    {
        // Arrange
        var model = new CreateTimesheetDto
        {
            UserId = 1,
            ProjectId = 1,
            FactTrackedInMinutes = 60,
            BillableTrackedInMinutes = 60,
            ReportForDate = _utcNow,
            TaskName = "Test 1"
        };

        _hasUserActiveAssignmentForDateBusinessRuleMock
            .Setup(rule => rule.IsCorrespondsToBusiness(model.ProjectId, model.UserId, model.ReportForDate, _token))
            .ReturnsAsync(false);

        _getProjectBillableStatusIfExistsDbQueryMock
            .Setup(query => query.ExecuteAsync(model.ProjectId, _token))
            .ReturnsAsync(true);

        _authenticatedUserContextMock.Setup(context => context.GetRoles()).Returns([userRole]);
        _authenticatedUserContextMock.Setup(context => context.UserId).Returns(model.UserId);

        _canTrackBillableHoursBusinessRuleMock
            .Setup(rule => rule.IsCorrespondsToBusiness(model.UserId, model.ProjectId, model.BillableTrackedInMinutes, _token))
            .ReturnsAsync(true);

        _canModifyTimesheetForDateBusinessRuleMock
            .Setup(rule => rule.IsCorrespondsToBusiness(model.UserId, model.ReportForDate, _token))
            .ReturnsAsync(true);

        _getUserActiveStatusDbQueryMock
            .Setup(query => query.ExecuteAsync(model.UserId, _token))
            .ReturnsAsync(true);

        _authenticatedUserContextMock.Setup(context => context.GetRoles()).Returns([userRole]);
        _authenticatedUserContextMock.Setup(context => context.DepartmentId).Returns("Net");
        _authenticatedUserContextMock.Setup(context => context.UserId).Returns(model.UserId);

        _getProjectActiveStatusDbQueryMock
            .Setup(query => query.ExecuteAsync(model.ProjectId, _token))
            .ReturnsAsync(true);

        // Act
        var result = () => _validator.ValidateAsync(model, _token);

        //Assert
        await result.Should()
            .ThrowAsync<InvalidDataAppException>()
            .WithMessage(PortalMessages.NoActiveAssignmentFoundForDate);
    }

    [Theory]
    [InlineData(PortalRoleNames.Employee)]
    [InlineData(PortalRoleNames.SystemAdministrator)]
    public async Task CreateTimesheetWhenNotManagerDataIsValidSuccessfulResult(string userRole)
    {
        // Arrange
        var model = new CreateTimesheetDto
        {
            UserId = 1,
            ProjectId = 1,
            FactTrackedInMinutes = 60,
            BillableTrackedInMinutes = 60,
            ReportForDate = _utcNow,
            TaskName = "Test 1"
        };

        _hasUserActiveAssignmentForDateBusinessRuleMock
            .Setup(rule => rule.IsCorrespondsToBusiness(model.ProjectId, model.UserId, model.ReportForDate, _token))
            .ReturnsAsync(true);

        _getProjectBillableStatusIfExistsDbQueryMock
            .Setup(query => query.ExecuteAsync(model.ProjectId, _token))
            .ReturnsAsync(true);

        _authenticatedUserContextMock.Setup(context => context.GetRoles()).Returns([userRole]);
        _authenticatedUserContextMock.Setup(context => context.UserId).Returns(model.UserId);

        _canTrackBillableHoursBusinessRuleMock
            .Setup(rule => rule.IsCorrespondsToBusiness(model.UserId, model.ProjectId, model.BillableTrackedInMinutes, _token))
            .ReturnsAsync(true);

        _canModifyTimesheetForDateBusinessRuleMock
            .Setup(rule => rule.IsCorrespondsToBusiness(model.UserId, model.ReportForDate, _token))
            .ReturnsAsync(true);

        _getUserActiveStatusDbQueryMock
            .Setup(query => query.ExecuteAsync(model.UserId, _token))
            .ReturnsAsync(true);

        _getProjectActiveStatusDbQueryMock
            .Setup(query => query.ExecuteAsync(model.ProjectId, _token))
            .ReturnsAsync(true);

        _authenticatedUserContextMock.Setup(context => context.GetRoles()).Returns([userRole]);
        _authenticatedUserContextMock.Setup(context => context.DepartmentId).Returns("Net");
        _authenticatedUserContextMock.Setup(context => context.UserId).Returns(model.UserId);

        // Act
        var result = () => _validator.ValidateAsync(model, _token);

        //Assert
        await result.Should().NotThrowAsync();
    }

    [Theory]
    [InlineData(PortalRoleNames.Ceo)]
    [InlineData(PortalRoleNames.Cto)]
    [InlineData(PortalRoleNames.HeadOfDepartment)]
    public async Task CreateTimesheetWhenManagerDataIsValidSuccessfulResult(string userRole)
    {
        // Arrange
        var model = new CreateTimesheetDto
        {
            UserId = 1,
            ProjectId = 1,
            FactTrackedInMinutes = 60,
            BillableTrackedInMinutes = 60,
            ReportForDate = _utcNow,
            TaskName = "Test 1"
        };

        _authenticatedUserContextMock.Setup(context => context.GetRoles()).Returns([userRole]);
        _authenticatedUserContextMock.Setup(context => context.DepartmentId).Returns("Net");
        _authenticatedUserContextMock.Setup(context => context.UserId).Returns(1);
        _authenticatedUserContextMock.Setup(context => context.IsManager).Returns(true);

        _getUserDetailsDbQuery
            .Setup(query => query.Execute(model.UserId, _token))
            .ReturnsAsync(new GetUserDetailsDbQueryResponse { DepartmentId = "Net", Role = PortalRoleNames.Employee });

        _canModifyTimesheetForDateBusinessRuleMock
            .Setup(rule => rule.IsCorrespondsToBusiness(model.UserId, model.ReportForDate, _token))
            .ReturnsAsync(true);

        _hasUserActiveAssignmentForDateBusinessRuleMock
            .Setup(rule => rule.IsCorrespondsToBusiness(model.ProjectId, model.UserId, model.ReportForDate, _token))
            .ReturnsAsync(true);

        _getProjectBillableStatusIfExistsDbQueryMock
            .Setup(query => query.ExecuteAsync(model.ProjectId, _token))
            .ReturnsAsync(true);

        _getProjectActiveStatusDbQueryMock
            .Setup(query => query.ExecuteAsync(model.ProjectId, _token))
            .ReturnsAsync(true);

        _getUserActiveStatusDbQueryMock
            .Setup(query => query.ExecuteAsync(model.UserId, _token))
            .ReturnsAsync(true);

        // Act
        var result = () => _validator.ValidateAsync(model, _token);

        //Assert
        await result.Should().NotThrowAsync();
    }
}
