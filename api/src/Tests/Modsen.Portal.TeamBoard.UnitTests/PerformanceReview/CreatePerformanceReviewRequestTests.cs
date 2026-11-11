using MockQueryable.Moq;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Core.UseCases;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.Reviews.PerformanceReviews.Requests;
using Modsen.Portal.TeamBoard.Requests.Reviews.PerformanceReviews.Requests.RequestDto;
using Moq;

namespace Modsen.Portal.TeamBoard.UnitTests.PerformanceReview;

public class CreatePerformanceReviewRequestTests
{
    private readonly Mock<ITeamBoardDbContext> _teamBoardDbContextMock = new();
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock = new();
    private readonly IClockService _clockService = new ClockService();
    private readonly CancellationToken _token = CancellationToken.None;
    private readonly Mock<ICanTeamBoardUserBeRetrievedBusinessRule> _canTeamBoardUserBeRetrievedBusinessRule = new();
    private readonly CreatePerformanceReviewRequest _handler;

    public CreatePerformanceReviewRequestTests()
    {
        _handler = new CreatePerformanceReviewRequest(
            _teamBoardDbContextMock.Object,
            _authenticatedUserContextMock.Object,
            _clockService,
            _canTeamBoardUserBeRetrievedBusinessRule.Object);
    }

    [Fact]
    public async Task HandleWhenPerformanceReviewDoesNotExistOnDateShouldCreatePerformanceReview()
    {
        // Arrange
        var userId = 3;
        var date = DateTime.UtcNow;

        var expectedResult = new PerformanceReviewDto
        {
            UserId = userId,
            Date = date
        };

        var createRequest = new CreatePerformanceReviewRequest.Command(new CreatePerformanceReviewDto(userId, date));

        _canTeamBoardUserBeRetrievedBusinessRule.Setup(x=>x.IsCorrespondsToBusiness(It.IsAny<int>(), Portal.Core.AccessOptions.Manager, _token))
                        .ReturnsAsync(true);

        SetupPerformanceReviews([]);

        _teamBoardDbContextMock
            .Setup(x => x.TeamBoardUsers)
            .Returns(new List<TeamBoardUser>
            {
                new()
                {
                    FirstName = "Test",
                    LastName = "Test",
                    Email = "test",
                    UserId = 1,
                    Position = "Employee",
                    DepartmentId = "Net",
                }
            }.BuildMockDbSet().Object);

        // Act
        var result = await _handler.Handle(createRequest, _token);

        // Assert
        result.Should().NotBeNull();
        result.Should().Be(expectedResult);
    }

    [Fact]
    public async Task HandleWhenUserIsInternShouldhrowActionNotAllowedException()
    {
        // Arrange
        var userId = 3;
        var date = DateTime.UtcNow;

        var createRequest = new CreatePerformanceReviewRequest.Command(new CreatePerformanceReviewDto(userId, date));

        SetupPerformanceReviews([]);

        _teamBoardDbContextMock
            .Setup(x => x.TeamBoardUsers)
            .Returns(new List<TeamBoardUser>
            {
                new()
                {
                    FirstName = "Intern",
                    LastName = "Intern",
                    Email = "Intern",
                    UserId = userId,
                    Position = PortalRoleNames.Intern,
                    DepartmentId = "LD",
                }
            }.BuildMockDbSet().Object);

        // Act
        var result = async () => await _handler.Handle(createRequest, _token);

        // Assert
        await result.Should()
            .ThrowAsync<ActionNotAllowedException>()
            .WithMessage(PortalMessages.NotAllowedOperationWithIntern);
    }

    [Fact]
    public async Task HandleWhenPerformanceReviewExistOnDateShouldThrowEntityConflictException()
    {
        // Arrange

        var userId = It.IsAny<int>();
        var date = DateTime.UtcNow;

        var request = new CreatePerformanceReviewRequest.Command(new CreatePerformanceReviewDto(userId, date));
        var exceptionMessage = "Perfomance review for the same date already exists";
        _canTeamBoardUserBeRetrievedBusinessRule.Setup(x => x.IsCorrespondsToBusiness(It.IsAny<int>(), _token))
                        .ReturnsAsync(true);
        var performanceReviews = new List<UserPerformanceReview>
        {
            new()
            {
                Id = 1,
                UserId = userId,
                InterviewDateFromUtc = date,
            }
        };

        SetupPerformanceReviews(performanceReviews);

        // Act
        var act = async () => await _handler.Handle(request, _token);

        // Assert
        await act.Should().ThrowAsync<EntityConflictException>(exceptionMessage);
    }

    private void SetupPerformanceReviews(List<UserPerformanceReview> performanceReviews)
    {
        _teamBoardDbContextMock
            .Setup(mock => mock.UserPerformanceReviews)
            .Returns(performanceReviews.BuildMockDbSet().Object);
    }
}
