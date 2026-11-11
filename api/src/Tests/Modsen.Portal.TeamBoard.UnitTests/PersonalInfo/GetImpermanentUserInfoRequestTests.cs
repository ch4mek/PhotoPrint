using MockQueryable.Moq;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Inquiries.Contract;
using Modsen.Portal.Inquiries.Contract.Models;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.Users.Requests;
using Moq;

namespace Modsen.Portal.TeamBoard.UnitTests.PersonalInfo;

public class GetImpermanentUserInfoRequestTests
{
    private readonly Mock<ITeamBoardDbContext> _dbContextMock = new();
    private readonly Mock<IInquiriesContract> _inquiriesMock = new();
    private readonly Mock<IClockService> _clockMock = new();

    private readonly GetImpermanentUserInfoRequest _handler;

    public GetImpermanentUserInfoRequestTests()
    {
        _handler = new GetImpermanentUserInfoRequest(
            _dbContextMock.Object,
            _clockMock.Object,
            _inquiriesMock.Object);
    }

    private static TeamBoardUser CreateUser(
        int userId, string email,
        List<UserPerformanceReview>? reviews = null)
    {
        return new TeamBoardUser
        {
            FirstName = "Test",
            LastName = "Test",
            Email = email,
            UserId = userId,
            UserPerformanceReviews = reviews ?? []
        };
    }

    [Fact]
    public async Task HandleShouldThrowWhenUserNotFound()
    {
        _dbContextMock.Setup(x => x.TeamBoardUsers)
            .Returns(new List<TeamBoardUser>().BuildMockDbSet().Object);

        var act = async () => await _handler.Handle(
            new GetImpermanentUserInfoRequest.Query("test1"),
            CancellationToken.None);

        await act.Should().ThrowAsync<EntityNotFoundException>();
    }

    [Fact]
    public async Task HandleShouldReturnCorrectData()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var now = DateTime.UtcNow;

        _clockMock
            .Setup(x => x.TodayStartOfDayUtc())
            .Returns(now);

        _dbContextMock.Setup(x => x.TeamBoardUsers)
            .Returns(new List<TeamBoardUser>
            {
                CreateUser(1, "testEmail1", new List<UserPerformanceReview>
                {
                    new()
                    {
                        InterviewDateFromUtc = now.AddDays(-1)
                    },
                    new()
                    {
                        InterviewDateFromUtc = now.AddDays(5)
                    }
                })
            }.BuildMockDbSet().Object);

        _inquiriesMock
            .Setup(x => x.GetUserInquiriesAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<UserInquiryDto>
            {
                new()
                {
                    Status = UserInquiryStatus.Open,
                    InquiryTemplateName = "Vacation",
                    InquiryType =  InquiryType.Vacation,
                    DateFrom = today,
                    DateTo = today.AddDays(2)
                },
                new()
                {
                    InquiryTemplateName = "Vacation",
                    Status = UserInquiryStatus.Approved
                }
            });

        _inquiriesMock.Setup(x => x.GetUserBalanceDays(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<UserBalanceDaysDto>
            {
                new()
                {
                    InquiryTemplateName = "Vacation",
                    InquiryType =  InquiryType.Vacation,
                    Balance = 10
                },
                new()
                {
                    InquiryTemplateName = "Document",
                    InquiryType =  InquiryType.Documents,
                    Balance = 0
                }
            });

        var result = await _handler.Handle(
            new GetImpermanentUserInfoRequest.Query("testEmail1"),
            CancellationToken.None);

        result.FuturePerformanceReviewDate.Should().Be(now.AddDays(5));

        result.OpenRequests.Should().HaveCount(1);
        result.OpenRequests.First().InquiryTemplateName.Should().Be("Vacation");

        result.BalanceDays.Should().HaveCount(1);
        result.BalanceDays.First().Balance.Should().Be(10);
    }

    [Fact]
    public async Task HandleShouldReturnNullFutureReviewWhenNoFutureReviews()
    {
        var today = DateTime.UtcNow.Date;

        _clockMock.Setup(x => x.TodayStartOfDayUtc())
            .Returns(today);

        _dbContextMock.Setup(x => x.TeamBoardUsers)
            .Returns(new List<TeamBoardUser>
            {
                CreateUser(1, "testEmail1", new List<UserPerformanceReview>
                {
                    new()
                    {
                        InterviewDateFromUtc = today.AddDays(-5)
                    }
                })
            }.BuildMockDbSet().Object);

        _inquiriesMock.Setup(x => x.GetUserInquiriesAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<UserInquiryDto>());

        _inquiriesMock.Setup(x => x.GetUserBalanceDays(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<UserBalanceDaysDto>());

        var result = await _handler.Handle(
            new GetImpermanentUserInfoRequest.Query("testEmail1"),
            CancellationToken.None);

        result.FuturePerformanceReviewDate.Should().BeNull();
    }

    [Fact]
    public async Task HandleShouldFilterOnlyOpenRequests()
    {
        var today = DateTime.UtcNow.Date;

        _clockMock.Setup(x => x.TodayStartOfDayUtc())
            .Returns(today);

        _dbContextMock.Setup(x => x.TeamBoardUsers)
            .Returns(new List<TeamBoardUser>
            {
                CreateUser(1, "testEmail1")
            }.BuildMockDbSet().Object);

        _inquiriesMock.Setup(x => x.GetUserInquiriesAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<UserInquiryDto>
            {
                new() { InquiryTemplateName = "Vacation", Status = UserInquiryStatus.Open },
                new() { InquiryTemplateName = "Vacation", Status = UserInquiryStatus.Approved },
                new() { InquiryTemplateName = "Vacation", Status = UserInquiryStatus.Open }
            });

        _inquiriesMock.Setup(x => x.GetUserBalanceDays(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<UserBalanceDaysDto>());

        var result = await _handler.Handle(
            new GetImpermanentUserInfoRequest.Query("testEmail1"),
            CancellationToken.None);

        result.OpenRequests.Should().HaveCount(2);
    }

    [Fact]
    public async Task HandleShouldReturnEmptyListsWhenNoData()
    {
        var today = DateTime.UtcNow.Date;

        _clockMock.Setup(x => x.TodayStartOfDayUtc())
            .Returns(today);

        _dbContextMock.Setup(x => x.TeamBoardUsers)
            .Returns(new List<TeamBoardUser>{CreateUser(1, "testEmail1") }.BuildMockDbSet().Object);

        _inquiriesMock.Setup(x => x.GetUserInquiriesAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<UserInquiryDto>());

        _inquiriesMock.Setup(x => x.GetUserBalanceDays(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<UserBalanceDaysDto>());

        var result = await _handler.Handle(
            new GetImpermanentUserInfoRequest.Query("testEmail1"),
            CancellationToken.None);

        result.OpenRequests.Should().BeEmpty();
        result.BalanceDays.Should().BeEmpty();
    }

    [Fact]
    public async Task HandleShouldReturnNearestFutureReviewNotFirstInCollection()
    {
        var today = DateTime.UtcNow.Date;

        _clockMock.Setup(x => x.TodayStartOfDayUtc())
            .Returns(today);

        var farFuture = today.AddDays(10);
        var nearFuture = today.AddDays(2);

        _dbContextMock.Setup(x => x.TeamBoardUsers)
            .Returns(new List<TeamBoardUser>
            {
            CreateUser(1, "testEmail1", new List<UserPerformanceReview>
            {
                new()
                {
                    InterviewDateFromUtc = farFuture 
                },
                new()
                {
                    InterviewDateFromUtc = nearFuture 
                }
            })
            }.BuildMockDbSet().Object);

        _inquiriesMock.Setup(x => x.GetUserInquiriesAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<UserInquiryDto>());

        _inquiriesMock.Setup(x => x.GetUserBalanceDays(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<UserBalanceDaysDto>());

        var result = await _handler.Handle(
            new GetImpermanentUserInfoRequest.Query("testEmail1"),
            CancellationToken.None);

        result.FuturePerformanceReviewDate.Should().Be(nearFuture);
    }
}
