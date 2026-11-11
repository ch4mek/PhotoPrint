namespace Modsen.Portal.Core;

public enum DocumentType
{
    None = 0,
    Pdf = 1,
    Word = 2,
    Excel = 3,
    Csv = 4,
    ExcelAccount = 5
}

public static class DocumentTypeExtensions
{
    public static string? GetExtensionWithDot(this DocumentType format)
    {
        return format switch
        {
            DocumentType.Pdf => PortalFileExtensionNames.Pdf,
            DocumentType.Word => PortalFileExtensionNames.Docx,
            DocumentType.Excel => PortalFileExtensionNames.Xlsx,
            DocumentType.ExcelAccount => PortalFileExtensionNames.Xlsx,
            DocumentType.Csv => PortalFileExtensionNames.Csv,
            _ => null
        };
    }
}
