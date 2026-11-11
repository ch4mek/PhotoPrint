using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.TimeTrack;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TimeTrack.UseCases.Core.DbQueries;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.DbQueries;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.Validators;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Requests.RequestDto;

namespace Modsen.Portal.TimeTrack.UnitTests.Timesheets.Core.Validators;

public class UpdateSubmitStatusValidatorTests
{
    private readonly Mock<IGetTimesheetSubmitStatusDbQuery> _getTimesheetSubmitStatusDbQueryMock = new();
    private readonly Mock<IAuthenticatedUserContext> _authContextMock = new();
    private readonly Mock<IGetTimeTrackUserDbQuery> _getUserDbQueryMock = new();

    private readonly UpdateSubmitStatusValidator _validator;
    private readonly CancellationToken _token = CancellationToken.None;

    public UpdateSubmitStatusValidatorTests()
    {
        _validator = new UpdateSubmitStatusValidator(
            _getTimesheetSubmitStatusDbQueryMock.Object,
            _authContextMock.Object,
            _getUserDbQueryMock.Object);
    }

    [Fact]
    public async Task ValidateAsyncWhenUserNotFoundThrowsInvalidDataAppException()
    {
        //Arrange
        var userId = 1;

        var updateDto = new UpdateUserSubmitStatusDto
        (
            UserId: userId, Month: 1, Year: 2025, Status: TimesheetSubmitStatusType.Rejected
        );

        _getUserDbQueryMock
            .Setup(query => query.ExecuteAsync(userId, _token))
            .ReturnsAsync(null as TimeTrackUser);

        //Act
        var action = () => _validator.ValidateAsync(updateDto, _token);

        //Assert
        await action.Should()
            .ThrowAsync<InvalidDataAppException>()
            .WithMessage(PortalMessages.UserNotFound);
    }

    [Fact]
    public async Task ValidateAsyncWhenStatusHasBeenAlreadySentThrowsInvalidDataAppException()
    {
        //Arrange
        var userId = 1;

        var updateDto = new UpdateUserSubmitStatusDto
        (
            UserId: userId, Month: 1, Year: 2025, Status: TimesheetSubmitStatusType.Rejected
        );

        _getUserDbQueryMock
            .Setup(query => query.ExecuteAsync(userId, _token))
            .ReturnsAsync(new TimeTrackUser { UserId = userId, FirstName = "test", LastName = "test", Email = "test" });

        _getTimesheetSubmitStatusDbQueryMock
            .Setup(query => query.ExecuteAsync(userId, It.IsAny<DateOnly>(), _token))
            .ReturnsAsync(TimesheetSubmitStatusType.Rejected);

        //Act
        var action = () => _validator.ValidateAsync(updateDto, _token);

        //Assert
        await action.Should()
            .ThrowAsync<InvalidDataAppException>()
            .WithMessage(PortalMessages.StatusHasAlreadyBeenSent);
    }

    [Theory]
    [InlineData(PortalRoleNames.Employee)]
    [InlineData(PortalRoleNames.SystemAdministrator)]
    public async Task ValidateAsyncUpdateOtherUsersStatusWhenIsNotManagerThrowsActionNotAllowedException(string roleName)
    {
        //Arrange
        var userId = 1;

        var updateDto = new UpdateUserSubmitStatusDto
        (
            UserId: userId, Month: 1, Year: 2025, Status: TimesheetSubmitStatusType.UnderReview
        );

        _authContextMock.Setup(context => context.GetRoles()).Returns([roleName]);

        //Act
        var action = () => _validator.ValidateAsync(updateDto, _token);

        //Assert
        await action.Should()
            .ThrowAsync<ActionNotAllowedException>()
            .WithMessage(PortalMessages.CannotChangeOtherUsersTimesheetStatus);
    }

    [Fact]
    public async Task ValidateAsyncWhenStatusIsApprovedAndUnderReviewStatusHaveBeenSentThrowsInvalidDataAppException()
    {
        //Arrange
        var userId = 1;

        var updateDto = new UpdateUserSubmitStatusDto
        (
            UserId: userId, Month: 1, Year: 2025, Status: TimesheetSubmitStatusType.UnderReview
        );

        _getUserDbQueryMock
            .Setup(query => query.ExecuteAsync(userId, _token))
            .ReturnsAsync(new TimeTrackUser { UserId = userId, FirstName = "test", LastName = "test", Email = "test" });

        _getTimesheetSubmitStatusDbQueryMock
            .Setup(query => query.ExecuteAsync(userId, It.IsAny<DateOnly>(), _token))
            .ReturnsAsync(TimesheetSubmitStatusType.Approved);

        //Act
        var action = () => _validator.ValidateAsync(updateDto, _token);

        //Assert
        await action.Should()
            .ThrowAsync<InvalidDataAppException>()
            .WithMessage(PortalMessages.MonthHasAlreadyBeenApproved);
    }

    [Fact]
    public async Task ValidateAsyncWhenStatusIsNullAndRejectedStatusHaveBeenSentThrowsInvalidDataAppException()
    {
        //Arrange
        var userId = 1;

        var updateDto = new UpdateUserSubmitStatusDto
        (
            UserId: userId, Month: 1, Year: 2025, Status: TimesheetSubmitStatusType.Rejected
        );

        _getUserDbQueryMock
            .Setup(query => query.ExecuteAsync(userId, _token))
            .ReturnsAsync(new TimeTrackUser { UserId = userId, FirstName = "test", LastName = "test", Email = "test" });

        _getTimesheetSubmitStatusDbQueryMock
            .Setup(query => query.ExecuteAsync(userId, It.IsAny<DateOnly>(), _token))
            .ReturnsAsync(null as TimesheetSubmitStatusType?);

        //Act
        var action = () => _validator.ValidateAsync(updateDto, _token);

        //Assert
        await action.Should()
            .ThrowAsync<InvalidDataAppException>()
            .WithMessage(PortalMessages.TimesheetCannotBeRejectedBeforeUnderReviewStatus);
    }

    [Fact]
    public async Task ValidateAsyncWhenAuthedAsHeadAndUpdateStatusForUserNotFromHisDepartmentThrowsActionNotAllowedException()
    {
        //Arrange
        var userId = 1;

        var updateDto = new UpdateUserSubmitStatusDto
        (
            UserId: userId, Month: 1, Year: 2025, Status: TimesheetSubmitStatusType.Approved
        );

        _getUserDbQueryMock
            .Setup(query => query.ExecuteAsync(userId, _token))
            .ReturnsAsync(new TimeTrackUser { UserId = userId, DepartmentId = "JS", FirstName = "test", LastName = "test", Email = "test" });

        _getTimesheetSubmitStatusDbQueryMock
            .Setup(query => query.ExecuteAsync(userId, It.IsAny<DateOnly>(), _token))
            .ReturnsAsync(TimesheetSubmitStatusType.Rejected);

        _authContextMock.Setup(context => context.GetRoles()).Returns([PortalRoleNames.HeadOfDepartment]);
        _authContextMock.Setup(context => context.DepartmentId).Returns("Net");

        //Act
        var action = () => _validator.ValidateAsync(updateDto, _token);

        //Assert
        await action.Should()
            .ThrowAsync<ActionNotAllowedException>()
            .WithMessage(PortalMessages.HeadCannotUpdateStatusForUserFromOtherDepartment);
    }

    [Fact]
    public async Task ValidateAsyncWhenAuthedAsHeadAndSendUnderReviewStatusThrowsActionNotAllowedException()
    {
        //Arrange
        var userId = 1;

        var updateDto = new UpdateUserSubmitStatusDto
        (
            UserId: userId, Month: 1, Year: 2025, Status: TimesheetSubmitStatusType.UnderReview
        );

        _getUserDbQueryMock
            .Setup(query => query.ExecuteAsync(userId, _token))
            .ReturnsAsync(new TimeTrackUser { UserId = userId, DepartmentId = "Net", FirstName = "test", LastName = "test", Email = "test" });

        _getTimesheetSubmitStatusDbQueryMock
            .Setup(query => query.ExecuteAsync(userId, It.IsAny<DateOnly>(), _token))
            .ReturnsAsync(null as TimesheetSubmitStatusType?);

        _authContextMock.Setup(context => context.GetRoles()).Returns([PortalRoleNames.HeadOfDepartment]);
        _authContextMock.Setup(context => context.DepartmentId).Returns("Net");

        //Act
        var action = () => _validator.ValidateAsync(updateDto, _token);

        //Assert
        await action.Should()
            .ThrowAsync<ActionNotAllowedException>()
            .WithMessage(PortalMessages.CurrentUserCanSendOnlyApprovedOrRejectedStatuses);
    }

    [Fact]
    public async Task ValidateAsyncWhenAuthedAsGroupManagerAndUpdateStatusForUserNotFromHisGroupThrowsActionNotAllowedException()
    {
        //Arrange
        var userId = 1;

        var updateDto = new UpdateUserSubmitStatusDto
        (
            UserId: userId, Month: 1, Year: 2025, Status: TimesheetSubmitStatusType.Rejected
        );

        _getUserDbQueryMock
            .Setup(query => query.ExecuteAsync(userId, _token))
            .ReturnsAsync(new TimeTrackUser { UserId = userId, DepartmentId = "Net", ResourceGroupId = 2, FirstName = "test", LastName = "test", Email = "test" });

        _getTimesheetSubmitStatusDbQueryMock
            .Setup(query => query.ExecuteAsync(userId, It.IsAny<DateOnly>(), _token))
            .ReturnsAsync(TimesheetSubmitStatusType.UnderReview);

        _authContextMock.Setup(context => context.GetRoles()).Returns([PortalRoleNames.GroupManager]);
        _authContextMock.Setup(context => context.DepartmentId).Returns("Net");
        _authContextMock.Setup(context => context.ResourceGroupId).Returns(1);

        //Act
        var action = () => _validator.ValidateAsync(updateDto, _token);

        //Assert
        await action.Should()
            .ThrowAsync<ActionNotAllowedException>()
            .WithMessage(PortalMessages.GmCannotUpdateStatusForUserFromOtherGroup);
    }

    [Fact]
    public async Task ValidateAsyncWhenAuthedAsGroupManagerAndSendUnderReviewStatusThrowsActionNotAllowedException()
    {
        //Arrange
        var userId = 1;

        var updateDto = new UpdateUserSubmitStatusDto
        (
            UserId: userId, Month: 1, Year: 2025, Status: TimesheetSubmitStatusType.UnderReview
        );

        _getUserDbQueryMock
            .Setup(query => query.ExecuteAsync(userId, _token))
            .ReturnsAsync(new TimeTrackUser { UserId = userId, DepartmentId = "Net", ResourceGroupId = 1, FirstName = "test", LastName = "test", Email = "test" });

        _getTimesheetSubmitStatusDbQueryMock
            .Setup(query => query.ExecuteAsync(userId, It.IsAny<DateOnly>(), _token))
            .ReturnsAsync(null as TimesheetSubmitStatusType?);

        _authContextMock.Setup(context => context.GetRoles()).Returns([PortalRoleNames.GroupManager]);
        _authContextMock.Setup(context => context.DepartmentId).Returns("Net");
        _authContextMock.Setup(context => context.ResourceGroupId).Returns(1);

        //Act
        var action = () => _validator.ValidateAsync(updateDto, _token);

        //Assert
        await action.Should()
            .ThrowAsync<ActionNotAllowedException>()
            .WithMessage(PortalMessages.CurrentUserCanSendOnlyApprovedOrRejectedStatuses);
    }

    [Theory]
    [InlineData(PortalRoleNames.Employee)]
    [InlineData(PortalRoleNames.SystemAdministrator)]
    public async Task ValidateAsyncWhenEmployeeOrSysAdminSendsNotUnderReviewStatusThrowsActionNotAllowedException(string roleName)
    {
        //Arrange
        var userId = 1;

        var updateDto = new UpdateUserSubmitStatusDto
        (
            UserId: userId, Month: 1, Year: 2025, Status: TimesheetSubmitStatusType.Rejected
        );

        _getUserDbQueryMock
            .Setup(query => query.ExecuteAsync(userId, _token))
            .ReturnsAsync(new TimeTrackUser { UserId = userId, DepartmentId = "Net", ResourceGroupId = 1, FirstName = "test", LastName = "test", Email = "test" });

        _getTimesheetSubmitStatusDbQueryMock
            .Setup(query => query.ExecuteAsync(userId, It.IsAny<DateOnly>(), _token))
            .ReturnsAsync(TimesheetSubmitStatusType.UnderReview);

        _authContextMock.Setup(context => context.GetRoles()).Returns([roleName]);
        _authContextMock.Setup(context => context.DepartmentId).Returns("Net");
        _authContextMock.Setup(context => context.ResourceGroupId).Returns(1);

        //Act
        var action = () => _validator.ValidateAsync(updateDto, _token);

        //Assert
        await action.Should()
            .ThrowAsync<ActionNotAllowedException>()
            .WithMessage(PortalMessages.CurrentUserCanOnlySendUnderReviewStatus);
    }

    [Theory]
    [InlineData(PortalRoleNames.Cto)]
    [InlineData(PortalRoleNames.Ceo)]
    public async Task ValidateAsyncWhenChiefSendsNotUnderReviewStatusThrowsActionNotAllowedException(string roleName)
    {
        //Arrange
        var userId = 1;

        var updateDto = new UpdateUserSubmitStatusDto
        (
            UserId: userId, Month: 1, Year: 2025, Status: TimesheetSubmitStatusType.UnderReview
        );

        _getUserDbQueryMock
            .Setup(query => query.ExecuteAsync(userId, _token))
            .ReturnsAsync(new TimeTrackUser { UserId = userId, DepartmentId = "Net", ResourceGroupId = 1, FirstName = "test", LastName = "test", Email = "test" });

        _getTimesheetSubmitStatusDbQueryMock
            .Setup(query => query.ExecuteAsync(userId, It.IsAny<DateOnly>(), _token))
            .ReturnsAsync(null as TimesheetSubmitStatusType?);

        _authContextMock.Setup(context => context.GetRoles()).Returns([roleName]);

        //Act
        var action = () => _validator.ValidateAsync(updateDto, _token);

        //Assert
        await action.Should()
            .ThrowAsync<ActionNotAllowedException>()
            .WithMessage(PortalMessages.CurrentUserCanSendOnlyApprovedOrRejectedStatuses);
    }

    [Fact]
    public async Task ValidateAsyncWhenInvalidRoleThrowsInvalidDataAppException()
    {
        //Arrange
        var userId = 1;

        var updateDto = new UpdateUserSubmitStatusDto
        (
            UserId: userId, Month: 1, Year: 2025, Status: TimesheetSubmitStatusType.Approved
        );

        _getUserDbQueryMock
            .Setup(query => query.ExecuteAsync(userId, _token))
            .ReturnsAsync(new TimeTrackUser { UserId = userId, DepartmentId = "Net", ResourceGroupId = 1, FirstName = "test", LastName = "test", Email = "test" });

        _getTimesheetSubmitStatusDbQueryMock
            .Setup(query => query.ExecuteAsync(userId, It.IsAny<DateOnly>(), _token))
            .ReturnsAsync(TimesheetSubmitStatusType.UnderReview);

        _authContextMock.Setup(context => context.GetRoles()).Returns(["some role"]);

        //Act
        var action = () => _validator.ValidateAsync(updateDto, _token);

        //Assert
        await action.Should()
            .ThrowAsync<InvalidDataAppException>()
            .WithMessage(PortalMessages.ThereIsNoAppropriateRole);
    }

    [Theory]
    [InlineData(PortalRoleNames.Cto)]
    [InlineData(PortalRoleNames.Ceo)]
    public async Task ValidateAsyncWhenRequestValidNotThrows(string roleName)
    {
        //Arrange
        var userId = 1;

        var updateDto = new UpdateUserSubmitStatusDto
        (
            UserId: userId, Month: 1, Year: 2025, Status: TimesheetSubmitStatusType.Approved
        );

        _getUserDbQueryMock
            .Setup(query => query.ExecuteAsync(userId, _token))
            .ReturnsAsync(new TimeTrackUser { UserId = userId, DepartmentId = "Net", ResourceGroupId = 1, FirstName = "test", LastName = "test", Email = "test" });

        _getTimesheetSubmitStatusDbQueryMock
            .Setup(query => query.ExecuteAsync(userId, It.IsAny<DateOnly>(), _token))
            .ReturnsAsync(TimesheetSubmitStatusType.UnderReview);

        _authContextMock.Setup(context => context.GetRoles()).Returns([roleName]);

        //Act
        var action = () => _validator.ValidateAsync(updateDto, _token);

        //Assert
        await action.Should().NotThrowAsync();
    }
}
