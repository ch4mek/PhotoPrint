using MockQueryable.Moq;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;
using Modsen.Portal.Inquiries.Requests.Assigns.Core.Resolvers;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.Resolvers;
using Moq;

namespace Modsen.Portal.Inquiries.UnitTests.Assigns.Core.Resolvers;

public class UnassignUserFromInquiryTemplatesResolverTests
{
    private readonly UnassignUserFromInquiryTemplatesResolver _resolver;
    private readonly Mock<IInquiriesDbContext> _inquiriesDbContextMock = new();
    private readonly Mock<IClockService> _clockServiceMock = new();
    private readonly Mock<IRejectInquiriesAfterUnassignResolver> _inquiriesResolverMock = new();

    public UnassignUserFromInquiryTemplatesResolverTests()
    {
        _resolver = new UnassignUserFromInquiryTemplatesResolver(
            _inquiriesDbContextMock.Object,
            _clockServiceMock.Object,
            _inquiriesResolverMock.Object);       
    }

    [Fact]
    public async Task ResolveShouldResolveValid()
    {
        // Arrange
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        _clockServiceMock.Setup(x => x.TodayUtc()).Returns(today);
        var userId = 1;
        var unassignUserId = 2;

        var userInquiryTemplateEntities = new List<UserInquiryTemplateEntity>
        {
            new()
            {
                Id = 1,
                AssignedToUserId = 1,
                DateFrom = today.AddDays(-10),
                DateTo = null,
                InquiryTemplateId = 1
            }
        };

        var userAgreementEntities = new List<UserAgreementEntity>
        {
            new()
            {
                Id = 1,
                UserId = 1,
                DateFrom = today.AddDays(-7),
                DateTo = null,
                IsDeleted = false,
                InquiryAgreementTemplateId = 2,
                InquiryTemplateAgreement = new InquiryTemplateAgreementEntity
                {
                    Id = 2,
                    InquiryTemplateId = 2
                }
            },
            new()
            {
                Id = 3,
                UserId = 1,
                DateFrom = today,
                DateTo = null,
                IsDeleted = false,
                InquiryAgreementTemplateId = 3,
                InquiryTemplateAgreement = new InquiryTemplateAgreementEntity
                {
                    Id = 3,
                    InquiryTemplateId = 3
                }
            }
        };

        _inquiriesDbContextMock.SetupGet(x => x.UserInquiryTemplates)
            .Returns(userInquiryTemplateEntities.BuildMockDbSet().Object);

        _inquiriesDbContextMock.SetupGet(x => x.UserAgreements)
            .Returns(userAgreementEntities.BuildMockDbSet().Object);

        var expectedDateTo = today.AddDays(-1);

        // Act
        await _resolver.ResolveAsync(userId, unassignUserId, CancellationToken.None);

        // Assert        
        _inquiriesDbContextMock.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once());
        _inquiriesResolverMock.Verify(x => x.ResolveAsync(userId, It.IsAny<int[]>(), unassignUserId, CancellationToken.None), Times.Once());
        userInquiryTemplateEntities.Should().OnlyContain(x => x.DateTo == today.AddDays(-1));
        userAgreementEntities.Should().OnlyContain(x => x.DateTo == x.DateFrom.AddDays(-1));
        userAgreementEntities.Should().OnlyContain(x => x.IsDeleted);
    }
}
