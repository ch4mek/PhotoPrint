using MockQueryable.Moq;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.BusinessRules;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.Composers;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.CoreDto;
using Modsen.Portal.Inquiries.Requests.Inquiries.Request;
using Moq;

namespace Modsen.Portal.Inquiries.UnitTests.Inquiries.Requests;

public class GetInquirySelectedDaysRequestTests
{
    private readonly Mock<IInquiryDatesComposer> _inquiryDatesComposerMock = new();
    private readonly Mock<ITotalAccountantDaysCalculator> _totalAccountantDaysCalculatorMock = new();
    private readonly Mock<IInquiriesDbContext> _inquiriesDbContextMock = new();
    private readonly Mock<IHasUserAccessToInquiryBusinessRule> _hasUserAccessToInquiryBusinessRuleMock = new();

    private readonly CancellationToken _token = CancellationToken.None;

    private readonly GetInquirySelectedDaysRequest _handler;

    public GetInquirySelectedDaysRequestTests()
    {
        _handler = new GetInquirySelectedDaysRequest(_inquiryDatesComposerMock.Object,
            _totalAccountantDaysCalculatorMock.Object,
            _inquiriesDbContextMock.Object,
            _hasUserAccessToInquiryBusinessRuleMock.Object);
    }

    [Fact]
    public async Task HandleWhenUserHasNoAccessToInquiriesShouldThrowActionNotAllowedException()
    {
        //Arrange
        const int userId = 1;

        _hasUserAccessToInquiryBusinessRuleMock
            .Setup(rule => rule.IsCorrespondsToBusiness(userId, AccessOptions.Manager | AccessOptions.Hr, _token))
            .ReturnsAsync(false);

        var query = new GetInquirySelectedDaysRequest.Query(
            1,
            1,
            DateOnly.FromDateTime(DateTime.UtcNow),
            DateOnly.FromDateTime(DateTime.UtcNow).AddDays(10));

        //Act
        var func = () => _handler.Handle(query, _token);

        //Assert
        await func.Should().ThrowAsync<ActionNotAllowedException>(PortalMessages.UserNotFound);
    }

    [Fact]
    public async Task HandleWhenUserHasNoCountryShouldThrowEntityNotFoundException()
    {
        //Arrange
        const int userId = 1;

        _hasUserAccessToInquiryBusinessRuleMock
            .Setup(rule => rule.IsCorrespondsToBusiness(userId, AccessOptions.Manager | AccessOptions.Hr, _token))
            .ReturnsAsync(true);

        _inquiriesDbContextMock
            .Setup(context => context.InquiriesUsers)
            .Returns(new List<InquiriesUserEntity>
            {
                new()
                {
                    UserId = userId,
                    RegisteredLocationId = null,
                    Email = "test",
                    FirstName = "test",
                    LastName = "test",
                }
            }.BuildMockDbSet().Object);

        var query = new GetInquirySelectedDaysRequest.Query(
            1,
            1,
            DateOnly.FromDateTime(DateTime.UtcNow),
            DateOnly.FromDateTime(DateTime.UtcNow).AddDays(10));

        //Act
        var func = () => _handler.Handle(query, _token);

        //Assert
        await func.Should()
            .ThrowAsync<EntityNotFoundException>($"Cannot find country for user with ID {userId}");

    }

    [Fact]
    public async Task HandleWhenInquiryTemplateNotExistsShouldThrowEntityNotFoundException()
    {
        //Arrange
        const int userId = 1;
        const int inquiryTemplateId = 1;

        _hasUserAccessToInquiryBusinessRuleMock
            .Setup(rule => rule.IsCorrespondsToBusiness(userId, AccessOptions.Manager | AccessOptions.Hr, _token))
            .ReturnsAsync(true);

        _inquiriesDbContextMock
            .Setup(context => context.InquiriesUsers)
            .Returns(new List<InquiriesUserEntity>
            {
                new()
                {
                    UserId = userId,
                    RegisteredLocationId = 1,
                    Email = "test",
                    FirstName = "test",
                    LastName = "test",
                }
            }.BuildMockDbSet().Object);

        _inquiriesDbContextMock
            .Setup(context => context.InquiryTemplates)
            .Returns(new List<InquiryTemplateEntity>
            {
                new()
                {
                    Id = inquiryTemplateId + 1,
                    Name = "test",
                }
            }.BuildMockDbSet().Object);

        var query = new GetInquirySelectedDaysRequest.Query(
            inquiryTemplateId,
            userId,
            DateOnly.FromDateTime(DateTime.UtcNow),
            DateOnly.FromDateTime(DateTime.UtcNow).AddDays(10));

        //Act
        var func = () => _handler.Handle(query, _token);

        //Assert
        await func.Should()
            .ThrowAsync<EntityNotFoundException>($"Cannot find inquiry template with ID {inquiryTemplateId}");

    }

    [Fact]
    public async Task HandleWhenRequestValidShouldReturnResult()
    {
        //Arrange
        const int userId = 1;
        const int inquiryTemplateId = 1;

        _hasUserAccessToInquiryBusinessRuleMock
            .Setup(rule => rule.IsCorrespondsToBusiness(userId, AccessOptions.Manager | AccessOptions.Hr, _token))
            .ReturnsAsync(true);

        _inquiriesDbContextMock
            .Setup(context => context.InquiriesUsers)
            .Returns(new List<InquiriesUserEntity>
            {
                new()
                {
                    UserId = userId,
                    RegisteredLocationId = 1,
                    Email = "test",
                    FirstName = "test",
                    LastName = "test",
                }
            }.BuildMockDbSet().Object);

        _inquiriesDbContextMock
            .Setup(context => context.InquiryTemplates)
            .Returns(new List<InquiryTemplateEntity>
            {
                new()
                {
                    Id = inquiryTemplateId,
                    Name = "test",
                }
            }.BuildMockDbSet().Object);

        _inquiriesDbContextMock
            .Setup(context => context.CalendarExclusions)
            .Returns(new List<CalendarExclusionEntity>().BuildMockDbSet().Object);

        var dateEntity = new UserInquiryDateEntity
        {
            DateType = DateType.WorkingDay,
            Date = new DateOnly(year: 2025, month: 12, day: 11)
        };

        _inquiryDatesComposerMock
            .Setup(composer => composer.Compose(It.IsAny<InquiryDatesComposerRequest>(), _token))
            .ReturnsAsync([dateEntity]);

        var days = new List<(DateType, DateOnly)> { (dateEntity.DateType, dateEntity.Date) };

        _totalAccountantDaysCalculatorMock
            .Setup(calculator => calculator.Calculate(
                It.IsAny<bool>(),
                It.IsAny<bool>(),
                days))
            .Returns(new TotalAccoundantDaysCoreDto(2.08m, 5.9m));

        var query = new GetInquirySelectedDaysRequest.Query(
            inquiryTemplateId,
            userId,
            DateOnly.FromDateTime(DateTime.UtcNow),
            DateOnly.FromDateTime(DateTime.UtcNow).AddDays(10));

        //Act
        var result = await _handler.Handle(query, _token);

        //Assert
        result.Should().Be(5);
    }

    [Fact]
    public async Task HandleWhenDateFromAfterDateToThrowsInvalidDataAppException()
    {
        // Arrange
        const int userId = 1;
        const int inquiryTemplateId = 1;
        var dateFrom = DateOnly.FromDateTime(DateTime.UtcNow);
        var dateTo = dateFrom.AddDays(-1);

        var query = new GetInquirySelectedDaysRequest.Query(inquiryTemplateId, userId, dateFrom, dateTo);

        // Act
        var result = async () => await _handler.Handle(query, _token);

        // Assert
        await result.Should().ThrowAsync<InvalidDataAppException>()
            .WithMessage("The date range is invalid");
    }

    [Fact]
    public async Task HandleWhenDateRangeExceedsOneYearThrowsInvalidDataAppException()
    {
        // Arrange
        const int userId = 1;
        const int inquiryTemplateId = 1;
        var dateFrom = DateOnly.FromDateTime(DateTime.UtcNow);
        var dateTo = dateFrom.AddYears(1).AddDays(1);

        var query = new GetInquirySelectedDaysRequest.Query(inquiryTemplateId, userId, dateFrom, dateTo);

        // Act
        var result = async () => await _handler.Handle(query, _token);

        // Assert
        await result.Should().ThrowAsync<InvalidDataAppException>()
            .WithMessage("The date range is invalid");
    }

    [Fact]
    public async Task HandleWhenRequestValidHasCalendarExclusionBothDatesInRangeShouldReturnResult()
    {
        //Arrange
        const int userId = 1;
        const int inquiryTemplateId = 1;
        var baseDate = DateOnly.FromDateTime(DateTime.UtcNow);

        _hasUserAccessToInquiryBusinessRuleMock
            .Setup(rule => rule.IsCorrespondsToBusiness(userId, AccessOptions.Manager | AccessOptions.Hr, _token))
            .ReturnsAsync(true);

        _inquiriesDbContextMock
            .Setup(context => context.InquiriesUsers)
            .Returns(new List<InquiriesUserEntity>
            {
                new()
                {
                    UserId = userId,
                    RegisteredLocationId = 1,
                    Email = "test",
                    FirstName = "test",
                    LastName = "test",
                }
            }.BuildMockDbSet().Object);

        _inquiriesDbContextMock
            .Setup(context => context.InquiryTemplates)
            .Returns(new List<InquiryTemplateEntity>
            {
                new()
                {
                    Id = inquiryTemplateId,
                    Name = "test",
                    IncludeWeekends = false
                }
            }.BuildMockDbSet().Object);

        _inquiriesDbContextMock
            .Setup(context => context.CalendarExclusions)
            .Returns(new List<CalendarExclusionEntity>
            {
                new()
                {
                    CountryId = 1,
                    Date = baseDate.AddDays(5),
                    NewDate = baseDate.AddDays(7)
                }
            }.BuildMockDbSet().Object);

        var dateEntity = new UserInquiryDateEntity
        {
            DateType = DateType.WorkingDay,
            Date = new DateOnly(year: 2025, month: 12, day: 11)
        };

        _inquiryDatesComposerMock
            .Setup(composer => composer.Compose(It.IsAny<InquiryDatesComposerRequest>(), _token))
            .ReturnsAsync([dateEntity]);

        var days = new List<(DateType, DateOnly)> { (dateEntity.DateType, dateEntity.Date) };

        _totalAccountantDaysCalculatorMock
            .Setup(calculator => calculator.Calculate(
                It.IsAny<bool>(),
                It.IsAny<bool>(),
                days))
            .Returns(new TotalAccoundantDaysCoreDto(2.08m, 5.9m));

        var query = new GetInquirySelectedDaysRequest.Query(
            inquiryTemplateId,
            userId,
            baseDate,
            baseDate.AddDays(10));

        //Act
        var result = await _handler.Handle(query, _token);

        //Assert
        result.Should().Be(5);
    }
}
