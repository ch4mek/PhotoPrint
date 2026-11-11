using Modsen.Portal.Identity.Contract.Models;

namespace Modsen.Portal.Identity.Contract
{
    public interface IIdentityCrossWriterContract
    {
        Task<int> UpdateUserInternal(CrossWriterUserDto user,
            CancellationToken cancellationToken);
    }
}
