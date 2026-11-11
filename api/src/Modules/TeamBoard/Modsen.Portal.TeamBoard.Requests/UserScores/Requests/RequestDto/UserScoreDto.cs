namespace Modsen.Portal.TeamBoard.Requests.UserScores.Requests.RequestDto
{
    public class UserScoreDto
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public string? FirstNameRu { get; set; }
        public string? LastNameRu { get; set; }
        public int Score { get; set; }
        public string? AvatarId { get; set; }
        public string? DepartmentName { get; set; }
    }
}
