using MockQueryable.Moq;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Core.TeamBoard;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.Inquiries.Contract;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.Home.Requests;
using Modsen.Portal.TeamBoard.Requests.Home.Requests.RequestsDto;
using Modsen.Portal.TeamBoard.Requests.Reviews.PerformanceReviews.Core.Calculators;
using Modsen.Portal.TeamBoard.Requests.TeamBoardPortalSettings;
using Modsen.Portal.TeamBoard.Requests.TeamBoardPortalSettings.CoreDto;
using Moq;

namespace Modsen.Portal.TeamBoard.UnitTests.Home;

public class GetUserUpcomingEventsRequestTests
{
    private readonly Mock<ICanTeamBoardUserBeRetrievedBusinessRule> _canEventsBeAccessedByUserMock = new();
    private readonly Mock<ITeamBoardPortalSettingsAccessor> _teamBoardPortalSettingsAccessorMock = new();
    private readonly Mock<IPerformanceReviewDateTypeCalculator> _reviewDateTypeCalculatorMock = new();    
    private readonly Mock<ITeamBoardDbContext> _dbContextMock = new();
    private readonly Mock<IInquiriesContract> _inquiriesContractMock = new();

    private readonly GetUserUpcomingEventsRequest _handler;

    private readonly CancellationToken _token = CancellationToken.None;

    public GetUserUpcomingEventsRequestTests()
    {
        _handler = new GetUserUpcomingEventsRequest(
            _canEventsBeAccessedByUserMock.Object,
            _teamBoardPortalSettingsAccessorMock.Object,
            _reviewDateTypeCalculatorMock.Object,
            _dbContextMock.Object,
            _inquiriesContractMock.Object);
    }

    [Fact]
    public async Task HandleWhenUserHasNoAccessToAnotherUserEventsThrowsNotAllowedException()
    {
        //Arrange
        const int userId = 1;

        _canEventsBeAccessedByUserMock
            .Setup(businessRule => businessRule.IsCorrespondsToBusiness(userId, _token))
            .ReturnsAsync(false);

        //Act
        var action = () => _handler.Handle(
            new GetUserUpcomingEventsRequest.Query(userId, 7), _token);

        //Assert
        await action.Should().ThrowAsync<ActionNotAllowedException>();

    }

    [Fact]
    public async Task HandleWhenUserHasNotPassedPerformanceReviewsInTheFutureReturnsPerformanceReviewsEvents()
    {
        //Arrange
        const int userId = 1;
        const int overdueDays = 7;

        var futureDate = DateTime.UtcNow.AddDays(3);

        _canEventsBeAccessedByUserMock
            .Setup(businessRule => businessRule.IsCorrespondsToBusiness(userId, _token))
            .ReturnsAsync(true);

        _teamBoardPortalSettingsAccessorMock
            .Setup(accessor => accessor.GetSettings(_token))
            .ReturnsAsync(new TeamBoardPortalSettingsDto { UpcomingIminentBorderDaysCount = 3 });

        _dbContextMock
            .Setup(context => context.DevelopmentPlans)
            .Returns(new List<DevelopmentPlan>().BuildMockDbSet().Object);

        _dbContextMock
            .Setup(context => context.UserContracts)
            .Returns(new List<UserContract>().BuildMockDbSet().Object);

        _dbContextMock
            .Setup(context => context.UserPerformanceReviews)
            .Returns(new List<UserPerformanceReview>
            {
                new()
                {
                    UserId = userId,
                    Status = UserPerformanceReviewStatus.Passed,
                    InterviewDateFromUtc = futureDate.AddDays(1),
                },
                new()
                {
                    UserId = userId,
                    Status = UserPerformanceReviewStatus.Pending,
                    InterviewDateFromUtc = futureDate,
                },
                new()
                {
                    UserId = userId,
                    Status = UserPerformanceReviewStatus.Pending,
                    InterviewDateFromUtc = futureDate.AddMonths(-4),
                },
            }.BuildMockDbSet().Object);

        _reviewDateTypeCalculatorMock
            .Setup(calculator => calculator.Calculate(It.IsAny<DateTime>(), overdueDays, 3))
            .Returns("Future");

        _inquiriesContractMock
            .Setup(x => x.GetUserInquiriesAsync(userId, _token))
            .ReturnsAsync([]);

        var expected = new List<UpcomingEventDto>
        {
            new()
            {
                PerformanceReviewDateType = "Future",
                StartDate = DateOnly.FromDateTime(futureDate.AddMonths(-4)),
                Type = UpcomingEventType.PerformanceReview
            },
            new()
            {
                PerformanceReviewDateType = "Future",
                StartDate = DateOnly.FromDateTime(futureDate),
                Type = UpcomingEventType.PerformanceReview
            }
        };

        //Act
        var result = await _handler.Handle(new GetUserUpcomingEventsRequest.Query(userId, overdueDays), _token);

        //Assert
        result.Count.Should().Be(expected.Count);
        result.Should().BeEquivalentTo(expected, config => config.WithStrictOrdering());
    }

    [Fact]
    public async Task HandleWhenUserHasNotApprovedDevPlansWithDeadlineInTheFutureReturnsDevPlansEvents()
    {
        //Arrange
        const int userId = 1;
        const int overdueDays = 7;

        var futureDate = DateTime.UtcNow.AddDays(3);

        _canEventsBeAccessedByUserMock
            .Setup(businessRule => businessRule.IsCorrespondsToBusiness(userId, _token))
            .ReturnsAsync(true);

        _teamBoardPortalSettingsAccessorMock
            .Setup(accessor => accessor.GetSettings(_token))
            .ReturnsAsync(new TeamBoardPortalSettingsDto { UpcomingIminentBorderDaysCount = 3 });

        _dbContextMock
            .Setup(context => context.DevelopmentPlans)
            .Returns(new List<DevelopmentPlan>
            {
                new()
                {
                    UserId = userId,
                    DeadlineDate = DateOnly.FromDateTime(futureDate),
                    Status = DevelopmentPlanStatusType.None,
                    Summary = "test",
                },
                new()
                {
                    UserId = userId,
                    DeadlineDate = DateOnly.FromDateTime(futureDate.AddMonths(-1)),
                    Status = DevelopmentPlanStatusType.None,
                    Summary = "test",
                },
                new()
                {
                    UserId = userId,
                    DeadlineDate = DateOnly.FromDateTime(futureDate.AddDays(1)),
                    Status = DevelopmentPlanStatusType.Approved,
                    Summary = "test",
                },
                new()
                {
                    UserId = userId,
                    DeadlineDate = DateOnly.FromDateTime(futureDate.AddDays(2)),
                    Status = DevelopmentPlanStatusType.Rejected,
                    Summary = "test",
                },
                new()
                {
                    UserId = userId,
                    DeadlineDate = DateOnly.FromDateTime(futureDate.AddDays(3)),
                    Status = DevelopmentPlanStatusType.UnderReview,
                    Summary = "test",
                }
            }.BuildMockDbSet().Object);

        _dbContextMock
            .Setup(context => context.UserContracts)
            .Returns(new List<UserContract>().BuildMockDbSet().Object);

        _dbContextMock
            .Setup(context => context.UserPerformanceReviews)
            .Returns(new List<UserPerformanceReview>().BuildMockDbSet().Object);

        _inquiriesContractMock
            .Setup(x => x.GetUserInquiriesAsync(userId, _token))
            .ReturnsAsync([]);

        //Act
        var result = await _handler.Handle(new GetUserUpcomingEventsRequest.Query(userId, overdueDays), _token);

        //Assert
        result.Count.Should().Be(3);

        result.FirstOrDefault()!.Type.Should().Be(UpcomingEventType.DevelopmentPlan);

    }

    [Fact]
    public async Task HandleWhenUserHasContractsWithContractEndInTheFutureReturnsContractsEvents()
    {
        //Arrange
        const int userId = 1;
        const int overdueDays = 7;

        var futureDate = DateTime.UtcNow.AddDays(3);

        _canEventsBeAccessedByUserMock
            .Setup(businessRule => businessRule.IsCorrespondsToBusiness(userId, _token))
            .ReturnsAsync(true);

        _teamBoardPortalSettingsAccessorMock
            .Setup(accessor => accessor.GetSettings(_token))
            .ReturnsAsync(new TeamBoardPortalSettingsDto { UpcomingIminentBorderDaysCount = 3 });

        _dbContextMock
            .Setup(context => context.DevelopmentPlans)
            .Returns(new List<DevelopmentPlan>().BuildMockDbSet().Object);

        _dbContextMock
            .Setup(context => context.UserContracts)
            .Returns(new List<UserContract>
            {
                new()
                {
                    UserId = userId,
                    ContractTo = DateOnly.FromDateTime(futureDate),
                },
                new()
                {
                    UserId = userId,
                    ContractTo = null,
                },
                new()
                {
                    UserId = userId,
                    ContractTo = DateOnly.FromDateTime(futureDate.AddMonths(-1)),
                },
                new()
                {
                    UserId = userId,
                    ContractTo = DateOnly.FromDateTime(futureDate.AddMonths(1)),
                },
            }.BuildMockDbSet().Object);

        _dbContextMock
            .Setup(context => context.UserPerformanceReviews)
            .Returns(new List<UserPerformanceReview>().BuildMockDbSet().Object);

        _inquiriesContractMock
            .Setup(x => x.GetUserInquiriesAsync(userId, _token))
            .ReturnsAsync([]);

        //Act
        var result = await _handler.Handle(new GetUserUpcomingEventsRequest.Query(userId, overdueDays), _token);

        //Assert
        result.Count.Should().Be(2);

        result.FirstOrDefault()!.Type.Should().Be(UpcomingEventType.Contract);

    }

    [Fact]
    public async Task HandleWhenUserHasEventsInFutureReturnsAllValidEventsWithValidOrder()
    {
        //Arrange
        const int userId = 1;
        const int overdueDays = 7;

        var futureDate = DateTime.UtcNow.AddDays(3);

        _canEventsBeAccessedByUserMock
            .Setup(businessRule => businessRule.IsCorrespondsToBusiness(userId, _token))
            .ReturnsAsync(true);

        _teamBoardPortalSettingsAccessorMock
            .Setup(accessor => accessor.GetSettings(_token))
            .ReturnsAsync(new TeamBoardPortalSettingsDto { UpcomingIminentBorderDaysCount = 3 });

        _reviewDateTypeCalculatorMock
            .Setup(calculator => calculator.Calculate(It.IsAny<DateTime>(), overdueDays, 3))
            .Returns("Future");

        _dbContextMock
            .Setup(context => context.DevelopmentPlans)
            .Returns(new List<DevelopmentPlan>
            {
                new()
                {
                    UserId = userId,
                    DeadlineDate = DateOnly.FromDateTime(futureDate),
                    Status = DevelopmentPlanStatusType.None,
                    Summary = "test",
                },
                new()
                {
                    UserId = userId,
                    DeadlineDate = DateOnly.FromDateTime(futureDate.AddMonths(-1)),
                    Status = DevelopmentPlanStatusType.None,
                    Summary = "test",
                },
                new()
                {
                    UserId = userId,
                    DeadlineDate = DateOnly.FromDateTime(futureDate.AddDays(1)),
                    Status = DevelopmentPlanStatusType.Approved,
                    Summary = "test",
                },
                new()
                {
                    UserId = userId,
                    DeadlineDate = DateOnly.FromDateTime(futureDate.AddDays(2)),
                    Status = DevelopmentPlanStatusType.Rejected,
                    Summary = "test",
                },
                new()
                {
                    UserId = userId,
                    DeadlineDate = DateOnly.FromDateTime(futureDate.AddDays(2)),
                    Status = DevelopmentPlanStatusType.UnderReview,
                    Summary = "test",
                }
            }.BuildMockDbSet().Object);

        _dbContextMock
            .Setup(context => context.UserContracts)
            .Returns(new List<UserContract>
            {
                new()
                {
                    UserId = userId,
                    ContractTo = DateOnly.FromDateTime(futureDate),
                },
                new()
                {
                    UserId = userId,
                    ContractTo = null,
                },
                new()
                {
                    UserId = userId,
                    ContractTo = DateOnly.FromDateTime(futureDate.AddMonths(-1)),
                },
                new()
                {
                    UserId = userId,
                    ContractTo = DateOnly.FromDateTime(futureDate.AddMonths(1)),
                },
            }.BuildMockDbSet().Object);

        _dbContextMock
            .Setup(context => context.UserPerformanceReviews)
            .Returns(new List<UserPerformanceReview>
            {
                new()
                {
                    UserId = userId,
                    Status = UserPerformanceReviewStatus.Passed,
                    InterviewDateFromUtc = futureDate.AddDays(1),
                },
                new()
                {
                    UserId = userId,
                    Status = UserPerformanceReviewStatus.Pending,
                    InterviewDateFromUtc = futureDate,
                },
                new()
                {
                    UserId = userId,
                    Status = UserPerformanceReviewStatus.Pending,
                    InterviewDateFromUtc = futureDate.AddMonths(-4),
                },
            }.BuildMockDbSet().Object);

        _inquiriesContractMock
            .Setup(x => x.GetUserInquiriesAsync(userId, _token))
            .ReturnsAsync([
                new()
                {
                    Id = 1,
                    UserId = userId,
                    Status = UserInquiryStatus.Open,
                    DateFrom = DateOnly.FromDateTime(futureDate),
                    DateTo = DateOnly.FromDateTime(futureDate),
                    InquiryTemplateName = "Name",
                    InquiryType = InquiryType.Vacation
                },

                new()
                {
                    Id = 2,
                    UserId = userId,
                    Status = UserInquiryStatus.Open,
                    DateFrom = DateOnly.FromDateTime(futureDate.AddDays(3)),
                    DateTo = DateOnly.FromDateTime(futureDate.AddDays(3)),
                    InquiryTemplateName = "Name name",
                    InquiryType = InquiryType.Vacation
                }

            ]);

        var expectedEvents = new List<UpcomingEventDto>
        {
            new()
            {
                Type = UpcomingEventType.PerformanceReview,
                StartDate = DateOnly.FromDateTime(futureDate.AddMonths(-4)),
                PerformanceReviewDateType = "Future"
            },
            new()
            {
                Type = UpcomingEventType.Absence,
                StartDate = DateOnly.FromDateTime(futureDate),
                EndDate = DateOnly.FromDateTime(futureDate),
                Status = UserInquiryStatus.Open.GetDescription(),
                Name = "Name",
                InquiryId = 1,
                InquiryType = InquiryType.Vacation
            },
            new()
            {
                Type = UpcomingEventType.PerformanceReview,
                StartDate = DateOnly.FromDateTime(futureDate),
                PerformanceReviewDateType = "Future",
                InquiryId = null
            },
            new()
            {
                Type = UpcomingEventType.DevelopmentPlan,
                StartDate = DateOnly.FromDateTime(futureDate)
            },
            new()
            {
                Type = UpcomingEventType.Contract,
                StartDate = DateOnly.FromDateTime(futureDate),
            },
            new()
            {
                Type = UpcomingEventType.DevelopmentPlan,
                StartDate = DateOnly.FromDateTime(futureDate.AddDays(2)),
            },
            new()
            {
                Type = UpcomingEventType.DevelopmentPlan,
                StartDate = DateOnly.FromDateTime(futureDate.AddDays(2)),
            },
             new()
            {
                Type = UpcomingEventType.Absence,
                StartDate = DateOnly.FromDateTime(futureDate.AddDays(3)),
                EndDate = DateOnly.FromDateTime(futureDate.AddDays(3)),
                Status = UserInquiryStatus.Open.GetDescription(),
                Name = "Name name",
                InquiryId = 2,
                InquiryType = InquiryType.Vacation
            },
            new()
            {
                Type = UpcomingEventType.Contract,
                StartDate = DateOnly.FromDateTime(futureDate.AddMonths(1)),
            }
        };
        
        //Act
        var result = await _handler.Handle(new GetUserUpcomingEventsRequest.Query(userId, overdueDays), _token);

        //Assert
        result.Count.Should().Be(expectedEvents.Count);
        result.Should().BeEquivalentTo(expectedEvents,
            config => config.WithStrictOrdering());

    }
}
