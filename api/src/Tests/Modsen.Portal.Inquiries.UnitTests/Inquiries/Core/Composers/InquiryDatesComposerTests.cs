using MockQueryable.Moq;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.Composers;
using Moq;

namespace Modsen.Portal.Inquiries.UnitTests.Inquiries.Core.Composers;

public class InquiryDatesComposerTests
{
    private readonly IInquiryDatesComposer _inquiryDatesComposer;
    private readonly CancellationToken _token = CancellationToken.None;

    private readonly List<CalendarExclusionEntity> _calendarExclusions =
        [
            new()
            {
                Id = 1,
                Date = new DateOnly(2025, 2, 10),
                DateType = DateType.Holiday,
                CountryId = 1,
                Name = "Test holiday"
            },
            new()
            {
                Id = 2,
                Date = new DateOnly(2025, 3, 10),
                DateType = DateType.Saturday,
                CountryId = 1,
                Name = "Test transfer"
            },
            new()
            {
                Id = 3,
                Date = new DateOnly(2025, 3, 15),
                DateType = DateType.WorkingDay,
                CountryId = 1,
                Name = "Test transfer"
            }
        ];

    public InquiryDatesComposerTests()
    {
        var inquiriesDbContextMock = new Mock<IInquiriesDbContext>();

        inquiriesDbContextMock
            .Setup(x => x.CalendarExclusions)
            .Returns(_calendarExclusions.BuildMockDbSet().Object);

        _inquiryDatesComposer = new InquiryDatesComposer(inquiriesDbContextMock.Object);

    }

    [Fact]
    public async Task CreateInquiryDatesWithHolidayExclusionShouldCreateInquiryDatesWithHolidayExclusion()
    {
        // Arrange
        var request = new InquiryDatesComposerRequest
        {
            DateFrom = new DateOnly(2025, 2, 9),
            DateTo = new DateOnly(2025, 2, 11),
            CountryId = 1,
            RequestedDays = 1
        };

        var createdInquiryDates = new List<UserInquiryDateEntity>
        {
            new()
            {
                Date = new DateOnly(2025, 2, 9),
                RequestedDays = 1,
                DateType = DateType.Sunday
            },
            new()
            {
                Date = new DateOnly(2025, 2, 10),
                RequestedDays = 1,
                DateType = DateType.Holiday,
                CalendarExclusionId = 1
            },
            new()
            {
                Date = new DateOnly(2025, 2, 11),
                RequestedDays = 1,
                DateType = DateType.WorkingDay
            }
        };

        // Act
        var result = await _inquiryDatesComposer.Compose(request, _token);

        // Assert
        result.Should().BeEquivalentTo(createdInquiryDates);
    }

    [Fact]
    public async Task CreateInquiryDatesWithTransferExclusionShouldCreateInquiryDatesWithTransferExclusion()
    {
        // Arrange
        var request = new InquiryDatesComposerRequest
        {
            DateFrom = new DateOnly(2025, 3, 15),
            DateTo = new DateOnly(2025, 3, 17),
            CountryId = 1,
            RequestedDays = 1
        };

        var createdInquiryDates = new List<UserInquiryDateEntity>
        {
            new()
            {
                Date = new DateOnly(2025, 3, 15),
                RequestedDays = 1,
                DateType = DateType.WorkingDay,
                CalendarExclusionId = 3
            },
            new()
            {
                Date = new DateOnly(2025, 3, 16),
                RequestedDays = 1,
                DateType = DateType.Sunday
            },
            new()
            {
                Date = new DateOnly(2025, 3, 17),
                RequestedDays = 1,
                DateType = DateType.WorkingDay
            }
        };

        // Act
        var result = await _inquiryDatesComposer.Compose(request, _token);

        // Assert
        result.Should().BeEquivalentTo(createdInquiryDates);
    }

    [Fact]
    public async Task CreateInquiryDatesWithoutExclosionsShouldCreateInquiryDatesWithoutExclosions()
    {
        // Arrange
        var request = new InquiryDatesComposerRequest
        {
            DateFrom = new DateOnly(2025, 2, 15),
            DateTo = new DateOnly(2025, 2, 17),
            CountryId = 1,
            RequestedDays = 1
        };

        var createdInquiryDates = new List<UserInquiryDateEntity>
        {
            new()
            {
                Date = new DateOnly(2025, 2, 15),
                RequestedDays = 1,
                DateType = DateType.Saturday
            },
            new()
            {
                Date = new DateOnly(2025, 2, 16),
                RequestedDays = 1,
                DateType = DateType.Sunday
            },
            new()
            {
                Date = new DateOnly(2025, 2, 17),
                RequestedDays = 1,
                DateType = DateType.WorkingDay
            }
        };

        // Act
        var result = await _inquiryDatesComposer.Compose(request, _token);

        // Assert
        result.Should().BeEquivalentTo(createdInquiryDates);
    }

    [Fact]
    public async Task CreateInquiryDatesWithInvalidDatesShouldCreateInquiryDatesWithInvalidDates()
    {
        // Arrange
        var request = new InquiryDatesComposerRequest
        {
            DateFrom = new DateOnly(2025, 2, 17),
            DateTo = new DateOnly(2025, 2, 15),
            CountryId = 1,
            RequestedDays = 1
        };

        // Act
        var action = () => _inquiryDatesComposer.Compose(request, _token);

        // Assert
        await action.Should().
            ThrowAsync<InvalidDataAppException>("It is impossible to create a inquiry with 0 or less days");
    }
}
