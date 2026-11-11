using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.BusinessRules;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.CoreDto;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.DbQueries;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.Resolvers;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.Validators;
using Timesheet = Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities.Timesheet;

namespace Modsen.Portal.TimeTrack.UnitTests.Timesheets.Core.Resolvers;

public class UpdateTimesheetResolverTests
{
    private readonly Mock<IGetProjectBillableStatusIfExistsDbQuery> _getProjectBillableStatusIfExistsDbQuery = new();
    private readonly Mock<ICanModifyTimesheetForDateBusinessRule> _canModifyTimesheetForDateBusinessRuleMock = new();
    private readonly Mock<IGetProjectUserBillableStatusDbQuery> _getProjectUserBillableStatusDbQueryMock = new();
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock = new();
    private readonly Mock<IUpdateTimesheetValidator> _updateTimesheetValidatorMock = new();
    private readonly Mock<IGetTimesheetDbQuery> _getTimesheetDbQueryMock = new();
    private readonly Mock<ITimeTrackDbContext> _dbContextMock = new();
    private readonly Mock<IGetProjectActiveStatusDbQuery> _getProjectActiveStatusDbQueryMock = new();

    private readonly UpdateTimesheetResolver _handler;

    public UpdateTimesheetResolverTests()
    {
        _handler = new UpdateTimesheetResolver(
            _updateTimesheetValidatorMock.Object,
            _getProjectUserBillableStatusDbQueryMock.Object,
            _authenticatedUserContextMock.Object,
            _canModifyTimesheetForDateBusinessRuleMock.Object,
            _getProjectBillableStatusIfExistsDbQuery.Object,
            _getTimesheetDbQueryMock.Object,
            _dbContextMock.Object,
            _getProjectActiveStatusDbQueryMock.Object
            );
    }

    [Fact]
    public void UpdateFactTrackedTimeWhenTimesheetDoesNotExistThrowsEntityNotFoundException()
    {
        //Arrange
        const int timesheetId = 1;

        var updateTrackedTimeCommand = new UpdateTimesheetDto
        {
            Id = timesheetId,
            FactTrackedInMinutes = 1,
        };

        _getTimesheetDbQueryMock
            .Setup(query => query.ExecuteAsync(timesheetId, CancellationToken.None))
            .ReturnsAsync(null as Timesheet);

        //Act
        var task = async () => await _handler
            .Resolve(updateTrackedTimeCommand, CancellationToken.None);


        //Assert
        task
            .Should()
            .ThrowAsync<EntityNotFoundException>()
            .WithMessage(PortalMessages.TimesheetNotFound);
    }
}
