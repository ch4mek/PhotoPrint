using System.Net;
using System.Net.Http.Headers;
using System.Net.Mime;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;

namespace Modsen.Portal.IntegrationTests.Identity.Controllers;

[Collection("GeneralCollection")]
public class ApplicationFileControllerIntegrationTests
{
    private readonly IntegrationTestFactory factory;

    public ApplicationFileControllerIntegrationTests(
        IntegrationTestFactory factory)
    {
        this.factory = factory;
    }

    [Fact]
    public async Task GetFileWhenFileExistsReturnFile()
    {
        // Arrange
        using var client = factory.CreateClient();
        await client.AuthAsCeo();

        var content = await File.ReadAllBytesAsync("./Identity/Controllers/images/cat.jpg");
        using var memory = new MemoryStream(content);
        using var imageContent = new StreamContent(memory);
        imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Image.Jpeg);

        using var form = new MultipartFormDataContent();
        form.Add(imageContent, "File", "cat.jpg");

        var postResponse = await client.CreateApplicationFile(form);

        //Act
        var fileResponse = Array.Empty<byte>();
        if ((postResponse?.FileId).IsNullOrWhiteSpace() is false)
        {
            fileResponse = await client.GetApplicationFile(postResponse.FileId);
        }

        //Assert
        fileResponse.SequenceEqual(content).Should().BeTrue();
    }

    [Fact]
    public async Task GetFileWhenFileDoesNotExistReturnNotFound()
    {
        // Arrange
        using var client = factory.CreateClient();
        await client.AuthAsCeo();
        const string nonExistingFileId = "9999";

        //Act
        var response = await client.GetApplicationFileAsHttpMessage(nonExistingFileId);

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UploadFileWhenFileIsValidReturnFile()
    {
        // Arrange
        using var client = factory.CreateClient();
        await client.AuthAsCeo();

        using var content = new MemoryStream("This is a dummy file"u8.ToArray());
        using var imageContent = new StreamContent(content);
        imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Image.Jpeg);

        using var form = new MultipartFormDataContent();
        form.Add(imageContent, "File", "dummy.jpg");

        //Act
        var response = await client.CreateApplicationFileAsHttpMessage(form);

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UploadFileWhenFileExtensionIsNotValidReturnBadRequest()
    {
        // Arrange
        using var client = factory.CreateClient();
        await client.AuthAsCeo();

        using var content = new MemoryStream("This is a dummy file"u8.ToArray());
        using var imageContent = new StreamContent(content);
        imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Image.Avif);

        using var form = new MultipartFormDataContent();
        form.Add(imageContent, "File", "dummy.jpg");

        //Act
        var response = await client.CreateApplicationFileAsHttpMessage(form);

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UploadFileWhenFileIsEmptyReturnBadRequest()
    {
        // Arrange
        using var client = factory.CreateClient();
        await client.AuthAsCeo();

        using var content = new MemoryStream([]);
        using var imageContent = new StreamContent(content);
        imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Image.Jpeg);

        using var form = new MultipartFormDataContent();
        form.Add(imageContent, "File", "dummy.jpg");

        //Act
        var response = await client.CreateApplicationFileAsHttpMessage(form);

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetFileWhenMerchRequestedSmallAndResizedNotReady_ReturnsOriginalWithFallbackHeader()
    {
        // Arrange
        using var client = factory.CreateClient();
        await client.AuthAsCeo();

        var content = await File.ReadAllBytesAsync("./Identity/Controllers/images/cat.jpg");

        using var memory = new MemoryStream(content);
        using var imageContent = new StreamContent(memory);
        imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Image.Jpeg);

        using var form = new MultipartFormDataContent();
        form.Add(imageContent, "File", "cat.jpg");

        var postResponse = await client.CreateApplicationFile(form, ApplicationFilePurpose.Merch);

        postResponse.Should().NotBeNull();
        postResponse!.FileId.Should().NotBeNullOrWhiteSpace();

        // Act: immediately request small
        var getResponse = await client.GetApplicationFileAsHttpMessage(postResponse.FileId, ApplicationFileSize.Small);

        // Assert
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var bytes = await getResponse.Content.ReadAsByteArrayAsync();

        bytes.Should().NotBeEmpty();
        bytes.SequenceEqual(content).Should().BeTrue();

        // Проверяем fallback контракт (если ты добавил эти хедеры)
        getResponse.Headers.TryGetValues("X-Image-Fallback", out var fallbackHeader).Should().BeTrue();
        fallbackHeader!.Single().Should().Be("original");

        getResponse.Headers.CacheControl.Should().NotBeNull();
        getResponse.Headers.CacheControl!.NoStore.Should().BeTrue();
    }
}
