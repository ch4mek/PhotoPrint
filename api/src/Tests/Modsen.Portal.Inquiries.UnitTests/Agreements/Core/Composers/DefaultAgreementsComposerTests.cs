using MockQueryable.Moq;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;
using Modsen.Portal.Inquiries.Requests.Agreements.Core.Composers;
using Moq;

namespace Modsen.Portal.Inquiries.UnitTests.Agreements.Core.Composers;

public class DefaultAgreementsComposerTests
{
    private readonly IDefaultAgreementsComposer _composer;
    private readonly Mock<IInquiriesDbContext> _inquiriesDbContextMock = new();
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public DefaultAgreementsComposerTests()
    {
        _composer = new DefaultAgreementsComposer(
            _inquiriesDbContextMock.Object);
    }

    [Fact]
    public async Task ComposeWhenDefaultAgreementsExistReturnsCorrectUserAgreements()
    {
        // Arrange
        var userId = 3;
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var hireDate = today.AddMonths(-5);
        var fireDate = today.AddMonths(6);

        var inquiryTemplates = new List<InquiryTemplateEntity>
        {
            new()
            {
                Id = 1,
                InquiryType = InquiryType.SickDay,
                IsDefault = true,
                IsActive = true,
                Name = "test",
                InquiryTemplateAgreements =
                [
                    new()
                    {
                        Id = 1,
                        Name = "5 days",
                        Type = AgreementType.SickDaysAgreement,
                        DaysPerContractYear = 5,
                        IsDefault = true,
                    },
                ]
            },
            new()
            {
                Id = 2,
                InquiryType = InquiryType.Vacation,
                IsDefault = true,
                IsActive = true,
                Name = "test",
                InquiryTemplateAgreements =
                [
                    new()
                    {
                        Id = 2,
                        Name = "25 days",
                        Type = AgreementType.ContractVacationAgreement,
                        DaysPerContractYear = 25,
                        IsDefault = true,
                    },
                ]
            }
        };

        _inquiriesDbContextMock
            .Setup(x => x.InquiryTemplateAgreements)
            .Returns(inquiryTemplates.SelectMany(x => x.InquiryTemplateAgreements).ToList().BuildMockDbSet().Object);
        _inquiriesDbContextMock
            .Setup(x => x.InquiryTemplates)
            .Returns(inquiryTemplates.BuildMockDbSet().Object);
        _inquiriesDbContextMock
            .Setup(x => x.UserAgreements)
            .Returns(new List<UserAgreementEntity>().BuildMockDbSet().Object);

        // Act
        var result = await _composer.Compose(userId, hireDate, _cancellationToken);

        // Assert
        result.Count.Should().Be(2);
        result.Should().ContainSingle(x => x.InquiryAgreementTemplateId == 1)
            .Which.Should().BeEquivalentTo(new UserAgreementEntity
            {
                UserId = userId,
                DateFrom = hireDate,
                DateTo = null,
                InquiryAgreementTemplateId = 1
            }, options => options.Excluding(x => x.Id).Excluding(x => x.InquiryTemplateAgreement));
        result.Should().ContainSingle(x => x.InquiryAgreementTemplateId == 2)
            .Which.Should().BeEquivalentTo(new UserAgreementEntity
            {
                UserId = userId,
                DateFrom = hireDate,
                DateTo = null,
                InquiryAgreementTemplateId = 2
            }, options => options.Excluding(x => x.Id).Excluding(x => x.InquiryTemplateAgreement));
    }

    [Fact]
    public async Task ComposeWhenNoDefaultAgreementsExistReturnsEmptyList()
    {
        // Arrange
        var userId = 3;
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var hireDate = today.AddMonths(-5);
        var fireDate = today.AddMonths(6);

        _inquiriesDbContextMock
            .Setup(x => x.InquiryTemplates)
            .Returns(new List<InquiryTemplateEntity>().BuildMockDbSet().Object);
        _inquiriesDbContextMock
            .Setup(x => x.InquiryTemplateAgreements)
            .Returns(new List<InquiryTemplateAgreementEntity>().BuildMockDbSet().Object);
        _inquiriesDbContextMock
            .Setup(x => x.UserAgreements)
            .Returns(new List<UserAgreementEntity>().BuildMockDbSet().Object);

        // Act
        var result = await _composer.Compose(userId, hireDate, _cancellationToken);

        // Assert
        result.Should().BeEmpty();
    }


    [Fact]
    public async Task ComposeWhenUserAgreementsExistReturnsCorrectUserAgreements()
    {
        // Arrange
        var userId = 3;
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var hireDate = today.AddMonths(-5);
        var fireDate = today.AddMonths(6);

        var inquiryTemplates = new List<InquiryTemplateEntity>
        {
            new()
            {
                Id = 1,
                InquiryType = InquiryType.SickDay,
                IsDefault = true,
                IsActive = true,
                Name = "test",
                InquiryTemplateAgreements =
                [
                    new()
                    {
                        Id = 1,
                        Name = "5 days",
                        Type = AgreementType.SickDaysAgreement,
                        DaysPerContractYear = 5,
                        IsDefault = true,
                        InquiryTemplateId = 1,
                    },
                ]
            },
            new()
            {
                Id = 2,
                InquiryType = InquiryType.Vacation,
                IsDefault = true,
                IsActive = true,
                Name = "test",
                InquiryTemplateAgreements =
                [
                    new()
                    {
                        Id = 2,
                        Name = "25 days",
                        Type = AgreementType.ContractVacationAgreement,
                        DaysPerContractYear = 25,
                        IsDefault = true,
                        InquiryTemplateId = 2,
                    },
                ]
            }
        };

        var userAgreements = new List<UserAgreementEntity>()
        {
            new()
            {
                UserId = userId,
                InquiryAgreementTemplateId = 2,
                InquiryTemplateAgreement = inquiryTemplates[1].InquiryTemplateAgreements[0],
            }
        };

        _inquiriesDbContextMock
            .Setup(x => x.InquiryTemplateAgreements)
            .Returns(inquiryTemplates.SelectMany(x => x.InquiryTemplateAgreements).ToList().BuildMockDbSet().Object);
        _inquiriesDbContextMock
            .Setup(x => x.InquiryTemplates)
            .Returns(inquiryTemplates.BuildMockDbSet().Object);
        _inquiriesDbContextMock
            .Setup(x => x.UserAgreements)
            .Returns(userAgreements.BuildMockDbSet().Object);

        // Act
        var result = await _composer.Compose(userId, hireDate, _cancellationToken);

        // Assert
        result.Count.Should().Be(1);
        result.Should().ContainSingle(x => x.InquiryAgreementTemplateId == 1)
            .Which.Should().BeEquivalentTo(new UserAgreementEntity
            {
                UserId = userId,
                DateFrom = hireDate,
                DateTo = null,
                InquiryAgreementTemplateId = 1
            }, options => options.Excluding(x => x.Id).Excluding(x => x.InquiryTemplateAgreement));
        result.Should().NotContain(x => x.InquiryAgreementTemplateId == 2);
    }
}
