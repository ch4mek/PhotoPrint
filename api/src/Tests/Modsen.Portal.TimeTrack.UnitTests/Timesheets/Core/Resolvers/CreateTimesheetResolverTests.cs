using MockQueryable.Moq;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Identity.Contract;
using Modsen.Portal.Identity.Contract.Models;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.CoreDto;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.DbQueries;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.Resolvers;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.Validators;

namespace Modsen.Portal.TimeTrack.UnitTests.Timesheets.Core.Resolvers;

public class CreateTimesheetResolverTests
{
    private readonly Mock<IGetProjectUserBillableStatusDbQuery> _getProjectUserBillableStatusDbQueryMock = new();
    private readonly Mock<ICreateTimesheetValidator> _createTimesheetValidatorMock = new();
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock = new();
    private readonly Mock<IGetTimesheetDbQuery> _getTimesheetDbQueryMock = new();
    private readonly Mock<IIdentityContract> _identityContractMock = new();
    private readonly Mock<ITimeTrackDbContext> _dbContextMock = new();
    private readonly Mock<IClockService> _clockServiceMock = new();

    private readonly CreateTimesheetResolver _resolver;

    private readonly CancellationToken _token = CancellationToken.None;
    private readonly DateOnly _utcNow = DateOnly.FromDateTime(DateTime.UtcNow);

    public CreateTimesheetResolverTests()
    {
        _resolver = new CreateTimesheetResolver(
            _createTimesheetValidatorMock.Object,
            _identityContractMock.Object,
            _authenticatedUserContextMock.Object,
            _getProjectUserBillableStatusDbQueryMock.Object,
            _getTimesheetDbQueryMock.Object,
            _dbContextMock.Object,
            _clockServiceMock.Object);
    }

    [Theory]
    [InlineData(PortalRoleNames.Employee)]
    [InlineData(PortalRoleNames.Ceo)]
    [InlineData(PortalRoleNames.Cto)]
    [InlineData(PortalRoleNames.HeadOfDepartment)]
    [InlineData(PortalRoleNames.SystemAdministrator)]
    public async Task ResolveWhenUserHasAssignmentAndTimesheetWithTheSameTaskNameExistsReturnEntityConflictException(string userRole)
    {
        // Arrange
        var dto = new CreateTimesheetDto
        {
            TaskName = "taskName",
            FactTrackedInMinutes = 180,
            ReportForDate = _utcNow,
            ProjectId = 1,
            UserId = 1
        };

        _dbContextMock
            .Setup(context => context.Timesheets)
            .Returns(new List<Timesheet>
            {
                new()
                {
                    Id = 1,
                    UserId = dto.UserId,
                    ProjectId = dto.ProjectId,
                    ReportYear = dto.ReportForDate.Year,
                    ReportDayOfYear = dto.ReportForDate.DayOfYear,
                    TaskName = dto.TaskName
                }
            }.BuildMockDbSet().Object);

        _identityContractMock
            .Setup(
                contract => contract.GetPermissionAssignment(
                    dto.UserId,
                    dto.ProjectId.ToString(),
                    PermissionResourceType.Project,
                    PermissionType.TrackBillableWorkingHours,
                    _token))
            .ReturnsAsync(new PermissionAssignmentDto
            {
                UserId = 1,
                Id = 1,
                ResourceId = dto.ProjectId.ToString(),
                PermissionType = PermissionType.TrackBillableWorkingHours,
                ResourceType = PermissionResourceType.Project
            });

        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns([userRole]);
        _authenticatedUserContextMock.Setup(context => context.UserId).Returns(1);
        _authenticatedUserContextMock.Setup(x => x.GetRequiredUserId()).Returns(1);

        // Act
        var result = () => _resolver.Resolve(dto, _token);

        //Assert
        await result.Should()
            .ThrowAsync<EntityConflictException>()
            .WithMessage(PortalMessages.TaskWithSameNameAlreadyExists);
    }

}
