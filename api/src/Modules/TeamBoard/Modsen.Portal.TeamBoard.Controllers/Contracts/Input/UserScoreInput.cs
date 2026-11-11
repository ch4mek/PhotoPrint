namespace Modsen.Portal.TeamBoard.Controllers.Contracts.Input
{
    internal class UserScoreInput
    {
        public int Score { get; set; }
        public long Timestamp { get; set; }
        public required string Signature { get; set; }
    }
}
