namespace Modsen.Portal.Storage.Requests.Files.Core.CoreDto;

public record FileCoreDto
{
    public required string FileId { get; set; }
    public required string VirtualDirectory { get; set; }
    public required string FilePath { get; set; }
    public required string ContentType { get; set; }
    public string? OriginalFileName { get; set; }
}
