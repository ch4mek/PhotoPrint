using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Users;
using Modsen.Portal.TimeTrack.UseCases.BusinessRules;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.Validators;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Requests;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Requests.RequestDto;

namespace Modsen.Portal.TimeTrack.UnitTests.Timesheets.Core.Validators;

public class GetTimesheetValidatorTests
{
    private readonly Mock<ICanUserBeProjectManagerBusinessRule> _canUserBeProjectManagerBusinessRule = new();
    private readonly Mock<IAuthenticatedUserContext> _authContextMock = new();
    private readonly Mock<IGetUserDetailsDbQuery> _getUserDetailsDbQueryMock = new();

    private readonly GetTimesheetValidator _validator;
    private readonly CancellationToken _token = CancellationToken.None;

    public GetTimesheetValidatorTests()
    {
        _validator = new GetTimesheetValidator(
            _canUserBeProjectManagerBusinessRule.Object,
            _authContextMock.Object,
            _getUserDetailsDbQueryMock.Object);
    }

    [Fact]
    public async Task ValidateAsyncWhenUserIdProvidedButUserNotFoundThrowsArgumentException()
    {
        //Arrange
        var searchQuery = new GetTimesheetsRequest.Query(SearchFilters: new GetTimesheetsDto
        {
            Top = 1,
            Skip = 0,
            UserId = 1,
            ReportForDateFrom = DateOnly.MaxValue,
            ReportForDateTo = DateOnly.MaxValue
        });

        _getUserDetailsDbQueryMock
            .Setup(query => query.Execute(searchQuery.SearchFilters.UserId!.Value, _token))
            .ReturnsAsync(null as GetUserDetailsDbQueryResponse);

        //Act
        var action = () => _validator.ValidateAsync(searchQuery, _token);

        //Assert
        await action.Should()
            .ThrowAsync<ArgumentException>()
            .WithMessage($"User with ID {searchQuery.SearchFilters.UserId!.Value} does not exists");
    }

    [Fact]
    public async Task ValidateAsyncWhenGroupManagerGetsTimesheetsOfUserFromAnotherResourceGroupThrowsActionNotAllowedException()
    {
        //Arrange
        var searchQuery = new GetTimesheetsRequest.Query(SearchFilters: new GetTimesheetsDto
        {
            Top = 1,
            Skip = 0,
            UserId = 1,
            ReportForDateFrom = DateOnly.MaxValue,
            ReportForDateTo = DateOnly.MaxValue
        });

        _getUserDetailsDbQueryMock
            .Setup(query => query.Execute(searchQuery.SearchFilters.UserId!.Value, _token))
            .ReturnsAsync(new GetUserDetailsDbQueryResponse{ResourceGroupId = 1});

        _authContextMock.Setup(context => context.ResourceGroupId).Returns(2);
        _authContextMock.Setup(context => context.GetRoles()).Returns([PortalRoleNames.GroupManager]);

        //Act
        var action = () => _validator.ValidateAsync(searchQuery, _token);

        //Assert
        await action.Should()
            .ThrowAsync<ActionNotAllowedException>()
            .WithMessage(PortalMessages.GroupManagerCanGetOnlyTheSameResourceGroup);
    }

    [Fact]
    public async Task ValidateAsyncWhenEmployeeGetsNotManagedProjectsThrowsActionNotAllowedException()
    {
        //Arrange
        var searchQuery = new GetTimesheetsRequest.Query(SearchFilters: new GetTimesheetsDto
        {
            Top = 1,
            Skip = 0,
            OnlyManagedProjects = false,
            ReportForDateFrom = DateOnly.MaxValue,
            ReportForDateTo = DateOnly.MaxValue
        });

        _authContextMock.Setup(context => context.GetRoles()).Returns([PortalRoleNames.Employee]);

        //Act
        var action = () => _validator.ValidateAsync(searchQuery, _token);

        //Assert
        await action.Should()
            .ThrowAsync<ActionNotAllowedException>()
            .WithMessage(PortalMessages.EmployeeCanGetOnlyManagedProjects);
    }

    [Fact]
    public async Task ValidateAsyncWhenGroupManagerGetsNotManagedProjectsThrowsActionNotAllowedException()
    {
        //Arrange
        var searchQuery = new GetTimesheetsRequest.Query(SearchFilters: new GetTimesheetsDto
        {
            Top = 1,
            Skip = 0,
            OnlyManagedProjects = true,
            ReportForDateFrom = DateOnly.MaxValue,
            ReportForDateTo = DateOnly.MaxValue
        });

        _authContextMock.Setup(context => context.ResourceGroupId).Returns(2);
        _authContextMock.Setup(x => x.GetRequiredUserId()).Returns(1);
        _authContextMock.Setup(context => context.GetRoles()).Returns([PortalRoleNames.GroupManager]);

        _canUserBeProjectManagerBusinessRule
            .Setup(rule => rule.IsCorrespondsToBusiness(1, _token))
            .ReturnsAsync(true);

        //Act
        var action = () => _validator.ValidateAsync(searchQuery, _token);

        //Assert
        await action.Should()
            .ThrowAsync<ActionNotAllowedException>()
            .WithMessage(PortalMessages.CurrentUserCantBeProjectManager);
    }

    [Fact]
    public async Task ValidateAsyncWhenEmployeeCannotBeAProjectManagerThrowsActionNotAllowedException()
    {
        //Arrange
        var searchQuery = new GetTimesheetsRequest.Query(SearchFilters: new GetTimesheetsDto
        {
            Top = 1,
            Skip = 0,
            OnlyManagedProjects = true,
            ReportForDateFrom = DateOnly.MaxValue,
            ReportForDateTo = DateOnly.MaxValue
        });

        _authContextMock.Setup(x => x.GetRequiredUserId()).Returns(1);
        _authContextMock.Setup(context => context.GetRoles()).Returns([PortalRoleNames.Employee]);

        _canUserBeProjectManagerBusinessRule
            .Setup(rule => rule.IsCorrespondsToBusiness(1, _token))
            .ReturnsAsync(false);

        //Act
        var action = () => _validator.ValidateAsync(searchQuery, _token);

        //Assert
        await action.Should()
            .ThrowAsync<ActionNotAllowedException>()
            .WithMessage(PortalMessages.CurrentUserCantBeProjectManager);
    }

    [Fact]
    public async Task ValidateAsyncWhenEmployeeCanBeAProjectManagerNotThrowsAnyException()
    {
        //Arrange
        var searchQuery = new GetTimesheetsRequest.Query(SearchFilters: new GetTimesheetsDto
        {
            Top = 1,
            Skip = 0,
            OnlyManagedProjects = true,
            ReportForDateFrom = DateOnly.MaxValue,
            ReportForDateTo = DateOnly.MaxValue
        });

        _authContextMock.Setup(x => x.GetRequiredUserId()).Returns(1);
        _authContextMock.Setup(context => context.GetRoles()).Returns([PortalRoleNames.Employee]);

        _canUserBeProjectManagerBusinessRule
            .Setup(rule => rule.IsCorrespondsToBusiness(1, _token))
            .ReturnsAsync(true);

        //Act
        var action = () => _validator.ValidateAsync(searchQuery, _token);

        //Assert
        await action.Should().NotThrowAsync();
    }

}
