using MockQueryable.Moq;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;
using Modsen.Portal.Inquiries.Requests.InquiryTemplates.Core;
using Moq;

namespace Modsen.Portal.Inquiries.UnitTests.Inquiries.Core.Loaders
{
    public class InquiryTemplateLoaderTests
    {
        private readonly Mock<IInquiriesDbContext> _dbContextMock;
        private readonly Mock<IClockService> _clockServiceMock;
        private readonly IInquiryTemplateLoader _loader;

        public InquiryTemplateLoaderTests()
        {
            _dbContextMock = new Mock<IInquiriesDbContext>();
            _clockServiceMock = new Mock<IClockService>();
            _loader = new InquiryTemplateLoader(_dbContextMock.Object, _clockServiceMock.Object);
        }

        [Fact]
        public async Task LoadAsyncWhenTemplateDoesNotExistReturnsEmptyList()
        {
            // Arrange
            _dbContextMock.Setup(c => c.InquiryTemplates)
                .Returns(new List<InquiryTemplateEntity>().BuildMockDbSet().Object);
            _dbContextMock.Setup(c => c.UserAgreementTransactions)
                .Returns(new List<UserAgreementTransactionEntity>().BuildMockDbSet().Object);

            _clockServiceMock.Setup(c => c.TodayUtc())
                .Returns(DateOnly.FromDateTime(DateTime.UtcNow));

            // Act
            var result = await _loader.LoadAsync([1], CancellationToken.None);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task LoadAsyncWhenTemplateExistsReturnsMappedDto()
        {
            // Arrange
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            const int templateId = 1;

            var userAgreements = new List<UserAgreementEntity>
            {
                new()
                {
                    DateFrom = today.AddDays(-1), DateTo = today.AddDays(1), UserId = 1,
                    User = new()
                    {
                        FirstName = "Test",
                        LastName = "Test",
                        Email = "Test",
                        IsActive = true
                    },
                    InquiryAgreementTemplateId = 1,
                }
            };

            var inquiryTemplateAgreements = new List<InquiryTemplateAgreementEntity>
            {
                new()
                {
                    Id = 1,
                    Name = "Agreement1",
                    IsDefault = true,
                    UserAgreements = userAgreements,
                    InquiryTemplateId = templateId,
                }
            };

            var templates = new List<InquiryTemplateEntity>
            {
                new()
                {
                    Id = templateId,
                    Name = "Vacation Template",
                    InquiryType = InquiryType.Vacation,
                    ActivityType = ActivityType.NonWorkingUnpaid,
                    Comment = "Comment",
                    IsActive = true,
                    CreatedAtUtc = DateTime.UtcNow,
                    IncludeWeekends = true,
                    IncludeHolidays = true,
                    RequireAttachments = false,
                    ProhibitOnProbationPeriod = false,
                    TrackingUnitType = TrackingUnit.Days,
                    IsStaked = true,
                    InquiryTemplateAgreements = inquiryTemplateAgreements,
                    InquiryTemplateAssigns = [],
                    InquiryTemplateAcceptPolicyPolicies = []
                }
            };

            _dbContextMock.Setup(c => c.InquiryTemplates)
                .Returns(templates.BuildMockDbSet().Object);
            _dbContextMock.Setup(c => c.InquiryTemplateAgreements)
                .Returns(inquiryTemplateAgreements.BuildMockDbSet().Object);
            _dbContextMock.Setup(c => c.UserAgreements)
                .Returns(userAgreements.BuildMockDbSet().Object);
            _dbContextMock.Setup(c => c.UserAgreementTransactions)
                .Returns(new List<UserAgreementTransactionEntity>().BuildMockDbSet().Object);

            _clockServiceMock.Setup(c => c.TodayUtc()).Returns(today);

            // Act
            var result = await _loader.LoadAsync([templateId], CancellationToken.None);

            // Assert
            result.Should().HaveCount(1);
            var dto = result.First();
            dto.Id.Should().Be(templateId);
            dto.AssignedUsersCount.Should().Be(1);
            dto.Agreements.Should().ContainSingle();
            dto.Agreements.First().AssignedUsersCount.Should().Be(1);
        }

        [Fact]
        public async Task LoadAsyncWhenTemplateIdsIsNullReturnsAllActiveTemplates()
        {
            // Arrange
            var templates = new List<InquiryTemplateEntity>
            {
                new() { Id = 1, IsActive = true, Name = "T1", InquiryType = InquiryType.Vacation },
                new() { Id = 2, IsActive = false, Name = "T2", InquiryType = InquiryType.SickDay },
                new() { Id = 3, IsActive = true, Name = "T3", InquiryType = InquiryType.MilitaryEnlistmentOffice }
            };

            _dbContextMock.Setup(c => c.InquiryTemplates)
                .Returns(templates.BuildMockDbSet().Object);
            _dbContextMock.Setup(c => c.InquiryTemplateAgreements)
                .Returns(new List<InquiryTemplateAgreementEntity>().BuildMockDbSet().Object);
            _dbContextMock.Setup(c => c.UserAgreements)
                .Returns(new List<UserAgreementEntity>().BuildMockDbSet().Object);
            _dbContextMock.Setup(c => c.UserInquiryTemplates)
                .Returns(new List<UserInquiryTemplateEntity>().BuildMockDbSet().Object);
            _dbContextMock.Setup(c => c.UserAgreementTransactions)
                .Returns(new List<UserAgreementTransactionEntity>().BuildMockDbSet().Object);

            _clockServiceMock.Setup(c => c.TodayUtc())
                .Returns(DateOnly.FromDateTime(DateTime.UtcNow));

            // Act
            var result = await _loader.LoadAsync(null, CancellationToken.None);

            // Assert
            result.Should().HaveCount(2);
            result.Select(x => x.Id).Should().BeEquivalentTo([1, 3]);
        }

        [Fact]
        public async Task LoadAsyncIncludesAssignedUsersAndAgreements()
        {
            // Arrange
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            var userAgreements = new List<UserAgreementEntity>
            {
                new() {
                    DateFrom = today.AddDays(-1), DateTo = today.AddDays(1), UserId = 1,
                    User = new()
                    {
                        FirstName = "Test",
                        LastName = "Test",
                        Email = "Test",
                        IsActive = true
                    }
                }
            };

            var inquiryTemplateAgreements = new List<InquiryTemplateAgreementEntity>
                { new() { Id = 100, UserAgreements = userAgreements, InquiryTemplateId = 10 } };

            var templates = new List<InquiryTemplateEntity>
            {
                new()
                {
                    Id = 10,
                    Name = "Test",
                    IsActive = true,
                    InquiryType = InquiryType.Vacation,
                    InquiryTemplateAgreements = inquiryTemplateAgreements,
                    InquiryTemplateAssigns =
                        [new() { DateFrom = today.AddDays(-1), DateTo = today.AddDays(1), AssignedToUserId = 2 }],
                    InquiryTemplateAcceptPolicyPolicies =
                    [
                        new()
                        {
                            Id = 1,
                            UserId = 3,
                            User = new InquiriesUserEntity
                            {
                                FirstName = "Anna", LastName = "Ivanova", Email = "test", UserId = 3
                            },
                            SubstituteUserId = 4,
                            SubstituteUser = new InquiriesUserEntity
                            {
                                FirstName = "Petr", LastName = "Petrov", Email = "test", UserId = 4
                            },
                            OrderWeight = 1
                        }
                    ]
                }
            };

            _dbContextMock.Setup(c => c.InquiryTemplates)
                .Returns(templates.BuildMockDbSet().Object);
            _dbContextMock.Setup(c => c.InquiryTemplateAgreements)
                .Returns(inquiryTemplateAgreements.BuildMockDbSet().Object);
            _dbContextMock.Setup(c => c.UserAgreements)
                .Returns(userAgreements.BuildMockDbSet().Object);
            _dbContextMock.Setup(c => c.UserAgreementTransactions)
                .Returns(new List<UserAgreementTransactionEntity>().BuildMockDbSet().Object);

            _clockServiceMock.Setup(c => c.TodayUtc()).Returns(today);

            // Act
            var result = await _loader.LoadAsync([10], CancellationToken.None);

            // Assert
            var dto = result.Single();
            dto.Agreements.Should().HaveCount(1);
            dto.AssignedUsersCount.Should().Be(1);
            dto.AcceptPolicies.Should().ContainSingle();
            dto.AcceptPolicies.First().UserFullName.Should().Be("Ivanova Anna");
            dto.AcceptPolicies.First().SubstituteUserFullName.Should().Be("Petrov Petr");
        }
    }
}
