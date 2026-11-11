using MockQueryable.Moq;
using Modsen.Portal.Core.TeamBoard;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.Interviews.Core;
using Modsen.Portal.TeamBoard.Requests.Interviews.Core.Validators;
using Modsen.Portal.TeamBoard.Requests.Interviews.Requests;
using Modsen.Portal.TeamBoard.Requests.Interviews.Requests.RequestDto;
using Moq;

namespace Modsen.Portal.TeamBoard.UnitTests.Interviews.Requests
{
    public class CreateInterviewRequestTests
    {
        private readonly Mock<ITeamBoardDbContext> _dbContextMock;
        private readonly Mock<IInterviewLoader> _interviewLoaderMock;
        private readonly Mock<IInterviewUsersValidator> _validatorMock;
        private readonly Mock<IInterviewSaleManagerValidator> _saleMalidatorMock;
        private readonly CreateInterviewRequest _handler;

        public CreateInterviewRequestTests()
        {
            _dbContextMock = new Mock<ITeamBoardDbContext>();
            _interviewLoaderMock = new Mock<IInterviewLoader>();
            _validatorMock = new Mock<IInterviewUsersValidator>();
            _saleMalidatorMock = new Mock<IInterviewSaleManagerValidator>();

            _handler = new CreateInterviewRequest(
                _dbContextMock.Object,
                _interviewLoaderMock.Object,
                _validatorMock.Object,
                _saleMalidatorMock.Object);
        }

        [Fact]
        public async Task HandleShouldCreateInterviewWhenValidInput()
        {
            // Arrange
            var now = DateTime.UtcNow;
            var command = new CreateInterviewRequest.Command(
                UserId: 1,
                Specialization: "Developer",
                Client: "Client A",
                JiraTask: "JIRA-123",
                Type: UserInterviewType.Prescreen,
                Status: UserInterviewStatus.WaitingForInterview,
                InterviewDate: DateOnly.FromDateTime(now),
                InterviewTimeFromUtc: TimeOnly.FromDateTime(now),
                SaleManagerId: 2,
                ReasonForRefusal: null,
                Notes: null
            );

            var user = new TeamBoardUser
            {
                FirstName = "First",
                LastName = "Last",
                Email = "user@example.com"
            };
            var saleManager = new TeamBoardUser
            {
                FirstName = "Sale",
                LastName = "Manager",
                Email = "manager@example.com"
            };

            _dbContextMock
                .Setup(x => x.TeamBoardUsers)
                .Returns(new List<TeamBoardUser> { user, saleManager }.BuildMockDbSet().Object);
            _dbContextMock
                .Setup(x => x.UserInterviews)
                .Returns(new List<UserInterview>().BuildMockDbSet().Object);

            _validatorMock.Setup(x => x.Validate(It.IsAny<InterviewUsersValidator.Request>(), It.IsAny<CancellationToken>()));

            var expectedInterviewDto = new UserInterviewDto
            {
                Id = 1,
                UserId = command.UserId,
                Specialization = command.Specialization,
                ClientName = command.Client,
                UserInterviewDate = command.InterviewDate,
                UserInterviewFromUtc = command.InterviewTimeFromUtc,
                Status = command.Status,
                JiraTask = command.JiraTask,
                SalesResponsibleUserId = command.SaleManagerId,
                Notes = command.Notes
            };

            _interviewLoaderMock
                .Setup(x => x.Load(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedInterviewDto);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            _dbContextMock.Verify(x => x.UserInterviews.AddAsync(It.IsAny<UserInterview>(), It.IsAny<CancellationToken>()), Times.Once);
            _dbContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
