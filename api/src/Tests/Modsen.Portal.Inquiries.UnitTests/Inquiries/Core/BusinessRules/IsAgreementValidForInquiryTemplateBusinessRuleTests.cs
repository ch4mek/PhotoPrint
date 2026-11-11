using MockQueryable.Moq;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.BusinessRules;
using Moq;

namespace Modsen.Portal.Inquiries.UnitTests.Inquiries.Core.BusinessRules;

public class IsAgreementValidForInquiryTemplateBusinessRuleTests
{
    private readonly Mock<IInquiriesDbContext> _dbContextMock = new();

    private readonly IsAgreementValidForInquiryTemplateBusinessRule _rule;
    private readonly CancellationToken _token = CancellationToken.None;

    public IsAgreementValidForInquiryTemplateBusinessRuleTests()
    {
        _rule = new IsAgreementValidForInquiryTemplateBusinessRule(_dbContextMock.Object);
    }

    [Theory]
    [InlineData([InquiryType.HomeOffice])]
    [InlineData([InquiryType.MilitaryEnlistmentOffice])]
    [InlineData([InquiryType.Unknown])]
    [InlineData([InquiryType.SickLeave])]
    [InlineData([InquiryType.OwnExpense])]
    [InlineData([InquiryType.WeekendTransfer])]
    [InlineData([InquiryType.SocialVacation])]
    public async Task IsCorrespondsToBusinessWhenTypeCannotHaveAgreementButItIsProvidedShouldReturnFalse(InquiryType type)
    {
        //Act
        var result = await _rule.IsCorrespondsToBusiness(type, templateId: 1, agreementId: 1, _token);

        //Assert
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData([InquiryType.HomeOffice])]
    [InlineData([InquiryType.MilitaryEnlistmentOffice])]
    [InlineData([InquiryType.Unknown])]
    [InlineData([InquiryType.SickLeave])]
    [InlineData([InquiryType.OwnExpense])]
    [InlineData([InquiryType.WeekendTransfer])]
    [InlineData([InquiryType.SocialVacation])]
    public async Task IsCorrespondsToBusinessWhenTypeCannotHaveAgreementAndItIsNotProvidedShouldReturnTrue(InquiryType type)
    {
        //Act
        var result = await _rule.IsCorrespondsToBusiness(type, templateId: 1, agreementId: null, _token);

        //Assert
        result.Should().BeTrue();
    }

    [Theory]
    [InlineData([InquiryType.Vacation])]
    [InlineData([InquiryType.SickDay])]
    public async Task IsCorrespondsToBusinessWhenTypeMustHaveAgreementAndItIsNotProvidedShouldReturnFalse(InquiryType type)
    {
        //Arrange
        _dbContextMock
            .Setup(context => context.InquiryTemplateAgreements)
            .Returns(new List<InquiryTemplateAgreementEntity>().BuildMockDbSet().Object);

        //Act
        var result = await _rule.IsCorrespondsToBusiness(type, templateId: 1, agreementId: null, _token);

        //Assert
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData([InquiryType.Vacation])]
    [InlineData([InquiryType.SickDay])]
    public async Task IsCorrespondsToBusinessWhenTypeMustHaveAgreementAndNotValidProvidedShouldReturnFalse(InquiryType type)
    {
        //Arrange
        _dbContextMock
            .Setup(context => context.InquiryTemplateAgreements)
            .Returns(new List<InquiryTemplateAgreementEntity>().BuildMockDbSet().Object);

        //Act
        var result = await _rule.IsCorrespondsToBusiness(type, templateId: 1, agreementId: 2, _token);

        //Assert
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData([InquiryType.Vacation])]
    [InlineData([InquiryType.SickDay])]
    public async Task IsCorrespondsToBusinessWhenTypeMustHaveAgreementAndValidProvidedShouldReturnTrue(InquiryType type)
    {
        //Arrange
        var agreementId = 2;
        var templateId = 1;

        _dbContextMock
            .Setup(context => context.InquiryTemplateAgreements)
            .Returns(new List<InquiryTemplateAgreementEntity>()
            {
                new()
                {
                    Id = agreementId,
                    InquiryTemplateId = templateId, 
                    InquiryTemplate = new InquiryTemplateEntity
                    {
                        InquiryType = type,
                        Name = "test",
                    }
                }
            }.BuildMockDbSet().Object);

        //Act
        var result = await _rule.IsCorrespondsToBusiness(type, templateId, agreementId, _token);

        //Assert
        result.Should().BeTrue();
    }
}
