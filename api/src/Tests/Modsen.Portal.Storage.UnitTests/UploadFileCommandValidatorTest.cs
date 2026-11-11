using System.Net.Mime;
using Modsen.Portal.Core;
using Modsen.Portal.Storage.Requests.Files.Requests;

namespace Modsen.Portal.Storage.UnitTests;

public class UploadFileCommandValidatorTest
{
    [Theory]
    [InlineData("application/pdf")]
    [InlineData("application/msword")]
    [InlineData("application/vnd.ms-excel")]
    public void ValidateWhenFileSizeIsGreaterThanHalfMbShouldReturnTrue(string contentType)
    {
        // Arrange
        var command = new UploadFileRequest.Command
        {
            FileContent = [1],
            FileName = "test.png",
            ContentType = contentType,
            FileSize = 1024 * 1024 / 2 + 100,
            UserId = 1,
            AvailableTill = null,
            FileSource = FileSource.System
        };

        var validator = new UploadFileRequest.Validator();

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void ValidateWhenFileSizeIsGreaterThanHalfMbShouldReturnFalse()
    {
        // Arrange
        var command = new UploadFileRequest.Command
        {
            FileContent = [1],
            FileName = "test.png",
            ContentType = MediaTypeNames.Image.Png,
            FileSize = 1024 * 1024 * 10 + 1,
            UserId = 1,
            AvailableTill = null,
            FileSource = FileSource.System
        };

        var validator = new UploadFileRequest.Validator();
        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void ValidateWhenFileSizeIsLessThanZeroShouldReturnFalse()
    {
        // Arrange
        var command = new UploadFileRequest.Command
        {
            FileContent = [1],
            FileName = "test.png",
            ContentType = MediaTypeNames.Image.Png,
            FileSize = -1,
            UserId = 1,
            AvailableTill = null,
            FileSource = FileSource.System
        };

        var validator = new UploadFileRequest.Validator();

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void ValidateWhenContentTypeIsNotSupportedShouldReturnFalse()
    {
        // Arrange
        var command = new UploadFileRequest.Command
        {
            FileContent = [1],
            FileName = "test.png",
            ContentType = "testInvalidContentType",
            FileSize = 1024 * 1024 / 2,
            UserId = 1,
            AvailableTill = null,
            FileSource = FileSource.System
        };

        var validator = new UploadFileRequest.Validator();

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void ValidateWhenCommandIsValidShouldReturnTrue()
    {
        // Arrange
        var command = new UploadFileRequest.Command
        {
            FileContent = [1],
            FileName = "test.png",
            ContentType = MediaTypeNames.Image.Png,
            FileSize = 1024 * 1024 / 2,
            UserId = 1,
            AvailableTill = null,
            FileSource = FileSource.System
        };

        var validator = new UploadFileRequest.Validator();

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void ValidateWhenContentTypeDocxShouldReturnTrue()
    {
        // Arrange
        var command = new UploadFileRequest.Command
        {
            FileContent = [1],
            FileName = "SampleDocx.docx",
            ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            FileSize = 1024 * 1024 / 2,
            UserId = 1,
            AvailableTill = null,
            FileSource = FileSource.System
        };

        var validator = new UploadFileRequest.Validator();

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }
}
