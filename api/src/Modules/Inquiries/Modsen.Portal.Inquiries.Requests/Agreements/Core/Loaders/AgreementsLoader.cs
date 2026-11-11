using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.Requests.Agreements.Core.CoreDto;

namespace Modsen.Portal.Inquiries.Requests.Agreements.Core.Loaders;

internal interface IAgreementsLoader
{
    Task<UserAgreementCoreDto> LoadAsync(int agreementId, CancellationToken cancellationToken);

    Task<List<UserAgreementCoreDto>> LoadAsync(int[] agreementIds, CancellationToken cancellationToken);
}

internal class AgreementsLoader : IAgreementsLoader
{
    private readonly IInquiriesDbContext _inquiriesDbContext;

    public AgreementsLoader(IInquiriesDbContext inquiriesDbContext)
    {
        _inquiriesDbContext = inquiriesDbContext;
    }

    public async Task<UserAgreementCoreDto> LoadAsync(int agreementId, CancellationToken cancellationToken)
    {
        var agreements = await LoadAsync([agreementId], cancellationToken);
        return agreements.FirstOrDefault()
            ?? throw new EntityNotFoundException($"Agreement by id {agreementId} not found");
    }

    public async Task<List<UserAgreementCoreDto>> LoadAsync(int[] agreementIds, CancellationToken cancellationToken)
    {
        return await _inquiriesDbContext.UserAgreements
            .Where(x => agreementIds.Contains(x.Id))
            .Select(agreement => new UserAgreementCoreDto
            {
                Id = agreement.Id,
                UserId = agreement.UserId,
                DateFrom = agreement.DateFrom,
            })
            .ToListAsync(cancellationToken);
    }
}
