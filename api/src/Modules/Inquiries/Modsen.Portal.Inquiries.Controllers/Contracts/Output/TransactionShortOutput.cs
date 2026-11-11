using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Inquiries.Requests.Agreements.Core.CoreDto;

namespace Modsen.Portal.Inquiries.Controllers.Contracts.Output;

internal sealed class TransactionShortOutput
{
    public int Id { get; set; }
    public decimal AmountOfDays { get; set; }
    public DateTime TransactionDate { get; set; }
    public UserAgreementTransactionType TransactionType { get; set; }
    public int? UserAgreementId { get; set; }

    public static TransactionShortOutput Of(AgreementTransactionShortCoreDto agreementTransactionShortCoreDto)
    {
        return new TransactionShortOutput
        {
            Id = agreementTransactionShortCoreDto.Id,
            AmountOfDays = agreementTransactionShortCoreDto.AmountOfDays,
            TransactionDate = agreementTransactionShortCoreDto.TransactionDate,
            TransactionType = agreementTransactionShortCoreDto.TransactionType,
            UserAgreementId = agreementTransactionShortCoreDto.UserAgreementId
        };
    }
}
