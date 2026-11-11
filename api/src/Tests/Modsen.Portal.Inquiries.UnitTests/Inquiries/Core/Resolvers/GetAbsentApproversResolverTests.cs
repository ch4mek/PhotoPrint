using MockQueryable.Moq;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.Resolvers;
using Moq;

namespace Modsen.Portal.Inquiries.UnitTests.Inquiries.Core.Resolvers;

public class GetAbsentApproversResolverTests
{
    private readonly GetAbsentApproversResolver _resolver;
    private readonly Mock<IInquiriesDbContext> _inquiriesDbContext = new();
    private readonly Mock<IClockService> _clockService = new();

    public GetAbsentApproversResolverTests()
    {
        _resolver = new(_inquiriesDbContext.Object, _clockService.Object);

        _clockService.Setup(x => x.TodayUtc())
            .Returns(DateOnly.FromDateTime(DateTime.UtcNow));
    }

    private readonly List<UserInquiryEntity> _userInquiries =
    [
        new UserInquiryEntity
        {
            UserId = 1,
            InquiryTemplate =
                new InquiryTemplateEntity { Name = "Inquiry", ActivityType = ActivityType.NonWorkingPaid },
            Status = UserInquiryStatus.Approved,
            DateFrom = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-1),
            DateTo = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(1)
        },


        new UserInquiryEntity
        {
            UserId = 2,
            InquiryTemplate =
                new InquiryTemplateEntity { Name = "Inquiry", ActivityType = ActivityType.NonWorkingPaid },
            Status = UserInquiryStatus.Approved,
            DateFrom = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(11),
            DateTo = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(12)
        },

        new UserInquiryEntity
        {
            UserId = 2,
            InquiryTemplate =
                new InquiryTemplateEntity { Name = "Inquiry 2", ActivityType = ActivityType.NonWorkingUnpaid },
            Status = UserInquiryStatus.Open,
            DateFrom = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-1),
            DateTo = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(1)
        },

        new UserInquiryEntity
        {
            UserId = 3,
            InquiryTemplate =
                new InquiryTemplateEntity { Name = "Inquiry 3", ActivityType = ActivityType.NonWorkingPaid },
            Status = UserInquiryStatus.Approved,
            DateFrom = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-1),
            DateTo = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(1)
        },

        new UserInquiryEntity
        {
            UserId = 1,
            InquiryTemplate =
                new InquiryTemplateEntity { Name = "Inquiry", ActivityType = ActivityType.WorkingPaid },
            Status = UserInquiryStatus.Approved,
            DateFrom = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-1),
            DateTo = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(1)
        }
    ];

    [Fact]
    public async Task ResolveShouldReturnAbsentApprovers()
    {
        // Arrange
        _inquiriesDbContext.SetupGet(x => x.UserInquiries)
            .Returns(_userInquiries.BuildMockDbSet().Object);

        var expected = new[] { 1 };

        // Act
        var result = await _resolver.ResolveAsync([1, 2], CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(expected);
    }
}
