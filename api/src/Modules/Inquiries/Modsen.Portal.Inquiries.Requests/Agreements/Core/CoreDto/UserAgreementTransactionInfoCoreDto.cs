namespace Modsen.Portal.Inquiries.Requests.Agreements.Core.CoreDto
{
    internal class UserAgreementTransactionInfoCoreDto
    {
        public int UserId { get; set; }
        public DateTime TransactionDate { get; set; }
        public DateOnly ContractStartDate { get; set; }
        public DateOnly? ContractEndDate { get; set; }
        public bool IsTransactionForMonth { get; set; }
        public bool IsTransactionForYear { get; set; }
        public bool IsTransactionForIncompleteYear { get; set; }
    }
}
