using MockQueryable.Moq;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.Builders;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.Composers;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.CoreDto;
using Moq;

namespace Modsen.Portal.Inquiries.UnitTests.Inquiries.Core.Builders
{
    public class UserInquiryBuilderTests
    {

        private readonly Mock<IInquiriesDbContext> _dbContextMock = new();
        private readonly Mock<IInquiryDatesComposer> _datesComposerMock = new();
        private readonly Mock<ITotalAccountantDaysCalculator> _daysCalculatorMock = new();
        private readonly Mock<IInquiryAcceptsComposer> _acceptsComposerMock = new();
        private readonly UserInquiryBuilder _builder;

        public UserInquiryBuilderTests()
        {
            _builder = new UserInquiryBuilder(
                _dbContextMock.Object,
                _datesComposerMock.Object,
                _daysCalculatorMock.Object,
                _acceptsComposerMock.Object);
        }

        [Fact]
        public async Task BuildShouldThrowEntityNotFoundExceptionWhenCountryIdNotFound()
        {
            // Arrange
            var request = CreateRequest(TrackingUnit.Days);
            var users = new List<InquiriesUserEntity>().BuildMockDbSet();
            _dbContextMock.Setup(x => x.InquiriesUsers).Returns(users.Object);

            // Act & Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(() => _builder.Build(request, CancellationToken.None));
        }

        [Fact]
        public async Task BuildShouldReturnEntityWithDaysTracking()
        {
            // Arrange
            var request = CreateRequest(TrackingUnit.Days);
            var users = new List<InquiriesUserEntity>
            {
                new InquiriesUserEntity { UserId = request.UserId, FirstName = "Alex", LastName = "Brown", Email = "test1@mail.ru", RegisteredLocationId = 100 }
            }.BuildMockDbSet();

            _dbContextMock.Setup(x => x.InquiriesUsers).Returns(users.Object);

            var templates = new List<InquiryTemplateEntity>
            {
                new InquiryTemplateEntity { Id = request.InquiryTemplateId, Name = "Test" }
            }.BuildMockDbSet();

            _dbContextMock.Setup(x => x.InquiryTemplates).Returns(templates.Object);

            var inquiryDates = new List<UserInquiryDateEntity>
            {
                new UserInquiryDateEntity { DateType = DateType.WorkingDay, Date = new DateOnly(year: 2025, month: 12, day: 11) }
            };
            _datesComposerMock.Setup(x => x.Compose(It.IsAny<InquiryDatesComposerRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(inquiryDates);

            var days = inquiryDates.Select(x => (x.DateType, x.Date)).ToList();

            _daysCalculatorMock
                .Setup(x => x.Calculate(request.IncludeHolidays, request.IncludeWeekends, days))
                .Returns(new TotalAccoundantDaysCoreDto( 5,4 ));

            _acceptsComposerMock.Setup(x => x.ComposeAsync(request.UserId, request.InquiryTemplateId, It.IsAny<CancellationToken>()))
                .ReturnsAsync([new UserInquiryAcceptEntity()]);

            // Act
            var result = await _builder.Build(request, CancellationToken.None);

            // Assert
            Assert.Equal(request.UserId, result.UserId);
            Assert.Equal(5, result.TotalDays);
            Assert.Equal(4, result.TotalAccountantDays);
            Assert.Single(result.UserInquiryDates);
            Assert.Single(result.UserInquiryAccepts);
        }

        [Fact]
        public async Task BuildShouldReturnEntityWithHoursTracking()
        {
            // Arrange
            var request = CreateRequest(TrackingUnit.Hours);
            request.Hours = 8;

            var users = new List<InquiriesUserEntity>
            {
                new InquiriesUserEntity { UserId = request.UserId,  FirstName = "Alex", LastName = "Brown", Email = "test1@mail.ru", RegisteredLocationId = 100 }
            }.BuildMockDbSet();
            _dbContextMock.Setup(x => x.InquiriesUsers).Returns(users.Object);

            var templates = new List<InquiryTemplateEntity>
            {
                new InquiryTemplateEntity { Id = request.InquiryTemplateId, Name = "Test" }
            }.BuildMockDbSet();
            _dbContextMock.Setup(x => x.InquiryTemplates).Returns(templates.Object);

            var days = new List<(DateType, DateOnly)>
            {
                (DateType.Holiday, DateOnly.FromDateTime(DateTime.UtcNow))
            };

            var dateEntities = new List<UserInquiryDateEntity>
            {
                new UserInquiryDateEntity
                {
                    Id = 1,
                    CalendarExclusion = new CalendarExclusionEntity(),
                    RequestedDays = 1,
                    DateType = DateType.Holiday,
                    Date = DateOnly.FromDateTime(DateTime.UtcNow)
                }
            };

            _datesComposerMock.Setup(x => x.Compose(It.IsAny<InquiryDatesComposerRequest>(), CancellationToken.None))
                .ReturnsAsync(dateEntities);

            var daysForCalculation = dateEntities.Select(x => (x.DateType, x.Date)).ToList();

            _daysCalculatorMock.Setup(x => x.Calculate(request.IncludeHolidays, request.IncludeWeekends, days))
                .Returns(new TotalAccoundantDaysCoreDto(1, 1));

            _acceptsComposerMock.Setup(x => x.ComposeAsync(request.UserId, request.InquiryTemplateId, It.IsAny<CancellationToken>()))
                .ReturnsAsync([]);

            // Act
            var result = await _builder.Build(request, CancellationToken.None);

            // Assert
            Assert.Equal(request.Hours, result.TotalHours);
            Assert.Null(result.TotalDays);
            Assert.Empty(result.UserInquiryDates);
        }

        [Fact]
        public async Task BuildWhenRequestOnHolidayShouldReturnEntityWithZeroHours()
        {
            // Arrange
            var request = CreateRequest(TrackingUnit.Hours);
            request.Hours = 8;
            request.IncludeHolidays = false;

            var users = new List<InquiriesUserEntity>
            {
                new InquiriesUserEntity { UserId = request.UserId,  FirstName = "Alex", LastName = "Brown", Email = "test1@mail.ru", RegisteredLocationId = 100 }
            }.BuildMockDbSet();
            _dbContextMock.Setup(x => x.InquiriesUsers).Returns(users.Object);

            var templates = new List<InquiryTemplateEntity>
            {
                new InquiryTemplateEntity { Id = request.InquiryTemplateId, Name = "Test" }
            }.BuildMockDbSet();
            _dbContextMock.Setup(x => x.InquiryTemplates).Returns(templates.Object);

            var days = new List<(DateType, DateOnly)>
            {
                (DateType.Holiday, DateOnly.FromDateTime(DateTime.UtcNow))
            };

            var dateEntities = new List<UserInquiryDateEntity>
            {
                new UserInquiryDateEntity
                {
                    Id = 1,
                    CalendarExclusion = new CalendarExclusionEntity(),
                    RequestedDays = 1,
                    DateType = DateType.Holiday,
                    Date = DateOnly.FromDateTime(DateTime.UtcNow)
                }
            };

            _datesComposerMock.Setup(x => x.Compose(It.IsAny<InquiryDatesComposerRequest>(), CancellationToken.None))
                .ReturnsAsync(dateEntities);

            var daysForCalculation = dateEntities.Select(x => (x.DateType, x.Date)).ToList();

            _daysCalculatorMock.Setup(x => x.Calculate(request.IncludeHolidays, request.IncludeWeekends, days))
                .Returns(new TotalAccoundantDaysCoreDto(1, 0));

            _acceptsComposerMock.Setup(x => x.ComposeAsync(request.UserId, request.InquiryTemplateId, It.IsAny<CancellationToken>()))
                .ReturnsAsync([]);

            // Act
            var result = await _builder.Build(request, CancellationToken.None);

            // Assert
            Assert.Equal(0, result.TotalHours);
            Assert.Null(result.TotalDays);
            Assert.Empty(result.UserInquiryDates);
        }

        private static UserInquiryBuilderRequest CreateRequest(TrackingUnit trackingUnit)
        {
            return new UserInquiryBuilderRequest
            {
                UserId = 1,
                InquiryTemplateId = 2,
                IncludeHolidays = true,
                IncludeWeekends = false,
                TrackingUnitType = trackingUnit,
                DateFrom = DateOnly.FromDateTime(DateTime.UtcNow),
                DateTo = DateOnly.FromDateTime(trackingUnit == TrackingUnit.Days
                    ? DateTime.UtcNow.AddDays(1)
                    : DateTime.UtcNow),
                Notes = "Test"
            };
        }
    }
}
