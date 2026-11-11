using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Storage.Contract;

namespace Modsen.Portal.KnowledgeBase.Requests.Discounts.Core;

internal interface IImageFileIdValidator
{
    Task Validate(string? imageFileId, CancellationToken cancellationToken);
}

internal class ImageFileIdValidator : IImageFileIdValidator
{
    private readonly IStorageContract _storageContract;

    public ImageFileIdValidator(IStorageContract storageContract)
    {
        _storageContract = storageContract;
    }

    public async Task Validate(string? imageFileId, CancellationToken cancellationToken)
    {
        if (imageFileId != null &&
            !await _storageContract.IsFileExists(imageFileId, cancellationToken))
        {
            throw new EntityNotFoundException(PortalMessages.FileIsNotFound);
        }
    }
}
