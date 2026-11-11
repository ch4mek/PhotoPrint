using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.Requests.Agreements.Core.CoreDto;

namespace Modsen.Portal.Inquiries.Requests.Agreements.Core;

internal interface IAgreementTransactionsLoader
{
    Task<List<AgreementTransactionShortCoreDto>> Load(int[] agreementIds, CancellationToken cancellationToken);
}

internal class AgreementTransactionsLoader : IAgreementTransactionsLoader
{
    private readonly IInquiriesDbContext _inquiriesDbContext;

    public AgreementTransactionsLoader(IInquiriesDbContext inquiriesDbContext)
    {
        _inquiriesDbContext = inquiriesDbContext;
    }

    public async Task<List<AgreementTransactionShortCoreDto>> Load(int[] agreementIds, CancellationToken cancellationToken)
    {
        return await _inquiriesDbContext.UserAgreementTransactions
            .Where(x => agreementIds.Contains(x.Id))
            .Select(agreement => new AgreementTransactionShortCoreDto
            {
                Id = agreement.Id,
                AmountOfDays = agreement.AmountOfDays,
                TransactionDate = agreement.TransactionDate,
                TransactionType = agreement.TransactionType,
                UserAgreementId = agreement.UserAgreementId
            })
            .OrderByDescending(x => x.TransactionDate)
            .ToListAsync(cancellationToken);
    }
}
