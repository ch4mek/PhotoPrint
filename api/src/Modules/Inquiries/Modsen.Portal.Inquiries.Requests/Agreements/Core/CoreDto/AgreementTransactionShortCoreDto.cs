using Modsen.Portal.Core.Inquiries;

namespace Modsen.Portal.Inquiries.Requests.Agreements.Core.CoreDto;

internal class AgreementTransactionShortCoreDto
{
    public int Id { get; set; }
    public decimal AmountOfDays { get; set; }
    public DateTime TransactionDate { get; set; }
    public UserAgreementTransactionType TransactionType { get; set; }
    public int? UserAgreementId { get; set; }
}
