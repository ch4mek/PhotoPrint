using MockQueryable.Moq;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;
using Modsen.Portal.Inquiries.Requests.InquiryTemplates.Requests;
using Moq;

namespace Modsen.Portal.Inquiries.UnitTests.InquiryTemplates.Requests;

public class GetInquiryTemplatesLookupRequestTests
{
    private readonly Mock<IInquiriesDbContext> _mockDbContext;
    private readonly GetInquiryTemplatesLookupRequest _handler;

    public GetInquiryTemplatesLookupRequestTests()
    {
        _mockDbContext = new Mock<IInquiriesDbContext>();
        _handler = new GetInquiryTemplatesLookupRequest(_mockDbContext.Object);
    }

    [Fact]
    public async Task HandleReturnsActiveInquiryTemplates()
    {
        // Arrange
        var inquiryTemplates = new List<InquiryTemplateEntity>
        {
            new() { Id = 1, Name = "Template 1", IsActive = true, InquiryTypeOrderWeight = 200,},
            new() { Id = 2, Name = "Template 2", IsActive = false, InquiryTypeOrderWeight = 500 },
            new() { Id = 3, Name = "Template 3", IsActive = true, InquiryTypeOrderWeight = 100 }
        };

        _mockDbContext
            .Setup(x => x.InquiryTemplates)
            .Returns(inquiryTemplates.BuildMockDbSet().Object);

        var command = new GetInquiryTemplatesLookupRequest.Command();

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Count.Should().Be(2);
        result.Should().OnlyContain(template => template.Id == 1 || template.Id == 3);
    }
}
