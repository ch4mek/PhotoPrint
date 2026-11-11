using MockQueryable.Moq;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;
using Modsen.Portal.Inquiries.Requests.Agreements.Requests;
using Moq;

namespace Modsen.Portal.Inquiries.UnitTests.Agreements.Requests;

public class DeleteAgreementTemplateRequestTests
{
    private readonly Mock<IInquiriesDbContext> _inquiriesDbContextMock = new();
    private readonly Mock<IClockService> _clockServiceMock = new();
    private readonly DeleteAgreementTemplateRequest _handler;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public DeleteAgreementTemplateRequestTests()
    {
        _handler = new DeleteAgreementTemplateRequest(
            _inquiriesDbContextMock.Object,
            _clockServiceMock.Object);
    }

    [Fact]
    public async Task HandleWhenRequestIsValidShouldReturnDeletedId()
    {
        // Arrange
        var templateAgreement = new InquiryTemplateAgreementEntity()
        {
            Id = 10,
            Name = "Test agreement",
            DaysPerContractYear = 25,
            IsDefault = true,
            Type = 0,
            InquiryTemplateId = 7,
        };

        _inquiriesDbContextMock
            .Setup(context => context.InquiryTemplateAgreements)
            .Returns(new List<InquiryTemplateAgreementEntity>
            {
                    templateAgreement
            }.BuildMockDbSet().Object);

        var userAgreement = new UserAgreementEntity()
        {
            Id = 1,
            UserId = 7,
            InquiryAgreementTemplateId = 10,
            DateFrom = new DateOnly(),
            DateTo = new DateOnly(),
        };

        _inquiriesDbContextMock
            .Setup(context => context.UserAgreements)
            .Returns(new List<UserAgreementEntity>
            {
                    userAgreement
            }.BuildMockDbSet().Object);

        // Act
        var result = await _handler.Handle(
                new DeleteAgreementTemplateRequest.Command(userAgreement.InquiryAgreementTemplateId),
                _cancellationToken);

        // Assert
        result.Should().Be(userAgreement.InquiryAgreementTemplateId);
        templateAgreement.IsDeleted.Should().BeTrue();  
        _inquiriesDbContextMock.Verify(
            x => x.SaveChangesAsync(_cancellationToken),
            Times.Once);
    }
}
