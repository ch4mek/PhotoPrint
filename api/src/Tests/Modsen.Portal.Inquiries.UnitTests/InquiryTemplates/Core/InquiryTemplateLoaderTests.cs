using MockQueryable.Moq;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;
using Modsen.Portal.Inquiries.Requests.InquiryTemplates.Core;
using Moq;

namespace Modsen.Portal.Inquiries.UnitTests.InquiryTemplates.Core;

public class InquiryTemplateLoaderTests
{
    private readonly InquiryTemplateLoader _loader;
    private readonly Mock<IInquiriesDbContext> _dbContext = new();
    private readonly Mock<IClockService> _clockService = new();
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public InquiryTemplateLoaderTests()
    {
        _loader = new InquiryTemplateLoader(_dbContext.Object, _clockService.Object);
    }

    private void ClockServiceDefaultSetup()
    {
        _clockService.Setup(x => x.TodayUtc())
            .Returns(DateOnly.FromDateTime(DateTime.UtcNow));
    }

    [Fact]
    public async Task LoadAsyncWhenThereAssignmentForFutureShouldReturnValid()
    {
        // Arrange
        var userInquiryTemplates = new List<UserInquiryTemplateEntity>
        {
            new()
            {
                Id = 1,
                AssignedToUser = new InquiriesUserEntity { Email = "asd", FirstName = "First", LastName = "Last", IsActive = true },
                DateTo = null,
                InquiryTemplateId = 1,
            }
        };

        var userAgreements = new List<UserAgreementEntity>
        {
            new()
            {
                Id = 1,
                UserId = 1,
                User = new InquiriesUserEntity { Email = "asd", FirstName = "First", LastName = "Last", IsActive = true },
                InquiryAgreementTemplateId = 1,
            }
        };

        var inquiryTemplateAgreements = new List<InquiryTemplateAgreementEntity>
            { new() { Id = 1, UserAgreements = userAgreements, InquiryTemplateId = 1 } };

        var inquiryTemplates = new List<InquiryTemplateEntity>()
        {
            new()
            {
                Id = 1,
                IncludeHolidays = true,
                IncludeWeekends = true,
                InquiryType = InquiryType.Vacation,
                IsActive = true,
                IsDefault = true,
                Name = "vacation",
                IsStaked = true,
                InquiryTemplateAssigns = userInquiryTemplates,
                InquiryTemplateAgreements = inquiryTemplateAgreements,
                UserInquiryAgreementAssigns =
                [
                    new UserInquiryEntity
                    {
                        AgreementTemplateId = 1,
                        UserId = 1,
                        User = new InquiriesUserEntity
                        {
                            Email = "asd", FirstName = "First", LastName = "Last", IsActive = true
                        },
                    }
                ],
                AccrualType = AccrualType.OnceInYear,
                ActivityType = ActivityType.NonWorkingUnpaid,
                Comment = "asdf",
                CreatedAtUtc = DateTime.UtcNow.AddDays(-2),
                CreatedById = 1,

            }
        };

        _dbContext.Setup(x => x.InquiryTemplates)
            .Returns(inquiryTemplates.BuildMockDbSet().Object);
        _dbContext.Setup(c => c.InquiryTemplateAgreements)
            .Returns(inquiryTemplateAgreements.BuildMockDbSet().Object);
        _dbContext.Setup(c => c.UserAgreements)
            .Returns(userAgreements.BuildMockDbSet().Object);
        _dbContext.Setup(c => c.UserInquiryTemplates)
            .Returns(userInquiryTemplates.BuildMockDbSet().Object);
        _dbContext.Setup(c => c.UserAgreementTransactions)
            .Returns(new List<UserAgreementTransactionEntity>().BuildMockDbSet().Object);

        ClockServiceDefaultSetup();

        // Act
        var result = await _loader.LoadAsync([1], _cancellationToken);

        // Assert
        result.First().AssignedUsersCount.Should().Be(1);
        result.First().Agreements.First().AssignedUsersCount.Should().Be(1);
    }
}
