using MockQueryable.Moq;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.Validators;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Requests;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Requests.RequestDto;
using Timesheet = Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities.Timesheet;

namespace Modsen.Portal.TimeTrack.UnitTests.Timesheets.Requests.UpdateTaskTimesheet;

public class UpdateTaskNameRequestTests
{
    private readonly Mock<IUpdateTaskNameValidator> _validatorMock = new();
    private readonly Mock<ITimeTrackDbContext> _dbContextMock = new();

    private readonly UpdateTaskNameRequest _handler;

    private readonly CancellationToken _token;

    public UpdateTaskNameRequestTests()
    {
        _handler = new UpdateTaskNameRequest(_validatorMock.Object, _dbContextMock.Object);

        _token = CancellationToken.None;
    }

    [Fact]
    public async Task UpdateTaskNameWhenTimesheetDoesNotExistShouldThrowEntityNotFoundException()
    {
        //Arrange
        var timesheetDto = new UpdateTaskNameDto
        {
            UserId = 2,
            ProjectId = 1,
            EndDate = new DateOnly(2022, 5, 2),
            StartDate = new DateOnly(2022, 3, 22),
            OldTaskName = "Test",
            TaskName = "Some other task"
        };

        _dbContextMock
            .Setup(context => context.Timesheets)
            .Returns(new List<Timesheet>().BuildMockDbSet().Object);

        //Act
        var result = async () =>
        {
            await _handler.Handle(new UpdateTaskNameRequest.Command(timesheetDto), _token);
        };

        //Assert
        await result.Should().ThrowAsync<EntityNotFoundException>();
    }
}
