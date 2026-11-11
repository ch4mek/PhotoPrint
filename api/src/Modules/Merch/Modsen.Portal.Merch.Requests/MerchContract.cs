using MediatR;
using Modsen.Portal.Merch.Contract;
using Modsen.Portal.Merch.Requests.Items.Requests;

namespace Modsen.Portal.Merch.Requests;

internal class MerchContract : IMerchContract
{
    private readonly ISender _sender;

    public MerchContract(ISender sender)
    {
        _sender = sender;
    }

    public Task<IReadOnlyCollection<MerchSizeDto>> GetMerchSizes(CancellationToken cancellationToken)
    {
        return _sender.Send(new GetMerchSizesRequest.Query(), cancellationToken);
    }

    public Task<IReadOnlyCollection<MerchStorageDto>> GetMerchStorages(CancellationToken cancellationToken)
    {
        return _sender.Send(new GetMerchStoragesRequest.Query(), cancellationToken);
    }
}
