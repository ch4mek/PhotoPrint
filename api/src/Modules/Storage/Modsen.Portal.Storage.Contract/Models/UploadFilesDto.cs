using Modsen.Portal.Core;

namespace Modsen.Portal.Storage.Contract.Models;

public class UploadFilesDto
{
    public required string Boundary { get; init; }
    public required Stream ContentStream { get; init; }
    public FileSource FileSource { get; init; }
    public ApplicationFilePurpose FilePurpose { get; init; }
}
