using Modsen.Portal.Core;

namespace Modsen.Portal.Storage.Contract.Models
{
    public class UploadFileDto
    {
        public required ReadOnlyMemory<byte> FileContent { get; init; }
        public required string FileName { get; init; }
        public required long FileSize { get; init; }
        public int? UserId { get; init; }
        public DateTime? AvailableTill { get; init; }
        public required string ContentType { get; init; }
        public FileSource FileSource { get; init; }
        public ApplicationFilePurpose FilePurpose { get; init; }
    }
}
