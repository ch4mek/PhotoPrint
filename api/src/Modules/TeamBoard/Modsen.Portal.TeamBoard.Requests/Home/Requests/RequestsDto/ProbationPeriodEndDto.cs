namespace Modsen.Portal.TeamBoard.Requests.Home.Requests.RequestsDto
{
    internal class ProbationPeriodEndDto
    {
        public required int UserId { get; set; }
        public required string UserFullName {  get; set; }
        public DateOnly Date {  get; set; }
        public required string? UserPosition {  get; set; }
    }
}
