using MockQueryable.Moq;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;
using Modsen.Portal.Inquiries.Requests.Assigns.Core.Calculators;
using Modsen.Portal.Inquiries.Requests.Dto;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.CoreDto;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.Resolvers;
using Moq;

namespace Modsen.Portal.Inquiries.UnitTests.Inquiries.Core.Resolvers;

public class HistoricalRemainingBalanceDaysHoursResolverTests
{
    private readonly HistoricalRemainingBalanceDaysHoursResolver _resolver;
    private readonly Mock<IInquiriesDbContext> _inquiriesDbContext = new();
    private readonly Mock<IUserAgreementInquiryAvailableDaysCalculator> _availableDaysCalculator = new();
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public HistoricalRemainingBalanceDaysHoursResolverTests()
    {
        _resolver = new(_inquiriesDbContext.Object, _availableDaysCalculator.Object);
    }

    private readonly List<UserInquiryEntity> _userInquiries =
    [
        new()
        {
            UserId = 1,
            TotalAccountantDays = 14,
            TotalDays = 16,
            TotalHours = null,
            Status = UserInquiryStatus.Approved,
            AgreementTemplateId = null,
            InquiryTemplateId = 1,
            InquiryTemplate = new() { Id = 1, Name = "Name", TrackingUnitType = TrackingUnit.Days }
        },

        new()
        {
            UserId = 1,
            TotalAccountantDays = 10,
            TotalDays = 16,
            TotalHours = null,
            Status = UserInquiryStatus.Approved,
            AgreementTemplateId = 1,
            InquiryTemplateId = 2,
            InquiryTemplate = new() { Id = 2, Name = "Name", TrackingUnitType = TrackingUnit.Days }
        },

        new()
        {
            UserId = 1,
            TotalAccountantDays = null,
            TotalDays = null,
            TotalHours = 7,
            Status = UserInquiryStatus.Approved,
            AgreementTemplateId = null,
            InquiryTemplateId = 3,
            InquiryTemplate = new() { Id = 3, Name = "Name", TrackingUnitType = TrackingUnit.Hours }
        }
    ];



    [Fact]
    public async Task ResolveWhenRejectedInquiryInDaysWithoutAgreementShouldSetBalanceDays()
    {
        // Arrange
        var inquiryId = 1;

        var userInquiry = new UserInquiryEntity
        {
            Id = inquiryId,
            Status = UserInquiryStatus.Rejected,
            AgreementTemplateId = null,
            InquiryTemplateId = 1,
            InquiryTemplate = new InquiryTemplateEntity
            {
                Name = "Vacation",
                TrackingUnitType = TrackingUnit.Days
            }
        };

        _inquiriesDbContext
            .Setup(x => x.UserInquiries)
            .Returns(new List<UserInquiryEntity> { userInquiry }.BuildMockDbSet().Object);

        // Act
        await _resolver.ResolveAsync(userInquiry, _cancellationToken);

        // Arrange
        userInquiry.HistoricalBalanceDays.Should().Be(0);
        userInquiry.HistoricalBalanceHours.Should().BeNull();
        userInquiry.HistoricalRemainingDays.Should().BeNull();
    }

    [Fact]
    public async Task ResolveWhenRejectedInquiryInHoursWithoutAgreementShouldSetBalanceHours()
    {
        // Arrange
        var inquiryId = 1;

        var userInquiry = new UserInquiryEntity
        {
            Id = inquiryId,
            Status = UserInquiryStatus.Rejected,
            AgreementTemplateId = null,
            InquiryTemplateId = 1,
            InquiryTemplate = new InquiryTemplateEntity
            {
                Name = "Vacation",
                TrackingUnitType = TrackingUnit.Hours
            }
        };

        _inquiriesDbContext
            .Setup(x => x.UserInquiries)
            .Returns(new List<UserInquiryEntity> { userInquiry }.BuildMockDbSet().Object);

        // Act
        await _resolver.ResolveAsync(userInquiry, _cancellationToken);

        // Arrange
        userInquiry.HistoricalBalanceDays.Should().BeNull();
        userInquiry.HistoricalRemainingDays.Should().BeNull();
        userInquiry.HistoricalBalanceHours.Should().Be(0);
    }

    [Fact]
    public async Task ResolveWhenRejectedInquiryWithAgreementShouldSetRemainingDays()
    {
        // Arrange
        var inquiryId = 1;
        var availableDays = 13m;
        var days = 3m;

        var userInquiry = new UserInquiryEntity
        {
            Id = inquiryId,
            Status = UserInquiryStatus.Rejected,
            AgreementTemplateId = 1,
            InquiryTemplateId = 1,
            InquiryTemplate = new InquiryTemplateEntity
            {
                Name = "Vacation",
                TrackingUnitType = TrackingUnit.Days
            },
            TotalAccountantDays = days
        };

        _inquiriesDbContext
            .Setup(x => x.UserInquiries)
            .Returns(new List<UserInquiryEntity> { userInquiry }.BuildMockDbSet().Object);

        _availableDaysCalculator
            .Setup(x => x.CalculateAsync(userInquiry.UserId, userInquiry.InquiryTemplateId, _cancellationToken))
            .ReturnsAsync(availableDays);

        // Act
        await _resolver.ResolveAsync(userInquiry, _cancellationToken);

        // Arrange
        userInquiry.HistoricalBalanceDays.Should().BeNull();
        userInquiry.HistoricalBalanceHours.Should().BeNull();
        userInquiry.HistoricalRemainingDays.Should().Be(availableDays + days);
    }

    [Fact]
    public async Task ResolveWhenApprovedInquiryInDaysWithoutAgreementShouldSetBalanceDays()
    {
        // Arrange
        var days = 25;

        var userInquiry = new UserInquiryEntity
        {
            UserId = 1,
            InquiryTemplateId = 1,
            Status = UserInquiryStatus.Approved,
            InquiryTemplate = new InquiryTemplateEntity
            {
                Id = 1,
                Name = "Name",
                InquiryType = InquiryType.OwnExpense,
                TrackingUnitType = TrackingUnit.Days
            },
            TotalAccountantDays = days
        };

        _inquiriesDbContext.SetupGet(x => x.UserInquiries)
            .Returns(_userInquiries.BuildMockDbSet().Object);

        // Act
        await _resolver.ResolveAsync(userInquiry, _cancellationToken);

        // Assert
        userInquiry.HistoricalRemainingDays.Should().BeNull();
        userInquiry.HistoricalBalanceHours.Should().BeNull();
        userInquiry.HistoricalBalanceDays.Should().Be(days + _userInquiries[0].TotalAccountantDays);
    }

    [Fact]
    public async Task ResolveWhenApprovedInquiryInHoursWithoutAgreementShouldSetBalanceHours()
    {
        // Arrange
        var hours = 8;

        var userInquiry = new UserInquiryEntity
        {
            UserId = 1,
            InquiryTemplateId = 3,
            Status = UserInquiryStatus.Approved,
            InquiryTemplate = new InquiryTemplateEntity
            {
                Id = 3,
                Name = "Name",
                InquiryType = InquiryType.OwnExpense,
                TrackingUnitType = TrackingUnit.Hours
            },
            TotalHours = hours,
        };

        _inquiriesDbContext.SetupGet(x => x.UserInquiries)
            .Returns(_userInquiries.BuildMockDbSet().Object);

        // Act
        await _resolver.ResolveAsync(userInquiry, _cancellationToken);

        // Assert
        userInquiry.HistoricalRemainingDays.Should().BeNull();
        userInquiry.HistoricalBalanceHours.Should().Be(hours + _userInquiries[2].TotalHours);
        userInquiry.HistoricalBalanceDays.Should().BeNull();
    }

    [Fact]
    public async Task ResolveWhenApprovedInquiryWithAgreementShouldSetRemainingDays()
    {
        // Arrange
        var days = 25m;

        var userInquiry = new UserInquiryEntity
        {
            UserId = 1,
            AgreementTemplateId = 1,
            InquiryTemplateId = 2,
            Status = UserInquiryStatus.Approved,
            InquiryTemplate = new InquiryTemplateEntity
            {
                Id = 2,
                Name = "Name",
                InquiryType = InquiryType.Vacation,
                TrackingUnitType = TrackingUnit.Days
            },
            TotalAccountantDays = 1m
        };

        var userAgreement = new UserAgreementEntity
        {
            UserId = userInquiry.UserId,
            InquiryAgreementTemplateId = userInquiry.AgreementTemplateId!.Value
        };

        _inquiriesDbContext.SetupGet(x => x.UserInquiries)
            .Returns(_userInquiries.BuildMockDbSet().Object);

        _availableDaysCalculator
            .Setup(x => x.CalculateAsync(userInquiry.UserId, userInquiry.InquiryTemplateId, _cancellationToken))
            .ReturnsAsync(days);

        // Act
        await _resolver.ResolveAsync(userInquiry, _cancellationToken);

        // Assert
        userInquiry.HistoricalRemainingDays.Should().Be(days);
        userInquiry.HistoricalBalanceHours.Should().BeNull();
        userInquiry.HistoricalBalanceDays.Should().BeNull();
    }

    [Fact]
    public async Task ResolveWhenOpenInquiryInDaysWithoutAgreementShouldSetBalanceDays()
    {
        // Arrange
        var userInquiry = new UserInquiryCoreDto
        {
            InquiryUserId = 1,
            AgreementTemplateId = 1,
            InquiryTemplateId = 1,
            InquiryTemplateName = "Name",
            InquiryType = InquiryType.OwnExpense.ToString(),
            CreatedByUser = new InquiriesUserDto { Email = "1", FirstName = "First", LastName = "Last" },
            CreatedForUser = new InquiriesUserDto { Email = "1", FirstName = "First", LastName = "Last" },
            Status = UserInquiryStatus.Open,
            TrackingUnitType = TrackingUnit.Days,
            TotalAccountantDays = 1m
        };

        _inquiriesDbContext.SetupGet(x => x.UserInquiries)
            .Returns(_userInquiries.BuildMockDbSet().Object);

        // Act
        await _resolver.ResolveAsync(userInquiry, _cancellationToken);

        // Assert
        userInquiry.HistoricalRemainingDays.Should().BeNull();
        userInquiry.HistoricalBalanceHours.Should().BeNull();
        userInquiry.HistoricalBalanceDays.Should().Be(_userInquiries[0].TotalAccountantDays);
    }

    [Fact]
    public async Task ResolveWhenOpenInquiryInHoursWithoutAgreementShouldSetBalanceHours()
    {
        // Arrange
        var userInquiry = new UserInquiryCoreDto
        {
            InquiryUserId = 1,
            InquiryTemplateId = 3,
            InquiryTemplateName = "Name",
            InquiryType = InquiryType.OwnExpense.ToString(),
            CreatedByUser = new InquiriesUserDto { Email = "1", FirstName = "First", LastName = "Last" },
            CreatedForUser = new InquiriesUserDto { Email = "1", FirstName = "First", LastName = "Last" },
            Status = UserInquiryStatus.Open,
            TrackingUnitType = TrackingUnit.Hours,
            TotalHours = 8m,
        };

        _inquiriesDbContext.SetupGet(x => x.UserInquiries)
            .Returns(_userInquiries.BuildMockDbSet().Object);

        // Act
        await _resolver.ResolveAsync(userInquiry, _cancellationToken);

        // Assert
        userInquiry.HistoricalRemainingDays.Should().BeNull();
        userInquiry.HistoricalBalanceHours.Should().Be(_userInquiries[2].TotalHours);
        userInquiry.HistoricalBalanceDays.Should().BeNull();
    }

    [Fact]
    public async Task ResolveWhenOpenInquiryWithAgreementShouldSetRemainingDays()
    {
        // Arrange
        var days = 25m;

        var userInquiry = new UserInquiryCoreDto
        {
            InquiryUserId = 1,
            AgreementTemplateId = 1,
            IsAgreement = true,
            InquiryTemplateId = 2,
            InquiryTemplateName = "Name",
            InquiryType = nameof(InquiryType.Vacation),
            CreatedByUser = new InquiriesUserDto { Email = "1", FirstName = "First", LastName = "Last" },
            CreatedForUser = new InquiriesUserDto { Email = "1", FirstName = "First", LastName = "Last" },
            Status = UserInquiryStatus.Open,
            TrackingUnitType = TrackingUnit.Days,
            TotalAccountantDays = 1m
        };

        _inquiriesDbContext.SetupGet(x => x.UserInquiries)
            .Returns(_userInquiries.BuildMockDbSet().Object);

        _availableDaysCalculator
            .Setup(x => x.CalculateAsync(userInquiry.InquiryUserId, userInquiry.InquiryTemplateId, _cancellationToken))
            .ReturnsAsync(days);

        // Act
        await _resolver.ResolveAsync(userInquiry, _cancellationToken);

        // Assert
        userInquiry.HistoricalRemainingDays.Should().Be(days);
        userInquiry.HistoricalBalanceHours.Should().BeNull();
        userInquiry.HistoricalBalanceDays.Should().BeNull();
    }
}
