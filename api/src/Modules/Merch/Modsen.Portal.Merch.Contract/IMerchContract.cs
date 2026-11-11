namespace Modsen.Portal.Merch.Contract;

public interface IMerchContract
{
    Task<IReadOnlyCollection<MerchSizeDto>> GetMerchSizes(CancellationToken cancellationToken);
    Task<IReadOnlyCollection<MerchStorageDto>> GetMerchStorages(CancellationToken cancellationToken);
}
