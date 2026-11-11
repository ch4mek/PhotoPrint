
namespace Modsen.Portal.Merch.Requests.Transactions.Requests.RequestDto
{
    public class TransactionDto
    {
        public int Id { get; set; }
        public int Amount { get; set; }
        public required string WearhouseName { get; set; }
        public required string Size { get; set; }
        public Guid GroupCode { get; set; }
    }
}
