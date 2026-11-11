using MockQueryable.Moq;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;
using Modsen.Portal.Inquiries.Requests.Agreements.Core.Composers;
using Moq;

namespace Modsen.Portal.Inquiries.UnitTests.Agreements.Core.Composers;

public class DefaultInquiryTemplatesComposerTests
{
    private readonly IDefaultInquiryTemplatesComposer _composer;
    private readonly Mock<IInquiriesDbContext> _inquiriesDbContextMock = new();
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public DefaultInquiryTemplatesComposerTests()
    {
        _composer = new DefaultInquiryTemplatesComposer(
            _inquiriesDbContextMock.Object);
    }

    [Fact]
    public async Task ComposeWhenDefaultInquiryTemplatesExistReturnsCorrectUserInquiryTemplate()
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
                InquiryType = InquiryType.SickLeave,
                IsDefault = true,
                IsActive = true,
                Name = "test",
            },
            new()
            {
                Id = 2,
                InquiryType = InquiryType.OwnExpense,
                IsDefault = true,
                IsActive = true,
                Name = "test",
            },
            new()
            {
                Id = 3,
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
        };

        _inquiriesDbContextMock
            .Setup(x => x.InquiryTemplateAgreements)
            .Returns(inquiryTemplates.SelectMany(x => x.InquiryTemplateAgreements).ToList().BuildMockDbSet().Object);
        _inquiriesDbContextMock
            .Setup(x => x.InquiryTemplates)
            .Returns(inquiryTemplates.BuildMockDbSet().Object);
        _inquiriesDbContextMock
            .Setup(x => x.UserInquiryTemplates)
            .Returns(new List<UserInquiryTemplateEntity>().BuildMockDbSet().Object);

        // Act
        var result = await _composer.Compose(userId, hireDate, today, _cancellationToken);

        // Assert
        result.Count.Should().Be(2);
        result.Should().ContainSingle(x => x.InquiryTemplateId == 1)
            .Which.Should().BeEquivalentTo(new UserInquiryTemplateEntity
            {
                AssignedToUserId = userId,
                DateFrom = hireDate,
                DateTo = null,
                InquiryTemplateId = 1,
            }, options => options.Excluding(x => x.Id).Excluding(x => x.InquiryTemplate));
        result.Should().ContainSingle(x => x.InquiryTemplateId == 2)
            .Which.Should().BeEquivalentTo(new UserInquiryTemplateEntity
            {
                AssignedToUserId = userId,
                DateFrom = hireDate,
                DateTo = null,
                InquiryTemplateId = 2,
            }, options => options.Excluding(x => x.Id).Excluding(x => x.InquiryTemplate));
    }

    [Fact]
    public async Task ComposeWhenNoDefaultInquiriesExistReturnsEmptyList()
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
            .Setup(x => x.UserInquiryTemplates)
            .Returns(new List<UserInquiryTemplateEntity>().BuildMockDbSet().Object);

        // Act
        var result = await _composer.Compose(userId, hireDate, today, _cancellationToken);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task ComposeWhenExistingInquiryTemplatesExistReturnsCorrectUserInquiryTemplate()
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
                InquiryType = InquiryType.SickLeave,
                IsDefault = true,
                IsActive = true,
                Name = "test",
            },
            new()
            {
                Id = 2,
                InquiryType = InquiryType.OwnExpense,
                IsDefault = true,
                IsActive = true,
                Name = "test",
            },
            new()
            {
                Id = 3,
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
        };

        var userInquiryTemplates = new List<UserInquiryTemplateEntity>
        {
            new()
            {
                AssignedToUserId = userId,
                InquiryTemplateId = 1,
                InquiryTemplate = inquiryTemplates[0],
            }
        };

        _inquiriesDbContextMock
            .Setup(x => x.InquiryTemplateAgreements)
            .Returns(inquiryTemplates.SelectMany(x => x.InquiryTemplateAgreements).ToList().BuildMockDbSet().Object);
        _inquiriesDbContextMock
            .Setup(x => x.InquiryTemplates)
            .Returns(inquiryTemplates.BuildMockDbSet().Object);
        _inquiriesDbContextMock
            .Setup(x => x.UserInquiryTemplates)
            .Returns(userInquiryTemplates.BuildMockDbSet().Object);

        // Act
        var result = await _composer.Compose(userId, hireDate, today, _cancellationToken);

        // Assert
        result.Count.Should().Be(1);
        result.Should().ContainSingle(x => x.InquiryTemplateId == 2)
            .Which.Should().BeEquivalentTo(new UserInquiryTemplateEntity
            {
                AssignedToUserId = userId,
                DateFrom = hireDate,
                DateTo = null,
                InquiryTemplateId = 2,
            }, options => options.Excluding(x => x.Id).Excluding(x => x.InquiryTemplate));
        result.Should().NotContain(x => x.InquiryTemplateId == 1);
    }

}
