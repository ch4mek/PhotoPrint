namespace Modsen.Portal.Core.UnitTests;

public class DocumentTypeExtensionsUnitTests
{
    [Theory]
    [InlineData(DocumentType.Pdf, PortalFileExtensionNames.Pdf)]
    [InlineData(DocumentType.Word, PortalFileExtensionNames.Docx)]
    [InlineData(DocumentType.Excel, PortalFileExtensionNames.Xlsx)]
    [InlineData(DocumentType.ExcelAccount, PortalFileExtensionNames.Xlsx)]
    [InlineData(DocumentType.Csv, PortalFileExtensionNames.Csv)]
    public void GetExtensionWithDotShouldReturnCorrectExtension(DocumentType type, string extension)
    {
        // Act
        var actual = type.GetExtensionWithDot();

        // Assert
        actual.Should().Be(extension);
    }

    [Theory]
    [InlineData((DocumentType)0, null)]
    [InlineData((DocumentType)6, null)]
    public void GetExtensionWithDotShouldReturnNull(DocumentType type, string? extension)
    {
        // Act
        var actual = type.GetExtensionWithDot();

        // Assert
        actual.Should().Be(extension);
    }
}
