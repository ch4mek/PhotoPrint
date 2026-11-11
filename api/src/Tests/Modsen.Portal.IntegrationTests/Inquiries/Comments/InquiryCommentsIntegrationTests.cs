using System.Net;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Inquiries;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;

namespace Modsen.Portal.IntegrationTests.Inquiries.Comments;

[Collection("InquiriesCollection")]
public class InquiryCommentsIntegrationTests : InquiriesUsageTestsBase
{
    private const string CommentJson = "[{\"type\":\"p\",\"children\":[{\"text\":\"Comment\"}]}]";

    public InquiryCommentsIntegrationTests(IntegrationTestFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task GetCommentsWhenInquiryNotExistsResponseWithNotFound()
    {
        // Arrange
        using var client = _factory.CreateClient();

        var inquiry = await PrepareInquiries(client);

        var created = await client.CreateInquiryCommentAsync(inquiry.Id, CommentJson);

        // Act
        var comments = await client.GetInquiryCommentsAsHttpResponseAsync(inquiry.Id + 1);

        // Assert
        comments!.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetCommentsWhenUserHasNoAccessResponseWithNotFound()
    {
        // Arrange
        using var client = _factory.CreateClient();

        var inquiry = await PrepareInquiries(client);

        var created = await client.CreateInquiryCommentAsync(inquiry.Id, CommentJson);

        await client.AuthAsEmployee();

        // Act
        var comments = await client.GetInquiryCommentsAsHttpResponseAsync(inquiry.Id + 1);

        // Assert
        comments!.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateCommentWhenRequestIsValidResponseWithOk()
    {
        // Arrange
        using var client = _factory.CreateClient();

        var inquiry = await PrepareInquiries(client);

        // Act
        var created = await client.CreateInquiryCommentAsync(inquiry.Id, CommentJson);

        // Assert
        created!.Comment.Should().Be(CommentJson);
    }

    [Fact]
    public async Task CreateCommentWhenInquiryNotExistsResponseWithNotFoundStatus()
    {
        // Arrange
        using var client = _factory.CreateClient();

        var inquiry = await PrepareInquiries(client);

        // Act
        var response = await client.CreateInquiryCommentAsHttpResponseAsync(inquiry.Id + 1, CommentJson);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateCommentWhenUserHasNoAccessResponseWithForbiddenStatus()
    {
        // Arrange
        using var client = _factory.CreateClient();

        var inquiry = await PrepareInquiries(client);

        await client.AuthAsEmployee();

        // Act
        var response = await client.CreateInquiryCommentAsHttpResponseAsync(inquiry.Id, CommentJson);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
