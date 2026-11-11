// TODO: FIX TESTS in next story

//using System.Net;
//using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Inquiries;
//using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;

//namespace Modsen.Portal.IntegrationTests.Inquiries.Attachments;

//[Collection("InquiriesCollection")]
//public class InquiryAttachmentsIntegrationTests : InquiriesUsageTestsBase
//{
//    public InquiryAttachmentsIntegrationTests(IntegrationTestFactory factory)
//        : base(factory)
//    {
//    }

//    [Fact]
//    public async Task GetAttachments_ResponseWithOk()
//    {
//        // Arrange
//        using var client = _factory.CreateClient();

//        var inquiry = await PrepareInquiries(client);

//        var createdResponse = await client.CreateAttachmentAsync(inquiry.Id, "file.txt");

//        // Act
//        var attachments = await client.GetAttachmentsAsync(inquiry.Id);

//        // Assert
//        attachments!.Count.Should().Be(1);
//    }

//    [Fact]
//    public async Task GetAttachments_When_InquiryNotExists_ResponseWithOk()
//    {
//        // Arrange
//        using var client = _factory.CreateClient();

//        var inquiry = await PrepareInquiries(client);

//        var createdResponse = await client.CreateAttachmentAsync(inquiry.Id, "file.txt");

//        // Act
//        var attachments = await client.GetAgreementsAsync(inquiry.Id + 1);

//        // Assert
//        attachments!.Count.Should().Be(0);
//    }

//    [Fact]
//    public async Task GetAttachments_When_UserHasNoAccess_ResponseWithOk()
//    {
//        // Arrange
//        using var client = _factory.CreateClient();

//        var inquiry = await PrepareInquiries(client);

//        var createdResponse = await client.CreateAttachmentAsync(inquiry.Id, "file.txt");

//        await client.AuthAsEmployee();

//        // Act
//        var attachments = await client.GetAttachmentsAsync(inquiry.Id);

//        // Assert

//        attachments!.Count.Should().Be(0);
//    }

//    [Fact]
//    public async Task CreateFile_ResponseWithOk()
//    {
//        // Arrange
//        using var client = _factory.CreateClient();

//        var inquiry = await PrepareInquiries(client);

//        // Act
//        var created = await client.CreateAttachmentAsync(inquiry.Id, "file.txt");

//        // Assert
//        created!.FileId.Should().Be("file.txt");
//    }

//    [Fact]
//    public async Task CreateFile_ResponseWithNotFoundStatus()
//    {
//        // Arrange
//        using var client = _factory.CreateClient();

//        var inquiry = await PrepareInquiries(client);

//        // Act
//        var response = await client.CreateAttachmentAsHttpResponseAsync(inquiry.Id + 1, "file.txt");

//        // Assert
//        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
//    }

//    [Fact]
//    public async Task CreateFile_ResponseWithForbiddenStatus()
//    {
//        // Arrange
//        using var client = _factory.CreateClient();

//        var inquiry = await PrepareInquiries(client);

//        await client.AuthAsEmployee();

//        // Act
//        var response = await client.CreateAttachmentAsHttpResponseAsync(inquiry.Id, "file.txt");

//        // Assert
//        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
//    }

//    [Fact]
//    public async Task DeleteFile_ResponseWithOk()
//    {
//        // Arrange
//        using var client = _factory.CreateClient();

//        var inquiry = await PrepareInquiries(client);

//        var attachment = await client.CreateAttachmentAsync(inquiry.Id, "file.txt");

//        // Act
//        var response = await client.DeleteAttachmentAsHttpResponseAsync(inquiry.Id, attachment!.Id);

//        // Assert
//        response.StatusCode.Should().Be(HttpStatusCode.OK);
//    }

//    [Fact]
//    public async Task DeleteComment_ResponseWithNotFoundStatus()
//    {
//        // Arrange
//        using var client = _factory.CreateClient();

//        var inquiry = await PrepareInquiries(client);

//        var attachment = await client.CreateAttachmentAsync(inquiry.Id, "file.txt");

//        // Act
//        var response = await client.DeleteAttachmentAsHttpResponseAsync(inquiry.Id + 1, attachment!.Id);

//        // Assert
//        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
//    }

//    [Fact]
//    public async Task DeleteComment_ResponseWithForbiddenStatus()
//    {
//        // Arrange
//        using var client = _factory.CreateClient();

//        var inquiry = await PrepareInquiries(client);

//        var attachment = await client.CreateAttachmentAsync(inquiry.Id, "file.txt");

//        await client.AuthAsEmployee();

//        // Act
//        var response = await client.DeleteAttachmentAsHttpResponseAsync(inquiry.Id, attachment!.Id);

//        // Assert
//        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
//    }
//}
