using MockQueryable.Moq;
using Modsen.Portal.Core.TeamBoard;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.Interviews.Core.AuthenticationFilters;
using Modsen.Portal.TeamBoard.Requests.Interviews.Requests;
using Moq;

namespace Modsen.Portal.TeamBoard.UnitTests.Interviews.Requests
{
    public class DeleteInterviewRequestTests
    {
        private readonly Mock<ITeamBoardDbContext> _dbContextMock;
        private readonly Mock<IInterviewsAuthenticationFilter> _interviewFilterMock; 
        private readonly DeleteInterviewRequest _handler;

        public DeleteInterviewRequestTests()
        {
            _dbContextMock = new Mock<ITeamBoardDbContext>();
            _interviewFilterMock = new Mock<IInterviewsAuthenticationFilter>();
            _handler = new DeleteInterviewRequest(
                _interviewFilterMock.Object,
                _dbContextMock.Object);
        }

        [Fact]
        public async Task HandleShouldUpdateInterviewWhenValidInput()
        {
            // Arrange
            var command = new DeleteInterviewRequest.Command(Id: 1);

            _dbContextMock
                .Setup(x => x.UserInterviews)
                .Returns(new List<UserInterview>().BuildMockDbSet().Object);

            var existingInterview = new UserInterview
            {
                Id = 1,
                Specialization = "Net",
                Status = UserInterviewStatus.WaitingForResult,
                UserId = 2,
                IsActive = true,
            };

            _interviewFilterMock
                .Setup(x => x.Apply(It.IsAny<IQueryable<UserInterview>>()))
                .Returns(new List<UserInterview> { existingInterview }.BuildMockDbSet().Object);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().Be(existingInterview.Id);
            existingInterview.IsActive.Should().BeFalse();
            _dbContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
