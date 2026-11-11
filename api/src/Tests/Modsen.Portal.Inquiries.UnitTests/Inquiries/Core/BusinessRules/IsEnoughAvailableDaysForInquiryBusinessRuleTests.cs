using MockQueryable.Moq;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;
using Modsen.Portal.Inquiries.Requests.Assigns.Core.Calculators;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.BusinessRules;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.Composers;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.CoreDto;
using Moq;

namespace Modsen.Portal.Inquiries.UnitTests.Inquiries.Core.BusinessRules;

public class IsEnoughAvailableDaysForInquiryBusinessRuleTests
{
    private readonly IsEnoughAvailableDaysForInquiryBusinessRule _businessRule;

    private readonly Mock<IInquiriesDbContext> _inquiryDbContext = new();
    private readonly Mock<ITotalAccountantDaysCalculator> _totalAccountantDaysCalculator = new();
    private readonly Mock<IInquiryDatesComposer> _inquiryDatesComposer = new();
    private readonly Mock<IUserAgreementInquiryAvailableDaysCalculator> _availableDaysCalculator = new();

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public IsEnoughAvailableDaysForInquiryBusinessRuleTests()
    {
        _businessRule = new(_inquiryDbContext.Object, _totalAccountantDaysCalculator.Object,
            _availableDaysCalculator.Object, _inquiryDatesComposer.Object);
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenInquiryIsNotSickDayTypeShouldReturnTrue()
    {
        // Arrange
        var dateFrom = DateOnly.FromDateTime(DateTime.UtcNow);
        var dateTo = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5));
        var userId = 1;
        var inquiryTemplateId = 1;

        _inquiryDbContext
            .Setup(x => x.InquiryTemplates)
            .Returns(new List<InquiryTemplateEntity>
            {
                new InquiryTemplateEntity()
                {
                    Id = 1,
                    Name = "Not Sick Day",
                    InquiryType = InquiryType.OwnExpense
                }
            }.BuildMockDbSet().Object);

        // Act
        var result = await _businessRule.IsCorrespondsToBusiness(inquiryTemplateId, userId, dateFrom, dateTo, _cancellationToken);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenUserDoesNotExistsShouldThrowEntityNotFound()
    {
        // Arrange
        var dateFrom = DateOnly.FromDateTime(DateTime.UtcNow);
        var dateTo = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5));
        var userId = 1;
        var inquiryTemplateId = 1;

        _inquiryDbContext
            .Setup(x => x.InquiryTemplates)
            .Returns(new List<InquiryTemplateEntity>
            {
                new InquiryTemplateEntity()
                {
                    Id = 1,
                    Name = "sick Day",
                    InquiryType = InquiryType.SickDay
                }
            }.BuildMockDbSet().Object);

        _inquiryDbContext
           .Setup(x => x.InquiriesUsers)
           .Returns(new List<InquiriesUserEntity>().BuildMockDbSet().Object);

        // Act
        var result = () => _businessRule.IsCorrespondsToBusiness(inquiryTemplateId, userId, dateFrom, dateTo, _cancellationToken);

        // Assert
        await result.Should().ThrowAsync<EntityNotFoundException>();
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenRegLocationIsNotSpecifiedShouldThrowEnityNotFound()
    {
        // Arrange
        var dateFrom = DateOnly.FromDateTime(DateTime.UtcNow);
        var dateTo = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5));
        var userId = 1;
        var inquiryTemplateId = 1;

        _inquiryDbContext
            .Setup(x => x.InquiryTemplates)
            .Returns(new List<InquiryTemplateEntity>
            {
                new InquiryTemplateEntity()
                {
                    Id = 1,
                    Name = "sick Day",
                    InquiryType = InquiryType.SickDay
                }
            }.BuildMockDbSet().Object);

        var user = new InquiriesUserEntity
        {
            UserId = userId,
            Email = "eee",
            FirstName = "Fffff",
            LastName = "Lllll",
            RegisteredLocationId = null
        };

        _inquiryDbContext
           .Setup(x => x.InquiriesUsers)
           .Returns(new List<InquiriesUserEntity> { user }.BuildMockDbSet().Object);

        // Act
        var result = () => _businessRule.IsCorrespondsToBusiness(inquiryTemplateId, userId, dateFrom, dateTo, _cancellationToken);

        // Assert
        await result.Should().ThrowAsync<EntityNotFoundException>();
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenIsEnoughtShouldReturnTrue()
    {
        // Arrange
        var dateFrom = DateOnly.FromDateTime(DateTime.UtcNow);
        var dateTo = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5));
        var userId = 1;
        var inquiryTemplateId = 1;

        var availableDays = 5.0m;
        var requestedDays = 4.0m;

        _inquiryDbContext
            .Setup(x => x.InquiryTemplates)
            .Returns(new List<InquiryTemplateEntity>
            {
                new InquiryTemplateEntity()
                {
                    Id = 1,
                    Name = "Sick Day",
                    InquiryType = InquiryType.SickDay
                }
            }.BuildMockDbSet().Object);

        var user = new InquiriesUserEntity
        {
            UserId = userId,
            Email = "eee",
            FirstName = "Fffff",
            LastName = "Lllll",
            RegisteredLocationId = 1
        };

        _inquiryDbContext
           .Setup(x => x.InquiriesUsers)
           .Returns(new List<InquiriesUserEntity> { user }.BuildMockDbSet().Object);

        _availableDaysCalculator
            .Setup(x => x.CalculateAsync(userId, 1, _cancellationToken))
            .ReturnsAsync(availableDays);

        var dateEntites = new List<UserInquiryDateEntity>
        {
            new UserInquiryDateEntity() {
                DateType = DateType.DayOff,
                Date = new DateOnly()
            }
        };

        _inquiryDatesComposer
            .Setup(x => x.Compose(It.IsAny<InquiryDatesComposerRequest>(), _cancellationToken))
            .ReturnsAsync(dateEntites);

        var days = dateEntites.Select(x => (x.DateType, x.Date)).ToList();

        _totalAccountantDaysCalculator
            .Setup(x => x.Calculate(It.IsAny<bool>(), It.IsAny<bool>(), days))
            .Returns(new TotalAccoundantDaysCoreDto(requestedDays, requestedDays));

        // Act
        var result = await _businessRule.IsCorrespondsToBusiness(inquiryTemplateId, userId, dateFrom, dateTo, _cancellationToken);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenIsNotEnoughtShouldReturnFalse()
    {
        // Arrange
        var dateFrom = DateOnly.FromDateTime(DateTime.UtcNow);
        var dateTo = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5));
        var userId = 1;
        var inquiryTemplateId = 1;

        var availableDays = 5.0m;
        var requestedDays = 7.0m;

        _inquiryDbContext
            .Setup(x => x.InquiryTemplates)
            .Returns(new List<InquiryTemplateEntity>
            {
                new InquiryTemplateEntity()
                {
                    Id = 1,
                    Name = "Sick Day",
                    InquiryType = InquiryType.SickDay
                }
            }.BuildMockDbSet().Object);

        var user = new InquiriesUserEntity
        {
            UserId = userId,
            Email = "eee",
            FirstName = "Fffff",
            LastName = "Lllll",
            RegisteredLocationId = 1
        };

        _inquiryDbContext
           .Setup(x => x.InquiriesUsers)
           .Returns(new List<InquiriesUserEntity> { user }.BuildMockDbSet().Object);

        _availableDaysCalculator
            .Setup(x => x.CalculateAsync(userId, 1, _cancellationToken))
            .ReturnsAsync(availableDays);

        _inquiryDatesComposer
            .Setup(x => x.Compose(It.IsAny<InquiryDatesComposerRequest>(), _cancellationToken))
            .ReturnsAsync([]);

        _totalAccountantDaysCalculator
            .Setup(x => x.Calculate(It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<List<(DateType, DateOnly)>>()))
            .Returns(new TotalAccoundantDaysCoreDto(requestedDays, requestedDays));

        // Act
        var result = await _businessRule.IsCorrespondsToBusiness(inquiryTemplateId, userId, dateFrom, dateTo, _cancellationToken);

        // Assert
        result.Should().BeFalse();
    }
}
