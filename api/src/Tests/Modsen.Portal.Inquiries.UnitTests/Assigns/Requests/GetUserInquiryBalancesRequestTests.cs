using MockQueryable.Moq;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;
using Modsen.Portal.Inquiries.Requests.Assigns.Core.Calculators;
using Modsen.Portal.Inquiries.Requests.Assigns.Requests;
using Moq;

namespace Modsen.Portal.Inquiries.UnitTests.Assigns.Requests;

public class GetUserInquiryBalancesRequestTests
{
    private readonly Mock<IInquiriesDbContext> _dbContextMock;
    private readonly Mock<IClockService> _clockServiceMock;
    private readonly Mock<IUserAgreementInquiryAvailableDaysCalculator> _availableDaysCalculatorMock;
    private readonly GetUserInquiryBalancesRequest _handler;

    public GetUserInquiryBalancesRequestTests()
    {
        _dbContextMock = new Mock<IInquiriesDbContext>();
        _clockServiceMock = new Mock<IClockService>();
        _availableDaysCalculatorMock = new Mock<IUserAgreementInquiryAvailableDaysCalculator>();
        _handler = new GetUserInquiryBalancesRequest(
            _dbContextMock.Object,
            _clockServiceMock.Object,
            _availableDaysCalculatorMock.Object
        );
    }

    [Fact]
    public async Task HandleWithAssignedInquiryTemplatesReturnsUserBalances()
    {
        // Arrange
        var query = new GetUserInquiryBalancesRequest.Query(1);
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var users = new List<InquiriesUserEntity>
        {
            new()
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                UserId = 1,
                IsActive = true,
            }
        };
        var agreements = new List<UserAgreementEntity>
        {
            new() {
                UserId = 1,
                InquiryTemplateAgreement = new InquiryTemplateAgreementEntity
                {
                    InquiryTemplate = new InquiryTemplateEntity
                    {
                        Id = 1,
                        InquiryType = InquiryType.Vacation,
                        Name = "TestTemplate",
                        InquiryTypeOrderWeight = 2,
                    }
                },
                User = users[0],
                DateFrom = today.AddDays(-10),
                DateTo = today.AddDays(10),
                IsDeleted = false
            }
        };

        var userInquiryTemplates = new List<UserInquiryTemplateEntity>
        {
            new()
            {
                InquiryTemplate = new InquiryTemplateEntity
                {
                    Id = 2,
                    InquiryType = InquiryType.SickLeave,
                    Name = "TestTemplate",
                    InquiryTypeOrderWeight = 1,
                    TrackingUnitType = TrackingUnit.Hours,
                },
                InquiryTemplateId = 2,
                AssignedToUser = users[0],
                AssignedToUserId = 1,
                DateFrom = today.AddDays(-10),
                DateTo = today.AddDays(10),
            }
        };

        var userInquiries = new List<UserInquiryEntity>
        {
            new()
            {
                UserId = 1,
                Status = UserInquiryStatus.Approved,
                TotalHours = 6,
                InquiryTemplateId = 2,
            }
        };

        _dbContextMock
            .Setup(db => db.UserAgreements)
            .Returns(agreements.BuildMockDbSet().Object);
        _dbContextMock
            .Setup(db => db.UserInquiryTemplates)
            .Returns(userInquiryTemplates.BuildMockDbSet().Object);
        _dbContextMock
            .Setup(db => db.UserInquiries)
            .Returns(userInquiries.BuildMockDbSet().Object);
        _dbContextMock
            .Setup(db => db.InquiriesUsers)
            .Returns(users.BuildMockDbSet().Object);
        _clockServiceMock
            .Setup(cs => cs.TodayUtc())
            .Returns(today);
        _availableDaysCalculatorMock
            .Setup(calc => calc.CalculateAsync(1, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(5);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeEmpty();
        result.Should().HaveCount(2);

        result[0].UserId.Should().Be(1);
        result[0].FullName.Should().Be("Doe John");
        result[0].Email.Should().Be("john.doe@example.com");
        result[0].Balance.Should().Be(5);
        result[0].TrackingUnit.Should().Be(TrackingUnit.Days);
        result[0].InquiryTemplateId.Should().Be(1);
        result[0].InquiryType.Should().Be(InquiryType.Vacation);
        result[0].InquiryTemplateName.Should().Be("TestTemplate");

        result[1].UserId.Should().Be(1);
        result[1].FullName.Should().Be("Doe John");
        result[1].Email.Should().Be("john.doe@example.com");
        result[1].Balance.Should().Be(6);
        result[1].TrackingUnit.Should().Be(TrackingUnit.Hours);
        result[1].InquiryTemplateId.Should().Be(2);
        result[1].InquiryType.Should().Be(InquiryType.SickLeave);
        result[1].InquiryTemplateName.Should().Be("TestTemplate");
    }
}
